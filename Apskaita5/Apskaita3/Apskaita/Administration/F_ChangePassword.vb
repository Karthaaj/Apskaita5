Imports AccControlsWinForms
Imports AccDataAccessLayer.Security
Public Class F_ChangePassword

    Private Sub F_ChangePassword_Activated(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Activated
        If Me.WindowState = FormWindowState.Maximized Then _
            Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub OkButtonI_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OkButtonI.Click

        If Not YesOrNo("Pakeisti slaptažodį?") Then Exit Sub


        Try
            Using busy As New StatusBusy
                CommandChangePassword.TheCommand(OldPasswordTextBox.Text, _
                    PasswordTextBox.Text, RepeatedPasswordTextBox.Text)
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try


        MsgBox("Slaptažodis sėkmingai pakeistas.", MsgBoxStyle.Information, "Info")

        Me.Hide()
        Me.Close()
    End Sub

End Class