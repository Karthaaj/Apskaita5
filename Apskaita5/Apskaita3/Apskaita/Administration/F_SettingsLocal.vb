Imports System.Data
Imports System.IO
Imports System.ComponentModel
Imports AccDataAccessLayer

Public Class F_SettingsLocal

    Private Obj As LocalSettings
    Private Loading As Boolean = True


    Private Sub F_SettingsLocal_Activated(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

    End Sub

    Private Sub Nustatymai_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) _
        Handles Me.FormClosing

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti programos nustatymus?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then
                e.Cancel = True
                Exit Sub
            End If
            If answ = "Taip" Then
                If Not SaveObj() Then
                    e.Cancel = True
                    Exit Sub
                End If
            Else
                CancelObj()
            End If
        End If

        GetDataGridViewLayOut(LocalUsersDataGridView)
        GetFormLayout(Me)

        If Not Obj Is Nothing Then MDIParent1.ToolStrip1.Visible = Obj.ShowToolStrip

    End Sub

    Private Sub Nustatymai_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        Try
            Obj = LoadObject(Of LocalSettings)(Nothing, "GetLocalSettings", False)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        LocalSettingsBindingSource.DataSource = Obj

        AddDGVColumnSelector(LocalUsersDataGridView)

        SetDataGridViewLayOut(LocalUsersDataGridView)
        SetFormLayout(Me)

        ConfigureEmailControls()

    End Sub

    


    Private Sub OkButton_Click(ByVal sender As System.Object, _
       ByVal e As System.EventArgs) Handles OkButtonI.Click

        If Not SaveObj() Then Exit Sub
        Me.Hide()
        Me.Close()

    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButtonI.Click
        If Not SaveObj() Then Exit Sub
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CancelButtonI.Click
        CancelObj()
    End Sub


    Private Sub Open_Image_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles Open_Image.Click

        Using opf As New OpenFileDialog
            opf.Multiselect = False
            If opf.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            If File.Exists(opf.FileName) Then
                Try
                    UserSignaturePictureBox1.Image = DirectCast(Bitmap.FromFile(opf.FileName).Clone, Bitmap)
                Catch ex As Exception
                    ShowError(New Exception("Klaida. Neatpažintas paveikslėlio formatas.", ex))
                End Try
            End If
        End Using

    End Sub

    Private Sub ClearImage_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ClearImage.Click
        UserSignaturePictureBox1.Image = Nothing
    End Sub


    Private Sub SignInvoicesWithLocalUserSignatureCheckBox1_CheckedChanged(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles SignInvoicesWithLocalUserSignatureCheckBox1.CheckedChanged, _
        SignInvoicesWithCompanySignatureCheckBox1.CheckedChanged, DontSignInvoicesCheckBox.CheckedChanged, _
        SignInvoicesWithRemoteUserSignatureCheckBox1.CheckedChanged

        If sender Is SignInvoicesWithLocalUserSignatureCheckBox1 Then
            Obj.SignInvoicesWithLocalUserSignature = SignInvoicesWithLocalUserSignatureCheckBox1.Checked
        ElseIf sender Is SignInvoicesWithCompanySignatureCheckBox1 Then
            Obj.SignInvoicesWithCompanySignature = SignInvoicesWithCompanySignatureCheckBox1.Checked
        ElseIf sender Is DontSignInvoicesCheckBox Then
            Obj.DontSignInvoices = DontSignInvoicesCheckBox.Checked
        ElseIf sender Is SignInvoicesWithRemoteUserSignatureCheckBox1 Then
            Obj.SignInvoicesWithRemoteUserSignature = SignInvoicesWithRemoteUserSignatureCheckBox1.Checked
        End If

    End Sub

    Private Sub EmailControls_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles UseDefaultEmailClientCheckBox.CheckedChanged, _
        UseEmbededEmailClientCheckBox.CheckedChanged, UseAuthForEmailCheckBox1.CheckedChanged
        If sender Is UseDefaultEmailClientCheckBox Then
            Obj.UseDefaultEmailClient = UseDefaultEmailClientCheckBox.Checked
        ElseIf sender Is UseEmbededEmailClientCheckBox Then
            Obj.UseEmbededEmailClient = UseEmbededEmailClientCheckBox.Checked
        ElseIf sender Is UseAuthForEmailCheckBox1 Then
            Obj.UseAuthForEmail = UseAuthForEmailCheckBox1.Checked
        End If
        ConfigureEmailControls()
    End Sub

    Private Sub ShowToolStripCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ShowToolStripCheckBox1.CheckedChanged
        MDIParent1.ToolStrip1.Visible = ShowToolStripCheckBox1.Checked
    End Sub


    Private Sub ConfigureEmailControls()
        ShowDefaultMailClientWindowCheckBox1.Enabled = UseDefaultEmailClientCheckBox.Checked
        UserEmailTextBox1.ReadOnly = UseDefaultEmailClientCheckBox.Checked
        SmtpServerTextBox1.ReadOnly = UseDefaultEmailClientCheckBox.Checked
        SmtpPortTextBox1.ReadOnly = UseDefaultEmailClientCheckBox.Checked
        UseSslForEmailCheckBox1.Enabled = Not UseDefaultEmailClientCheckBox.Checked
        UseAuthForEmailCheckBox1.Enabled = Not UseDefaultEmailClientCheckBox.Checked
        UserEmailAccountTextBox1.ReadOnly = UseDefaultEmailClientCheckBox.Checked _
            OrElse Not UseAuthForEmailCheckBox1.Checked
        UserEmailPasswordTextBox1.ReadOnly = UseDefaultEmailClientCheckBox.Checked _
            OrElse Not UseAuthForEmailCheckBox1.Checked
    End Sub


    Private Function SaveObj() As Boolean

        If Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllErrors, MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf _
                & Obj.GetAllWarnings & vbCrLf
        Else
            Question = ""
        End If
        Question = Question & "Ar tikrai norite pakeisti programos nustatymus?"

        If Not YesOrNo(Question) Then Return False

        Using bm As New BindingsManager(LocalSettingsBindingSource, _
            LocalUsersBindingSource, Nothing, True, False)

            Try
                Obj = LoadObject(Of LocalSettings)(Obj, "Save", False)
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        MsgBox("Programos nustatymai sėkmingai pakeisti.", MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        If Obj Is Nothing OrElse Obj.IsNew OrElse Not Obj.IsDirty Then Exit Sub
        Using bm As New BindingsManager(LocalSettingsBindingSource, _
            LocalUsersBindingSource, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        DataGridViewTextBoxColumn8.DataSource = AccDataAccessLayer.GetConnectionTypeDataSource(False)
        DataGridViewTextBoxColumn9.DataSource = AccDataAccessLayer.GetSqlServerTypeDataSource(False, True)

        Try

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class