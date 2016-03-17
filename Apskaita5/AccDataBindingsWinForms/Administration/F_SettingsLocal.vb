Imports AccDataAccessLayer.Security
Imports AccControlsWinForms
Imports AccDataBindingsWinForms.Settings
Imports System.Drawing

Friend Class F_SettingsLocal
    Implements ISingleInstanceForm

    Private WithEvents _FormManager As CslaActionExtenderEditForm(Of LocalSettings)
    Private _ListViewManager As DataListViewEditControlManager(Of LocalUser)


    Private Sub F_SettingsLocal_FormClosed(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Not _FormManager.DataSource Is Nothing Then
            Try
                DirectCast(CurrentMdiParent, Object).ToolStrip1.Visible = _FormManager.DataSource.ShowToolStrip
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Nustatymai_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            _ListViewManager = New DataListViewEditControlManager(Of LocalUser) _
                (LocalUsersDataListView, Nothing, AddressOf OnItemsDelete, _
                 AddressOf OnItemAdd, Nothing)

            Dim connTypeComboBox As New ComboBox
            connTypeComboBox.DataSource = AccDataAccessLayer.GetConnectionTypeDataSource(False)
            _ListViewManager.AddCustomEditControl("ConnectionTechnologyHumanReadable", connTypeComboBox)

            Dim sqlServerTypeComboBox As New ComboBox
            sqlServerTypeComboBox.DataSource = AccDataAccessLayer.GetSqlServerTypeDataSource(False, True)
            _ListViewManager.AddCustomEditControl("SqlServerTypeHumanReadable", sqlServerTypeComboBox)

            SetupDefaultControls(Of LocalSettings)(Me, LocalSettingsBindingSource)

            Dim dataSource As LocalSettings = LocalSettings.GetLocalSettings()

            _FormManager = New CslaActionExtenderEditForm(Of LocalSettings)(Me, LocalSettingsBindingSource, _
                dataSource, Nothing, OkButtonI, ApplyButtonI, CancelButtonI, Nothing, ProgressFiller1)

            _FormManager.ManageDataListViewStates(LocalUsersDataListView)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        ConfigureEmailControls()

    End Sub


    Private Sub OnItemsDelete(ByVal items As LocalUser())
        If items Is Nothing OrElse items.Length < 1 OrElse _FormManager.DataSource Is Nothing Then Exit Sub
        For Each item As LocalUser In items
            _FormManager.DataSource.LocalUsers.Remove(item)
        Next
    End Sub

    Private Sub OnItemAdd()
        If _FormManager.DataSource Is Nothing Then Exit Sub
        _FormManager.DataSource.LocalUsers.AddNew()
    End Sub

    Private Sub Open_Image_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles Open_Image.Click

        Using opf As New OpenFileDialog
            opf.Multiselect = False
            If opf.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            If IO.File.Exists(opf.FileName) Then
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
            _FormManager.DataSource.SignInvoicesWithLocalUserSignature = SignInvoicesWithLocalUserSignatureCheckBox1.Checked
        ElseIf sender Is SignInvoicesWithCompanySignatureCheckBox1 Then
            _FormManager.DataSource.SignInvoicesWithCompanySignature = SignInvoicesWithCompanySignatureCheckBox1.Checked
        ElseIf sender Is DontSignInvoicesCheckBox Then
            _FormManager.DataSource.DontSignInvoices = DontSignInvoicesCheckBox.Checked
        ElseIf sender Is SignInvoicesWithRemoteUserSignatureCheckBox1 Then
            _FormManager.DataSource.SignInvoicesWithRemoteUserSignature = SignInvoicesWithRemoteUserSignatureCheckBox1.Checked
        End If

    End Sub

    Private Sub EmailControls_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles UseDefaultEmailClientCheckBox.CheckedChanged, _
        UseEmbededEmailClientCheckBox.CheckedChanged, UseAuthForEmailCheckBox1.CheckedChanged
        If sender Is UseDefaultEmailClientCheckBox Then
            _FormManager.DataSource.UseDefaultEmailClient = UseDefaultEmailClientCheckBox.Checked
        ElseIf sender Is UseEmbededEmailClientCheckBox Then
            _FormManager.DataSource.UseEmbededEmailClient = UseEmbededEmailClientCheckBox.Checked
        ElseIf sender Is UseAuthForEmailCheckBox1 Then
            _FormManager.DataSource.UseAuthForEmail = UseAuthForEmailCheckBox1.Checked
        End If
        ConfigureEmailControls()
    End Sub

    Private Sub ShowToolStripCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ShowToolStripCheckBox1.CheckedChanged
        If Not _FormManager.DataSource Is Nothing Then
            Try
                DirectCast(CurrentMdiParent, Object).ToolStrip1.Visible = ShowToolStripCheckBox1.Checked
            Catch ex As Exception
            End Try
        End If
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

End Class