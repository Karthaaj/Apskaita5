Imports ApskaitaObjects.Documents
Imports AccControlsWinForms
Imports AccDataBindingsWinForms.Printing
Imports AccDataBindingsWinForms.CachedInfoLists

Friend Class F_BankOperation
    Implements ISupportsPrinting, IObjectEditForm

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.DocumentSerialInfoList), GetType(HelperLists.PersonInfoList), _
         GetType(HelperLists.AccountInfoList), GetType(HelperLists.CashAccountInfoList)}

    Private WithEvents _FormManager As CslaActionExtenderEditForm(Of BankOperation)
    Private _ListViewManager As DataListViewEditControlManager(Of General.BookEntry)
    Private _QueryManager As CslaActionExtenderQueryObject

    Private _DocumentToEdit As BankOperation = Nothing
    Private _ImportedOperation As ImportedBankOperation = Nothing


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
            Return GetType(BankOperation)
        End Get
    End Property


    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal documentToEdit As BankOperation)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _DocumentToEdit = documentToEdit

    End Sub

    Public Sub New(ByVal importedOperation As ImportedBankOperation)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _ImportedOperation = importedOperation

    End Sub


    Private Sub F_BankOperation_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        If _DocumentToEdit Is Nothing AndAlso Not _ImportedOperation Is Nothing Then

            Try
                _DocumentToEdit = BankOperation.NewBankOperation(_ImportedOperation.OperationData, _
                    _ImportedOperation.OperationAccount)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try

        ElseIf _DocumentToEdit Is Nothing Then

            _DocumentToEdit = BankOperation.NewBankOperation

        End If

        Try

            _FormManager = New CslaActionExtenderEditForm(Of BankOperation) _
                (Me, BankOperationBindingSource, _DocumentToEdit, _
                _RequiredCachedLists, IOkButton, IApplyButton, ICancelButton, _
                LimitationsButton, ProgressFiller1)

            _FormManager.ManageDataListViewStates(BookEntryItemsDataListView)

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

            _ListViewManager = New DataListViewEditControlManager(Of General.BookEntry) _
                (BookEntryItemsDataListView, Nothing, AddressOf OnItemsDelete, _
                 AddressOf OnItemAdd, Nothing)

            Dim personListListBox As New AccListComboBox
            LoadPersonInfoListToListCombo(personListListBox, True, True, True, True)
            _ListViewManager.AddCustomEditControl("Person", personListListBox)

            _QueryManager = New CslaActionExtenderQueryObject(Me, ProgressFiller2)

            SetupDefaultControls(Of BankOperation)(Me, BankOperationBindingSource)

            LoadCurrencyCodeListToComboBox(CurrencyCodeComboBox, False)

            LoadCashAccountInfoListToListCombo(AccountAccGridComboBox, True)
            LoadCashAccountInfoListToListCombo(CreditCashAccountAccGridComboBox, True)
            LoadPersonInfoListToListCombo(PersonAccGridComboBox, True, True, True, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function


    Private Sub OnItemsDelete(ByVal items As General.BookEntry())
        If items Is Nothing OrElse items.Length < 1 OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        If Not _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange Then
            MsgBox(String.Format("Klaida. Keisti dokumento finansinių duomenų negalima, įskaitant kontavimų pridėjimą ar ištrynimą:{0}{1}", vbCrLf, _
                _FormManager.DataSource.ChronologicValidator.FinancialDataCanChangeExplanation), _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If
        For Each item As General.BookEntry In items
            _FormManager.DataSource.BookEntryItems.Remove(item)
        Next
    End Sub

    Private Sub OnItemAdd()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        If Not _FormManager.DataSource.ChronologicValidator.FinancialDataCanChange Then
            MsgBox(String.Format("Klaida. Keisti dokumento finansinių duomenų negalima, įskaitant kontavimų pridėjimą ar ištrynimą:{0}{1}", _
                vbCrLf, _FormManager.DataSource.ChronologicValidator.FinancialDataCanChangeExplanation), _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If
        _FormManager.DataSource.BookEntryItems.AddNew()
    End Sub

    Private Sub GetCurrencyRatesButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GetCurrencyRatesButton.Click

        If _FormManager.DataSource Is Nothing Then Exit Sub

        Dim paramList As New AccWebCrawler.CurrencyRateList

        paramList.Add(_FormManager.DataSource.Date, _FormManager.DataSource.CurrencyCode)
        paramList.Add(_FormManager.DataSource.Date, _FormManager.DataSource.AccountCurrency)

        If paramList.Count < 1 Then Exit Sub

        If Not YesOrNo("Gauti valiutos kursą?") Then Exit Sub

        Using frm As New AccWebCrawler.F_LaunchWebCrawler(paramList, GetCurrentCompany.BaseCurrency)

            If frm.ShowDialog <> Windows.Forms.DialogResult.OK OrElse frm.result Is Nothing _
                OrElse Not TypeOf frm.result Is AccWebCrawler.CurrencyRateList _
                OrElse DirectCast(frm.result, AccWebCrawler.CurrencyRateList).Count < 1 Then Exit Sub

            If Not _FormManager.DataSource.CurrencyRateIsReadOnly Then
                _FormManager.DataSource.CurrencyRate = DirectCast(frm.result, AccWebCrawler.CurrencyRateList). _
                    GetCurrencyRate(_FormManager.DataSource.Date, _FormManager.DataSource.CurrencyCode)
            End If
            If Not _FormManager.DataSource.CurrencyRateInAccountIsReadOnly Then
                _FormManager.DataSource.CurrencyRateInAccount = DirectCast(frm.result, AccWebCrawler.CurrencyRateList). _
                    GetCurrencyRate(_FormManager.DataSource.Date, _FormManager.DataSource.AccountCurrency)
            End If

        End Using

    End Sub

    Private Sub Control_Validated(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles IsTransferBetweenAccountsCheckBox.Validated, _
        AccountAccGridComboBox.Validated, CreditCashAccountAccGridComboBox.Validated, _
        IsDebitRadioButton.Validated, IsCreditRadioButton.Validated, CurrencyCodeComboBox.Validated

        If _FormManager Is Nothing OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        ConfigureButtons()

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click
        If _FormManager.DataSource Is Nothing OrElse Not _FormManager.DataSource.JournalEntryID > 0 Then Exit Sub
        OpenJournalEntryEditForm(_QueryManager, _FormManager.DataSource.JournalEntryID)
    End Sub

    Private Sub CalculateCurrencyRateChangeImpactButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CalculateCurrencyRateChangeImpactButton.Click

        If _FormManager.DataSource Is Nothing OrElse _FormManager.DataSource.CurrencyRateChangeImpactIsReadOnly Then Exit Sub

        _FormManager.DataSource.CalculateCurrencyRateChangeImpact()

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
            PrintObject(_FormManager.DataSource, False, 0, "BankoOperacija", Me, "")
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If _FormManager.DataSource Is Nothing Then Exit Sub
        Try
            PrintObject(_FormManager.DataSource, True, 0, "BankoOperacija", Me, "")
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

        AccountAccGridComboBox.Enabled = Not _FormManager.DataSource.AccountIsReadOnly
        PersonAccGridComboBox.Enabled = Not _FormManager.DataSource.PersonIsReadOnly
        IsTransferBetweenAccountsCheckBox.Enabled = Not _FormManager.DataSource.IsTransferBetweenAccountsIsReadOnly
        CreditCashAccountAccGridComboBox.Enabled = Not _FormManager.DataSource.CreditCashAccountIsReadOnly
        UniqueCodeInCreditAccountTextBox.ReadOnly = _FormManager.DataSource.UniqueCodeInCreditAccountIsReadOnly
        IsDebitRadioButton.Enabled = Not _FormManager.DataSource.IsDebitIsReadOnly
        IsCreditRadioButton.Enabled = Not _FormManager.DataSource.IsCreditIsReadOnly
        SumAccTextBox.ReadOnly = _FormManager.DataSource.SumIsReadOnly
        SumInAccountAccTextBox.ReadOnly = _FormManager.DataSource.SumInAccountIsReadOnly
        CurrencyCodeComboBox.Enabled = Not _FormManager.DataSource.CurrencyCodeIsReadOnly
        CurrencyRateAccTextBox.ReadOnly = _FormManager.DataSource.CurrencyRateIsReadOnly
        CurrencyRateChangeImpactAccTextBox.ReadOnly = _FormManager.DataSource.CurrencyRateChangeImpactIsReadOnly
        CalculateCurrencyRateChangeImpactButton.Enabled = Not _FormManager.DataSource.CurrencyRateChangeImpactIsReadOnly
        AccountCurrencyRateChangeImpactAccGridComboBox.Enabled = Not _FormManager.DataSource.AccountCurrencyRateChangeImpactIsReadOnly
        CurrencyRateInAccountAccTextBox.ReadOnly = _FormManager.DataSource.CurrencyRateInAccountIsReadOnly
        CurrencyRateInAccountAccTextBox.ReadOnly = _FormManager.DataSource.CurrencyRateInAccountIsReadOnly
        AccountBankCurrencyConversionCostsAccGridComboBox.Enabled = Not _FormManager.DataSource.AccountBankCurrencyConversionCostsIsReadOnly

        GetCurrencyRatesButton.Enabled = Not _FormManager.DataSource.CurrencyRateIsReadOnly OrElse Not _FormManager.DataSource.CurrencyRateInAccountIsReadOnly

        _ListViewManager.IsReadOnly = _FormManager.DataSource.BookEntryItemsIsReadOnly

    End Sub

End Class