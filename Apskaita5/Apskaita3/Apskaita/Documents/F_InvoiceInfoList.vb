Imports ApskaitaObjects.HelperLists
Imports ApskaitaObjects.ActiveReports
Public Class F_InvoiceInfoList
    Implements ISupportsPrinting

    Private Obj As InvoiceInfoItemList
    Private Loading As Boolean = True
    Private PrintDropDown As Windows.Forms.ToolStripDropDown = Nothing
    Private EmailDropDown As Windows.Forms.ToolStripDropDown = Nothing


    Private Sub F_InvoiceInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub

    End Sub

    Private Sub F_InvoiceInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(InvoiceInfoItemListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_InvoiceInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        DateFromDateTimePicker.Value = Today.Subtract(New TimeSpan(30, 0, 0, 0))
        RegisterTypeComboBox.SelectedIndex = 0

        Dim CM As New ContextMenu()
        Dim CMItem1 As New MenuItem("v. 1", AddressOf ExportFFDataButton_Click)
        CM.MenuItems.Add(CMItem1)

        AddDGVColumnSelector(InvoiceInfoItemListDataGridView)

        SetDataGridViewLayOut(InvoiceInfoItemListDataGridView)
        SetFormLayout(Me)

    End Sub


    Private Sub InvoiceInfoItemListDataGridView_CellDoubleClick(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles InvoiceInfoItemListDataGridView.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub

        ' get the selected book entry
        Dim tmp As InvoiceInfoItem = Nothing
        Try
            tmp = CType(InvoiceInfoItemListDataGridView.Rows(e.RowIndex).DataBoundItem, InvoiceInfoItem)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        If tmp Is Nothing Then
            MsgBox("Klaida. Nepavyko nustatyti pasirinktos sąskaitos.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        Dim SelectedObjectEditFormType As Type
        If Obj.InfoType = InvoiceInfoType.InvoiceMade Then
            SelectedObjectEditFormType = GetType(F_InvoiceMade)
        Else
            SelectedObjectEditFormType = GetType(F_InvoiceReceived)
        End If
        For Each frm As Form In MDIParent1.MdiChildren
            If frm.GetType Is SelectedObjectEditFormType AndAlso _
                DirectCast(frm, IObjectEditForm).ObjectID = tmp.ID Then
                frm.Activate()
                Exit Sub
            End If
        Next

        ' ask what to do
        Dim ats As String
        ats = Ask("", New ButtonStructure("Taisyti", "Keisti sąskaitos faktūros duomenis."), _
            New ButtonStructure("Kopijuoti", "Kopijuoti sąskaitą faktūrą."), _
            New ButtonStructure("Ištrinti", "Pašalinti sąskaitos faktūros duomenis iš duomenų bazės."), _
            New ButtonStructure("Atšaukti", "Nieko nedaryti."))

        If ats <> "Taisyti" AndAlso ats <> "Ištrinti" AndAlso ats <> "Kopijuoti" Then Exit Sub

        If ats = "Ištrinti" Then

            If Not YesOrNo("Ar tikrai norite pašalinti dokumento duomenis iš duomenų bazės?") Then Exit Sub

            Using busy As New StatusBusy
                Try
                    If Obj.InfoType = InvoiceInfoType.InvoiceMade Then
                        Documents.InvoiceMade.DeleteInvoiceMade(tmp.ID)
                    Else
                        Documents.InvoiceReceived.DeleteInvoiceReceived(tmp.ID)
                    End If
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
            End Using

            If Not YesOrNo("Dokumento duomenys sėkmingai pašalinti iš įmonės duomenų bazės. " _
                & "Atnaujinti sąrašą?") Then Exit Sub

            Using bm As New BindingsManager(InvoiceInfoItemListBindingSource, _
                Nothing, Nothing, False, True)

                Try
                    Obj = LoadObject(Of InvoiceInfoItemList)(Nothing, "GetList", True, _
                        Obj.InfoType, Obj.DateFrom, Obj.DateTo, Obj.Person)
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try

                bm.SetNewDataSource(Obj.GetSortedList)

            End Using

        ElseIf ats = "Taisyti" Then

            MDIParent1.LaunchForm(SelectedObjectEditFormType, False, False, tmp.ID, tmp.ID)

        Else

            MDIParent1.LaunchForm(SelectedObjectEditFormType, False, False, tmp.ID, tmp.ID, True)

        End If

    End Sub

    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        If RegisterTypeComboBox.SelectedIndex < 0 OrElse RegisterTypeComboBox.SelectedItem Is Nothing Then
            MsgBox("Klaida. Nepasirinktas registro tipas.", MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If
        Dim nRegisterType As InvoiceInfoType
        Try
            nRegisterType = ConvertEnumHumanReadable(Of InvoiceInfoType)(RegisterTypeComboBox.SelectedItem)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        Using bm As New BindingsManager(InvoiceInfoItemListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of InvoiceInfoItemList)(Nothing, "GetList", True, _
                    nRegisterType, DateFromDateTimePicker.Value.Date, _
                    DateToDateTimePicker.Value.Date, LoadObjectFromCombo(Of PersonInfo) _
                    (PersonAccGridComboBox, ""))
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj.GetSortedList)

        End Using

        CheckDataGridViewColumn.Visible = (Obj.InfoType = InvoiceInfoType.InvoiceMade)

        InvoiceInfoItemListDataGridView.Select()

    End Sub

    Private Sub ExportFFDataButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ExportFFDataButton.Click

        If Obj Is Nothing Then Exit Sub

        Dim FileName As String = ""

        Using SFD As New SaveFileDialog
            SFD.Filter = "FFData failai|*.ffdata|Visi failai|*.*"
            SFD.CheckFileExists = False
            SFD.AddExtension = True
            SFD.DefaultExt = ".ffdata"
            If SFD.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            FileName = SFD.FileName.Trim
        End Using

        If String.IsNullOrEmpty(FileName) Then Exit Sub

        Try

            Obj.SaveToFFData(FileName, IIf(sender.Text.ToString.Trim.ToLower.Contains("1"), 1, 2))

            If YesOrNo("Failas sėkmingai išsaugotas. Atidaryti?") Then _
                System.Diagnostics.Process.Start(FileName)

        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub


    Public Function GetMailDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetMailDropDownItems

        If EmailDropDown Is Nothing Then
            EmailDropDown = New ToolStripDropDown
            EmailDropDown.Items.Add("Siųsti pasirinktas", Nothing, AddressOf OnMailClick)
            EmailDropDown.Items.Add("Siųsti pasirinktas lietuvių klb.", Nothing, AddressOf OnMailClick)
        End If

        Return EmailDropDown

    End Function

    Public Function GetPrintDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintDropDownItems

        If PrintDropDown Is Nothing Then
            PrintDropDown = New ToolStripDropDown
            PrintDropDown.Items.Add("Spausdinti pasirinktas", Nothing, AddressOf OnPrintClick)
            PrintDropDown.Items.Add("Spausdinti pasirinktas Lietuvių klb.", Nothing, AddressOf OnPrintClick)

        End If

        Return PrintDropDown

    End Function

    Public Function GetPrintPreviewDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintPreviewDropDownItems
        Return Nothing
    End Function

    Public Sub OnMailClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnMailClick

        If Obj Is Nothing Then Exit Sub

        If sender.Text.ToLower.Contains("pasirinktas") Then

            If Obj.InfoType <> InvoiceInfoType.InvoiceMade Then Exit Sub

            Dim SelectedInvoices As Documents.InvoiceMadeList
            Dim FormVersionToUse As Integer

            If sender.Text.ToLower.Contains("lietuvių") Then
                FormVersionToUse = 1
            Else
                FormVersionToUse = 0
            End If

            Try
                SelectedInvoices = LoadObject(Of Documents.InvoiceMadeList)(Nothing, "GetList", _
                    True, GetCheckedInvoicesIds(False))
                Using busy As New StatusBusy

                    For Each item As Documents.InvoiceMade In SelectedInvoices

                        Dim MailSubject As String = "Saskaita - faktura (invoice) " & _
                        item.Date.ToShortDateString & " Nr. " & item.Serial & item.FullNumber

                        SendObjectToEmail(item, item.Payer.Email, MailSubject, _
                            MyCustomSettings.EmailMessageText, FormVersionToUse, "Invoice_" & _
                            item.Date.Year.ToString & "_" & item.Date.Month.ToString & "_" & _
                            item.Date.Day.ToString & "_" & item.FullNumber)

                    Next

                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        Else

            Using frm As New F_SendObjToEmail(Obj, 0)
                frm.ShowDialog()
            End Using

        End If

    End Sub

    Public Sub OnPrintClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintClick

        If Obj Is Nothing Then Exit Sub

        If sender.Text.ToLower.Contains("pasirinktas") Then

            If Obj.InfoType <> InvoiceInfoType.InvoiceMade Then Exit Sub

            Dim mPrinterName As String
            Using dlgPrint As New PrintDialog
                If dlgPrint.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then Exit Sub
                mPrinterName = dlgPrint.PrinterSettings.PrinterName
            End Using

            Dim SelectedInvoices As Documents.InvoiceMadeList
            Dim FormVersionToUse As Integer
            If sender.Text.ToLower.Contains("lietuvių") Then
                FormVersionToUse = 1
            Else
                FormVersionToUse = 0
            End If
            Try
                SelectedInvoices = LoadObject(Of Documents.InvoiceMadeList)(Nothing, "GetList", _
                    True, GetCheckedInvoicesIds(False))
                Using busy As New StatusBusy
                    For Each item As Documents.InvoiceMade In SelectedInvoices
                        PrintObject(item, False, FormVersionToUse, mPrinterName)
                    Next
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        Else

            Try
                PrintObject(Obj, False, 0)
            Catch ex As Exception
                ShowError(ex)
            End Try

        End If

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
            RegisterTypeComboBox.DataSource = GetEnumValuesHumanReadableList(GetType(InvoiceInfoType), False)
            LoadPersonInfoListToGridCombo(PersonAccGridComboBox, True, True, True, False)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Private Function GetCheckedInvoicesIds(ByVal ThrowIfNoClientEmail As Boolean) As Integer()

        Dim result As New List(Of Integer)

        For Each dgr As DataGridViewRow In InvoiceInfoItemListDataGridView.Rows

            If InvoiceInfoItemListDataGridView.Item(CheckDataGridViewColumn.Index, dgr.Index).Value AndAlso _
                Not dgr.DataBoundItem Is Nothing AndAlso CType(dgr.DataBoundItem, InvoiceInfoItem).ID > 0 Then

                If ThrowIfNoClientEmail AndAlso String.IsNullOrEmpty( _
                    CType(dgr.DataBoundItem, InvoiceInfoItem).PersonEmail.Trim) Then _
                        Throw New Exception("Klaida. Nenurodytas kliento '" & _
                        CType(dgr.DataBoundItem, InvoiceInfoItem).PersonName & "' e-paštas.")

                result.Add(CType(dgr.DataBoundItem, InvoiceInfoItem).ID)

            End If

        Next

        If result.Count < 1 Then Throw New Exception("Klaida. Nepasirinkta nė viena sąskaita.")

        Return result.ToArray

    End Function

End Class