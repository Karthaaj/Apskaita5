Imports AccControlsWinForms
Imports AccDataBindingsWinForms.CachedInfoLists
Imports ApskaitaObjects.General

Friend Class F_TemplateJournalEntry
    Implements IObjectEditForm

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.AccountInfoList)}

    Private WithEvents _FormManager As CslaActionExtenderEditForm(Of TemplateJournalEntry)
    Private _ListViewManagerCredit As DataListViewEditControlManager(Of TemplateBookEntry)
    Private _ListViewManagerDebit As DataListViewEditControlManager(Of TemplateBookEntry)

    Private _TemplateToEdit As TemplateJournalEntry


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If _FormManager Is Nothing OrElse _FormManager.DataSource Is Nothing Then
                If _TemplateToEdit Is Nothing OrElse _TemplateToEdit.IsNew Then
                    Return Integer.MinValue
                Else
                    Return _TemplateToEdit.ID
                End If
            ElseIf _FormManager.DataSource.IsNew Then
                Return Integer.MinValue
            End If
            Return _FormManager.DataSource.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(General.TemplateJournalEntry)
        End Get
    End Property


    Public Sub New(ByVal templateToEdit As TemplateJournalEntry)
        InitializeComponent()
        _TemplateToEdit = templateToEdit
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub F_Template_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        If _TemplateToEdit Is Nothing Then
            _TemplateToEdit = TemplateJournalEntry.NewTemplateJournalEntry()
        End If

        Try

            _FormManager = New CslaActionExtenderEditForm(Of TemplateJournalEntry) _
                (Me, TemplateJournalEntryBindingSource, _TemplateToEdit, _
                 _RequiredCachedLists, nOkButton, ApplyButton, nCancelButton, _
                 Nothing, ProgressFiller1)

            _FormManager.AddNewDataSourceButton(NewItemButton, "NewTemplateJournalEntry")

            ConfigureButtons()

        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko gauti bendrojo žurnalo įrašo šablono duomenų.", ex), Nothing)
            DisableAllControls(Me)
            Exit Sub
        End Try

    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, _RequiredCachedLists) Then Return False

        Try

            _ListViewManagerCredit = New DataListViewEditControlManager(Of TemplateBookEntry) _
                (_CreditListDataListView, Nothing, AddressOf OnItemsDeleteCredit, _
                 AddressOf OnItemAddCredit, Nothing, _TemplateToEdit)

            _ListViewManagerDebit = New DataListViewEditControlManager(Of TemplateBookEntry) _
                (_DebetListDataListView, Nothing, AddressOf OnItemsDeleteDebit, _
                 AddressOf OnItemAddDebit, Nothing, _TemplateToEdit)

            SetupDefaultControls(Of TemplateJournalEntry)(Me, _
                TemplateJournalEntryBindingSource, _TemplateToEdit)

        Catch ex As Exception
            ShowError(ex, Nothing)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function


    Private Sub OnItemsDeleteCredit(ByVal items As TemplateBookEntry())
        If items Is Nothing OrElse items.Length < 1 OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        For Each item As TemplateBookEntry In items
            _FormManager.DataSource.CreditList.Remove(item)
        Next
    End Sub

    Private Sub OnItemsDeleteDebit(ByVal items As TemplateBookEntry())
        If items Is Nothing OrElse items.Length < 1 OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        For Each item As TemplateBookEntry In items
            _FormManager.DataSource.DebetList.Remove(item)
        Next
    End Sub

    Private Sub OnItemAddCredit()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        _FormManager.DataSource.CreditList.AddNew()
    End Sub

    Private Sub OnItemAddDebit()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        _FormManager.DataSource.DebetList.AddNew()
    End Sub


    Private Sub _FormManager_DataSourceStateHasChanged(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles _FormManager.DataSourceStateHasChanged
        ConfigureButtons()
    End Sub

    Private Sub ConfigureButtons()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        EditedBaner.Visible = Not (_FormManager.DataSource.IsNew)
        nOkButton.Enabled = ((_FormManager.DataSource.IsNew AndAlso TemplateJournalEntry.CanAddObject) OrElse _
            (Not _FormManager.DataSource.IsNew AndAlso TemplateJournalEntry.CanEditObject))
        ApplyButton.Enabled = ((_FormManager.DataSource.IsNew AndAlso TemplateJournalEntry.CanAddObject) OrElse _
            (Not _FormManager.DataSource.IsNew AndAlso TemplateJournalEntry.CanEditObject))
        nCancelButton.Enabled = Not _FormManager.DataSource.IsNew
    End Sub

End Class