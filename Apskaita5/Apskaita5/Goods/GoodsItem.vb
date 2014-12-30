Imports ApskaitaObjects.Documents
Namespace Goods

    <Serializable()> _
    Public Class GoodsItem
        Inherits BusinessBase(Of GoodsItem)
        Implements IIsDirtyEnough, IRegionalDataObject

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _MeasureUnit As String = "Vnt."
        Private _AccountSalesNetCosts As Long = 0
        Private _AccountPurchases As Long = 0
        Private _AccountDiscounts As Long = 0
        Private _AccountValueReduction As Long = 0
        Private _DefaultVatRateSales As Double = 0
        Private _DefaultVatRatePurchase As Double = 0
        Private _Group As GoodsGroupInfo = Nothing
        Private _OldAccountingMethod As GoodsAccountingMethod = GoodsAccountingMethod.Persistent
        Private _AccountingMethod As GoodsAccountingMethod = GoodsAccountingMethod.Persistent
        Private _AccountingMethodHumanReadable As String = convertenumhumanreadable(_AccountingMethod)
        Private _DefaultValuationMethod As GoodsValuationMethod = GoodsValuationMethod.FIFO
        Private _DefaultValuationMethodHumanReadable As String = convertenumhumanreadable(_DefaultValuationMethod)
        Private _TradedType As TradedItemType = TradedItemType.All
        Private _TradedTypeHumanReadable As String = convertenumhumanreadable(_TradedType)
        Private _InternalCode As String = ""
        Private _Barcode As String = ""
        Private _DefaultWarehouse As WarehouseInfo = Nothing
        Private _IsObsolete As Boolean = False
        Private _RegionalContents As RegionalContentList
        Private _RegionalPrices As RegionalPriceList
        Private _IsInUse As Boolean = False
        Private _PriceCutsExist As Boolean = False
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now

        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _RegionalContentsSortedList As Csla.SortedBindingList(Of RegionalContent) = Nothing
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _RegionalPricesSortedList As Csla.SortedBindingList(Of RegionalPrice) = Nothing

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

        Public Property MeasureUnit() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _MeasureUnit.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _MeasureUnit.Trim <> value.Trim Then
                    _MeasureUnit = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountSalesNetCosts() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountSalesNetCosts
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                If _IsInUse AndAlso _accountingmethod = GoodsAccountingMethod.Periodic Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _AccountSalesNetCosts <> value Then
                    _AccountSalesNetCosts = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountPurchases() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountPurchases
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                If _IsInUse AndAlso _accountingmethod = GoodsAccountingMethod.Periodic Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _AccountPurchases <> value Then
                    _AccountPurchases = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountDiscounts() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountDiscounts
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                If _IsInUse AndAlso _accountingmethod = GoodsAccountingMethod.Periodic Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _AccountDiscounts <> value Then
                    _AccountDiscounts = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountValueReduction() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountValueReduction
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                If _PriceCutsExist Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _AccountValueReduction <> value Then
                    _AccountValueReduction = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property DefaultVatRateSales() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRateSales)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_DefaultVatRateSales) <> CRound(value) Then
                    _DefaultVatRateSales = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property DefaultVatRatePurchase() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRatePurchase)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_DefaultVatRatePurchase) <> CRound(value) Then
                    _DefaultVatRatePurchase = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Group() As GoodsGroupInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Group
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As GoodsGroupInfo)
                CanWriteProperty(True)
                If Not (_Group Is Nothing AndAlso value Is Nothing) AndAlso Not _
                    (Not _Group Is Nothing AndAlso Not value Is Nothing AndAlso _Group.id = value.id) Then
                    _Group = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property OldAccountingMethod() As GoodsAccountingMethod
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldAccountingMethod
            End Get
        End Property

        Public Property AccountingMethod() As GoodsAccountingMethod
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountingMethod
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As GoodsAccountingMethod)
                If _IsInUse Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _AccountingMethod <> value Then
                    _AccountingMethod = value
                    PropertyHasChanged()
                    _AccountingMethodHumanReadable = convertenumhumanreadable(_AccountingMethod)
                    PropertyHasChanged("AccountingMethodHumanReadable")
                End If
            End Set
        End Property

        Public Property AccountingMethodHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountingMethodHumanReadable.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If _IsInUse Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If convertenumhumanreadable(Of GoodsAccountingMethod)(value.Trim) <> _AccountingMethod Then
                    _AccountingMethod = convertenumhumanreadable(Of GoodsAccountingMethod)(value.Trim)
                    _AccountingMethodHumanReadable = convertenumhumanreadable(_AccountingMethod)
                    PropertyHasChanged()
                    PropertyHasChanged("AccountingMethod")
                End If
            End Set
        End Property

        Public Property DefaultValuationMethod() As GoodsValuationMethod
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultValuationMethod
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As GoodsValuationMethod)
                If _IsInUse Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If _DefaultValuationMethod <> value Then
                    _DefaultValuationMethod = value
                    PropertyHasChanged()
                    _DefaultValuationMethodHumanReadable = convertenumhumanreadable(_DefaultValuationMethod)
                    PropertyHasChanged("DefaultValuationMethodHumanReadable")
                End If
            End Set
        End Property

        Public Property DefaultValuationMethodHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultValuationMethodHumanReadable.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If _IsInUse Then
                    PropertyHasChanged()
                    Exit Property
                End If
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If convertenumhumanreadable(Of GoodsValuationMethod)(value.Trim) <> _DefaultValuationMethod Then
                    _DefaultValuationMethod = convertenumhumanreadable(Of GoodsValuationMethod)(value.Trim)
                    _DefaultValuationMethodHumanReadable = convertenumhumanreadable(_DefaultValuationMethod)
                    PropertyHasChanged()
                    PropertyHasChanged("DefaultValuationMethod")
                End If
            End Set
        End Property

        Public Property TradedType() As TradedItemType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TradedType
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As TradedItemType)
                CanWriteProperty(True)
                If _TradedType <> value Then
                    _TradedType = value
                    PropertyHasChanged()
                    _TradedTypeHumanReadable = convertenumhumanreadable(_TradedType)
                    PropertyHasChanged("TradedTypeHumanReadable")
                End If
            End Set
        End Property

        Public Property TradedTypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TradedTypeHumanReadable.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If convertenumhumanreadable(Of TradedItemType)(value.Trim) <> _TradedType Then
                    _TradedType = convertenumhumanreadable(Of TradedItemType)(value.Trim)
                    _TradedTypeHumanReadable = convertenumhumanreadable(_TradedType)
                    PropertyHasChanged()
                    PropertyHasChanged("TradedType")
                End If
            End Set
        End Property

        Public Property InternalCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InternalCode.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _InternalCode.Trim <> value.Trim Then
                    _InternalCode = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Barcode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Barcode.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Barcode.Trim <> value.Trim Then
                    _Barcode = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property DefaultWarehouse() As WarehouseInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultWarehouse
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WarehouseInfo)
                CanWriteProperty(True)
                If Not (_DefaultWarehouse Is Nothing AndAlso value Is Nothing) AndAlso Not _
                    (Not _DefaultWarehouse Is Nothing AndAlso Not value Is Nothing AndAlso _
                    _DefaultWarehouse.ID = value.ID) Then
                    _DefaultWarehouse = value
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

        Public ReadOnly Property RegionalContents() As RegionalContentList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _RegionalContents
            End Get
        End Property

        Public ReadOnly Property RegionalPrices() As RegionalPriceList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _RegionalPrices
            End Get
        End Property

        Public ReadOnly Property RegionalContentsSorted() As Csla.SortedBindingList(Of RegionalContent)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _RegionalContentsSortedList Is Nothing Then _RegionalContentsSortedList _
                    = New Csla.SortedBindingList(Of RegionalContent)(_RegionalContents)
                Return _RegionalContentsSortedList
            End Get
        End Property

        Public ReadOnly Property RegionalPricesSorted() As Csla.SortedBindingList(Of RegionalPrice)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _RegionalPricesSortedList Is Nothing Then _RegionalPricesSortedList _
                    = New Csla.SortedBindingList(Of RegionalPrice)(_RegionalPrices)
                Return _RegionalPricesSortedList
            End Get
        End Property

        Public ReadOnly Property IsInUse() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsInUse
            End Get
        End Property

        Public ReadOnly Property PriceCutsExist() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PriceCutsExist
            End Get
        End Property

        Public ReadOnly Property InsertDate() As DateTime
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InsertDate
            End Get
        End Property

        Public ReadOnly Property UpdateDate() As DateTime
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _UpdateDate
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_Name.Trim) _
                    OrElse Not String.IsNullOrEmpty(_MeasureUnit.Trim) _
                    OrElse Not String.IsNullOrEmpty(_AccountingMethodHumanReadable.Trim) _
                    OrElse Not String.IsNullOrEmpty(_DefaultValuationMethodHumanReadable.Trim) _
                    OrElse Not String.IsNullOrEmpty(_TradedTypeHumanReadable.Trim) _
                    OrElse Not String.IsNullOrEmpty(_InternalCode.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Barcode.Trim) _
                    OrElse _RegionalContents.count > 0 _
                    OrElse _RegionalPrices.count > 0)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _RegionalContents.IsDirty OrElse _RegionalPrices.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _RegionalContents.IsValid AndAlso _RegionalPrices.IsValid
            End Get
        End Property


        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
            If Not _RegionalContents.IsValid Then result = AddWithNewLine(result, _
                _RegionalContents.GetAllBrokenRules, False)
            If Not _RegionalPrices.IsValid Then result = AddWithNewLine(result, _
                _RegionalPrices.GetAllBrokenRules, False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If MyBase.BrokenRulesCollection.WarningCount > 0 Then _
                result = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
            result = AddWithNewLine(result, _RegionalContents.GetAllWarnings, False)
            result = AddWithNewLine(result, _RegionalPrices.GetAllWarnings, False)
            If _RegionalContents.Count < 1 Then result = AddWithNewLine(result, _
                "Neįvesti prekės pavadinimai nė viena kalba.", False)
            If _RegionalPrices.Count < 1 Then result = AddWithNewLine(result, _
                "Neįvestos prekės kainos nė viena valiuta.", False)
            Return result
        End Function


        Public Overrides Function Save() As GoodsItem

            If IsNew AndAlso Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            If Not IsNew AndAlso Not CanEditObject() Then Throw New system.security.securityexception( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbcrlf & Me.GetAllBrokenRules)

            Dim result As GoodsItem = MyBase.Save
            HelperLists.GoodsInfoList.InvalidateCache()
            Return result

        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "prekės pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("MeasureUnit", "prekės mato vnt."))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountValueReduction", "nukainojimo sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("DefaultVatRateSales", _
                "pardavimo PVM tarifas", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("DefaultVatRatePurchase", _
                "pirkimo PVM tarifas", Validation.RuleSeverity.Warning))

            ValidationRules.AddRule(AddressOf AccountSalesNetCostsValidation, _
                New Validation.RuleArgs("AccountSalesNetCosts"))
            ValidationRules.AddRule(AddressOf AccountPurchasesValidation, _
                New Validation.RuleArgs("AccountPurchases"))
            ValidationRules.AddRule(AddressOf AccountDiscountsValidation, _
                New Validation.RuleArgs("AccountDiscounts"))

            ValidationRules.AddDependantProperty("AccountingMethod", "AccountSalesNetCosts", False)
            ValidationRules.AddDependantProperty("AccountingMethod", "AccountPurchases", False)
            ValidationRules.AddDependantProperty("AccountingMethod", "AccountDiscounts", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property AccountSalesNetCosts is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountSalesNetCostsValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsItem = DirectCast(target, GoodsItem)

            If ValObj._AccountingMethod = GoodsAccountingMethod.Periodic AndAlso _
                Not ValObj._AccountSalesNetCosts > 0 Then
                e.Description = "Nenurodyta pardavimų sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf Not ValObj._AccountSalesNetCosts > 0 Then
                e.Description = "Nenurodyta pardavimų sąskaita."
                e.Severity = Validation.RuleSeverity.Warning
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property AccountPurchases is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountPurchasesValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsItem = DirectCast(target, GoodsItem)

            If ValObj._AccountingMethod = GoodsAccountingMethod.Periodic AndAlso _
                Not ValObj._AccountPurchases > 0 Then
                e.Description = "Nenurodyta pirkimų sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property AccountDiscounts is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountDiscountsValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsItem = DirectCast(target, GoodsItem)

            If ValObj._AccountingMethod = GoodsAccountingMethod.Periodic AndAlso _
                Not ValObj._AccountDiscounts > 0 Then
                e.Description = "Nenurodyta gautų nuolaidų sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Goods.Goods2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.Goods1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.Goods2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.Goods3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.Goods3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewGoodsItem() As GoodsItem

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim result As GoodsItem = New GoodsItem
            result._RegionalContents = RegionalContentList.NewRegionalContentList
            result._RegionalPrices = RegionalPriceList.NewRegionalPriceList
            result.ValidationRules.CheckRules()
            Return result

        End Function

        Public Shared Function GetGoodsItem(ByVal nID As Integer) As GoodsItem
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")
            Return DataPortal.Fetch(Of GoodsItem)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteGoodsItem(ByVal id As Integer)
            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka sąskaitos duomenų ištrynimui.")
            DataPortal.Delete(New Criteria(id))
            HelperLists.GoodsInfoList.InvalidateCache()
        End Sub


        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _ID As Integer
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public Sub New(ByVal nID As Integer)
                _ID = nID
            End Sub
        End Class


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            Dim myComm As New SQLCommand("FetchGoodsItem")
            myComm.AddParam("?GD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception("Klaida. Prekė, kurio ID='" _
                    & criteria.ID & "', nerasta.)")

                Dim dr As DataRow = myData.Rows(0)

                _ID = CIntSafe(dr.Item(0), 0)
                _Name = CStrSafe(dr.Item(1)).Trim
                _MeasureUnit = CStrSafe(dr.Item(2)).Trim
                _AccountSalesNetCosts = CIntSafe(dr.Item(3), 0)
                _AccountPurchases = CIntSafe(dr.Item(4), 0)
                _AccountDiscounts = CIntSafe(dr.Item(5), 0)
                _AccountValueReduction = CIntSafe(dr.Item(6), 0)
                _DefaultVatRateSales = CDblSafe(dr.Item(7), 2, 0)
                _DefaultVatRatePurchase = CDblSafe(dr.Item(8), 2, 0)
                _AccountingMethod = ConvertEnumDatabaseCode(Of GoodsAccountingMethod)(CIntSafe(dr.Item(9), 1))
                _DefaultValuationMethod = ConvertEnumDatabaseCode(Of GoodsValuationMethod)(CIntSafe(dr.Item(10), 1))
                _TradedType = ConvertEnumDatabaseCode(Of TradedItemType)(CIntSafe(dr.Item(11), 0))
                _InternalCode = CStrSafe(dr.Item(12)).Trim
                _Barcode = CStrSafe(dr.Item(13)).Trim
                _IsObsolete = ConvertDbBoolean(CIntSafe(dr.Item(14), 0))
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(15), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(16), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _IsInUse = ConvertDbBoolean(CIntSafe(dr.Item(17), 0))
                _PriceCutsExist = ConvertDbBoolean(CIntSafe(dr.Item(18), 0))

                _RegionalContents = RegionalContentList.GetRegionalContentList(CStrSafe(dr.Item(19)).Trim)
                _RegionalPrices = RegionalPriceList.GetRegionalPriceList(CStrSafe(dr.Item(20)).Trim)

                _Group = GoodsGroupInfo.GetGoodsGroupInfo(dr, 21)
                _DefaultWarehouse = WarehouseInfo.GetWarehouseInfo(dr, 23)

                _AccountingMethodHumanReadable = ConvertEnumHumanReadable(_AccountingMethod)
                _DefaultValuationMethodHumanReadable = ConvertEnumHumanReadable(_DefaultValuationMethod)
                _TradedTypeHumanReadable = ConvertEnumHumanReadable(_TradedType)
                _OldAccountingMethod = _AccountingMethod

            End Using

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not IsGoodsNameUnique(_ID, _Name, _Group) Then Throw New Exception( _
                "Klaida. Tai pačiai grupei yra priskirta kita prekių rūšis tokiu pat pavadinimu.")

            DoInsert()

            MarkOld()

        End Sub

        Friend Sub DoInsert()

            Dim myComm As New SQLCommand("InsertGoodsItem")
            AddWithGeneralParams(myComm)
            AddWithMethodsParams(myComm)
            AddWithAccountsParams(myComm)
            myComm.AddParam("?AF", _AccountValueReduction)

            Dim transactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not transactionExisted Then DatabaseAccess.TransactionBegin()

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            RegionalContents.Update(Me)
            RegionalPrices.Update(Me)

            If Not transactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not IsGoodsNameUnique(_ID, _Name, _Group) Then Throw New Exception( _
                "Klaida. Tai pačiai grupei yra priskirta kita prekių rūšis tokiu pat pavadinimu.")

            CheckIfUpdateDateChanged()

            Dim myComm As SQLCommand
            If _IsInUse AndAlso _OldAccountingMethod = GoodsAccountingMethod.Persistent _
                AndAlso Not _PriceCutsExist Then
                ' everything except for methods
                myComm = New SQLCommand("UpdateGoodsItemLimited1")
                AddWithAccountsParams(myComm)
                myComm.AddParam("?AF", _AccountValueReduction)
            ElseIf _IsInUse AndAlso _OldAccountingMethod = GoodsAccountingMethod.Periodic _
                AndAlso Not _PriceCutsExist Then
                ' everything except for methods and accounts, but including price cut account
                myComm = New SQLCommand("UpdateGoodsItemLimited2")
                myComm.AddParam("?AF", _AccountValueReduction)
            ElseIf _IsInUse AndAlso _PriceCutsExist AndAlso _OldAccountingMethod _
                = GoodsAccountingMethod.Periodic Then
                ' everything except for methods and accounts
                myComm = New SQLCommand("UpdateGoodsItemLimited3")
            ElseIf _IsInUse AndAlso _PriceCutsExist AndAlso _OldAccountingMethod _
                = GoodsAccountingMethod.Persistent Then
                ' everything except for methods price cut account
                myComm = New SQLCommand("UpdateGoodsItemLimited4")
                AddWithAccountsParams(myComm)
            Else
                ' everything 
                myComm = New SQLCommand("UpdateGoodsItemFull")
                AddWithMethodsParams(myComm)
                AddWithAccountsParams(myComm)
                myComm.AddParam("?AF", _AccountValueReduction)
            End If
            AddWithGeneralParams(myComm)
            myComm.AddParam("?CD", _ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            RegionalContents.Update(Me)
            RegionalPrices.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If AnyOperationExists(DirectCast(criteria, Criteria).ID) Then Throw New Exception( _
                "Klaida. Su šia prekių rūšimi yra registruota operacijų.")

            Dim myComm As SQLCommand = New SQLCommand("DeleteGoodsItem")
            myComm.AddParam("?CD", DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            RegionalContentList.Delete(DirectCast(criteria, Criteria).ID, 1)
            RegionalPriceList.Delete(DirectCast(criteria, Criteria).ID, 1)

            DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub

        Private Sub AddWithGeneralParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _Name.Trim)
            myComm.AddParam("?AB", _MeasureUnit.Trim)
            myComm.AddParam("?AG", CRound(_DefaultVatRateSales))
            myComm.AddParam("?AH", CRound(_DefaultVatRatePurchase))
            myComm.AddParam("?AK", ConvertEnumDatabaseCode(_TradedType))
            myComm.AddParam("?AL", _InternalCode.Trim)
            myComm.AddParam("?AM", _Barcode.Trim)
            myComm.AddParam("?AN", ConvertDbBoolean(_IsObsolete))
            If Not _Group Is Nothing AndAlso _Group.ID > 0 Then
                myComm.AddParam("?AO", _Group.ID)
            Else
                myComm.AddParam("?AO", 0)
            End If
            If Not _DefaultWarehouse Is Nothing AndAlso _DefaultWarehouse.ID > 0 Then
                myComm.AddParam("?AQ", _DefaultWarehouse.ID)
            Else
                myComm.AddParam("?AQ", 0)
            End If

            myComm.AddParam("?AP", _RegionalPrices.GetLithPriceSale)
            myComm.AddParam("?AR", _RegionalPrices.GetLithPricePurchase)

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?AT", _UpdateDate.ToUniversalTime)

        End Sub

        Private Sub AddWithMethodsParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?AI", ConvertEnumDatabaseCode(_AccountingMethod))
            myComm.AddParam("?AJ", ConvertEnumDatabaseCode(_DefaultValuationMethod))
        End Sub

        Private Sub AddWithAccountsParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?AC", _AccountSalesNetCosts)
            myComm.AddParam("?AD", _AccountPurchases)
            myComm.AddParam("?AE", _AccountDiscounts)
        End Sub


        Private Shared Function IsGoodsNameUnique(ByVal GID As Integer, ByVal nName As String, _
            ByVal nGroup As GoodsGroupInfo) As Boolean

            Dim myComm As New SQLCommand("IsGoodsItemNameUnique")
            myComm.AddParam("?IO", GID)
            myComm.AddParam("?NM", nName)
            If nGroup Is Nothing OrElse nGroup.ID < 1 Then
                myComm.AddParam("?GR", 0)
            Else
                myComm.AddParam("?GR", nGroup.ID)
            End If

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then Return False
            End Using

            Return True

        End Function

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfGoodsItemUpdateDateChanged")
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 OrElse CDateTimeSafe(myData.Rows(0).Item(0), _
                    Date.MinValue) = Date.MinValue Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID=" & _ID.ToString & ", nerastas.")
                If DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> _UpdateDate Then Throw New Exception( _
                    "Klaida. Dokumento atnaujinimo data pasikeitė. Teigtina, kad kitas " _
                    & "vartotojas redagavo šį objektą.")
                _IsInUse = ConvertDbBoolean(CIntSafe(myData.Rows(0).Item(1), 0))
                _PriceCutsExist = ConvertDbBoolean(CIntSafe(myData.Rows(0).Item(2), 0))
            End Using

        End Sub

        Private Shared Function AnyOperationExists(ByVal nID As Integer) As Boolean

            Dim myComm As New SQLCommand("CheckIfAnyGoodsOperationsExist")
            myComm.AddParam("?CD", nID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then Return True
            End Using

            Return False

        End Function

#End Region

    End Class

End Namespace