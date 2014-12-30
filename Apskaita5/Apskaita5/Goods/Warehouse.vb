Namespace Goods

    <Serializable()> _
    Public Class Warehouse
        Inherits BusinessBase(Of Warehouse)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _Location As String = ""
        Private _Description As String = ""
        Private _IsProduction As Boolean = False
        Private _IsObsolete As Boolean = False
        Private _LastOperationDate As Date = Date.MinValue
        Private _ContainsGoods As Boolean = False
        Private _IsProductionOld As Boolean = False
        Private _WarehouseAccount As Long = 0
        Private _WarehouseAccountOld As Long = 0


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public Property Name() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Name.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Name.Trim <> value.Trim Then
                    _Name = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Location() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Location.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Location.Trim <> value.Trim Then
                    _Location = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Description() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Description.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Description.Trim <> value.Trim Then
                    _Description = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property WarehouseAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseAccount
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _WarehouseAccount <> value Then
                    If _ContainsGoods Then
                        PropertyHasChanged()
                        Exit Property
                    End If
                    _WarehouseAccount = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IsProduction() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsProduction
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _ContainsGoods Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _IsProduction <> value Then
                    _IsProduction = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IsObsolete() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsObsolete
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _IsObsolete <> value Then
                    _IsObsolete = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property LastOperationDate() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _LastOperationDate = Date.MinValue Then Return ""
                Return _LastOperationDate.ToString("yyyy-MM-dd")
            End Get
        End Property

        Public ReadOnly Property ContainsGoods() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ContainsGoods
            End Get
        End Property


        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _Name & "': " & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Name & "' gali būti klaida: " & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function

        Friend Sub RestoreIsProduction()
            _IsProduction = _IsProductionOld
            _WarehouseAccount = _WarehouseAccountOld
        End Sub


        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "sandėlio pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("Name", "sandėlio pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("WarehouseAccount", "apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("WarehouseAccount", "apskaitos sąskaita"))
        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewWarehouse() As Warehouse
            Dim result As New Warehouse
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function GetWarehouse(ByVal dr As DataRow) As Warehouse
            Return New Warehouse(dr)
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

            _ID = CIntSafe(dr.item(0), 0)
            _Name = CStrSafe(dr.Item(1)).Trim
            _Location = CStrSafe(dr.Item(2)).Trim
            _Description = CStrSafe(dr.Item(3)).Trim
            _IsProduction = ConvertDbBoolean(CIntSafe(dr.Item(4), 0))
            _IsProductionOld = _IsProduction
            _IsObsolete = ConvertDbBoolean(CIntSafe(dr.Item(5), 0))
            _WarehouseAccount = CLongSafe(dr.Item(6), 0)
            _LastOperationDate = CDateSafe(dr.Item(7), Date.MinValue)
            _ContainsGoods = (_LastOperationDate <> Date.MinValue)

            MarkOld()

        End Sub

        Friend Sub Insert(ByVal parent As WarehouseList)

            Dim myComm As New SQLCommand("InsertWarehouse")
            AddWithParams(myComm)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As WarehouseList)

            Dim myComm As New SQLCommand("UpdateWarehouse")
            myComm.AddParam("?WD", _ID)
            AddWithParams(myComm)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteWarehouse")
            myComm.AddParam("?WD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub


        Private Sub AddWithParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?AA", _Name.Trim)
            myComm.AddParam("?AB", _Location.Trim)
            myComm.AddParam("?AC", _Description.Trim)
            myComm.AddParam("?AD", ConvertDbBoolean(_IsProduction))
            myComm.AddParam("?AE", ConvertDbBoolean(_IsObsolete))
            myComm.AddParam("?AF", _WarehouseAccount)
        End Sub

#End Region

    End Class

End Namespace