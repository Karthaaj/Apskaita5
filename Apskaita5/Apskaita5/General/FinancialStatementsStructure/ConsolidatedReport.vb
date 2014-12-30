Imports System.IO
Imports System.Xml
Namespace General

    <Serializable()> _
    Public Class ConsolidatedReport
        Inherits BusinessBase(Of ConsolidatedReport)

#Region " Business Methods "

        Private _ID As Guid = Guid.NewGuid
        Private _Children As ConsolidatedReportItem


        ''' <summary>
        ''' Gets a top level ConsolidatedReportItemList (2 items for BalanceSheet, 1 - for IncomeStatement).
        ''' </summary>
        Public ReadOnly Property Children() As ConsolidatedReportItem
            Get
                Return _Children
            End Get
        End Property


        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Dim s As String = ""
                Return MyBase.IsValid AndAlso _Children.IsValid AndAlso NamesUnique(s)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _Children.IsDirty
            End Get
        End Property


        Public Overrides Function Save() As ConsolidatedReport
            ' databindings don't work with TreeView control in .NET 2.0
            ' so we need to triger rules checking before saving
            Me.ValidationRules.CheckRules()
            Dim result As ConsolidatedReport
            result = MyBase.Save()
            AssignableCRItemList.InvalidateCache()
            Return result
        End Function

        Public Function SaveToFile(ByVal FileName As String) As ConsolidatedReport

            Dim writer As New XmlTextWriter(FileName, System.Text.Encoding.UTF8)

            writer.WriteStartDocument(True)
            writer.Formatting = Formatting.Indented
            writer.Indentation = 2
            writer.WriteStartElement("FinancialReportingStructure")

            _Children.WriteXmlNode(CType(writer, XmlWriter))

            writer.WriteEndElement()
            writer.WriteEndDocument()
            writer.Close()

            MarkNew()

            Return Me

        End Function


        Public Function NamesUnique(ByRef NonUniqueNames As String) As Boolean

            Dim Names As New List(Of String)
            _Children.AddNames(Names)

            Dim i, j As Integer
            NonUniqueNames = ""
            Dim result As Boolean = True
            For i = 1 To Names.Count - 1
                For j = i + 1 To Names.Count
                    If Names(i - 1).Trim.ToUpper = Names(j - 1).Trim.ToUpper Then
                        result = True
                        NonUniqueNames = AddWithNewLine(NonUniqueNames, Names(i - 1).Trim, False)
                    End If
                Next
            Next

            If result Then
                NonUniqueNames = String.Join("; ", NonUniqueNames.Trim.Split( _
                    New String() {vbCrLf}, StringSplitOptions.None))
            Else
                NonUniqueNames = ""
            End If

            Return result

        End Function

        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
            result = AddWithNewLine(result, _Children.GetAllBrokenRules, False).Trim
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Me.BrokenRulesCollection.WarningCount > 0 Then result = _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
            result = AddWithNewLine(result, _Children.GetAllWarnings, False).Trim
            Return result
        End Function

        Public Sub CheckAllRules()
            Me.ValidationRules.CheckRules()
            _Children.CheckRules()
        End Sub


        ''' <summary>
        ''' Helper method for UI. Trigers check of all the rules. 
        ''' It is used because databindings is not available for TreeView in .NET 2.0.
        ''' </summary>
        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            
        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

            ' no property level authorization is needed

            ' TODO: add authorization rules
            'AuthorizationRules.AllowWrite("", "")

        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.AccountList3")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.AccountList1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.AccountList3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            ' Consolidated report cannot be deleted without replacing it with a new one
            Return False
        End Function

#End Region

#Region " Factory Methods "

        ''' <summary>
        ''' Returns new instance of ConsolidatedReport of the type provided.
        ''' </summary>
        Public Shared Function GetNewConsolidatedReport() As ConsolidatedReport

            Dim result As New ConsolidatedReport
            result._Children = ConsolidatedReportItem.NewConsolidatedReportItem(True)
            result.ValidationRules.CheckRules()
            result.MarkNew()
            Return result

        End Function

        ''' <summary>
        ''' Gets a ConsolidatedReport of the type provided from database.
        ''' </summary>
        Public Shared Function GetConsolidatedReport() As ConsolidatedReport
            Return DataPortal.Fetch(Of ConsolidatedReport)(New Criteria())
        End Function

        Public Shared Function GetConsolidatedReportFromFile(ByVal FileName As String) As ConsolidatedReport
            Return New ConsolidatedReport(FileName)
        End Function


        Private Sub New(ByVal FileName As String)
            ' require use of factory methods
            Fetch(FileName)
        End Sub

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Public Sub New()
            End Sub
        End Class


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka finansinės atskaitomybės struktūrai gauti.")

            Dim myComm As New SQLCommand("FetchConsolidatedReport")

            Using myData As DataTable = myComm.Fetch
                _Children = ConsolidatedReportItem.GetConsolidatedReportItem(myData, 0)
            End Using

            Me.ValidationRules.CheckRules()

            Me.MarkOld()

        End Sub

        Private Sub Fetch(ByVal FileName As String)

            Dim xmldoc As New XmlDataDocument()

            Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)

                xmldoc.Load(fs)

                If xmldoc.ChildNodes.Count <> 2 OrElse xmldoc.ChildNodes(1).Name.Trim.ToLower _
                    <> "financialreportingstructure" OrElse xmldoc.ChildNodes(1).ChildNodes.Count <> 1 _
                    OrElse xmldoc.ChildNodes(1).ChildNodes(0).FirstChild.InnerText.Trim.ToLower <> _
                    FinancialStatementsRootName.Trim.ToLower Then Throw New Exception( _
                    "Klaida. Pasirinktame faile nėra saugomi finansinės atskaitomybės struktūros duomenys.")

                If xmldoc.ChildNodes(1).ChildNodes(0).Item("Children").ChildNodes.Count <> 2 OrElse _
                    xmldoc.ChildNodes(1).ChildNodes(0).Item("Children").ChildNodes(0).FirstChild. _
                    InnerText.Trim.ToLower <> BalanceStatementRootName.Trim.ToLower OrElse _
                    xmldoc.ChildNodes(1).ChildNodes(0).Item("Children").ChildNodes(1).FirstChild. _
                    InnerText.Trim.ToLower <> IncomeStatementRootName.Trim.ToLower Then Throw New Exception( _
                    "Klaida. Pasirinktame faile nėra saugomi finansinės atskaitomybės struktūros duomenys.")

                _Children = ConsolidatedReportItem.GetConsolidatedReportItem( _
                    xmldoc.ChildNodes(1).ChildNodes(0), 0)

                fs.Close()

            End Using

            MarkNew()

        End Sub

        Protected Overrides Sub DataPortal_Insert()
            ' ConsolidatedReport data due to the lack of tree databindings is allways rewriten
            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka finansinės atskaitomybės struktūrai nustatyti ar keisti.")
            SaveToDatabase()
        End Sub

        Protected Overrides Sub DataPortal_Update()
            ' ConsolidatedReport data due to the lack of tree databindings is allways rewriten
            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka finansinės atskaitomybės struktūrai nustatyti ar keisti.")
            SaveToDatabase()
        End Sub

        Friend Sub SaveToDatabase()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka finansinės atskaitomybės struktūrai nustatyti ar keisti.")

            ' only start new transaction if it doesn't already exists
            Dim TransactionExisted As Boolean = SQLUtilities.TransactionExists
            If Not TransactionExisted Then SQLUtilities.TransactionBegin()

            If IsNew Then

                ' new structure -> clear old structure
                Dim myComm As New SQLCommand("DeleteAllConsolidatedReportItems")
                myComm.Execute()

                myComm = New SQLCommand("UpdateAllConsolidatedReportAccounts")
                myComm.Execute()

            End If

            _Children.Save()

            ' only end transaction started by this method
            If Not TransactionExisted Then SQLUtilities.TransactionCommit()

        End Sub

#End Region

    End Class

End Namespace