Namespace Goods

    <Serializable()> _
    Public Class ProductionCostItem
        Inherits BusinessBase(Of ProductionCostItem)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Account As Long = 0
        Private _Amount As Double = 0


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

        Public Property Amount() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Amount)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_Amount) <> CRound(value) Then
                    _Amount = CRound(value)
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
            Return _Account.ToString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Account", "sąnaudų apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Amount", "kiekis"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewProductionCostItem() As ProductionCostItem
            Dim result As New ProductionCostItem
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetProductionCostItem(ByVal dr As DataRow) As ProductionCostItem
            Return New ProductionCostItem(dr)
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

            _ID = CIntSafe(dr.Item(1), 0)
            _Account = CLongSafe(dr.Item(2), 0)
            _Amount = CDblSafe(dr.Item(3), 6, 0)

            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parent As ProductionCalculation)

            Dim myComm As New SQLCommand("InsertProductionItem")
            AddWithParams(myComm)
            myComm.AddParam("?AA", parent.ID)
            myComm.AddParam("?AB", ConvertEnumDatabaseStringCode(ProductionComponentType.Costs))

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As ProductionCalculation)

            Dim myComm As New SQLCommand("UpdateProductionItem")
            myComm.AddParam("?CD", _ID)
            AddWithParams(myComm)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteProductionItem")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AC", 0)
            myComm.AddParam("?AD", _Account)
            myComm.AddParam("?AE", CRound(_Amount, 6))
            myComm.AddParam("?AF", 0)

        End Sub

#End Region

    End Class

End Namespace