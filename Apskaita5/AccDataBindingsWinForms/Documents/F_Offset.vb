Imports ApskaitaObjects.Documents
Imports AccControlsWinForms
Imports AccDataBindingsWinForms.CachedInfoLists
Imports AccDataBindingsWinForms.Printing

Friend Class F_Offset
    Implements IObjectEditForm, ISupportsPrinting

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.PersonInfoList), GetType(HelperLists.AccountInfoList)}

    Private WithEvents _FormManager As CslaActionExtenderEditForm(Of Offset)
    Private _ListViewManager As DataListViewEditControlManager(Of OffsetItem)
    Private _QueryManager As CslaActionExtenderQueryObject

    Private _DocumentToEdit As Offset = Nothing


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If _FormManager Is Nothing OrElse _FormManager.DataSource Is Nothing Then
                If _DocumentToEdit Is Nothing OrElse _DocumentToEdit.IsNew Then
                    Return Integer.MinValue
                Else
                    Return _DocumentToEdit.ID
                End If
            End If
            Return _FormManager.DataSource.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(Offset)
        End Get
    End Property


    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal documentToEdit As Offset)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _DocumentToEdit = documentToEdit

    End Sub


    Private Sub F_Offset_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        If _DocumentToEdit Is Nothing Then
            _DocumentToEdit = Offset.NewOffset()
        End If

        Try

            _FormManager = New CslaActionExtenderEditForm(Of Offset)(Me, OffsetBindingSource, _
                _DocumentToEdit, _RequiredCachedLists, IOkButton, IApplyButton, ICancelButton, _
                LimitationsButton, ProgressFiller1)

            _FormManager.ManageDataListViewStates(ItemListDataListView)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        ConfigureButtons()

    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, _RequiredCachedLists) Then Return False

        Try

            _ListViewManager = New DataListViewEditControlManager(Of OffsetItem) _
                (ItemListDataListView, Nothing, AddressOf OnItemsDelete, _
                 AddressOf OnItemAdd, Nothing)

            Dim typeComboBox As New ComboBox
            typeComboBox.DataSource = Utilities.GetLocalizedNameList(GetType(BookEntryType))
            _ListViewManager.AddCustomEditControl("TypeHumanReadable", typeComboBox)

            Dim currencyComboBox As New ComboBox
            LoadCurrencyCodeListToComboBox(currencyComboBox, False)
            _ListViewManager.AddCustomEditControl("CurrencyCode", currencyComboBox)

            Dim personListListComboBox As New AccListComboBox
            LoadPersonInfoListToListCombo(personListListComboBox, True, True, True, True)
            _ListViewManager.AddCustomEditControl("Person", personListListComboBox)

            _QueryManager = New CslaActionExtenderQueryObject(Me, ProgressFiller2)

            SetupDefaultControls(Of Offset)(Me, OffsetBindingSource)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function


    Private Sub OnItemsDelete(ByVal items As OffsetItem())
        If items Is Nothing OrElse items.Length < 1 OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        If Not _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange Then
            MsgBox(String.Format("Klaida. Keisti dokumento finansinių duomenų negalima, įskaitant eilučių pridėjimą ar ištrynimą:{0}{1}", vbCrLf, _
                _FormManager.DataSource.ChronologicValidator.FinancialDataCanChangeExplanation), _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If
        For Each item As OffsetItem In items
            _FormManager.DataSource.ItemList.Remove(item)
        Next
    End Sub

    Private Sub OnItemAdd()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        If Not _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange Then
            MsgBox(String.Format("Klaida. Keisti dokumento finansinių duomenų negalima, įskaitant eilučių pridėjimą ar ištrynimą:{0}{1}", vbCrLf, _
                _FormManager.DataSource.ChronologicValidator.FinancialDataCanChangeExplanation), _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If
        _FormManager.DataSource.ItemList.AddNew()
    End Sub

    Private Sub GetCurrencyRatesButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GetCurrencyRatesButton.Click

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Dim paramList As New AccWebCrawler.CurrencyRateList

        For Each b As OffsetItem In _FormManager.DataSource.ItemList
            paramList.Add(_FormManager.DataSource.Date, b.CurrencyCode)
        Next

        If paramList.Count < 1 Then Exit Sub

        If Not YesOrNo("Gauti valiutos kursą?") Then Exit Sub

        Using frm As New AccWebCrawler.F_LaunchWebCrawler(paramList, GetCurrentCompany.BaseCurrency)

            If frm.ShowDialog <> Windows.Forms.DialogResult.OK OrElse frm.result Is Nothing _
                OrElse Not TypeOf frm.result Is AccWebCrawler.CurrencyRateList _
                OrElse DirectCast(frm.result, AccWebCrawler.CurrencyRateList).Count < 1 Then Exit Sub

            For Each b As OffsetItem In _FormManager.DataSource.ItemList
                b.CurrencyRate = DirectCast(frm.result, AccWebCrawler.CurrencyRateList). _
                    GetCurrencyRate(_FormManager.DataSource.Date, b.CurrencyCode)
            Next

        End Using

    End Sub

    Private Sub BalanceButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles BalanceButton.Click

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Try
            Using busy As New StatusBusy
                _FormManager.DataSource.CalculateBalance()
            End Using
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click
        If _FormManager.DataSource Is Nothing OrElse _FormManager.DataSource.IsNew Then Exit Sub
        OpenJournalEntryEditForm(_QueryManager, _FormManager.DataSource.ID)
    End Sub


    Public Function GetMailDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetMailDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintPreviewDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintPreviewDropDownItems
        Return Nothing
    End Function

    Public Sub OnMailClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnMailClick

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Using frm As New F_SendObjToEmail(_FormManager.DataSource, 0)
            frm.ShowDialog()
        End Using

    End Sub

    Public Sub OnPrintClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintClick

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Try
            PrintObject(_FormManager.DataSource, False, 0, "Uzskaita", Me, "")
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Try
            PrintObject(_FormManager.DataSource, True, 0, "Uzskaita", Me, "")
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function


    Private Sub _FormManager_DataSourceStateHasChanged(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles _FormManager.DataSourceStateHasChanged
        ConfigureButtons()
    End Sub

    Private Sub ConfigureButtons()

        If _FormManager.DataSource Is Nothing Then Exit Sub

        BalanceSumAccTextBox.ReadOnly = Not _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange
        BalanceAccountAccGridComboBox.Enabled = _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange
        BalanceButton.Enabled = _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange
        GetCurrencyRatesButton.Enabled = _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange

        ICancelButton.Enabled = Not _FormManager.DataSource.IsNew

    End Sub

End Class