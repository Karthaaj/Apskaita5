Namespace General

    <Serializable()> _
    Public Class CompanyAccount
        Inherits BusinessBase(Of CompanyAccount)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Type As DefaultAccountType = DefaultAccountType.Bank
        Private _TypeHumanReadable As String = ""
        Private _Value As Long = 0


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property [Type]() As DefaultAccountType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
        End Property

        Public ReadOnly Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TypeHumanReadable.Trim
            End Get
        End Property

        Public Property Value() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Value
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _Value <> value Then
                    _Value = value
                    PropertyHasChanged()
                End If
            End Set
        End Property



        Public Function GetErrorString() As String _
        Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _TypeHumanReadable & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
        Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _TypeHumanReadable & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function

        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return _TypeHumanReadable & " = " & _Value.ToString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Value", "sąskaita", Validation.RuleSeverity.Warning))
        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewCompanyAccount(ByVal OfType As DefaultAccountType) As CompanyAccount
            Dim result As New CompanyAccount
            result._Type = OfType
            result._TypeHumanReadable = ConvertEnumHumanReadable(OfType)
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetCompanyAccount(ByVal dr As DataRow) As CompanyAccount
            Return New CompanyAccount(dr)
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

            _ID = CIntSafe(dr.Item(0), 0)
            _Type = ConvertEnumDatabaseCode(Of DefaultAccountType)(CIntSafe(dr.Item(1), 0))
            _TypeHumanReadable = ConvertEnumHumanReadable(_Type)
            _Value = CIntSafe(dr.Item(2), 0)

            ValidationRules.CheckRules()
            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parent As CompanyAccountList)

            Dim myComm As New SQLCommand("InsertCompanyAccount")
            myComm.AddParam("?AA", ConvertEnumDatabaseCode(_Type))
            myComm.AddParam("?AB", _Value)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As CompanyAccountList)

            Dim myComm As New SQLCommand("UpdateCompanyAccount")
            myComm.AddParam("?AB", _Value)
            myComm.AddParam("?AD", _ID)

            myComm.Execute()

            MarkOld()

        End Sub

#End Region

    End Class

End Namespace