Imports AccControlsWinForms
Imports ApskaitaObjects.ActiveReports
Imports AccDataBindingsWinForms.CachedInfoLists
Imports AccDataBindingsWinForms.Printing

Friend Class F_JournalEntryInfoList
    Implements ISupportsPrinting

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.AccountInfoList), GetType(HelperLists.PersonInfoList), _
        GetType(HelperLists.PersonGroupInfoList)}

    Private _TypesAbleToDelete As DocumentType() = New DocumentType() _
        {DocumentType.Offset, DocumentType.AccumulatedCosts, _
        DocumentType.TransferOfBalance, DocumentType.HolidayReserve, _
        DocumentType.None, DocumentType.ClosingEntries}

    Private _FormManager As CslaActionExtenderReportForm(Of JournalEntryInfoList)
    Private _ListViewManager As DataListViewEditControlManager(Of JournalEntryInfo)
    Private _QueryManager As CslaActionExtenderQueryObject
    Private _DeleteSuccessMessage As String = ""


    Private Sub F_GeneralLedger_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub



    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, _RequiredCachedLists) Then Return False

        Try

            _ListViewManager = New DataListViewEditControlManager(Of JournalEntryInfo) _
                (ItemsDataListView, ContextMenuStrip1, Nothing, Nothing, _
                 AddressOf ItemActionIsAvailable)

            _ListViewManager.AddCancelButton = True
            _ListViewManager.AddButtonHandler(My.Resources.F_JournalEntryInfoList_EditLabel, _
                My.Resources.F_JournalEntryInfoList_EditTooltipLabel, AddressOf EditItem)
            _ListViewManager.AddButtonHandler(My.Resources.F_JournalEntryInfoList_DeleteLabel, _
                My.Resources.F_JournalEntryInfoList_DeleteTooltipLabel, AddressOf DeleteItem)
            _ListViewManager.AddButtonHandler(My.Resources.F_JournalEntryInfoList_CorrespondencesLabel, _
                My.Resources.F_JournalEntryInfoList_CorrespondencesTooltipLabel, AddressOf ShowItemJournalEntry)
            _ListViewManager.AddButtonHandler(My.Resources.F_JournalEntryInfoList_RelationsLabel, _
                My.Resources.F_JournalEntryInfoList_RelationsTooltipLabel, AddressOf ShowItemRelations)

            _ListViewManager.AddMenuItemHandler(EditToolStripMenuItem, AddressOf EditItem)
            _ListViewManager.AddMenuItemHandler(DeleteToolStripMenuItem, AddressOf DeleteItem)
            _ListViewManager.AddMenuItemHandler(CorrespondencesToolStripMenuItem, AddressOf ShowItemJournalEntry)
            _ListViewManager.AddMenuItemHandler(RelationsToolStripMenuItem, AddressOf ShowItemRelations)

            _FormManager = New CslaActionExtenderReportForm(Of JournalEntryInfoList) _
                (Me, JournalEntryInfoListBindingSource, Nothing, _RequiredCachedLists, _
                 RefreshButton, ProgressFiller1, "GetList", AddressOf GetReportParams)

            _FormManager.ManageDataListViewStates(ItemsDataListView)

            _QueryManager = New CslaActionExtenderQueryObject(Me, ProgressFiller2)

            LoadPersonGroupInfoListToListCombo(PersonGroupComboBox)
            LoadPersonInfoListToListCombo(PersonAccGridComboBox, True, True, True, True)
            LoadEnumLocalizedListToComboBox(DocTypeComboBox, GetType(DocumentType), True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        DateFromDateTimePicker.Value = Today.AddMonths(-1)

        Return True

    End Function


    Private Function GetReportParams() As Object()

        Dim personID As Integer = 0
        Dim personName As String = ""
        Try
            personID = DirectCast(PersonAccGridComboBox.SelectedValue, HelperLists.PersonInfo).ID
            personName = DirectCast(PersonAccGridComboBox.SelectedValue, HelperLists.PersonInfo).Name
        Catch ex As Exception
        End Try

        Dim groupID As Integer
        Dim groupName As String = ""
        Try
            groupID = DirectCast(PersonGroupComboBox.SelectedValue, HelperLists.PersonGroupInfo).ID
            groupName = DirectCast(PersonGroupComboBox.SelectedValue, HelperLists.PersonGroupInfo).Name
        Catch ex As Exception
        End Try

        Dim documentFilter As DocumentType = DocumentType.None
        Dim applyDocFilter As Boolean = False
        If DocTypeComboBox.SelectedIndex > 0 Then
            applyDocFilter = True
            documentFilter = ConvertLocalizedName(Of DocumentType)(DocTypeComboBox.SelectedItem.ToString)
        End If

        Return New Object() {DateFromDateTimePicker.Value.Date, DateToDateTimePicker.Value.Date, _
            ContentTextBox.Text.Trim, personID, groupID, documentFilter, applyDocFilter, _
            personName, groupName}

    End Function

    Private Function ItemActionIsAvailable(ByVal item As JournalEntryInfo, _
        ByVal action As String) As Boolean

        If item Is Nothing OrElse action Is Nothing Then Return False

        If action.Trim.ToLower = "EditItem".ToLower Then

            If item.DocType = DocumentType.ClosingEntries Then
                Return False
            Else
                Return True
            End If

        ElseIf action.Trim.ToLower = "DeleteItem".ToLower Then

            If Array.IndexOf(_TypesAbleToDelete, item.DocType) < 0 Then
                Return False
            Else
                Return True
            End If

        ElseIf action.Trim.ToLower = "ShowItemJournalEntry".ToLower Then

            If item.DocType = DocumentType.None Then
                Return False
            Else
                Return True
            End If

        ElseIf action.Trim.ToLower = "ShowItemRelations".ToLower Then
            Return True
        End If

        Return False

    End Function

    Private Sub EditItem(ByVal item As JournalEntryInfo)
        OpenObjectEditForm(_QueryManager, item.Id, item.DocType)
    End Sub

    Private Sub DeleteItem(ByVal item As JournalEntryInfo)

        If item.DocType = DocumentType.ClosingEntries Then
            If Not YesOrNo("Ar tikrai norite pašalinti 5/6 klasių uždarymo duomenis iš duomenų bazės?") Then Exit Sub
        ElseIf item.DocType = DocumentType.None Then
            If Not YesOrNo("Ar tikrai norite pašalinti bendrojo žurnalo įrašo duomenis iš duomenų bazės?") Then Exit Sub
        Else
            If Not YesOrNo("Ar tikrai norite pašalinti dokumento duomenis iš duomenų bazės?") Then Exit Sub
        End If

        _DeleteSuccessMessage = "Dokumento duomenys sėkmingai pašalinti iš įmonės duomenų bazės."
        If item.DocType = DocumentType.ClosingEntries Then
            _DeleteSuccessMessage = "5/6 klasių uždarymo duomenys sėkmingai pašalinti iš įmonės duomenų bazės."
        ElseIf item.DocType = DocumentType.None Then
            _DeleteSuccessMessage = "Bendojo žurnalo įrašo duomenys sėkmingai pašalinti iš įmonės duomenų bazės."
        End If

        If item.DocType = DocumentType.Offset Then
            _QueryManager.InvokeQuery(Of Documents.Offset)(Nothing, "DeleteOffset", False, _
                AddressOf OnItemDeleted, item.Id)
        ElseIf item.DocType = DocumentType.AccumulatedCosts Then
            _QueryManager.InvokeQuery(Of Documents.AccumulativeCosts)(Nothing, "DeleteAccumulativeCosts", False, _
                AddressOf OnItemDeleted, item.Id)
        ElseIf item.DocType = DocumentType.TransferOfBalance Then
            _QueryManager.InvokeQuery(Of General.TransferOfBalance)(Nothing, "DeleteTransferOfBalance", False, _
                AddressOf OnItemDeleted)
        ElseIf item.DocType = DocumentType.HolidayReserve Then
            _QueryManager.InvokeQuery(Of Workers.HolidayPayReserve)(Nothing, "DeleteHolidayPayReserve", False, _
                AddressOf OnItemDeleted, item.Id)
        Else
            _QueryManager.InvokeQuery(Of General.JournalEntry)(Nothing, "DeleteJournalEntry", False, _
                AddressOf OnItemDeleted, item.Id)
        End If

    End Sub

    Private Sub OnItemDeleted(ByVal nullResult As Object, ByVal exceptionHandled As Boolean)
        If exceptionHandled Then Exit Sub
        If Not YesOrNo(_DeleteSuccessMessage & vbCrLf & "Atnaujinti ataskaitos duomenis?") Then Exit Sub
        RefreshButton.PerformClick()
    End Sub

    Private Sub ShowItemJournalEntry(ByVal item As JournalEntryInfo)
        OpenJournalEntryEditForm(_QueryManager, item.Id)
    End Sub

    Private Sub ShowItemRelations(ByVal item As JournalEntryInfo)
        If item Is Nothing Then Exit Sub
        _QueryManager.InvokeQuery(Of HelperLists.IndirectRelationInfoList) _
            (Nothing, "GetIndirectRelationInfoList", True, _
             AddressOf OpenObjectEditForm, item.Id)
    End Sub


    Private Sub ClearButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ClearButton.Click
        DocTypeComboBox.SelectedIndex = -1
        PersonGroupComboBox.SelectedIndex = -1
        PersonAccGridComboBox.SelectedValue = Nothing
        ContentTextBox.Text = ""
    End Sub

    Private Sub CloseAccountsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CloseAccountsButton.Click
        OpenNewForm(Of General.ClosingEntriesCommand)()
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
            PrintObject(_FormManager.DataSource, False, 0, "BendrasisZurnalas", Me, _
                _ListViewManager.GetCurrentFilterDescription(), _ListViewManager.GetDisplayOrderIndexes())
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If _FormManager.DataSource Is Nothing Then Exit Sub
        Try
            PrintObject(_FormManager.DataSource, True, 0, "BendrasisZurnalas", Me, _
                _ListViewManager.GetCurrentFilterDescription(), _ListViewManager.GetDisplayOrderIndexes())
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function

End Class