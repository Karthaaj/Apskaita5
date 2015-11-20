Imports ApskaitaObjects.ActiveReports
Imports ApskaitaObjects.HelperLists

Public Class F_WorkerInfoCard
    Implements ISupportsPrinting

    Private Loading As Boolean = True
    Private Obj As WorkerWageInfoReport


    Private Sub F_WorkerInfoCard_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub

    End Sub

    Private Sub F_WorkerInfoCard_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetFormLayout(Me)
    End Sub

    Private Sub F_WorkerInfoCard_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not WorkerWageInfoReport.CanGetObject Then
            MsgBox("Klaida. Vartotojo teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida")
            DisableAllControls(Me)
            Exit Sub
        End If

        If Not SetDataSources() Then Exit Sub

        SetFormLayout(Me)

        Dim dateFrom As Date = Today.AddMonths(-4)
        dateFrom = New Date(dateFrom.Year, dateFrom.Month, 1)
        Dim dateTo As Date = Today.AddMonths(-1)
        dateTo = New Date(dateTo.Year, dateTo.Month, Date.DaysInMonth(dateTo.Year, dateTo.Month))

        DateFromDateTimePicker.Value = dateFrom
        DateToDateTimePicker.Value = dateTo

    End Sub


    Private Sub RefreshLabourContractsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshLabourContractsButton.Click

        Dim currentWorker As HelperLists.PersonInfo = Nothing
        Try
            currentWorker = DirectCast(WorkerAccGridComboBox.SelectedValue, HelperLists.PersonInfo)
        Catch ex As Exception
        End Try
        If currentWorker Is Nothing OrElse currentWorker.IsEmpty Then
            MsgBox("Klaida. Nepasirinktas darbuotojas.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        Dim contractList As ShortLabourContractList
        Try
            contractList = LoadObject(Of ShortLabourContractList) _
                (Nothing, "GetList", True, currentWorker.ID)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        LabourContractComboBox.DataSource = Nothing
        LabourContractComboBox.DataSource = contractList
        If contractList.Count > 0 Then
            LabourContractComboBox.SelectedIndex = contractList.Count - 1
        Else
            LabourContractComboBox.SelectedIndex = -1
            MsgBox("Klaida. Šiam darbuotojui nėra registruotų darbo sutarčių.", _
                MsgBoxStyle.Exclamation, "Klaida.")
        End If

    End Sub

    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Dim nPersonID As Integer = 0
        Try
            nPersonID = DirectCast(WorkerAccGridComboBox.SelectedValue, HelperLists.PersonInfo).ID
        Catch ex As Exception
        End Try

        Dim nSerial As String = ""
        Dim nNumber As Integer = 0
        Try
            nSerial = DirectCast(LabourContractComboBox.SelectedItem, ShortLabourContract).Serial
            nNumber = DirectCast(LabourContractComboBox.SelectedItem, ShortLabourContract).Number
        Catch ex As Exception
        End Try
        
        Using bm As New BindingsManager(WageSheetBindingSource, ItemsBindingSource, _
            Nothing, False, True)

            Try
                Obj = LoadObject(Of WorkerWageInfoReport)(Nothing, "GetWorkerWageInfoReport", True, _
                    DateFromDateTimePicker.Value.Date, DateToDateTimePicker.Value.Date, _
                    nPersonID, nSerial, nNumber, IsConsolidatedCheckBox.Checked)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        ItemsDataGridView.Select()

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
        If Obj Is Nothing Then Exit Sub

        Using frm As New F_SendObjToEmail(Obj, 0)
            frm.ShowDialog()
        End Using

    End Sub

    Public Sub OnPrintClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, False, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, True, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function


    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Return False

        Try
            LoadPersonInfoListToGridCombo(WorkerAccGridComboBox, True, False, False, True)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class