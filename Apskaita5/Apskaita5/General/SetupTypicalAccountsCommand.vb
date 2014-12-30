Namespace General

    <Serializable()> _
    Public Class SetupTypicalAccountsCommand
        Inherits CommandBase

#Region " Authorization Rules "

        Public Shared Function CanExecuteCommand() As Boolean
            Return Csla.ApplicationContext.User.IsInRole("General.AccountList3")
        End Function

#End Region

#Region " Client-side Code "

        Private mResult As Boolean

        Public ReadOnly Property Result() As Boolean
            Get
                Return mResult
            End Get
        End Property

        Private Sub BeforeServer()
            ' implement code to run on client
            ' before server is called
        End Sub

        Private Sub AfterServer()
            HelperLists.AccountInfoList.InvalidateCache()
        End Sub

#End Region

#Region " Factory Methods "

        Public Shared Function TheCommand() As Boolean

            Dim cmd As New SetupTypicalAccountsCommand
            cmd.BeforeServer()
            cmd = DataPortal.Execute(Of SetupTypicalAccountsCommand)(cmd)
            HelperLists.AssignableCRItemList.InvalidateCache()
            HelperLists.AccountInfoList.InvalidateCache()
            cmd.AfterServer()
            Return cmd.Result

        End Function

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Server-side Code "

        Protected Overrides Sub DataPortal_Execute()

            If Not CanExecuteCommand() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai operacijai atlikti.")

            CheckIfCanSetupAccountListFromFile()

            Dim FinancialStatementsStructure As ConsolidatedReport = _
                ConsolidatedReport.GetConsolidatedReportFromFile(AppPath() & "\" & FINANCIALSTATEMENTS_FILE)
            Dim AccountsStructure As AccountList = _
                AccountList.GetAccountListFromFile(IO.Path.Combine(AppPath(), C_ACCOUNTS_FILE))

            If Not AccountsStructure.IsSettingsDictionaryAvailable Then _
                Throw New Exception("Klaida. Neįkrauti sąskaitų plano arba nustatymų duomenys.")

            DatabaseAccess.TransactionBegin()

            FinancialStatementsStructure.SaveToDatabase()
            AccountsStructure.SaveToDatabase()

            DatabaseAccess.TransactionCommit()

            ApskaitaObjects.Settings.CompanyInfo.LoadCompanyInfoToGlobalContext("", "")

            mResult = True

        End Sub

        Private Sub CheckIfCanSetupAccountListFromFile()

            Dim myComm As New SQLCommand("CheckIfCanSetupAccountListFromFile")

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then
                    Throw New Exception("Klaida. Tipinį sąskaitų planą galima generuoti " _
                    & "tik nesant jau įvestų sąskaitų.")
                ElseIf myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(1), 0) > 0 Then
                    Throw New Exception("Klaida. Tipinį sąskaitų planą galima generuoti " _
                    & "tik nesant jau įvestų finansinės atskaitomybės formų.")
                End If

            End Using


        End Sub

#End Region

    End Class

End Namespace