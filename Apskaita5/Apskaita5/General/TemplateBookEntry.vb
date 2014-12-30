Namespace General

    <Serializable()> _
    Public Class TemplateBookEntry
        Inherits BusinessBase(Of TemplateBookEntry)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Account As Long = 0


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public Property Account() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Account
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _Account <> value Then
                    _Account = value
                    PropertyHasChanged()
                End If
            End Set
        End Property


        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _Account.ToString & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Account.ToString & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function

        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Account.ToString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Account", "sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("Account", "sąskaita", Validation.RuleSeverity.Warning))
        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewTemplateBookEntry() As TemplateBookEntry
            Dim result As New TemplateBookEntry
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetTemplateBookEntry(ByVal dr As DataRow) As TemplateBookEntry
            Return New TemplateBookEntry(dr)
        End Function

        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub

        Private Sub New(ByVal dr As DataRow)
            MarkAsChild()
            Fetch(dr)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Fetch(ByVal dr As DataRow)

            _ID = CIntSafe(dr.Item(0))
            _Account = CLongSafe(dr.Item(2))

            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parentlist As TemplateBookEntryList, _
            ByVal parent As TemplateJournalEntry)

            Dim myComm As New SQLCommand("InsertTemplateBookEntry")
            myComm.AddParam("?AA", _Account)
            myComm.AddParam("?LD", parent.ID)
            myComm.AddParam("?TP", ConvertEnumDatabaseStringCode(parentlist.Type))

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parentlist As TemplateBookEntryList, _
            ByVal parent As TemplateJournalEntry)

            Dim myComm As New SQLCommand("UpdateTemplateBookEntry")
            myComm.AddParam("?AA", _Account)
            myComm.AddParam("?TD", _ID)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteTemplateBookEntry")
            myComm.AddParam("?TD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

#End Region

    End Class

End Namespace