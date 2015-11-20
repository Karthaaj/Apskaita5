Imports System.io
Imports AccDataAccessLayer.Security

Public Class F_BackUp
    Private CharSetDirPath As String = ""
    Private BaseDirPath As String = ""
    Private DefaultFileExtension As String = "sql"


    Private Sub F_BackUp_Activated(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles MyBase.Activated
        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub BackUp_Load(ByVal sender As Object, _
            ByVal e As System.EventArgs) Handles Me.Load

        SaveAsFileNameTextBox.Text = ""
        OpenFileNameTextbox.Text = ""

        Dim CurrentIdentity As AccIdentity = GetCurrentIdentity()

        If Not CurrentIdentity.IsLocalUser Then
            If CurrentIdentity.ConnectionType <> AccDataAccessLayer.ConnectionType.Local _
                OrElse (CurrentIdentity.ConnectionType = AccDataAccessLayer.ConnectionType.Local _
                AndAlso CurrentIdentity.SQLServer.Trim.ToLower <> "localhost" AndAlso _
                CurrentIdentity.SQLServer.Trim.ToLower <> "127.0.0.1") Then

                MsgBox("Klaida. Duomenų bazių atsargines kopijas galima kurti/atkurti " _
                    & "tik tame pačiame kompiuteryje, kuriame instaliuotas serveris.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub

            ElseIf CurrentIdentity.ConnectionType = AccDataAccessLayer.ConnectionType.Local _
                AndAlso (CurrentIdentity.SQLServer.Trim.ToLower = "localhost" OrElse _
                CurrentIdentity.SQLServer.Trim.ToLower = "127.0.0.1") AndAlso _
                CurrentIdentity.Name.Trim.ToLower <> GetRootName().Trim.ToLower Then

                MsgBox("Klaida. Duomenų bazių atsargines kopijas gali kurti/atkurti " _
                    & "tik pagrindinis (root) vartotojas.", MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub

            End If
        End If

        If CurrentIdentity.SqlServerType = AccDataAccessLayer.SqlServerType.MySQL Then

            Using busy As New StatusBusy
                Try
                    Dim tmp As CharSetDir
                    tmp = CharSetDir.GetCharSetDir
                    CharSetDirPath = tmp.CharSetDir
                    BaseDirPath = tmp.BaseDir
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                End Try
            End Using

            If StringIsNullOrEmpty(CharSetDirPath) Then

                If Not RequestUserFolder("Klaida. Nenustatytas MySQL serverio CharSet folderis.", _
                    "MySQL CharSet Folderis", "Klaida. Dėl trūkstamų duomenų apie mysql instaliacijos " _
                    & "vietą nebus galimybės atkurti duomenų bazę.", "", "", CharSetDirPath) Then
                    OpenFileNameTextbox.Enabled = False
                    OpenFileButton2.Enabled = False
                    RecoverFromFileButton.Enabled = False
                End If

            End If

            If StringIsNullOrEmpty(BaseDirPath) Then

                If Not RequestUserFolder("Klaida. Nenustatytas MySQL serverio instaliacijos folderis.", _
                    "MySQL CharSet Folderis", "Klaida. Dėl trūkstamų duomenų apie mysql instaliacijos " _
                    & "vietą nebus galimybės nei atkurti duomenų bazę, nei sukurti atsarginę kopiją.", _
                    "bin\mysql.exe", "Klaida. Pasirinktas folderis nėra MySQL instaliacija. Dėl trūkstamų " _
                    & "duomenų apie mysql instaliacijos vietą nebus galimybės nei atkurti duomenų bazę, " _
                    & "nei sukurti atsarginę kopiją.", BaseDirPath) Then
                    DisableAllControls(Me)
                End If

            End If

            If Not StringIsNullOrEmpty(CharSetDirPath) Then
                CharSetDirPath = Chr(34) & CharSetDirPath & Chr(34)
            End If

        End If

        DefaultFileExtension = "sql"

        If IsLoggedInDB() Then
            SaveAsFileNameTextBox.Text = IO.Path.Combine(Environment.GetFolderPath( _
                Environment.SpecialFolder.Desktop), GetDefaultFileName())
        Else
            SaveAsFileNameTextBox.Enabled = False
            OpenFileButton.Enabled = False
            SaveToFileButton.Enabled = False
        End If

    End Sub


    Private Sub OpenFileButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenFileButton.Click

        Using sfd As New SaveFileDialog
            sfd.AddExtension = True
            sfd.DefaultExt = "." & DefaultFileExtension
            sfd.Filter = "Atsarginė kopija|*." & DefaultFileExtension & "|Visi failai|*.*"
            sfd.ValidateNames = True
            sfd.FileName = GetDefaultFileName()
            If sfd.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            SaveAsFileNameTextBox.Text = sfd.FileName
        End Using

    End Sub

    Private Sub OpenFileButton2_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenFileButton2.Click

        Using ofd As New OpenFileDialog
            ofd.Filter = "Atsarginė kopija|*." & DefaultFileExtension & "|Visi failai|*.*"
            ofd.Multiselect = False
            If ofd.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            OpenFileNameTextbox.Text = ofd.FileName
        End Using

    End Sub


    Private Function RequestUserFolder(ByVal requestMessage As String, _
        ByVal DialogDescription As String, ByVal errorMessage As String, _
        ByVal FileToCheck As String, ByVal errorFileNotFoundMessage As String, _
        ByRef result As String) As Boolean

        MsgBox(requestMessage, MsgBoxStyle.Exclamation, "")

        Using ofd As New FolderBrowserDialog

            ofd.Description = DialogDescription
            ofd.ShowNewFolderButton = False

            If ofd.ShowDialog <> Windows.Forms.DialogResult.OK Then Return False

            If ofd.SelectedPath Is Nothing OrElse String.IsNullOrEmpty(ofd.SelectedPath.Trim) _
                OrElse Not IO.Directory.Exists(ofd.SelectedPath) Then
                MsgBox(errorMessage, MsgBoxStyle.Exclamation, "Klaida")
                Return False
            End If

            If Not FileToCheck Is Nothing AndAlso Not String.IsNullOrEmpty(FileToCheck.Trim) _
                AndAlso Not IO.File.Exists(IO.Path.Combine(ofd.SelectedPath, FileToCheck)) Then
                MsgBox(errorFileNotFoundMessage, MsgBoxStyle.Exclamation, "Klaida")
                Return False
            End If

            result = ofd.SelectedPath

        End Using

        Return True

    End Function


    Private Sub SaveToFileButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles SaveToFileButton.Click

        If String.IsNullOrEmpty(SaveAsFileNameTextBox.Text.Trim) Then
            MsgBox("Klaida. Nenurodytas failo pavadinimas.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        Dim CurrentIdentity As AccIdentity = GetCurrentIdentity()

        If Not IsLoggedInDB() Then
            MsgBox("Klaida. Neprisijungta prie jokios duomenų bazės.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        Try
            Using busy As New StatusBusy
                If CurrentIdentity.IsLocalUser Then
                    LocalUserMakeBackUp(CurrentIdentity)
                ElseIf CurrentIdentity.SqlServerType = AccDataAccessLayer.SqlServerType.MySQL Then
                    MySqlMakeBackUp(CurrentIdentity)
                Else
                    Throw New NotImplementedException("Klaida. SQL serverio tipas '" _
                        & CurrentIdentity.SqlServerType.ToString & "' nepalaikomas.")
                End If
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        MsgBox("Įmonės duomenų bazės atsarginė kopija sėkmingai sukurta.", _
            MsgBoxStyle.Information, "Info")
        Me.Hide()
        Me.Close()

    End Sub

    Private Sub LocalUserMakeBackUp(ByVal CurrentIdentity As AccIdentity)

        Dim oldPass As String = ""
        If Not String.IsNullOrEmpty(GetCurrentIdentity.Password) Then
            oldPass = GetCurrentIdentity.Password
            Try
                CommandChangePassword.TheCommand(oldPass, "", "")
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko laikinai nuimti slaptažodžio: " & ex.Message, ex)
            End Try
        End If

        Try

            Using busy As New StatusBusy

                Using ShellProcess As New Process

                    ShellProcess.StartInfo.FileName = "cmd.exe"
                    ShellProcess.StartInfo.UseShellExecute = False
                    ShellProcess.StartInfo.WorkingDirectory = AppPath()
                    ShellProcess.StartInfo.RedirectStandardInput = True
                    ShellProcess.StartInfo.RedirectStandardOutput = True
                    ShellProcess.StartInfo.RedirectStandardError = True
                    ShellProcess.Start()
                    Dim myStreamWriter As StreamWriter = ShellProcess.StandardInput
                    Dim cmdText As String = "sqlite3.exe " & Chr(34) & GetFullPathToSQLiteDbFile( _
                        CurrentIdentity.Database).Trim & Chr(34) & " .dump > " & Chr(34) _
                        & SaveAsFileNameTextBox.Text & Chr(34)
                    myStreamWriter.WriteLine("cd " & Chr(34) & AppPath() & Chr(34))
                    myStreamWriter.WriteLine(cmdText)
                    myStreamWriter.Close()

                    ShellProcess.WaitForExit()

                    Dim TmpS As String = ShellProcess.StandardError.ReadToEnd
                    If Not TmpS Is Nothing AndAlso Not String.IsNullOrEmpty(TmpS.Trim) Then
                        ShellProcess.Close()
                        Throw New Exception(TmpS)
                    End If

                    ShellProcess.Close()

                End Using

            End Using

        Catch ex As Exception

            If Not String.IsNullOrEmpty(oldPass) Then
                Try
                    CommandChangePassword.TheCommand("", oldPass, oldPass)
                Catch e As Exception
                    Throw New Exception("Klaida kuriant atsarginę kopiją: " & ex.Message _
                        & vbCrLf & "Klaida. Nepavyko grąžinti laikinai nuimto slaptažodžio: " & e.Message, ex)
                End Try
            End If

            Throw New Exception("Klaida kuriant atsarginę kopiją: " & ex.Message, ex)

        End Try

        If Not String.IsNullOrEmpty(oldPass) Then
            Try
                CommandChangePassword.TheCommand("", oldPass, oldPass)
            Catch e As Exception
                MsgBox("Klaida. Nepavyko grąžinti laikinai nuimto slaptažodžio: " _
                    & e.Message, MsgBoxStyle.Exclamation, "")
            End Try
        End If

    End Sub

    Private Sub MySqlMakeBackUp(ByVal CurrentIdentity As AccIdentity)
        Try
            Using ShellProcess As New Process

                If StringIsNullOrEmpty(BaseDirPath) Then
                    ShellProcess.StartInfo.FileName = "mysqldump.exe"
                Else
                    ShellProcess.StartInfo.FileName = IO.Path.Combine(IO.Path.Combine( _
                        BaseDirPath, "bin"), "mysqldump.exe")
                End If
                ShellProcess.StartInfo.Arguments = "--opt --user=root --password=" & _
                    GetCurrentIdentity.Password & " --no-create-db --routines --result-file=" & Chr(34) & _
                    SaveAsFileNameTextBox.Text & Chr(34) & " " & CurrentIdentity.Database
                ShellProcess.StartInfo.WorkingDirectory = IO.Path.Combine(BaseDirPath, "bin")
                ShellProcess.StartInfo.UseShellExecute = False
                ShellProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                ShellProcess.StartInfo.RedirectStandardError = True

                ShellProcess.Start()

                ShellProcess.WaitForExit()

                Dim TmpS As String = ShellProcess.StandardError.ReadToEnd
                If TmpS Is Nothing Then TmpS = ""
                If Not String.IsNullOrEmpty(TmpS.Trim) Then
                    If TmpS.ToLower.Contains("warning") Then
                        MsgBox("Įspėjimas. Kuriant duomenų bazės atsarginę kopija galėjo kilti klaida: " _
                               & TmpS, MsgBoxStyle.Information, "Įspėjimas")
                    Else
                        ShellProcess.Close()
                        Throw New Exception(TmpS)
                    End If
                End If

                ShellProcess.Close()

            End Using

        Catch ex As Exception
            Throw New Exception("Klaida kuriant atsarginę kopiją: " & ex.Message, ex)
        End Try
    End Sub


    Private Sub RecoverFromFile_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RecoverFromFileButton.Click

        If String.IsNullOrEmpty(OpenFileNameTextbox.Text.Trim) Then Exit Sub

        Dim FileTimeStamp As String

        Try
            Dim tf As New FileInfo(OpenFileNameTextbox.Text)

            If Not tf.Exists Then
                MsgBox("Klaida. Nepasirinktas atsarginės kopijos dokumentas.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                Exit Sub
            End If

            FileTimeStamp = tf.CreationTime.ToShortDateString _
                & " " & tf.CreationTime.ToShortTimeString

        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko įkrauti atsarginės kopijos failo: " & ex.Message, ex))
            Exit Sub
        End Try

        Dim CurrentIdentity As AccIdentity = GetCurrentIdentity()

        If Not CurrentIdentity.IsLocalUser AndAlso String.IsNullOrEmpty(CharSetDirPath.Trim) Then
            MsgBox("Klaida. Duomenų bazės atkūrimui trūksta duomenų (nežinomas CharSetDir kelias).", _
                MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        If Not CurrentIdentity.IsLocalUser AndAlso String.IsNullOrEmpty(BaseDirPath.Trim) Then
            MsgBox("Klaida. Duomenų bazės atkūrimui trūksta duomenų (nežinomas MySQL instaliacijos kelias).", _
                MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If


        Dim BackupCompanyName As String = ""
        Dim BackupCompanyCode As String = ""
        Dim BackupContainsCreateDatabase As Boolean

        Try
            Using busy As New StatusBusy

                If CurrentIdentity.IsLocalUser Then
                    SQLiteGetCompanyDataFromBackupFile(OpenFileNameTextbox.Text, BackupCompanyName, _
                        BackupCompanyCode, BackupContainsCreateDatabase)
                ElseIf CurrentIdentity.SqlServerType = AccDataAccessLayer.SqlServerType.MySQL Then
                    MySqlGetCompanyDataFromBackupFile(OpenFileNameTextbox.Text, BackupCompanyName, _
                        BackupCompanyCode, BackupContainsCreateDatabase)
                Else
                    Throw New NotImplementedException("Klaida. SQL serverio tipas '" _
                        & CurrentIdentity.SqlServerType.ToString & "' nepalaikomas.")
                End If

            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        Dim DbList As DatabaseInfoList
        Try
            Using busy As New StatusBusy
                DbList = DatabaseInfoList.GetDatabaseList
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko gauti įmonių sąrašo", ex))
            Exit Sub
        End Try

        If IsLoggedInDB() Then

            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login("", New CustomCacheManager)
                    LogOffToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        End If

        Dim ExistingDB As Boolean = False
        Dim IndexExistingDB As Integer = 0
        For i As Integer = 1 To DbList.Count

            If Not String.IsNullOrEmpty(BackupCompanyCode.Trim) AndAlso _
                DbList(i - 1).Id.Trim.ToLower = BackupCompanyCode.Trim.ToLower Then
                ExistingDB = True
                IndexExistingDB = i - 1
            ElseIf String.IsNullOrEmpty(BackupCompanyCode.Trim) AndAlso _
                (DbList(i - 1).CompanyName.Trim.ToLower = BackupCompanyName.Trim.ToLower _
                OrElse DbList(i - 1).DatabaseName.Trim.ToLower = DbList.GetNewLocalDatabaseName( _
                BackupCompanyName.Trim)) Then
                ExistingDB = True
                IndexExistingDB = i - 1
            End If

        Next

        Dim ats As String = ""
        If ExistingDB Then
            ats = Ask("DĖMESIO. Atsarginėje kopijoje išsaugoti įmonės duomenys: duomenų bazė - " _
                & DbList(IndexExistingDB).DatabaseName & ", įmonės pavadinimas - " _
                & DbList(IndexExistingDB).CompanyName & ", kodas - " & DbList(IndexExistingDB).Id _
                & "." & vbCrLf & "Duomenų bazės atsarginė kopija buvo padaryta " & FileTimeStamp _
                & ". Atkūrus duomenų bazę iš šios atsarginės kopijos, visų vėliau registruotų " _
                & "operacijų duomenys bus negrįžtamai prarasti. Ar tikrai norite atkurti " _
                & "duomenų bazę iš atsarginės kopijos?", _
                New ButtonStructure("Atkurti", "Perrašyti duomenis ""ant viršaus""."), _
                New ButtonStructure("Įtraukti naują", "Sukurti naują įmonę pagal atsarginės kopijos duomenis."), _
                New ButtonStructure("Atšaukti", "Nieko nedaryti."))
            If ats = "Atšaukti" Then Exit Sub
        Else
            If Not YesOrNo("Sukurti naują duomenų bazę įmonei """ & BackupCompanyName & """?") Then Exit Sub
        End If

        Dim ModifiedBackupFilePath As String = OpenFileNameTextbox.Text
        If BackupContainsCreateDatabase Then

            ' defining backup file encoding
            Dim enc As System.Text.Encoding = Nothing
            'Try
            '    enc = GetFileEncoding(OpenFileNameTextbox.Text, True)
            'Catch ex As Exception
            '    If Not YesOrNo("Dėmesio. Nepavyko identifikuoti atsarginės kopijos failo " _
            '        & "duomenų koduotės (encoding). Gali būti blogai atkurti lietuviškų " _
            '        & "raidžių simboliai. Ar norite tęsti atkūrimą?") Then Exit Sub
            'End Try

            Try
                Using busy As New StatusBusy

                    If CurrentIdentity.IsLocalUser Then
                        ' do nothing, sqlite file cannot contain create database
                    ElseIf CurrentIdentity.SqlServerType = AccDataAccessLayer.SqlServerType.MySQL Then
                        ModifiedBackupFilePath = MySqlCreateCleanBackupFile(ModifiedBackupFilePath, enc)
                    Else
                        Throw New NotImplementedException("Klaida. SQL serverio tipas '" _
                            & CurrentIdentity.SqlServerType.ToString & "' nepalaikomas.")
                    End If

                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        End If

        Try
            Using busy As New StatusBusy

                Dim DbNameToRestore As String = ""
                If ats = "Atkurti" Then DbNameToRestore = DbList(IndexExistingDB).DatabaseName
                If CurrentIdentity.IsLocalUser Then
                    SqliteRestoreDatabase(ModifiedBackupFilePath, DbNameToRestore, BackupCompanyName)
                ElseIf CurrentIdentity.SqlServerType = AccDataAccessLayer.SqlServerType.MySQL Then
                    MySqlRestoreDatabase(ModifiedBackupFilePath, DbNameToRestore)
                Else
                    Throw New NotImplementedException("Klaida. SQL serverio tipas '" _
                        & CurrentIdentity.SqlServerType.ToString & "' nepalaikomas.")
                End If

            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        MsgBox("Įmonės '" & BackupCompanyName & "' duomenų bazė sėkmingai atkurta. " _
            & "Atkūrimo data ir laikas: " & FileTimeStamp & ".", MsgBoxStyle.Information, "Info")

        DatabaseInfoList.InvalidateCache()
        Try
            Using busy As New StatusBusy
                DbList = DatabaseInfoList.GetDatabaseList
                DatabasesToMenu()
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo po atkūrimo.", ex))
        End Try

        Try
            If ModifiedBackupFilePath.Trim <> OpenFileNameTextbox.Text.Trim AndAlso _
                IO.File.Exists(ModifiedBackupFilePath) Then IO.File.Delete(ModifiedBackupFilePath)
        Catch ex As Exception
            ' may warn the user that modified backup copy remains in program folder?
        End Try

        Me.Hide()
        Me.Close()

    End Sub

    Private Sub MySqlGetCompanyDataFromBackupFile(ByVal BackupFilePath As String, _
        ByRef cCompanyName As String, ByRef cCompanyCode As String, _
        ByRef cBackupContainsCreateDatabase As Boolean)

        cCompanyName = ""
        cCompanyCode = ""
        cBackupContainsCreateDatabase = False

        Dim ContainsPragma As Boolean = False
        Dim ContainsCompany As Boolean = False

        Using objReader As StreamReader = New StreamReader(BackupFilePath)
            Dim s As String
            While objReader.Peek <> -1
                s = objReader.ReadLine()
                If s.Contains("INSERT") And s.Trim.ToLower.Contains("`imone`") Then
                    cCompanyName = s.Substring(s.IndexOf(",'") + 2, _
                        s.IndexOf("'", s.IndexOf(",'") + 2) - s.IndexOf(",'") - 2)
                    cCompanyCode = s.Substring(s.IndexOf("(") + 1, s.IndexOf(",") - s.IndexOf("(") - 1)
                    ContainsCompany = True
                    Exit While
                ElseIf s.Contains("CREATE DATABASE") Then
                    cBackupContainsCreateDatabase = True
                ElseIf s.Contains("PRAGMA ") Then
                    ContainsPragma = True
                End If
            End While
            objReader.Close()
        End Using

        If ContainsPragma Then Throw New Exception("Klaida. Atsarginės kopijos failas " _
            & "buvo sukurtas ne MySQL duomenų bazei.")
        If Not ContainsCompany Then Throw New Exception("Klaida. Atsarginės kopijos faile " _
            & "nėra saugoma įmonės apskaitos duomenų bazė.")

    End Sub

    Private Sub SQLiteGetCompanyDataFromBackupFile(ByVal BackupFilePath As String, _
        ByRef cCompanyName As String, ByRef cCompanyCode As String, _
        ByRef cBackupContainsCreateDatabase As Boolean)

        cCompanyName = ""
        cCompanyCode = ""
        cBackupContainsCreateDatabase = False

        Dim ContainsPragma As Boolean = False
        Dim ContainsCompany As Boolean = False

        Using objReader As New StreamReader(BackupFilePath)
            Dim s As String
            While objReader.Peek <> -1
                s = objReader.ReadLine()
                If s.Contains("INSERT") And s.Trim.ToLower.Contains("""imone""") Then
                    cCompanyName = s.Substring(s.IndexOf(",'") + 2, _
                        s.IndexOf("'", s.IndexOf(",'") + 2) - s.IndexOf(",'") - 2)
                    cCompanyCode = s.Substring(s.IndexOf("(") + 1, s.IndexOf(",") - s.IndexOf("(") - 1)
                    ContainsCompany = True
                    If ContainsPragma Then Exit While
                ElseIf s.Contains("PRAGMA ") Then
                    ContainsPragma = True
                    If ContainsCompany Then Exit While
                End If
            End While
            objReader.Close()
        End Using

        If Not ContainsPragma Then Throw New Exception("Klaida. Atsarginės kopijos failas " _
            & "buvo sukurtas ne SQLite duomenų bazei.")
        If Not ContainsCompany Then Throw New Exception("Klaida. Atsarginės kopijos faile " _
            & "nėra saugoma įmonės apskaitos duomenų bazė.")

    End Sub

    Private Function MySqlCreateCleanBackupFile(ByVal BackupFilePath As String, _
        ByVal BackupFileEncoding As System.Text.Encoding) As String

        Dim ModifiedBackupFilePath As String = IO.Path.Combine(IO.Path.GetTempPath, "tmp_backup.sql")

        If BackupFileEncoding Is Nothing Then BackupFileEncoding = System.Text.Encoding.UTF8

        Using busy As New StatusBusy

            Using objWriter As New StreamWriter(ModifiedBackupFilePath, False, BackupFileEncoding)

                Using objReader As New StreamReader(BackupFilePath)
                    Dim s As String
                    While objReader.Peek <> -1
                        s = objReader.ReadLine()
                        If Not s.Trim.StartsWith("CREATE DATABASE ") AndAlso Not s.Trim.StartsWith("USE ") Then
                            objWriter.WriteLine(s)
                        End If
                    End While
                    objReader.Close()
                End Using

                objWriter.Close()

            End Using

        End Using

        Return ModifiedBackupFilePath

    End Function

    Private Sub MySqlRestoreDatabase(ByVal modifiedBackupFilePath As String, _
        ByRef dbNameToRestore As String)

        Dim newDbCreated As Boolean = False

        If StringIsNullOrEmpty(dbNameToRestore) Then

            Try
                dbNameToRestore = AccDataAccessLayer.Security.CommandGetNewDatabaseName. _
                    TheCommand("")
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko gauti naujos duomenų bazės pavadinimo.", ex)
            End Try

            Try
                Dim myComm As New SQLCommand("RawSQL", "CREATE DATABASE " & dbNameToRestore.Trim _
                    & " CHARACTER SET=cp1257;")
                myComm.Execute()
                newDbCreated = True
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko sukurti naujos duomenų bazės.", ex)
            End Try

        End If

        Dim cmdText As String = "mysql --user=root --password=" & GetCurrentIdentity.Password _
            & " --character-sets-dir=" & CharSetDirPath & " " & dbNameToRestore & " < " _
            & Chr(34) & modifiedBackupFilePath & Chr(34)

        Using myProcess As New Process()

            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.UseShellExecute = False
            myProcess.StartInfo.WorkingDirectory = IO.Path.Combine(BaseDirPath, "bin")
            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            myProcess.Start()

            Dim myStreamWriter As StreamWriter = myProcess.StandardInput
            If Not StringIsNullOrEmpty(BaseDirPath) Then
                myStreamWriter.WriteLine("cd " & Chr(34) & BaseDirPath & Chr(34))
            End If
            myStreamWriter.WriteLine(cmdText)
            myStreamWriter.Close()

            myProcess.WaitForExit()

            Dim standardError As String = myProcess.StandardError.ReadToEnd
            If Not StringIsNullOrEmpty(standardError) Then

                If standardError.ToLower.Contains("warning") Then

                    MsgBox("Įspėjimas. Atkuriant duomenų bazę galėjo kilti klaida: " & standardError, MsgBoxStyle.Information, "Įspėjimas")

                Else

                    If newDbCreated Then

                        Try
                            Dim myComm As New SQLCommand("RawSQL", "DROP DATABASE " & dbNameToRestore.Trim & ";")
                            myComm.Execute()
                        Catch ex As Exception
                            myProcess.Close()
                            Throw New Exception("Klaida atkuriant atsarginę kopiją: " & standardError & vbCrLf _
                                & "Klaida. Nepavyko panaikinti naujos (tuščios) duomenų bazės: " & ex.Message, ex)
                        End Try

                    End If

                    myProcess.Close()

                    Throw New Exception("Klaida atkuriant atsarginę kopiją: " & standardError)

                End If

            End If

            myProcess.Close()

        End Using

    End Sub

    Private Sub SqliteRestoreDatabase(ByVal modifiedBackupFilePath As String, _
        ByRef dbNameToRestore As String, ByVal newCompanyName As String)

        Dim dbHasBeenBackedUp As Boolean = False

        Dim dbBackupPath As String = IO.Path.Combine(IO.Path.GetTempPath, "temp_sqlite.bak")
        Try
            If IO.File.Exists(dbBackupPath) Then
                IO.File.Delete(dbBackupPath)
            End If
        Catch ex As Exception
        End Try

        Dim dbFilePath As String

        If StringIsNullOrEmpty(dbNameToRestore) Then

            Try
                dbNameToRestore = AccDataAccessLayer.Security.CommandGetNewDatabaseName. _
                    TheCommand(newCompanyName)
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko gauti naujos duomenų bazės pavadinimo.", ex)
            End Try

            dbFilePath = GetFullPathToSQLiteDbFile(dbNameToRestore.Trim)

        Else

            dbFilePath = GetFullPathToSQLiteDbFile(dbNameToRestore.Trim)

            Try
                IO.File.Copy(dbFilePath, dbBackupPath)
                IO.File.Delete(dbFilePath)
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko backupinti atnaujinamos duomenų bazės: " & ex.Message, ex)
            End Try

            dbHasBeenBackedUp = True

        End If

        Dim cmdText As String = "sqlite3 " & Chr(34) & dbFilePath & Chr(34) & " < " _
            & Chr(34) & modifiedBackupFilePath & Chr(34)

        Using myProcess As New Process()

            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.UseShellExecute = False
            myProcess.StartInfo.WorkingDirectory = AppPath()
            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            myProcess.Start()

            Dim myStreamWriter As StreamWriter = myProcess.StandardInput
            myStreamWriter.WriteLine("cd " & Chr(34) & AppPath() & Chr(34))
            myStreamWriter.WriteLine(cmdText)
            myStreamWriter.Close()

            myProcess.WaitForExit()

            Dim standardError As String = myProcess.StandardError.ReadToEnd
            If Not StringIsNullOrEmpty(standardError) Then

                If dbHasBeenBackedUp Then
                    Try
                        IO.File.Copy(dbBackupPath, dbFilePath)
                        IO.File.Delete(dbBackupPath)
                    Catch ex As Exception
                        myProcess.Close()
                        Throw New Exception("Klaida atkuriant atsarginę kopiją: " & standardError _
                            & vbCrLf & "DĖMESIO. Sugadintas pirminis duomenų bazės failas.", ex)
                    End Try
                End If

                myProcess.Close()

                Throw New Exception("Klaida atkuriant atsarginę kopiją: " & standardError)

            End If

            myProcess.Close()

        End Using

        If dbHasBeenBackedUp Then
            Try
                IO.File.Delete(dbBackupPath)
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Function GetDefaultFileName() As String
        Return DatabaseInfoList.GetLocalDatabaseNameByCompanyName( _
            GetCurrentCompany.Name) & Today.ToString("yyyyMMdd") _
            & "." & DefaultFileExtension
    End Function

    'Private Function LocalUserRestoreFromBackUp(ByVal CurrentIdentity As AccIdentity, _
    '    ByVal FileTimeStamp As String) As Boolean

    '    Dim BackupCompanyName, BackupCompanyCode, OriginalCompanyName, OriginalCompanyCode As String

    '    Try
    '        FetchCompanyFromDbFile(OpenFileNameTextbox.Text, AccDataAccessLayer.SqlServerType.SQLite, _
    '            BackupDatabasePasswordTextBox.Text.Trim, BackupCompanyName, BackupCompanyCode)
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko atidaryti atsarginės kopijos failo. " _
    '            & ex.Message, ex))
    '        Return False
    '    End Try

    '    Dim DbList As DatabaseInfoList
    '    Try
    '        DbList = DatabaseInfoList.GetDatabaseList
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko gauti įmonių sąrašo", ex))
    '        Return False
    '    End Try

    '    If RecoveryTargetDbComboBox.SelectedIndex = 0 Then

    '        Dim NewDbFileName As String = DbList.GetLocalDatabaseNameByCompanyName(BackupCompanyName)

    '        If DbList.DatabaseNameExists(NewDbFileName) Then

    '            Dim NewDbFileNameModified As String = DbList.GetNewLocalDatabaseName(BackupCompanyName)

    '            If Not YesOrNo("Duomenų bazė pavadinimu '" & NewDbFileName _
    '                & "' jau yra. Įtraukti pavadinimu '" & NewDbFileNameModified & "'?") Then Return False

    '            NewDbFileName = NewDbFileNameModified

    '        Else

    '            If Not YesOrNo("Atsarginės kopijos failas buvo sukurtas " & FileTimeStamp _
    '                & ". Jame saugomi duomenys apie įmonę '" & BackupCompanyName.Trim _
    '                & "', kurios kodas yra " & BackupCompanyCode.Trim _
    '                & ". Sukurti naują duomenų bazę pavadinimu '" & NewDbFileName _
    '                & "' iš šios atsarginės kopijos duomenų?") Then Return False

    '        End If

    '        Try
    '            IO.File.Copy(OpenFileNameTextbox.Text, GetFullPathToSQLiteDbFile(NewDbFileName), False)
    '        Catch ex As Exception
    '            ShowError(ex)
    '            Return False
    '        End Try

    '        MsgBox("Duomenų bazė '" & NewDbFileName & "' sėkmingai sukurta iš atsarginės kopijos.", _
    '             MsgBoxStyle.Information, "Info")

    '        DatabaseInfoList.InvalidateCache()
    '        Try
    '            DbList = DatabaseInfoList.GetDatabaseList
    '            DatabasesToMenu()
    '        Catch ex As Exception
    '            ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo.", ex))
    '        End Try

    '        Return True

    '    End If

    '    Try
    '        FetchCompanyFromDbFile(GetFullPathToSQLiteDbFile( _
    '            RecoveryTargetDbComboBox.SelectedItem.ToString.Trim), _
    '            AccDataAccessLayer.SqlServerType.SQLite, _
    '            LocalDatabasePasswordTextBox.Text.Trim, OriginalCompanyName, OriginalCompanyCode)
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko atidaryti atkuriamos duomenų bazės. " _
    '            & ex.Message, ex))
    '        Return False
    '    End Try

    '    If BackupCompanyCode.Trim.ToLower <> OriginalCompanyCode.Trim.ToLower Then

    '        Dim result As String = Ask("Pasirinktoje esamoje duomenų bazėje yra saugomi įmonės '" _
    '            & OriginalCompanyName & "', kurios kodas yra " & OriginalCompanyCode _
    '            & ", duomenys. Atsarginėje kopijoje yra saugomi kitos įmonės duomenys - '" _
    '            & BackupCompanyName & "', kurios kodas yra " & BackupCompanyCode & "." _
    '            & vbCrLf & "Jūs galite perrašyti naujus duomenis ir ignoruoti įmonių " _
    '            & "neatitikimą arba sukurti naują duomenų bazę iš šios atsarginės kopijos.", _
    '            New ButtonStructure("Perrašyti", "Atsarginės kopijos duomenys pakeis dabartinį " _
    '            & "šios duomenų bazės turinį."), New ButtonStructure("Įtraukti kaip naują", _
    '            "Iš atsarginės kopijos duomenų bus sukurta nauja duomenų bazė."), _
    '            New ButtonStructure("Atšaukti", "Nedaryti nieko"))

    '        If result <> "Perrašyti" AndAlso result <> "Įtraukti kaip naują" Then Return False

    '        If result = "Įtraukti kaip naują" Then

    '            Dim NewDbFileName As String = AccDataAccessLayer.ConvertFromLithuanianToEnglishLetters( _
    '                BackupCompanyName.Trim).Replace(" ", "")

    '            If DbList.DatabaseNameExists(NewDbFileName) Then

    '                Dim NewDbFileNameModified As String = SQLiteGetNewDbName(NewDbFileName, DbList)

    '                If Not YesOrNo("Duomenų bazė pavadinimu '" & NewDbFileName _
    '                    & "' jau yra. Įtraukti pavadinimu '" & NewDbFileNameModified & "'?") Then Return False

    '                NewDbFileName = NewDbFileNameModified

    '            End If


    '            Try
    '                IO.File.Copy(OpenFileNameTextbox.Text, GetFullPathToSQLiteDbFile(NewDbFileName), False)
    '            Catch ex As Exception
    '                ShowError(ex)
    '                Return False
    '            End Try

    '            MsgBox("Duomenų bazė '" & NewDbFileName & "' sėkmingai sukurta iš atsarginės kopijos.", _
    '                 MsgBoxStyle.Information, "Info")

    '            DatabaseInfoList.InvalidateCache()
    '            Try
    '                DbList = DatabaseInfoList.GetDatabaseList
    '                DatabasesToMenu()
    '            Catch ex As Exception
    '                ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo.", ex))
    '            End Try

    '            Return True

    '        Else

    '            Try
    '                IO.File.Copy(OpenFileNameTextbox.Text, GetFullPathToSQLiteDbFile( _
    '                    RecoveryTargetDbComboBox.SelectedItem.ToString.Trim), True)
    '            Catch ex As Exception
    '                ShowError(ex)
    '                Return False
    '            End Try

    '            MsgBox("Duomenų bazė '" & RecoveryTargetDbComboBox.SelectedItem.ToString.Trim _
    '                & "' sėkmingai atkurta iš atsarginės kopijos.", MsgBoxStyle.Information, "Info")

    '            DatabaseInfoList.InvalidateCache()
    '            Try
    '                DbList = DatabaseInfoList.GetDatabaseList
    '                DatabasesToMenu()
    '            Catch ex As Exception
    '                ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo po atkūrimo.", ex))
    '            End Try

    '            Return True

    '        End If

    '    Else

    '        If Not YesOrNo("Atsarginės kopijos failas buvo sukurtas " & FileTimeStamp _
    '            & ". Atkūrus duomenų bazę iš šios atsarginės kopijos, visų vėliau registruotų " _
    '            & "operacijų duomenys bus negrįžtamai prarasti. Ar tikrai norite atkurti " _
    '            & "duomenų bazę iš atsarginės kopijos?") Then Return False

    '        Try
    '            IO.File.Copy(OpenFileNameTextbox.Text, GetFullPathToSQLiteDbFile( _
    '                RecoveryTargetDbComboBox.SelectedItem.ToString.Trim), True)
    '        Catch ex As Exception
    '            ShowError(ex)
    '            Return False
    '        End Try

    '        MsgBox("Duomenų bazė '" & RecoveryTargetDbComboBox.SelectedItem.ToString.Trim _
    '            & "' sėkmingai atkurta iš atsarginės kopijos.", MsgBoxStyle.Information, "Info")

    '        DatabaseInfoList.InvalidateCache()
    '        Try
    '            DbList = DatabaseInfoList.GetDatabaseList
    '            DatabasesToMenu()
    '        Catch ex As Exception
    '            ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo po atkūrimo.", ex))
    '        End Try

    '        Return True

    '    End If

    'End Function

    'Private Function SQLiteGetNewDbName(ByVal BasicName As String, _
    '    ByVal DbList As DatabaseInfoList) As String

    '    For Each db As DatabaseInfo In DbList
    '        If db.DatabaseName.Trim.ToLower = BasicName.Trim.ToLower Then

    '            For i As Integer = 2 To 999
    '                If Not DbList.DatabaseNameExists(BasicName.Trim & i.ToString) Then _
    '                    Return BasicName.Trim & i.ToString
    '            Next

    '        End If
    '    Next

    '    Return BasicName.Trim

    'End Function

    'Private Function MySqlRestoreFromBackUp(ByVal CurrentIdentity As AccIdentity, _
    '    ByVal FileTimeStamp As String) As Boolean

    '    Dim pv As String
    '    'OpenFileNameTextbox.Text

    '    Dim BackupCompanyName, BackupCompanyCode, BackupDatabaseName, _
    '        OriginalCompanyName, OriginalCompanyCode, OriginalDatabaseName As String

    '    Try
    '        MySqlGetCompanyDataFromBackupFile(OpenFileNameTextbox.Text, BackupCompanyName, _
    '            BackupCompanyCode, BackupDatabaseName)
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko įkrauti atsarginės kopijos failo. " _
    '            & "Tikėtina, kad šis failas nėra duomenų bazės atsarginė kopija" & ex.Message, ex))
    '        Return False
    '    End Try

    '    If BackupCompanyCode Is Nothing OrElse String.IsNullOrEmpty(BackupCompanyCode.Trim) Then
    '        MsgBox("Klaida. Nepavyko nustatyti atsarginėje kopijoje saugomos įmonės kodo.", _
    '             MsgBoxStyle.Exclamation, "Klaida.")
    '        Return False
    '    End If

    '    If BackupDatabaseName Is Nothing OrElse String.IsNullOrEmpty(BackupDatabaseName.Trim) Then
    '        MsgBox("Klaida. Nepavyko nustatyti atsarginėje kopijoje saugomos duomenų bazės pavadinimo.", _
    '             MsgBoxStyle.Exclamation, "Klaida.")
    '        Return False
    '    End If

    '    Dim DbList As DatabaseInfoList
    '    Try
    '        DbList = DatabaseInfoList.GetDatabaseList
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko gauti įmonių sąrašo", ex))
    '        Return False
    '    End Try

    '    pv = CurrentIdentity.Password
    '    If IsLoggedInDB() Then
    '        AccPrincipal.Login("", New CustomCacheManager)
    '        LogOffToGUI()
    '    End If

    '    Dim DuplicateDB As Boolean = False
    '    Dim MismatchDB As Boolean = False
    '    Dim IndexDuplicate As Integer
    '    Dim IndexMismatch As Integer

    '    For i As Integer = 1 To DbList.Count
    '        If DbList(i - 1).DatabaseName.Trim = BackupDatabaseName.Trim _
    '            AndAlso DbList(i - 1).Id.Trim.ToLower <> BackupCompanyCode.Trim.ToLower Then
    '            DuplicateDB = True
    '            IndexDuplicate = i
    '        ElseIf DbList(i - 1).DatabaseName.Trim <> BackupDatabaseName.Trim _
    '            AndAlso DbList(i - 1).Id.Trim.ToLower = BackupCompanyCode.Trim.ToLower Then
    '            MismatchDB = True
    '            IndexMismatch = i
    '        End If
    '    Next

    '    Dim ats As String = ""
    '    If DuplicateDB And MismatchDB Then
    '        ats = Ask("DĖMESIO. Atsarginėje kopijoje išsaugoti įmonės " & _
    '        "duomenys: duomenų bazė - " & BackupDatabaseName & ", įmonės pavadinimas - " & BackupCompanyName & _
    '        ", kodas - " & BackupCompanyCode & "." & vbCrLf & _
    '        "To pačio pavadinimo serverio duomenų bazėje saugomi kitos įmonės duomenys: " & _
    '        DbList(IndexDuplicate - 1).CompanyName & ", kodas " & DbList(IndexDuplicate - 1).Id & "." & _
    '        vbCrLf & "Serveryje įmonės, kurios duomenis norima atkurti, " & _
    '        "duomenys yra saugomi kitoje duomenų bazėje: " & DbList(IndexMismatch - 1).DatabaseName & ".", _
    '        New ButtonStructure("Overwrite", "Perrašyti duomenis ""ant viršaus"" ištrinant " & _
    '        "prieš tai buvusį faile nurodytos duomenų bazės turinį."), New ButtonStructure("Naudoti esamą", _
    '        "Naudoti duomenų bazę, kurioje šiuo metu saugomi tos įmonės duomenys."), _
    '        New ButtonStructure("Įtraukti naują", "Sukurti naują įmonę pagal atsarginės kopijos duomenis."), _
    '        New ButtonStructure("Atšaukti", "Nieko nedaryti."))
    '    ElseIf DuplicateDB Then
    '        ats = Ask("DĖMESIO. Atsarginėje kopijoje išsaugoti įmonės " & _
    '        "duomenys: duomenų bazė - " & BackupDatabaseName & ", įmonės pavadinimas - " & BackupCompanyName & _
    '        ", kodas - " & BackupCompanyCode & "." & vbCrLf & _
    '        "To pačio pavadinimo serverio duomenų bazėje saugomi kitos įmonės duomenys: " & _
    '        DbList(IndexDuplicate - 1).CompanyName & ", kodas " & DbList(IndexDuplicate - 1).Id & ".", _
    '        New ButtonStructure("Overwrite", "Perrašyti duomenis ""ant viršaus"" ištrinant " & _
    '        "prieš tai buvusį faile nurodytos duomenų bazės turinį."), _
    '        New ButtonStructure("Įtraukti naują", "Sukurti naują įmonę pagal atsarginės kopijos duomenis."), _
    '        New ButtonStructure("Atšaukti", "Nieko nedaryti."))
    '    ElseIf MismatchDB Then
    '        ats = Ask("DĖMESIO. Atsarginėje kopijoje išsaugoti įmonės " & _
    '        "duomenys: duomenų bazė - " & BackupDatabaseName & ", įmonės pavadinimas - " & BackupCompanyName & _
    '        ", kodas - " & BackupCompanyCode & "." & vbCrLf & "Serveryje tos pačios įmonės " & _
    '        "duomenys saugomi kitoje duomenų bazėje: " & DbList(IndexMismatch - 1).DatabaseName & ".", _
    '        New ButtonStructure("Naudoti esamą", "Naudoti duomenų bazę, kurioje šiuo metu saugomi tos įmonės duomenys."), _
    '        New ButtonStructure("Įtraukti naują", "Sukurti naują įmonę pagal failo duomenis."), _
    '        New ButtonStructure("Atšaukti", "Nieko nedaryti."))
    '    Else
    '        If Not YesOrNo("DĖMESIO. Įmonės " & BackupCompanyName & " duomenų bazės atsarginė kopija buvo padaryta " _
    '            & FileTimeStamp & ". Atkūrus duomenų bazę iš šios atsarginės kopijos, visų vėliau registruotų " _
    '            & "operacijų duomenys bus negrįžtamai prarasti. Ar tikrai norite atkurti " _
    '            & "duomenų bazę iš atsarginės kopijos?") Then Exit Function
    '    End If

    '    If (DuplicateDB OrElse MismatchDB) AndAlso ats <> "Overwrite" AndAlso ats <> "Naudoti esamą" _
    '        AndAlso ats <> "Įtraukti naują" Then Return False

    '    Dim ModifiedBackupFilePath As String = OpenFileNameTextbox.Text

    '    If (DuplicateDB OrElse MismatchDB) AndAlso ats <> "Overwrite" Then

    '        ' defining backup file encoding
    '        Dim enc As System.Text.Encoding
    '        Try
    '            enc = GetFileEncoding(OpenFileNameTextbox.Text, True)
    '        Catch ex As Exception
    '            If Not YesOrNo("Dėmesio. Nepavyko identifikuoti atsarginės kopijos failo " _
    '                & "duomenų koduotės (encoding). Gali būti blogai atkurti lietuviškų " _
    '                & "raidžių simboliai. Ar norite tęsti atkūrimą?") Then Return False
    '        End Try

    '        Dim NewDatabaseName As String = ""
    '        Try
    '            NewDatabaseName = AccDataAccessLayer.Security.CommandGetNewDatabaseName.TheCommand
    '        Catch ex As Exception
    '            ShowError(New Exception("Klaida. Nepavyko gauti naujos duomenų bazės pavadinimo.", ex))
    '            Return False
    '        End Try

    '        Try
    '            ' changing DB name in backup file
    '            Dim backupStr() As String = File.ReadAllLines(OpenFileNameTextbox.Text, enc)

    '            For j As Integer = 1 To backupStr.Length
    '                If backupStr(j - 1).Contains(BackupDatabaseName.Trim) Then
    '                    If ats = "Įtraukti naują" Then
    '                        backupStr(j - 1) = backupStr(j - 1).Replace(BackupDatabaseName.Trim, _
    '                        NewDatabaseName)
    '                    Else ' same entity different DB
    '                        backupStr(j - 1) = backupStr(j - 1).Replace(BackupDatabaseName.Trim, _
    '                            DbList(IndexMismatch - 1).DatabaseName.Trim)
    '                    End If
    '                End If
    '                If backupStr(j - 1).Contains("CREATE TABLE") Then Exit For
    '            Next

    '            ModifiedBackupFilePath = IO.Path.Combine(IO.Path.GetTempPath, "tmp_backup.sql")

    '            File.WriteAllLines(ModifiedBackupFilePath, backupStr, enc)

    '        Catch ex As Exception
    '            ShowError(New Exception("Klaida. Nepavyko pakoreguoti atsarginės kopijos duomenų: " _
    '                & ex.Message, ex))
    '            Return False
    '        End Try

    '    End If

    '    Dim BatchFilePath As String = IO.Path.Combine(IO.Path.GetTempPath, "tempFile.bat")

    '    Try
    '        Using fs As New FileStream(BatchFilePath, FileMode.Create, FileAccess.Write)
    '            Try
    '                Using s As New StreamWriter(fs)

    '                    Try
    '                        s.BaseStream.Seek(0, SeekOrigin.End)
    '                        s.WriteLine("@echo off")
    '                        s.WriteLine("mysql --user=root --password=" & pv _
    '                        & " --character-sets-dir=" & CharSetDirPath & " < " & Chr(34) & _
    '                            ModifiedBackupFilePath & Chr(34))
    '                        s.Close()
    '                        fs.Close()
    '                    Catch ex As Exception
    '                        s.Close()
    '                        Throw ex
    '                    End Try

    '                End Using
    '            Catch ex As Exception
    '                fs.Close()
    '            End Try
    '        End Using
    '    Catch ex As Exception
    '        Throw New Exception("Klaida. Nepavyko sukurti batch failo (skripto), " _
    '            & "reikalingo duomenų bazės atkūrimui." & ex.Message, ex)
    '    End Try


    '    Try
    '        Using ShellProcess As New Process

    '            ShellProcess.StartInfo.FileName = BatchFilePath
    '            ShellProcess.StartInfo.UseShellExecute = False
    '            ShellProcess.StartInfo.RedirectStandardError = True
    '            ShellProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
    '            ShellProcess.Start()
    '            ShellProcess.WaitForExit()

    '            Dim ShellErrorMessage As String = ShellProcess.StandardError.ReadToEnd

    '            If Not String.IsNullOrEmpty(ShellErrorMessage) Then
    '                MsgBox("Klaida atkuriant duomenų bazę: " & ShellErrorMessage, _
    '                    MsgBoxStyle.Exclamation, "Klaida.")
    '                Return False
    '            End If

    '        End Using
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida startuojant arba vykdant atkūrimo procesą: " _
    '            & ex.Message, ex))
    '        Return False
    '    End Try

    '    MsgBox("Įmonės '" & BackupCompanyName & "' duomenų bazė sėkmingai atkurta. " _
    '        & "Atkūrimo data ir laikas: " & FileTimeStamp & ".", MsgBoxStyle.Information, "Info")

    '    DatabaseInfoList.InvalidateCache()
    '    Try
    '        DbList = DatabaseInfoList.GetDatabaseList
    '        DatabasesToMenu()
    '    Catch ex As Exception
    '        ShowError(New Exception("Klaida. Nepavyko atnaujinti duomenų bazių sąrašo po atkūrimo.", ex))
    '    End Try

    '    Try
    '        If IO.File.Exists(BatchFilePath) Then IO.File.Delete(BatchFilePath)
    '    Catch ex As Exception
    '    End Try

    '    Try

    '        If ModifiedBackupFilePath.Trim <> OpenFileNameTextbox.Text.Trim AndAlso _
    '            IO.File.Exists(ModifiedBackupFilePath) Then IO.File.Delete(ModifiedBackupFilePath)
    '    Catch ex As Exception
    '        ' may warn the user that modified backup copy remains in program folder?
    '    End Try

    '    Return True

    'End Function

End Class