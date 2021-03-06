Imports ApskaitaObjects.Attributes
Imports ApskaitaObjects.My.Resources
Imports ApskaitaObjects.Documents

Namespace Goods

    ''' <summary>
    ''' Represents a helper object that contains information about a goods state 
    ''' at the goods operation date (aggregated information).
    ''' </summary>
    ''' <remarks>Should only be used as a child of a goods operation.</remarks>
    <Serializable()> _
    Public NotInheritable Class GoodsSummary
        Inherits ReadOnlyBase(Of GoodsSummary)

#Region " Business Methods "

        Private ReadOnly _Guid As Guid = Guid.NewGuid()
        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _MeasureUnit As String = ""
        Private _GroupName As String = ""
        Private _DefaultWarehouse As WarehouseInfo
        Private _DefaultWarehouseName As String = ""
        Private _AccountGeneral As Long = 0
        Private _AccountSalesNetCosts As Long = 0
        Private _AccountPurchases As Long = 0
        Private _AccountDiscounts As Long = 0
        Private _AccountValueReduction As Long = 0
        Private _AccountSalesIncome As Long = 0
        Private _PriceSale As Double = 0
        Private _PricePurchase As Double = 0
        Private _DefaultVatRateSales As Double = 0
        Private _DeclarationSchemaSales As VatDeclarationSchemaInfo = Nothing
        Private _DefaultVatRatePurchase As Double = 0
        Private _DeclarationSchemaPurchase As VatDeclarationSchemaInfo = Nothing
        Private _AccountingMethod As GoodsAccountingMethod = GoodsAccountingMethod.Persistent
        Private _ValuationMethod As GoodsValuationMethod = GoodsValuationMethod.FIFO
        Private _AccountingMethodHumanReadable As String = Utilities.ConvertLocalizedName(GoodsAccountingMethod.Persistent)
        Private _ValuationMethodHumanReadable As String = Utilities.ConvertLocalizedName(GoodsValuationMethod.FIFO)


        ''' <summary>
        ''' Gets an ID of the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.ID">GoodsItem.ID</see>.</remarks>
        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        ''' <summary>
        ''' Gets a name of the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.Name">GoodsItem.Name</see>.</remarks>
        Public ReadOnly Property Name() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Name.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets a measure unit of the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.MeasureUnit">GoodsItem.MeasureUnit</see>.</remarks>
        Public ReadOnly Property MeasureUnit() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _MeasureUnit.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets a name of custom goods group that the goods are assigned to.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.Group">GoodsItem.Group</see>.</remarks>
        Public ReadOnly Property GroupName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _GroupName.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets a default warehouse for the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.DefaultWarehouse">GoodsItem.DefaultWarehouse</see>.</remarks>
        Public ReadOnly Property DefaultWarehouse() As WarehouseInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultWarehouse
            End Get
        End Property

        ''' <summary>
        ''' Gets a name of the default warehouse for the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.DefaultWarehouse">GoodsItem.DefaultWarehouse</see>.</remarks>
        Public ReadOnly Property DefaultWarehouseName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultWarehouseName
            End Get
        End Property

        ''' <summary>
        ''' Gets an ID of the default warehouse for the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.DefaultWarehouse">GoodsItem.DefaultWarehouse</see>.</remarks>
        Public ReadOnly Property DefaultWarehouseID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _DefaultWarehouse Is Nothing OrElse _DefaultWarehouse.IsEmpty Then Return 0
                Return _DefaultWarehouse.ID
            End Get
        End Property

        ''' <summary>
        ''' Gets an <see cref="General.Account.ID">account</see> of the default warehouse 
        ''' for the goods.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="Warehouse.WarehouseAccount">Warehouse.WarehouseAccount</see>.</remarks>
        Public ReadOnly Property AccountGeneral() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountGeneral
            End Get
        End Property

        ''' <summary>
        ''' Gets an <see cref="General.Account.ID">account</see> that is used for
        ''' the value of goods discarded (sold etc.). 
        ''' If the accounting method is set to<see cref="GoodsAccountingMethod.Periodic">
        ''' Periodic</see>, this account is fixed and mainly used by an <see cref="GoodsComplexOperationInventorization">
        ''' inventorization</see> operation (also in some cases by discount and additional costs). 
        ''' If the accounting method is set to<see cref="GoodsAccountingMethod.Persistent">
        ''' Persistent</see>, this account is used as a default goods discard costs
        ''' account by almost every operation, i.e. an operation can override it.
        ''' </summary>
        ''' <remarks>See methodology for BAS No 9 ""Stores"" para. 5.2 and 40.
        ''' Corresponds to <see cref="GoodsItem.AccountSalesNetCosts">GoodsItem.AccountSalesNetCosts</see>,
        ''' could be prospectively changed by a <see cref="GoodsOperationAccountChange">GoodsOperationAccountChange</see>.</remarks>
        Public ReadOnly Property AccountSalesNetCosts() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountSalesNetCosts
            End Get
        End Property

        ''' <summary>
        ''' Gets an <see cref="General.Account.ID">account</see> that is used for
        ''' the value of goods received (bought) by the <see cref="GoodsAccountingMethod.Periodic">
        ''' periodic accounting method</see>, not applicable for persistent accounting method,
        ''' that uses <see cref="Warehouse.WarehouseAccount">warehouse account</see>
        ''' for the same purpose.
        ''' </summary>
        ''' <remarks>See methodology for BAS No 9 ""Stores"" para. 8.
        ''' Corresponds to <see cref="GoodsItem.AccountPurchases">GoodsItem.AccountPurchases</see>,
        ''' could be prospectively changed by a <see cref="GoodsOperationAccountChange">GoodsOperationAccountChange</see>.</remarks>
        Public ReadOnly Property AccountPurchases() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountPurchases
            End Get
        End Property

        ''' <summary>
        ''' Gets an <see cref="General.Account.ID">account</see> that is used for
        ''' discounts received by the <see cref="GoodsAccountingMethod.Periodic">
        ''' periodic accounting method</see>, not applicable for persistent accounting method.
        ''' </summary>
        ''' <remarks>See methodology for BAS No 9 ""Stores"" para. 5.2.
        ''' Corresponds to <see cref="GoodsItem.AccountDiscounts">GoodsItem.AccountDiscounts</see>,
        ''' could be prospectively changed by a <see cref="GoodsOperationAccountChange">GoodsOperationAccountChange</see>.</remarks>
        Public ReadOnly Property AccountDiscounts() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountDiscounts
            End Get
        End Property

        ''' <summary>
        ''' Gets an <see cref="General.Account.ID">account</see> that is used for
        ''' goods value reduction (when goods are revalued to match market prices). 
        ''' Handling of this account does not depend on the accounting method.
        ''' </summary>
        ''' <remarks>See methodology for BAS No 9 ""Stores"" para. 24 - 33.
        ''' Corresponds to <see cref="GoodsItem.AccountDiscounts">GoodsItem.AccountDiscounts</see>,
        ''' could be prospectively changed by a <see cref="GoodsOperationAccountChange">GoodsOperationAccountChange</see>.</remarks>
        Public ReadOnly Property AccountValueReduction() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountValueReduction
            End Get
        End Property

        ''' <summary>
        ''' Gets a default <see cref="General.Account.ID">account</see> that is 
        ''' used for the goods sales income, i.e. an operation can override it.
        ''' </summary>
        ''' <remarks>Data is stored in database field goods.AccountSalesIncome.</remarks>
        <AccountField(ValueRequiredLevel.Optional, False, 5)> _
        Public ReadOnly Property AccountSalesIncome() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountSalesIncome
            End Get
        End Property

        ''' <summary>
        ''' Gets a default goods sale price per unit in the base currency.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.RegionalPrices">GoodsItem.RegionalPrices</see>
        ''' entry for the base currency.</remarks>
        <DoubleField(ValueRequiredLevel.Optional, False, ROUNDUNITINVOICEMADE)> _
        Public ReadOnly Property PriceSale() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_PriceSale, ROUNDUNITINVOICEMADE)
            End Get
        End Property

        ''' <summary>
        ''' Gets a default goods purchase price per unit in the base currency.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.RegionalPrices">GoodsItem.RegionalPrices</see>
        ''' entry for the base currency.</remarks>
        <DoubleField(ValueRequiredLevel.Optional, False, ROUNDUNITINVOICERECEIVED)> _
        Public ReadOnly Property PricePurchase() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_PricePurchase, ROUNDUNITINVOICERECEIVED)
            End Get
        End Property

        ''' <summary>
        ''' Gets a default VAT rate for the goods beeing sold.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.DefaultVatRateSales">GoodsItem.DefaultVatRateSales</see>.</remarks>
        <DoubleField(ValueRequiredLevel.Optional, False, 2)> _
        Public ReadOnly Property DefaultVatRateSales() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRateSales)
            End Get
        End Property

        ''' <summary>
        ''' Gets the applicable VAT declaration schema for the goods sold.
        ''' </summary>
        ''' <remarks>Value is stored in the database table goods.DeclarationSchemaIDSales.</remarks>
        <VatDeclarationSchemaField(ValueRequiredLevel.Optional, TradedItemType.Sales)> _
        Public ReadOnly Property DeclarationSchemaSales() As VatDeclarationSchemaInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DeclarationSchemaSales
            End Get
        End Property

        ''' <summary>
        ''' Gets a default VAT rate for the goods beeing purchased.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.DefaultVatRatePurchase">GoodsItem.DefaultVatRatePurchase</see>.</remarks>
        <DoubleField(ValueRequiredLevel.Optional, False, 2)> _
        Public ReadOnly Property DefaultVatRatePurchase() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRatePurchase)
            End Get
        End Property

        ''' <summary>
        ''' Gets the applicable VAT declaration schema for the goods bought.
        ''' </summary>
        ''' <remarks>Value is stored in the database table goods.DeclarationSchemaIDPurchase.</remarks>
        <VatDeclarationSchemaField(ValueRequiredLevel.Optional, TradedItemType.All)> _
        Public ReadOnly Property DeclarationSchemaPurchase() As VatDeclarationSchemaInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DeclarationSchemaPurchase
            End Get
        End Property

        ''' <summary>
        ''' Gets a goods accounting method (periodic/persistent).
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.AccountingMethod">GoodsItem.AccountingMethod</see>.
        ''' Cannot be changed after the first operation with the goods.</remarks>
        Public ReadOnly Property AccountingMethod() As GoodsAccountingMethod
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountingMethod
            End Get
        End Property

        ''' <summary>
        ''' Gets a goods valuation method (FIFO, LIFO, average, etc.).
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.AccountingMethod">GoodsItem.AccountingMethod</see>,
        ''' Could be prospectively changed by a <see cref="GoodsOperationValuationMethod">
        ''' valuation method change operation</see>.</remarks>
        Public ReadOnly Property ValuationMethod() As GoodsValuationMethod
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ValuationMethod
            End Get
        End Property

        ''' <summary>
        ''' Gets a goods accounting method (periodic/persistent) as a localized 
        ''' human readable string.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.AccountingMethod">GoodsItem.AccountingMethod</see>.
        ''' Cannot be changed after the first operation with the goods.</remarks>
        Public ReadOnly Property AccountingMethodHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountingMethodHumanReadable.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets a goods valuation method (FIFO, LIFO, average, etc.) as a localized 
        ''' human readable string.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="GoodsItem.AccountingMethod">GoodsItem.AccountingMethod</see>,
        ''' Could be prospectively changed by a <see cref="GoodsOperationValuationMethod">
        ''' valuation method change operation</see>.</remarks>
        Public ReadOnly Property ValuationMethodHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ValuationMethodHumanReadable.Trim
            End Get
        End Property



        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return String.Format(My.Resources.Goods_GoodsSummary_ToString, _Name)
        End Function

