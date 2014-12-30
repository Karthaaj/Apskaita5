Imports ApskaitaObjects.HelperLists
Namespace Documents

    <Serializable()> _
    Public Class InvoiceReceived
        Inherits BusinessBase(Of InvoiceReceived)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = -1
        Private _ChronologyValidator As ComplexChronologicValidator
        Private _Supplier As PersonInfo = Nothing
        Private _AccountSupplier As Long = 0
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _Number As String = ""
        Private _Content As String = ""
        Private _Type As InvoiceType = InvoiceType.Normal
        Private _DefaultVatRate As Double = 0
        Private WithEvents _InvoiceItems As InvoiceReceivedItemList
        Private _CurrencyCode As String = GetCurrentCompany.BaseCurrency
        Private _CurrencyRate As Double = 1
        Private _CommentsInternal As String = ""
        Private _SumLTL As Double = 0
        Private _SumVatLTL As Double = 0
        Private _SumTotalLTL As Double = 0
        Private _Sum As Double = 0
        Private _SumVat As Double = 0
        Private _SumTotal As Double = 0
        Private _ExternalID As String = ""
        Private _IndirectVatSum As Double = 0
        Private _IndirectVatAccount As Long = 0
        Private _IndirectVatCostsAccount As Long = 0
        Private _ActualDate As Date = Today
        Private _ActualDateIsApplicable As Boolean = False
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now

        Private SuspendChildListChangedEvents As Boolean = False


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property ChronologyValidator() As ComplexChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologyValidator
            End Get
        End Property

        Public Property Supplier() As PersonInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Supplier
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As PersonInfo)
                CanWriteProperty(True)
                If Not (_Supplier Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Supplier Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _Supplier = value) Then

                    _Supplier = value
                    PropertyHasChanged()

                    If Not _Supplier Is Nothing AndAlso _Supplier.ID > 0 Then
                        CurrencyCode = _Supplier.CurrencyCode
                        AccountSupplier = _Supplier.AccountAgainstBankSupplyer
                    End If

                End If
            End Set
        End Property

        Public Property AccountSupplier() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountSupplier
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If _AccountSupplier <> value Then
                    _AccountSupplier = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property [Date]() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Date
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Date)
                CanWriteProperty(True)
                If _Date.Date <> value.Date Then
                    _InvoiceItems.UpdateDate(value.Date)
                    _Date = value.Date
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property OldDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldDate
            End Get
        End Property

        Public Property Number() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Number.Trim.ToUpper <> value.Trim.ToUpper Then
                    _Number = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Content() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Content.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Content.Trim <> value.Trim Then
                    _Content = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property InvoiceItemsSorted() As Csla.SortedBindingList(Of InvoiceReceivedItem)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceItems.GetSortedList
            End Get
        End Property

        Public Property CurrencyCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrencyCode.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If value Is Nothing OrElse String.IsNullOrEmpty(value.Trim) Then value = GetCurrentCompany.BaseCurrency
                If _CurrencyCode.Trim <> value.Trim Then
                    _CurrencyCode = value.Trim
                    PropertyHasChanged()
                    If Not String.IsNullOrEmpty(_CurrencyCode.Trim) AndAlso _
                        _CurrencyCode.Trim.ToUpper <> GetCurrentCompany.BaseCurrency Then
                        CurrencyRate = 0
                    ElseIf Not String.IsNullOrEmpty(_CurrencyCode.Trim) AndAlso _
                        _CurrencyCode.Trim.ToUpper = GetCurrentCompany.BaseCurrency Then
                        CurrencyRate = 1
                    End If
                End If
            End Set
        End Property

        Public Property CurrencyRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrencyRate, 6)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If CRound(_CurrencyRate, 6) <> CRound(value, 6) Then
                    _CurrencyRate = CRound(value, 6)
                    PropertyHasChanged()
                    If CRound(_CurrencyRate, 6) > 0 Then _InvoiceItems.UpdateCurrencyRate(_CurrencyRate, _CurrencyCode)
                End If
            End Set
        End Property

        Public Property CommentsInternal() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CommentsInternal.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _CommentsInternal.Trim <> value.Trim Then
                    _CommentsInternal = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property [Type]() As InvoiceType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As InvoiceType)
                CanWriteProperty(True)
                If _Type <> value Then
                    _Type = value
                    PropertyHasChanged()
                    PropertyHasChanged("TypeHumanReadable")
                End If
            End Set
        End Property

        Public Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return ConvertEnumHumanReadable(_Type)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If ConvertEnumHumanReadable(Of InvoiceType)(value) <> _Type Then
                    _Type = ConvertEnumHumanReadable(Of InvoiceType)(value)
                    PropertyHasChanged()
                    PropertyHasChanged("Type")
                End If
            End Set
        End Property

        Public Property IndirectVatSum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_IndirectVatSum)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If value < 0 Then value = 0
                If CRound(_IndirectVatSum) <> CRound(value) Then
                    _IndirectVatSum = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IndirectVatAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IndirectVatAccount
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If _IndirectVatAccount <> value Then
                    _IndirectVatAccount = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IndirectVatCostsAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IndirectVatCostsAccount
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If Not _ChronologyValidator.ParentFinancialDataCanChange Then Exit Property
                If _IndirectVatCostsAccount <> value Then
                    _IndirectVatCostsAccount = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property ActualDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ActualDate
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Date)
                CanWriteProperty(True)
                If Not _ActualDateIsApplicable Then Exit Property
                If _ActualDate.Date <> value.Date Then
                    _ActualDate = value.Date
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property ActualDateIsApplicable() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ActualDateIsApplicable
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _ActualDateIsApplicable <> value Then
                    _ActualDateIsApplicable = value
                    PropertyHasChanged()
                End If
            End Set
        End Property


        Public ReadOnly Property SumLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumLTL)
            End Get
        End Property

        Public ReadOnly Property SumVatLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumVatLTL)
            End Get
        End Property

        Public ReadOnly Property SumTotalLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumTotalLTL)
            End Get
        End Property

        Public ReadOnly Property Sum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Sum)
            End Get
        End Property

        Public ReadOnly Property SumVat() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumVat)
            End Get
        End Property

        Public ReadOnly Property SumTotal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumTotal)
            End Get
        End Property

        Public ReadOnly Property ExternalID() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ExternalID.Trim
            End Get
        End Property


        Public ReadOnly Property InsertDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InsertDate
            End Get
        End Property

        Public ReadOnly Property UpdateDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _UpdateDate
            End Get
        End Property


        Public ReadOnly Property DefaultVatRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRate)
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
        Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (IsNew AndAlso _InvoiceItems.Count > 0) _
                OrElse (Not IsNew AndAlso _InvoiceItems.IsDirty) _
                OrElse (Not String.IsNullOrEmpty(_Content.Trim) _
                OrElse Not String.IsNullOrEmpty(_CommentsInternal.Trim))
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _InvoiceItems.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _InvoiceItems.IsValid
            End Get
        End Property



        Public Sub AttachNewObject(ByVal newAttachedObject As InvoiceAttachedObject)

            _InvoiceItems.Add(InvoiceReceivedItem.NewInvoiceReceivedItem(newAttachedObject))
            newAttachedObject.FillWithInvoiceDate(_Date)

            _ChronologyValidator.MergeNewValidationItem(newAttachedObject.ChronologyValidator)

        End Sub

        Public Sub AttachNewObject(ByVal nService As ServiceInfo)

            Dim obj As InvoiceAttachedObject = InvoiceAttachedObject.NewInvoiceAttachedObject( _
                nService, _ChronologyValidator.BaseValidator)
            Dim item As InvoiceReceivedItem = InvoiceReceivedItem.NewInvoiceReceivedItem(obj)
            _InvoiceItems.Add(item)
            item.UpdateServiceInfo(True, True)

        End Sub


        Friend Sub SetExternalID(ByVal newExternalID As String, ByVal RaisePropertyHasChanged As Boolean)
            If newExternalID Is Nothing Then newExternalID = ""
            If newExternalID.Trim <> _ExternalID.Trim Then
                _ExternalID = newExternalID.Trim
                If RaisePropertyHasChanged Then PropertyHasChanged("ExternalID")
            End If
        End Sub

        Public Function GetInvoiceInfo(ByVal SystemGuid As String) As InvoiceInfo.InvoiceInfo

            Dim result As New InvoiceInfo.InvoiceInfo

            result.AddDateToNumberOptionWasUsed = False
            result.CommentsInternal = Me._CommentsInternal
            result.Content = Me._Content
            result.CurrencyCode = Me._CurrencyCode
            result.CurrencyRate = Me._CurrencyRate
            result.CustomInfo = ""
            result.CustomInfoAltLng = ""
            result.Date = Me._Date
            result.Discount = 0
            result.DiscountLTL = 0
            result.DiscountVat = 0
            result.DiscountVatLTL = 0
            result.ExternalID = Me._ExternalID
            result.FullNumber = Me._Number
            result.ID = Me._ID.ToString
            result.LanguageCode = LanguageCodeLith.Trim.ToUpper
            result.Number = 0
            result.NumbersInInvoice = 0
            result.ProjectCode = ""
            result.Serial = ""
            result.SystemGuid = SystemGuid
            result.Sum = Me._Sum
            result.SumLTL = Me._SumLTL
            result.SumReceived = 0
            result.SumTotal = Me._SumTotal
            result.SumTotalLTL = Me._SumTotalLTL
            result.SumVat = Me._SumVat
            result.SumVatLTL = Me._SumVatLTL
            result.UpdateDate = Me._UpdateDate
            result.VatExemptions = ""
            result.VatExemptionsAltLng = ""

            If Not _Supplier Is Nothing AndAlso _Supplier.ID > 0 Then
                result.Payer.Address = _Supplier.Address
                result.Payer.BalanceAtBegining = 0
                result.Payer.BreedCode = _Supplier.InternalCode
                result.Payer.Code = _Supplier.Code
                result.Payer.CodeVAT = _Supplier.CodeVAT
                result.Payer.Contacts = _Supplier.ContactInfo
                result.Payer.CurrencyCode = _Supplier.CurrencyCode
                result.Payer.Email = _Supplier.Email
                result.Payer.ExternalID = ""
                result.Payer.ID = _Supplier.ID.ToString
                result.Payer.IsClient = _Supplier.IsClient
                result.Payer.IsCodeLocal = False
                result.Payer.IsNaturalPerson = _Supplier.IsNaturalPerson
                result.Payer.IsObsolete = _Supplier.IsObsolete
                result.Payer.IsSupplier = _Supplier.IsSupplier
                result.Payer.IsWorker = _Supplier.IsWorker
                result.Payer.LanguageCode = _Supplier.LanguageCode
                result.Payer.Name = _Supplier.Name
                result.Payer.VatExemption = ""
                result.Payer.VatExemptionAltLng = ""
            End If

            result.InvoiceItems = Me._InvoiceItems.GetInvoiceItemInfoList

            Return result

        End Function


        Public Function GetAllBrokenRules() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error).Trim
            result = AddWithNewLine(result, _InvoiceItems.GetAllBrokenRules, False)

            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning).Trim
            result = AddWithNewLine(result, _InvoiceItems.GetAllWarnings(), False)


            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function


        Public Overrides Function Save() As InvoiceReceived

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Sąskaitos - faktūros duomenyse yra klaidų: " _
                & vbCrLf & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Public Sub CalculateIndirectVat()
            _IndirectVatSum = CRound(_SumLTL * _DefaultVatRate / 100)
            PropertyHasChanged("IndirectVatSum")
        End Sub


        Private Sub InvoiceItems_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _InvoiceItems.ListChanged
            If SuspendChildListChangedEvents Then Exit Sub
            If IsNew AndAlso _InvoiceItems.Count > 0 Then Content = _InvoiceItems(0).NameInvoice
            CalculateSubTotals(True)
            If e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted OrElse _
                e.ListChangedType = ComponentModel.ListChangedType.Reset Then _
                _ChronologyValidator.ReloadValidationItems(_InvoiceItems.GetChronologyValidators())
        End Sub

        Private Sub CalculateSubTotals(ByVal RaisePropertyChangedEvents As Boolean)

            _SumLTL = 0
            _SumVatLTL = 0
            _Sum = 0
            _SumVat = 0
                        _
            For Each i As InvoiceReceivedItem In _InvoiceItems
                _SumLTL = CRound(_SumLTL + i.SumLTL)
                _SumVatLTL = CRound(_SumVatLTL + i.SumVatLTL)
                _Sum = CRound(_Sum + i.Sum)
                _SumVat = CRound(_SumVat + i.SumVat)
            Next

            _SumTotalLTL = CRound(_SumLTL + _SumVatLTL)
            _SumTotal = CRound(_Sum + _SumVat)

            If RaisePropertyChangedEvents Then
                PropertyHasChanged("SumLTL")
                PropertyHasChanged("SumVatLTL")
                PropertyHasChanged("Sum")
                PropertyHasChanged("SumVat")
                PropertyHasChanged("SumTotalLTL")
                PropertyHasChanged("SumTotal")
            End If

        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As InvoiceReceived = DirectCast(MyBase.GetClone(), InvoiceReceived)
            result.RestoreChildListsHandles()
            Return result
        End Function

        Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.OnDeserialized(context)
            RestoreChildListsHandles()
        End Sub

        Protected Overrides Sub UndoChangesComplete()
            MyBase.UndoChangesComplete()
            RestoreChildListsHandles()
        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Friend Sub RestoreChildListsHandles()
            Try
                RemoveHandler _InvoiceItems.ListChanged, AddressOf InvoiceItems_Changed
            Catch ex As Exception
            End Try
            AddHandler _InvoiceItems.ListChanged, AddressOf InvoiceItems_Changed
        End Sub


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return _Content
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Number", "sąskaitos numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "sąskaitos turinys"))
            ValidationRules.AddRule(AddressOf CommonValidation.CurrencyValid, _
                New CommonValidation.SimpleRuleArgs("CurrencyCode", ""))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("CurrencyRate", "valiutos kursas"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("Supplier", "tiekėjas", "ID"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologyValidator"))

            ValidationRules.AddRule(AddressOf SumLTLValidation, "SumLTL")
            ValidationRules.AddRule(AddressOf AccountSupplierValidation, "AccountSupplier")
            ValidationRules.AddRule(AddressOf IndirectVatAccountsValidation, "IndirectVatAccount")
            ValidationRules.AddRule(AddressOf IndirectVatAccountsValidation, "IndirectVatCostsAccount")

            ValidationRules.AddDependantProperty("Type", "SumLTL", False)
            ValidationRules.AddDependantProperty("Supplier", "AccountSupplier", False)
            ValidationRules.AddDependantProperty("IndirectVatSum", "IndirectVatAccount", False)
            ValidationRules.AddDependantProperty("IndirectVatSum", "IndirectVatCostsAccount", False)
            ValidationRules.AddDependantProperty("SumVatLTL", "IndirectVatAccount", False)
            ValidationRules.AddDependantProperty("SumVatLTL", "IndirectVatCostsAccount", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that any items exist.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function SumLTLValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As InvoiceReceived = DirectCast(target, InvoiceReceived)

            If Not ValObj._InvoiceItems.Count > 0 Then

                e.Description = "Nenurodyta nė viena sąskaitos eilutė."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf ValObj._Type = InvoiceType.Credit AndAlso Not CRound(ValObj._Sum) < 0 Then

                e.Description = "Kreditinė sąskaita turėtų būti neigiama."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that AccountSupplier is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountSupplierValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As InvoiceReceived = DirectCast(target, InvoiceReceived)

            If Not ValObj._Supplier Is Nothing AndAlso ValObj._Supplier.ID > 0 AndAlso _
                Not ValObj._Supplier.AccountAgainstBankSupplyer > 0 AndAlso _
                Not ValObj._AccountSupplier > 0 Then

                e.Description = "Nenurodyta tiekėjo koresponduojanti sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that IndirectVatAccount and IndirectVatCostsAccount are valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function IndirectVatAccountsValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As InvoiceReceived = DirectCast(target, InvoiceReceived)

            If CRound(ValObj._IndirectVatSum) > 0 Then

                If Not ValObj._IndirectVatAccount > 0 AndAlso e.PropertyName.Trim.ToLower _
                    = "indirectvataccount" Then

                    e.Description = "Nenurodyta netiesioginio PVM apskaitos sąskaita."
                    e.Severity = Validation.RuleSeverity.Error
                    Return False

                ElseIf Not ValObj._IndirectVatCostsAccount > 0 AndAlso e.PropertyName.Trim.ToLower _
                    = "indirectvatcostsaccount" Then

                    e.Description = "Nenurodyta netiesioginio PVM sąnaudų apskaitos sąskaita."
                    e.Severity = Validation.RuleSeverity.Error
                    Return False

                ElseIf ValObj._SumVatLTL > 0 Then

                    e.Description = "Netiesioginis PVM gali negali būti taikomas, jei " _
                        & "sąskaitoje naudojami ne nuliniai PVM tarifai."
                    e.Severity = Validation.RuleSeverity.Error
                    Return False

                End If

            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Documents.InvoiceReceived2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.InvoiceReceived2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.InvoiceReceived1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.InvoiceReceived3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.InvoiceReceived3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewInvoiceReceived() As InvoiceReceived

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim CC As ApskaitaObjects.Settings.CompanyInfo = GetCurrentCompany()

            Return New InvoiceReceived(CC.GetDefaultRate(RateType.Vat), CC.MeasureUnitInvoiceReceived, _
                CC.DefaultInvoiceReceivedContent)

        End Function

        Public Shared Function NewInvoiceReceived(ByVal info As InvoiceInfo.InvoiceInfo, _
            ByVal SystemGuid As String, ByVal UseImportedObjectExternalID As Boolean, _
            ByVal clientList As PersonInfoList, ByRef unknownPerson As InvoiceInfo.ClientInfo) As InvoiceReceived

            Dim result As New InvoiceReceived(info, SystemGuid, UseImportedObjectExternalID, clientList)

            If result.Supplier Is Nothing AndAlso Not info.Payer.Code Is Nothing _
                AndAlso Not String.IsNullOrEmpty(info.Payer.Code.Trim) Then
                unknownPerson = info.Payer
            Else
                unknownPerson = Nothing
            End If

            Return result

        End Function


        Public Shared Function GetInvoiceReceived(ByVal nID As Integer) As InvoiceReceived

            Return DataPortal.Fetch(Of InvoiceReceived)(New Criteria(nID, _
                GetCurrentCompany.Rates.GetRate(RateType.Vat)))

        End Function


        Public Shared Sub DeleteInvoiceReceived(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub


        Public Function GetInvoiceCopy() As InvoiceReceived

            Dim result As InvoiceReceived = Me.Clone
            result._ID = -1
            result._Number = ""
            result._Date = Today
            result._ChronologyValidator = ComplexChronologicValidator.NewComplexChronologicValidator( _
                ConvertEnumHumanReadable(DocumentType.InvoiceReceived), _
                SimpleChronologicValidator.NewSimpleChronologicValidator( _
                ConvertEnumHumanReadable(DocumentType.InvoiceReceived)), Nothing)
            result._InvoiceItems.MarkAsCopy()
            result.MarkNew()

            result.ValidationRules.CheckRules()

            Return result

        End Function


        Private Sub New()
            ' require use of factory methods

        End Sub


        Private Sub New(ByVal nVatRate As Double, ByVal nMeasureUnitInvoiceReceived As String, _
            ByVal nDefaultInvoiceReceivedContent As String)
            Create(nVatRate, nMeasureUnitInvoiceReceived, nDefaultInvoiceReceivedContent)
        End Sub

        Private Sub New(ByVal info As InvoiceInfo.InvoiceInfo, ByVal SystemGuid As String, _
            ByVal UseImportedObjectExternalID As Boolean, ByVal clientList As PersonInfoList)
            FetchInvoiceInfo(info, SystemGuid, UseImportedObjectExternalID, clientList)
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _ID As Integer
            Private _DefaultVatRate As Double
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property DefaultVatRate() As Double
                Get
                    Return _DefaultVatRate
                End Get
            End Property
            Public Sub New(ByVal nID As Integer, ByVal nDefaultVatRate As Double)
                _ID = nID
                _DefaultVatRate = nDefaultVatRate
            End Sub
            Public Sub New(ByVal nID As Integer)
                _ID = nID
                _DefaultVatRate = 0
            End Sub
        End Class


        Private Sub Create(ByVal nVatRate As Double, ByVal nMeasureUnitInvoiceReceived As String, _
            ByVal nDefaultInvoiceReceivedContent As String)

            _DefaultVatRate = nVatRate
            _InvoiceItems = InvoiceReceivedItemList.NewInvoiceReceivedItemList( _
                nVatRate, nMeasureUnitInvoiceReceived)
            _Content = nDefaultInvoiceReceivedContent

            Dim _BaseChronologyValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                NewSimpleChronologicValidator(ConvertEnumHumanReadable(DocumentType.InvoiceReceived))
            _ChronologyValidator = ComplexChronologicValidator.NewComplexChronologicValidator( _
                ConvertEnumHumanReadable(DocumentType.InvoiceReceived), _BaseChronologyValidator, _
                New IChronologicValidator() {})

            ValidationRules.CheckRules()

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As New SQLCommand("FetchInvoiceReceived")
            myComm.AddParam("?MD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Sąskaitos, kurios ID='" & criteria.ID & "', duomenys nerasti.)")

                _ID = criteria.ID
                _DefaultVatRate = criteria.DefaultVatRate

                Dim dr As DataRow = myData.Rows(0)

                _Date = CDateSafe(dr.Item(0), Today)
                _OldDate = _Date
                _Number = CStrSafe(dr.Item(1)).Trim
                _Content = CStrSafe(dr.Item(2)).Trim
                _CurrencyCode = CStrSafe(dr.Item(3)).Trim
                _CurrencyRate = CDblSafe(dr.Item(4), 6, 0)
                _CommentsInternal = CStrSafe(dr.Item(5)).Trim
                _AccountSupplier = CLongSafe(dr.Item(6), 0)
                _Type = ConvertEnumDatabaseCode(Of InvoiceType)(CIntSafe(dr.Item(7), 0))
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(8), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(9), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _ExternalID = CStrSafe(dr.Item(10)).Trim
                _IndirectVatSum = CDblSafe(dr.Item(11), 2, 0)
                _IndirectVatAccount = CLongSafe(dr.Item(12), 0)
                _IndirectVatCostsAccount = CLongSafe(dr.Item(13), 0)
                _ActualDate = CDateSafe(dr.Item(14), Date.MinValue)
                If _ActualDate = Date.MinValue Then
                    _ActualDate = _Date
                    _ActualDateIsApplicable = False
                Else
                    _ActualDateIsApplicable = True
                End If

                _Supplier = PersonInfo.GetPersonInfo(dr, 15)

            End Using

            Dim _BaseChronologyValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                GetSimpleChronologicValidator(_ID, _Date, ConvertEnumHumanReadable(DocumentType.InvoiceReceived))

            _InvoiceItems = InvoiceReceivedItemList.GetInvoiceReceivedItemList(_ID, _CurrencyRate, _
                _DefaultVatRate, GetCurrentCompany.MeasureUnitInvoiceReceived, _BaseChronologyValidator)

            CalculateSubTotals(False)

            _ChronologyValidator = ComplexChronologicValidator.GetComplexChronologicValidator( _
                _ID, _Date, ConvertEnumHumanReadable(DocumentType.InvoiceReceived), _
                _BaseChronologyValidator, _InvoiceItems.GetChronologyValidators())

            MarkOld()

            ValidationRules.CheckRules()

        End Sub

        Private Sub FetchInvoiceInfo(ByVal info As InvoiceInfo.InvoiceInfo, _
            ByVal SystemGuid As String, ByVal UseImportedObjectExternalID As Boolean, _
            ByVal clientList As PersonInfoList)

            If info.SystemGuid.Trim = SystemGuid.Trim Then
                info.ID = ""
                info.ExternalID = ""
                info.Number = 0
            End If
            If UseImportedObjectExternalID Then
                Me._ExternalID = info.ExternalID
            Else
                Me._ExternalID = info.ID
            End If

            Me._CommentsInternal = info.CommentsInternal
            Me._Content = info.Content
            Me._CurrencyCode = info.CurrencyCode
            Me._CurrencyRate = info.CurrencyRate
            Me._Date = info.Date
            Me._Number = info.FullNumber
            Me._UpdateDate = info.UpdateDate

            If Not info.Payer.Code Is Nothing AndAlso Not String.IsNullOrEmpty(info.Payer.Code.Trim) Then
                Me._Supplier = clientList.GetPersonInfo(info.Payer.Code)
                If Not Me._Supplier Is Nothing Then Me._AccountSupplier = Me._Supplier.AccountAgainstBankSupplyer
            End If

            _InvoiceItems = InvoiceReceivedItemList.NewInvoiceReceivedItemList( _
                info, _CurrencyRate, _CurrencyCode, GetCurrentCompany.MeasureUnitInvoiceReceived)

            CalculateSubTotals(False)

            Dim _BaseChronologyValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                NewSimpleChronologicValidator(ConvertEnumHumanReadable(DocumentType.InvoiceReceived))
            _ChronologyValidator = ComplexChronologicValidator.NewComplexChronologicValidator( _
                ConvertEnumHumanReadable(DocumentType.InvoiceReceived), _BaseChronologyValidator, _
                New IChronologicValidator() {})

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims įvesti.")

            _InvoiceItems.CheckRules(_ChronologyValidator.BaseValidator)
            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & GetAllBrokenRules())

            CheckIfExternalIdUnique()

            Dim JE As General.JournalEntry = GetJournalEntry()

            DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()

            _ID = JE.ID

            Dim myComm As New SQLCommand("InsertInvoiceReceived")
            AddParamsGeneral(myComm)
            AddParamsFinancial(myComm)

            myComm.Execute()

            _InvoiceItems.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            _InvoiceItems.CheckRules(_ChronologyValidator.BaseValidator)
            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & GetAllBrokenRules())

            CheckIfExternalIdUnique()

            CheckIfUpdateDateChanged()

            Dim JE As General.JournalEntry = GetJournalEntry()

            Dim myComm As SQLCommand
            If _ChronologyValidator.FinancialDataCanChange Then
                myComm = New SQLCommand("UpdateInvoiceReceived")
                AddParamsFinancial(myComm)
            Else
                myComm = New SQLCommand("UpdateInvoiceReceivedGeneral")
            End If
            AddParamsGeneral(myComm)
            
            DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()

            myComm.Execute()

            _InvoiceItems.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Private Sub AddParamsGeneral(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _ID)
            myComm.AddParam("?AD", _CommentsInternal.Trim)
            myComm.AddParam("?AF", ConvertEnumDatabaseCode(_Type))
            myComm.AddParam("?AG", _ExternalID.Trim)
            If _ActualDateIsApplicable Then
                myComm.AddParam("?AL", _ActualDate.Date)
            Else
                myComm.AddParam("?AL", Nothing, GetType(Date))
            End If
            
            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?AH", _UpdateDate.ToUniversalTime)

        End Sub

        Private Sub AddParamsFinancial(ByRef myComm As SQLCommand)

            myComm.AddParam("?AB", _CurrencyCode.Trim)
            myComm.AddParam("?AC", CRound(_CurrencyRate, 6))
            myComm.AddParam("?AE", _AccountSupplier)
            myComm.AddParam("?AI", CRound(_IndirectVatSum))
            myComm.AddParam("?AJ", _IndirectVatAccount)
            myComm.AddParam("?AK", _IndirectVatCostsAccount)

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            Dim cInvoice As New InvoiceReceived
            cInvoice.DataPortal_Fetch(New Criteria(DirectCast(criteria, Criteria).ID))

            For Each item As InvoiceReceivedItem In cInvoice._InvoiceItems
                item.CheckIfCanDelete(cInvoice._ChronologyValidator.BaseValidator)
            Next

            IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted(DirectCast(criteria, Criteria).ID, _
                DocumentType.InvoiceReceived)

            Dim myComm As New SQLCommand("DeleteInvoiceReceived")
            myComm.AddParam("?MD", DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionBegin()

            For Each item As InvoiceReceivedItem In cInvoice._InvoiceItems
                item.DeleteSelf()
            Next

            myComm.Execute()

            General.JournalEntry.DoDelete(DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionCommit()

        End Sub


        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry
            If IsNew Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.InvoiceReceived)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_ID, DocumentType.InvoiceReceived)
            End If

            result.Content = _Content
            result.Date = _Date
            result.DocNumber = _Number
            result.Person = _Supplier

            If IsNew OrElse _ChronologyValidator.ParentFinancialDataCanChange Then

                Dim FullBookEntryList As BookEntryInternalList = BookEntryInternalList. _
                NewBookEntryInternalList(BookEntryType.Debetas)

                Dim applicableAccountSupplier As Long = _AccountSupplier
                If Not applicableAccountSupplier > 0 Then applicableAccountSupplier = _
                    _Supplier.AccountAgainstBankSupplyer

                For Each i As InvoiceReceivedItem In _InvoiceItems
                    FullBookEntryList.AddRange(i.GetBookEntryInternalList(applicableAccountSupplier))
                Next

                If CRound(_IndirectVatSum) > 0 AndAlso _IndirectVatAccount > 0 AndAlso _
                    _IndirectVatCostsAccount > 0 Then

                    FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas, _
                        _IndirectVatCostsAccount, CRound(_IndirectVatSum), Nothing))

                    FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas, _
                        _IndirectVatAccount, CRound(_IndirectVatSum), Nothing))

                End If

                FullBookEntryList.Aggregate()

                result.DebetList.Clear()
                result.CreditList.Clear()

                result.DebetList.LoadBookEntryListFromInternalList(FullBookEntryList, False)
                result.CreditList.LoadBookEntryListFromInternalList(FullBookEntryList, False)

            End If

            If Not result.IsValid Then Throw New Exception("Klaida. Nepavyko generuoti " _
                & "bendrojo žurnalo įrašo: " & vbCrLf & result.GetAllBrokenRules)

            Return result

        End Function


        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfInvoiceReceivedUpdateDateChanged")
            myComm.AddParam("?SD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 Then
                    Throw New Exception("Klaida. Objektas, kurio ID=" & _ID.ToString & ", nerastas.")
                ElseIf DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> _UpdateDate Then
                    Throw New Exception("Klaida. Objekto paskutinio taisymo data pasikeitė. " _
                        & "Teigtina, kad kitas vartotojas pakeitė dokumentą.")
                End If
            End Using

        End Sub

        Private Sub CheckIfExternalIdUnique()

            If _ExternalID Is Nothing OrElse String.IsNullOrEmpty(_ExternalID.Trim) Then Exit Sub

            Dim myComm As New SQLCommand("FetchInvoiceReceivedIdByExternalID")
            myComm.AddParam("?ND", _ID)
            myComm.AddParam("?ED", _ExternalID.Trim)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                    Throw New Exception("Klaida. Išorinis ID='" & _ExternalID.Trim _
                        & "' jau egzistuoja duomenų bazėje.")
            End Using

        End Sub

#End Region

    End Class

End Namespace