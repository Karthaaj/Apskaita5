Imports AccControls.MessageBoxExLib

''' <summary>
''' Contains public shared functions for dialogs withing AccControls
''' </summary>
Public Module PublicFunctions

    ''' <summary>
    ''' Creates a modal dialog, asks user a question and provides a list of possible answers as buttons.
    ''' </summary>
    ''' <param name="Question">The text of the question of the dialog.</param>
    ''' <param name="buttons">List of possible answers as button params (used as New ButtonStructure())</param>
    ''' <returns>The text on the button pressed by user. </returns>
    Public Function Ask(ByVal Question As String, ByVal ParamArray buttons As ButtonStructure()) As String
        Dim msgBox As MessageBoxEx = MessageBoxExManager.CreateMessageBox(Nothing)
        msgBox.Caption = "Nepykite už durnus klausimus, bet..."
        msgBox.Text = Question
        For Each s As ButtonStructure In buttons
            Dim button As New MessageBoxExButton
            button.Text = s.ButtonText
            button.HelpText = s.Buttoncomment
            button.Value = s.ButtonText
            msgBox.AddButton(button)
        Next
        msgBox.Icon = MessageBoxExIcon.Question

        Return msgBox.Show
    End Function

    ''' <summary>
    ''' Provides a dialog with short/full info about the exception provided.
    ''' </summary>
    Public Sub ShowError(ByVal [Exception] As Exception)
        Dim msgBox As MessageBoxEx = MessageBoxExManager.CreateMessageBox(Nothing)
        msgBox.Caption = "Ir atleiskite nusidėjėliams už jų kaltes :)"

        msgBox.BaseException = GetBaseException([Exception])
        msgBox.Exception = [Exception]
        msgBox.Show()

    End Sub

    Private Function GetBaseException(ByVal ex As Exception) As Exception

        If ex Is Nothing Then Return ex

        Dim securityEx As System.Security.SecurityException = Nothing
        Dim simpleEx As Exception = Nothing
        Dim lastEx As Exception = Nothing

        GetExceptionCategories(ex, securityEx, simpleEx, lastEx)

        If Not securityEx Is Nothing Then
            Return securityEx
        ElseIf Not simpleEx Is Nothing Then
            Return simpleEx
        Else
            Return lastEx
        End If

    End Function

    Private Sub GetExceptionCategories(ByVal ex As Exception, _
        ByRef securityEx As System.Security.SecurityException, ByRef simpleEx As Exception, _
        ByRef lastEx As Exception)

        If TypeOf ex Is System.Security.SecurityException Then
            securityEx = ex
        ElseIf simpleEx Is Nothing AndAlso ex.GetType.FullName = GetType(Exception).FullName Then
            simpleEx = ex
        End If

        If ex.InnerException Is Nothing Then
            lastEx = ex
        Else
            GetExceptionCategories(ex.InnerException, securityEx, simpleEx, lastEx)
        End If

    End Sub


    ''' <summary>
    ''' Creates a modal dialog, asks user a question and provides an alternative Yes/No as buttons.
    ''' </summary>
    ''' <param name="Question">The text of the question (Yes or No) of the dialog.</param>
    ''' <returns>TRUE if user pressed "Taip" (i.e. "Yes") button, else - false. </returns>
    Public Function YesOrNo(ByVal Question As String) As Boolean
        Dim msgBox As MessageBoxEx = MessageBoxExManager.CreateMessageBox(Nothing)
        msgBox.Caption = "Nepykite už durnus klausimus, bet..."
        msgBox.Text = Question
        msgBox.AddButtons(Windows.Forms.MessageBoxButtons.YesNo)
        msgBox.Icon = MessageBoxExIcon.Question

        Dim result As String = msgBox.Show()

        Return (result = "Taip")
    End Function

    ''' <summary>
    ''' Returns path to the folder where the program (.exe) is executing.
    ''' </summary>
    Public Function AppPath() As String
        'parodo .exe failo absoliucia vieta kompe
        Return System.IO.Path.GetDirectoryName(Reflection.Assembly _
            .GetEntryAssembly().Location)
    End Function

    ''' <summary>
    ''' Adds DataGridViewColumnSelector control to the DataGridView 
    '''(allows setting the visibility of columns).
    ''' </summary>
    ''' <param name="nDataGridView">The DataGridView which 
    '''the DataGridViewColumnSelector control should be added to.</param>
    Public Sub AddDGVColumnSelector(ByRef nDataGridView As DataGridView)
        Dim cs As DataGridViewColumnSelector = _
            New DataGridViewColumnSelector(nDataGridView)
        cs.MaxHeight = CInt(2 * Screen.PrimaryScreen.Bounds.Height / 3)
        cs.Width = 200
    End Sub

    ''' <summary>
    ''' Disables all controls in the target form.
    ''' </summary>
    Public Sub DisableAllControls(ByRef TargetForm As Control)
        Dim ignoreTypes As Type() = {GetType(Windows.Forms.GroupBox), GetType(Windows.Forms.Label), _
            GetType(Windows.Forms.LinkLabel), GetType(Windows.Forms.Panel), _
            GetType(Windows.Forms.SplitContainer), GetType(Windows.Forms.TabControl), _
            GetType(Windows.Forms.TableLayoutPanel), GetType(Windows.Forms.TabPage), _
            GetType(Windows.Forms.Timer), GetType(Windows.Forms.TableLayoutPanel), _
            GetType(Windows.Forms.FlowLayoutPanel), GetType(System.Windows.Forms.HScrollBar), _
            GetType(System.Windows.Forms.VScrollBar)}
        For Each ctrl As Control In TargetForm.Controls
            If Array.IndexOf(ignoreTypes, ctrl.GetType) < 0 Then
                Try
                    If TypeOf ctrl Is DataGridView Then
                        DirectCast(ctrl, DataGridView).ReadOnly = True
                        DirectCast(ctrl, DataGridView).AllowUserToAddRows = False
                        DirectCast(ctrl, DataGridView).AllowUserToDeleteRows = False
                    Else
                        DirectCast(ctrl, Object).Readonly = True
                    End If
                Catch ex As Exception
                    ctrl.Enabled = False
                End Try
            End If
            If ctrl.Controls.Count > 0 Then DisableAllControls(ctrl)
        Next
    End Sub

    ''' <summary>
    ''' Clones an object by using the <see cref="BinaryFormatter" />.
    ''' </summary>
    ''' <param name="obj">The object to clone.</param>
    ''' <remarks>
    ''' The object to be cloned must be serializable.
    ''' </remarks>
    Public Function Clone(ByVal obj As Object) As Object

        Using buffer As New IO.MemoryStream()
            Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()

            formatter.Serialize(buffer, obj)
            buffer.Position = 0
            Dim temp As Object = formatter.Deserialize(buffer)
            Return temp
        End Using

    End Function

    ''' <summary>
    ''' Gets a rounded value of d using Asymmetric Arithmetic Rounding algorithm
    ''' </summary>
    Friend Function CRound(ByVal d As Double, ByVal r As Integer) As Double
        Dim i As Long = CLng(Math.Floor(d * Math.Pow(10, r)))
        If i + 0.5 > CType(d * Math.Pow(10, r), Decimal) Then
            Return i / Math.Pow(10, r)
        Else
            Return (i + 1) / Math.Pow(10, r)
        End If
    End Function
    Friend Function CRound(ByVal d As Double) As Double
        Return CRound(d, 2)
    End Function

    ''' <summary>
    ''' Adds LocalReport LR with the datasource RDS.
    ''' </summary>
    ''' <param name="LR">The LocalReport to add with datasources.</param>
    ''' <param name="RDS">Datasource of type ReportData.</param>
    ''' <param name="NumberOfTablesInUse">Number of details tebles in use by the report.</param>
    ''' <param name="ReportFileStream">Report definition as Byte().</param>
    ''' <param name="ReportFileName">Report file name (if ReportFileStream is nothing).</param>
    Public Sub AddReportWithDatasource(ByRef LR As Microsoft.Reporting.WinForms.LocalReport, _
        ByVal RDS As ReportData, ByVal NumberOfTablesInUse As Byte, _
        ByVal ReportFileStream As Byte(), ByVal ReportFileName As String)

        If Not ReportFileStream Is Nothing AndAlso ReportFileStream.Length < 100 Then _
            ReportFileStream = Nothing
        If ReportFileName Is Nothing Then ReportFileName = ""

        If ReportFileStream Is Nothing AndAlso String.IsNullOrEmpty(ReportFileName.Trim) Then _
            Throw New Exception("Klaida. Nenurodyta nei forma, nei jos failo pavadinimas.")

        If NumberOfTablesInUse < 0 Then NumberOfTablesInUse = 0
        Using RD As New ReportData
            If NumberOfTablesInUse > RD.Tables.Count - 2 Then _
                NumberOfTablesInUse = RD.Tables.Count - 2
        End Using

        Dim TblGeneral As New BindingSource(RDS, "TableGeneral")
        Dim NewSourceGeneral As New Microsoft.Reporting.WinForms.ReportDataSource()
        NewSourceGeneral.Value = TblGeneral
        NewSourceGeneral.Name = "ReportData_TableGeneral"
        LR.DataSources.Add(NewSourceGeneral)

        Dim TblCompany As New BindingSource(RDS, "TableCompany")
        Dim NewSourceCompany As New Microsoft.Reporting.WinForms.ReportDataSource
        NewSourceCompany.Value = TblCompany
        NewSourceCompany.Name = "ReportData_TableCompany"
        LR.DataSources.Add(NewSourceCompany)

        For i As Integer = 1 To NumberOfTablesInUse

            Dim Tbl As New BindingSource(RDS, "Table" & i.ToString)
            Dim NewSource As New Microsoft.Reporting.WinForms.ReportDataSource
            NewSource.Value = Tbl
            NewSource.Name = "ReportData_Table" & i.ToString
            LR.DataSources.Add(NewSource)

        Next

        If Not ReportFileStream Is Nothing Then
            LR.LoadReportDefinition(New IO.MemoryStream(ReportFileStream))
        Else
            LR.ReportPath = ReportFileName.Trim
        End If

    End Sub

    Public Sub DisposeLocalReportDatasources(ByRef LR As Microsoft.Reporting.WinForms.LocalReport)

        For Each ds As Microsoft.Reporting.WinForms.ReportDataSource In LR.DataSources
            CType(ds.Value, BindingSource).Dispose()
        Next
        LR.DataSources.Clear()

    End Sub

    Public Sub PrintReport(ByVal ShowPreview As Boolean, ByVal MdiParentForm As Form, _
        ByVal RDS As ReportData, ByVal NumberOfTablesInUse As Integer, _
        ByVal ReportDefinitionStream As Byte(), ByVal ReportFileName As String, _
        ByVal DefaultReportName As String, ByVal PrinterName As String)

        If Not ReportDefinitionStream Is Nothing AndAlso _
            ReportDefinitionStream.Length < 100 Then ReportDefinitionStream = Nothing
        If ReportFileName Is Nothing Then ReportFileName = ""
        If DefaultReportName Is Nothing Then DefaultReportName = ""

        If ReportDefinitionStream Is Nothing AndAlso String.IsNullOrEmpty(ReportFileName.Trim) Then _
            Throw New Exception("Klaida. Nenurodyta nei ataskaitos forma, nei jos failo pavadinimas.")

        If ShowPreview Then

            Dim frm As AccControls.F_PrintReport

            If ReportDefinitionStream Is Nothing Then

                frm = New AccControls.F_PrintReport(RDS, NumberOfTablesInUse, _
                    ReportFileName, DefaultReportName)

            Else

                frm = New AccControls.F_PrintReport(RDS, NumberOfTablesInUse, _
                    ReportDefinitionStream, DefaultReportName)

            End If

            frm.MdiParent = MdiParentForm
            frm.Show()

        Else

            Dim NumberOfCopies As Integer = 1

            If String.IsNullOrEmpty(PrinterName.Trim) Then
                Using dlgPrint As New PrintDialog
                    If dlgPrint.ShowDialog() <> DialogResult.OK Then Exit Sub
                    PrinterName = dlgPrint.PrinterSettings.PrinterName
                    NumberOfCopies = dlgPrint.PrinterSettings.Copies
                End Using
            End If

            If ReportDefinitionStream Is Nothing Then
                Using PrintObject As New PrintReportInternal(RDS, NumberOfTablesInUse, ReportFileName)
                    PrintObject.Run(NumberOfCopies, PrinterName)
                End Using
            Else
                Using PrintObject As New PrintReportInternal(RDS, NumberOfTablesInUse, ReportDefinitionStream)
                    PrintObject.Run(NumberOfCopies, PrinterName)
                End Using
            End If

        End If

    End Sub

    Public Function GetPdfStream(ByVal RDS As ReportData, ByVal NumberOfTablesInUse As Integer, _
        ByVal ReportDefinitionStream As Byte(), ByVal ReportFileName As String) As Byte()

        Dim result As Byte() = Nothing
        If ReportDefinitionStream Is Nothing Then
            Using PrintObject As New PrintReportInternal(RDS, NumberOfTablesInUse, ReportFileName)
                result = PrintObject.GetPDFStream
            End Using
        Else
            Using PrintObject As New PrintReportInternal(RDS, NumberOfTablesInUse, ReportDefinitionStream)
                result = PrintObject.GetPDFStream
            End Using
        End If

        Return result

    End Function

    Public Function SaveReportToPDF(ByVal RDS As ReportData, ByVal NumberOfTablesInUse As Integer, _
        ByVal ReportDefinitionStream As Byte(), ByVal ReportFileName As String, _
        ByVal PdfFileName As String) As String

        If Not ReportDefinitionStream Is Nothing AndAlso _
            ReportDefinitionStream.Length < 100 Then ReportDefinitionStream = Nothing
        If ReportFileName Is Nothing Then ReportFileName = ""

        If ReportDefinitionStream Is Nothing AndAlso String.IsNullOrEmpty(ReportFileName.Trim) Then _
            Throw New Exception("Klaida. Nenurodyta nei ataskaitos forma, nei jos failo pavadinimas.")

        If PdfFileName Is Nothing OrElse String.IsNullOrEmpty(PdfFileName.Trim) Then
            PdfFileName = IO.Path.GetTempPath & "\Report.pdf"
        Else
            PdfFileName = IO.Path.GetTempPath & "\" & PdfFileName.Trim & ".pdf"
        End If

        If IO.File.Exists(PdfFileName) Then IO.File.Delete(PdfFileName)

        Dim bytes As Byte() = GetPdfStream(RDS, NumberOfTablesInUse, ReportDefinitionStream, ReportFileName)

        Dim fs As New IO.FileStream(PdfFileName, IO.FileMode.Create)
        fs.Write(bytes, 0, bytes.Length)
        fs.Close()

        Return PdfFileName

    End Function

    ''' <summary>
    ''' Gets a substring from Tab (CHR9) delimited string.
    ''' </summary>
    ''' <param name="SourceString">Tab (CHR9) delimited string.</param>
    ''' <param name="index">Number (index) of substring to retrieve.</param>
    Public Function GetElement(ByVal SourceString As String, ByVal index As Integer) As String
        Dim SubStrings As String() = SourceString.Split(Chr(9))
        If SubStrings.Length > index Then
            Return SubStrings(index)
        Else
            Return ""
        End If
    End Function

    Public Function SafeConvertStrToInt(ByVal IntegerString As String, ByVal MinValue As Integer) As Integer
        If Integer.TryParse(IntegerString, New Integer) Then Return CInt(IntegerString)
        Return MinValue
    End Function


End Module