#End Region

#Region " Factory Methods "

        ''' <summary>
        ''' Gets a new GoodsSummary instance for a new goods operation.
        ''' </summary>
        ''' <param name="goodsID">an <see cref="GoodsItem.ID">ID of the goods</see>
        ''' that the goods operation operates with.</param>
        ''' <remarks></remarks>
        Friend Shared Function NewGoodsSummary(ByVal goodsID As Integer) As GoodsSummary
            Return New GoodsSummary(goodsID)
        End Function

        ''' <summary>
        ''' Gets a GoodsSummary instance for an existing goods operation.
        ''' </summary>
        ''' <param name="dr">database query result</param>
        ''' <param name="offset">an offset by which the GoodsSummary data
        ''' is offseted in the query result</param>
        ''' <remarks></remarks>
        Friend Shared Function GetGoodsSummary(ByVal dr As DataRow, ByVal offset As Integer) As GoodsSummary
            Return New GoodsSummary(dr, offset)
        End Function


        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal goodsID As Integer)
            Create(goodsID)
        End Sub

        Private Sub New(ByVal dr As DataRow, ByVal offset As Integer)
            Fetch(dr, offset)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Create(ByVal goodsID As Integer)

            If Not goodsID > 0 Then
                Throw New ArgumentNullException("goodsID", Goods_GoodsSummary_GoodsIdNull)
            End If

            Dim myComm As New SQLCommand("FetchGoodsSummary")
            myComm.AddParam("?GD", goodsID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception(String.Format( _
                    My.Resources.Common_ObjectNotFound, My.Resources.Goods_GoodsItem_TypeName, _
                    goodsID.ToString()))

                Fetch(myData.Rows(0), 0)

            End Using

        End Sub

        Private Sub Fetch(ByVal dr As DataRow, ByVal offset As Integer)

            _ID = CIntSafe(dr.Item(0 + offset), 0)
            _Name = CStrSafe(dr.Item(1 + offset)).Trim
            _MeasureUnit = CStrSafe(dr.Item(2 + offset)).Trim
            _AccountSalesNetCosts = CLongSafe(dr.Item(3 + offset), 0)
            _AccountPurchases = CLongSafe(dr.Item(4 + offset), 0)
            _AccountDiscounts = CLongSafe(dr.Item(5 + offset), 0)
            _AccountValueReduction = CLongSafe(dr.Item(6 + offset), 0)
            _PriceSale = CDblSafe(dr.Item(7 + offset), ROUNDUNITINVOICEMADE, 0)
            _PricePurchase = CDblSafe(dr.Item(8 + offset), ROUNDUNITINVOICERECEIVED, 0)
            _DefaultVatRateSales = CDblSafe(dr.Item(9 + offset), 2, 0)
            _DefaultVatRatePurchase = CDblSafe(dr.Item(10 + offset), 2, 0)
            _GroupName = CStrSafe(dr.Item(11 + offset)).Trim
            _AccountingMethod = Utilities.ConvertDatabaseID(Of GoodsAccountingMethod) _
                (CIntSafe(dr.Item(12 + offset), 0))
            _ValuationMethod = Utilities.ConvertDatabaseID(Of GoodsValuationMethod) _
                (CIntSafe(dr.Item(13 + offset), 0))
            _AccountingMethodHumanReadable = Utilities.ConvertLocalizedName(_AccountingMethod)
            _ValuationMethodHumanReadable = Utilities.ConvertLocalizedName(_ValuationMethod)
            _AccountSalesIncome = CLongSafe(dr.Item(14 + offset), 0)
            _DefaultWarehouse = WarehouseInfo.GetWarehouseInfo(dr, 15 + offset)
            _DefaultWarehouseName = _DefaultWarehouse.ToString
            _AccountGeneral = _DefaultWarehouse.WarehouseAccount
            _DeclarationSchemaSales = VatDeclarationSchemaInfo. _
                GetVatDeclarationSchemaInfo(dr, 20 + offset)
            _DeclarationSchemaPurchase = VatDeclarationSchemaInfo. _
                GetVatDeclarationSchemaInfo(dr, 27 + offset)

        End Sub

#End Region

    End Class

End Namespace