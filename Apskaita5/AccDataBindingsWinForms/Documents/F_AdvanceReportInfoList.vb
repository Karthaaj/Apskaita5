Imports ApskaitaObjects.ActiveReports
Imports ApskaitaObjects.Documents
Imports AccControlsWinForms
Imports AccDataBindingsWinForms.CachedInfoLists
Imports AccDataBindingsWinForms.Printing

Friend Class F_AdvanceReportInfoList
    Implements ISupportsPrinting

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.PersonInfoList)}

    Private _FormManager As CslaActionExtenderReportForm(Of AdvanceReportInfoList)
    Private _ListViewManager As DataListViewEditControlManager(Of AdvanceReportInfo)
    Private _QueryManager As CslaActionExtenderQueryObject


    Private Sub F_AdvanceReportInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub



    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, _RequiredCachedLists) Then Return False

        Try

            _ListViewManager = New DataListViewEditControlManager(Of AdvanceReportInfo) _
                (AdvanceReportInfoListDataListView, ContextMenuStrip1, Nothing, _
                 Nothing, Nothing)

            _ListViewManager.AddCancelButton = True
            _ListViewManager.AddButtonHandler("Keisti", "Keisti avanso apyskaitos duomenis.", _
                AddressOf ChangeItem)
            _ListViewManager.AddButtonHandler("Ištrinti", "Pašalinti avanso apyskaitos duomenis iš duomenų bazės.", _
                AddressOf DeleteItem)
            _ListViewManager.AddButtonHandler("Orderis", "Atidaryti susietą/naują kasos orderį.", _
                AddressOf AttachedOrder)

            _ListViewManager.AddMenuItemHandler(ChangeItem_MenuItem, AddressOf ChangeItem)
            _ListViewManager.AddMenuItemHandler(DeleteItem_MenuItem, AddressOf DeleteItem)
            _ListViewManager.AddMenuItemHandler(NewItem_MenuItem, AddressOf NewItem)
            _ListViewManager.AddMenuItemHandler(AttachedOrder_MenuItem, AddressOf AttachedOrder)

            _QueryManager = New CslaActionExtenderQueryObject(Me, ProgressFiller2)

            ' AdvanceReportInfoList.GetAdvanceReportInfoList(dateFrom, dateTo, person)
            _FormManager = New CslaActionExtenderReportForm(Of AdvanceReportInfoList) _
                (Me, AdvanceReportInfoListBindingSource, Nothing, _RequiredCachedLists, RefreshButton, _
                 ProgressFiller1, "GetAdvanceReportInfoList", AddressOf GetReportParams)

            _FormManager.ManageDataListViewStates(AdvanceReportInfoListDataListView)

            LoadPersonInfoListToListCombo(PersonInfoAccGridComboBox, True, False, False, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        DateFromDateTimePicker.Value = Today.Subtract(New TimeSpan(30, 0, 0, 0))

        Return True

    End Function


    Private Function GetReportParams() As Object()

        Dim personFilter As HelperLists.PersonInfo = Nothing
        Try
            personFilter = DirectCast(PersonInfoAccGridComboBox.SelectedValue, HelperLists.PersonInfo)
        Catch ex As Exception
        End Try

        'AdvanceReportInfoList.GetAdvanceReportInfoList(DateFromDateTimePicker.Value.Date, _
        '  DateToDateTimePicker.Value.Date, personFilter)
        Return New Object() {DateFromDateTimePicker.Value.Date, _
          DateToDateTimePicker.Value.Date, personFilter}

    End Function

    Private Sub ChangeItem(ByVal item As AdvanceReportInfo)
        If item Is Nothing Then Exit Sub
        ' AdvanceReport.GetAdvanceReport(item.ID)
        _QueryManager.InvokeQuery(Of AdvanceReport)(Nothing, "GetAdvanceReport", True, _
            AddressOf OpenObjectEditForm, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As AdvanceReportInfo)

        If item Is Nothing Then Exit Sub

        If CheckIfObjectEditFormOpen(Of AdvanceReport)(item.ID, True, True) Then Exit Sub

        If Not YesOrNo("Ar tikrai norite pašalinti dokumento duomenis iš duomenų bazės?") Then Exit Sub

        ' AdvanceReport.DeleteAdvanceReport(item.ID)
        _QueryManager.InvokeQuery(Of AdvanceReport)(Nothing, "DeleteAdvanceReport", False, _
            AddressOf OnItemDeleted, item.ID)

    End Sub

    Private Sub OnItemDeleted(ByVal result As Object, ByVal exceptionHandled As Boolean)
        If exceptionHandled Then Exit Sub
        If Not YesOrNo("Dokumento duomenys sėkmingai pašalinti iš įmonės duomenų bazės. Atnaujinti sąrašą?") Then Exit Sub
        RefreshButton.PerformClick()
    End Sub

    Private Sub NewItem(ByVal item As AdvanceReportInfo)
        OpenNewForm(Of AdvanceReport)()
    End Sub

    Private Sub AttachedOrder(ByVal item As AdvanceReportInfo)

        If item.TillOrderID > 0 Then

            If item.IsIncomeTillOrder Then
                ' TillIncomeOrder.GetTillIncomeOrder(item.TillOrderID)
                _QueryManager.InvokeQuery(Of TillIncomeOrder)(Nothing, "GetTillIncomeOrder", True, _
                    AddressOf OpenObjectEditForm, item.TillOrderID)
            Else
                ' TillSpendingsOrder.GetTillSpendingsOrder(item.TillOrderID)
                _QueryManager.InvokeQuery(Of TillSpendingsOrder)(Nothing, "GetTillSpendingsOrder", True, _
                    AddressOf OpenObjectEditForm, item.TillOrderID)
            End If

        Else

            Dim personList As HelperLists.PersonInfoList = HelperLists.PersonInfoList.GetList()

            If item.IsIncomeTillOrder Then

                Dim result As TillIncomeOrder
                Try
                    result = TillIncomeOrder.NewTillIncomeOrder()
                    result.LoadAdvanceReport(item, personList)
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
                OpenObjectEditForm(result)

            Else

                Dim result As TillSpendingsOrder
                Try
                    result = TillSpendingsOrder.NewTillSpendingsOrder()
                    result.LoadAdvanceReport(item, personList)
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
                OpenObjectEditForm(result)

            End If

        End If

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
            PrintObject(_FormManager.DataSource, False, 0, "AvansoApyskaituSarasas", Me, _
                _ListViewManager.GetCurrentFilterDescription(), _
                _ListViewManager.GetDisplayOrderIndexes())
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If _FormManager.DataSource Is Nothing Then Exit Sub
        Try
            PrintObject(_FormManager.DataSource, True, 0, "AvansoApyskaituSarasas", Me, _
                _ListViewManager.GetCurrentFilterDescription(), _
                _ListViewManager.GetDisplayOrderIndexes())
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function

End Class