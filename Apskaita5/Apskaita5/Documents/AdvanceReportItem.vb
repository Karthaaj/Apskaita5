Namespace Documents

    <Serializable()> _
    Public Class AdvanceReportItem
        Inherits BusinessBase(Of AdvanceReportItem)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _GUID As Guid = Guid.NewGuid
        Private _FinancialDataCanChange As Boolean = True
        Private _ID As Integer = 0
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _DocumentNumber As String = ""
        Private _Person As PersonInfo = Nothing
        Private _Content As String = ""
        Private _Income As Boolean = False
        Private _Account As Long = 0
        Private _AccountVat As Long = 0
        Private _Sum As Double = 0
        Private _VatRate As Double = 0
        Private _SumVat As Double = 0
        Private _SumVatCorrection As Integer = 0
        Private _SumTotal As Double = 0
        Private _SumLTL As Double = 0
        Private _SumCorrectionLTL As Integer = 0
        Private _SumVatLTL As Double = 0
        Private _SumVatCorrectionLTL As Integer = 0
        Private _SumTotalLTL As Double = 0
        Private _CurrencyRateChangeEffect As Double = 0
        Private _AccountCurrencyRateChangeEffect As Long = 0
        Private _InvoiceID As Integer = 0
        Private _InvoiceIsMade As Boolean = False
        Private _InvoiceDateAndNumber As String = ""
        Private _InvoiceContent As String = ""
        Private _InvoiceCurrencyCode As String = ""
        Private _InvoiceCurrencyRate As Double = 0
        Private _InvoiceSumOriginal As Double = 0
        Private _InvoiceSumVatOriginal As Double = 0
        Private _InvoiceSumTotalOriginal As Double = 0
        Private _InvoiceSumLTL As Double = 0
        Private _InvoiceSumVatLTL As Double = 0
        Private _InvoiceSumTotalLTL As Double = 0
        Private _InvoiceSumTotal As Double = 0


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property FinancialDataCanChange() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _FinancialDataCanChange
            End Get
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

        Public Property DocumentNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocumentNumber.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _DocumentNumber.Trim <> value.Trim Then
                    _DocumentNumber = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Person() As PersonInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Person
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As PersonInfo)
                CanWriteProperty(True)
                If PersonIsReadOnly Then Exit Property
                If Not (_Person Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Person Is Nothing AndAlso Not value Is Nothing AndAlso _Person = value) Then
                    _Person = value
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

        Public Property Income() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Income
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If IncomeIsReadOnly Then Exit Property
                If _Income <> value Then
                    _Income = value
                    PropertyHasChanged()
                    PropertyHasChanged("Expenses")
                End If
            End Set
        End Property

        Public Property Expenses() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _Income
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If ExpensesIsReadOnly Then Exit Property
                If _Income = value Then
                    _Income = Not value
                    PropertyHasChanged()
                    PropertyHasChanged("Income")
                End If
            End Set
        End Property

        Public Property Account() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Account
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If AccountIsReadOnly Then Exit Property
                If _Account <> value Then
                    _Account = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountVat() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountVat
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If AccountVatIsReadOnly Then Exit Property
                If _AccountVat <> value Then
                    _AccountVat = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Sum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Sum)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If SumIsReadOnly Then Exit Property
                If CRound(_Sum) <> CRound(value) Then
                    _Sum = CRound(value)
                    PropertyHasChanged()
                    CalculateSumVat()
                    CalculateSumLTL(0)
                End If
            End Set
        End Property

        Public Property VatRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_VatRate)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If VatRateIsReadOnly Then Exit Property
                If CRound(_VatRate) <> CRound(value) Then
                    _VatRate = CRound(value)
                    PropertyHasChanged()
                    CalculateSumVat()
                    CalculateSumVatLTL(0)
                End If
            End Set
        End Property

        Public ReadOnly Property SumVat() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumVat)
            End Get
        End Property

        Public Property SumVatCorrection() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SumVatCorrection
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If SumVatCorrectionIsReadOnly Then Exit Property
                If _SumVatCorrection <> value Then
                    _SumVatCorrection = value
                    PropertyHasChanged()
                    CalculateSumVat()
                    CalculateSumVatLTL(0)
                End If
            End Set
        End Property

        Public ReadOnly Property SumTotal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumTotal)
            End Get
        End Property

        Public ReadOnly Property SumLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumLTL)
            End Get
        End Property

        Public Property SumCorrectionLTL() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SumCorrectionLTL
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If SumCorrectionLTLIsReadOnly Then Exit Property
                If _SumCorrectionLTL <> value Then
                    _SumCorrectionLTL = value
                    PropertyHasChanged()
                    CalculateSumLTL(0)
                End If
            End Set
        End Property

        Public ReadOnly Property SumVatLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumVatLTL)
            End Get
        End Property

        Public Property SumVatCorrectionLTL() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SumVatCorrectionLTL
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If SumVatCorrectionLTLIsReadOnly Then Exit Property
                If _SumVatCorrectionLTL <> value Then
                    _SumVatCorrectionLTL = value
                    PropertyHasChanged()
                    CalculateSumVatLTL(0)
                End If
            End Set
        End Property

        Public ReadOnly Property SumTotalLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumTotalLTL)
            End Get
        End Property

        Public Property CurrencyRateChangeEffect() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrencyRateChangeEffect)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CurrencyRateChangeEffectIsReadOnly Then Exit Property
                If CRound(_CurrencyRateChangeEffect) <> CRound(value) Then
                    _CurrencyRateChangeEffect = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountCurrencyRateChangeEffect() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountCurrencyRateChangeEffect
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If AccountCurrencyRateChangeEffectIsReadOnly Then Exit Property
                If _AccountCurrencyRateChangeEffect <> value Then
                    _AccountCurrencyRateChangeEffect = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property InvoiceID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceID
            End Get
        End Property

        Public ReadOnly Property InvoiceIsMade() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceIsMade
            End Get
        End Property

        Public ReadOnly Property InvoiceDateAndNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceDateAndNumber.Trim
            End Get
        End Property

        Public ReadOnly Property InvoiceContent() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceContent.Trim
            End Get
        End Property

        Public ReadOnly Property InvoiceCurrencyCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InvoiceCurrencyCode.Trim
            End Get
        End Property

        Public ReadOnly Property InvoiceCurrencyRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceCurrencyRate, 6)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumOriginal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumOriginal)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumVatOriginal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumVatOriginal)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumTotalOriginal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumTotalOriginal)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumLTL)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumVatLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumVatLTL)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumTotalLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumTotalLTL)
            End Get
        End Property

        Public ReadOnly Property InvoiceSumTotal() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InvoiceSumTotal)
            End Get
        End Property


        Public ReadOnly Property PersonIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0)
            End Get
        End Property

        Public ReadOnly Property IncomeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property ExpensesIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property AccountIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property AccountVatIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property SumIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property VatRateIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property SumVatCorrectionIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property SumCorrectionLTLIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property SumVatCorrectionLTLIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (_InvoiceID > 0 OrElse Not _FinancialDataCanChange)
            End Get
        End Property

        Public ReadOnly Property CurrencyRateChangeEffectIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property AccountCurrencyRateChangeEffectIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _FinancialDataCanChange
            End Get
        End Property

        

        Private Sub CalculateSumVatLTL(ByVal pCurrencyRate As Double)
            _SumVatLTL = CRound(CRound(_SumVat * GetCurrencyRate(pCurrencyRate)) + _SumVatCorrectionLTL / 100)
            _SumTotalLTL = CRound(_SumLTL + _SumVatLTL)
            PropertyHasChanged("SumVatLTL")
            PropertyHasChanged("SumTotalLTL")
        End Sub

        Friend Sub CalculateSumLTL(ByVal pCurrencyRate As Double)
            _SumLTL = CRound(CRound(_Sum * GetCurrencyRate(pCurrencyRate)) + _SumCorrectionLTL / 100)
            PropertyHasChanged("SumLTL")
            CalculateSumVatLTL(pCurrencyRate)
        End Sub

        Private Sub CalculateSumVat()
            _SumVat = CRound(CRound(_Sum * _VatRate / 100) + _SumVatCorrection / 100)
            _SumTotal = CRound(_Sum + _SumVat)
            PropertyHasChanged("SumVat")
            PropertyHasChanged("SumTotal")
        End Sub

        Private Function GetCurrencyRate(ByVal pCurrencyRate As Double) As Double
            If CRound(pCurrencyRate, 6) > 0 Then Return pCurrencyRate
            If Parent Is Nothing Then Return 1
            If DirectCast(Parent, AdvanceReportItemList).CurrencyRate > 0 Then _
                Return DirectCast(Parent, AdvanceReportItemList).CurrencyRate
            Return 1
        End Function


        Friend Sub UpdateCurrencyRate(ByVal nCurrencyRate As Double)

            CalculateSumLTL(nCurrencyRate)

            If _InvoiceID > 0 Then
                If DirectCast(Parent, AdvanceReportItemList).CurrencyCode.Trim.ToUpper _
                    = _InvoiceCurrencyCode.Trim.ToUpper Then
                    _InvoiceSumTotal = _InvoiceSumTotalOriginal
                ElseIf CRound(nCurrencyRate, 6) > 0 Then
                    _InvoiceSumTotal = CRound(_InvoiceSumTotalLTL / CRound(nCurrencyRate, 6))
                Else
                    _InvoiceSumTotal = _InvoiceSumTotalLTL
                End If
                PropertyHasChanged("InvoiceSumTotal")
            End If

        End Sub

        Friend Sub UpdateCurrencyCode(ByVal nCurrencyCode As String)

            ValidationRules.CheckRules("CurrencyRateChangeEffect")

            If Not _InvoiceID > 0 Then Exit Sub

            If nCurrencyCode.Trim.ToUpper = _InvoiceCurrencyCode.Trim.ToUpper Then
                _InvoiceSumTotal = _InvoiceSumTotalOriginal
            ElseIf DirectCast(Parent, AdvanceReportItemList).CurrencyRate > 0 Then
                _InvoiceSumTotal = CRound(_InvoiceSumTotalLTL / _
                    DirectCast(Parent, AdvanceReportItemList).CurrencyRate)
            Else
                _InvoiceSumTotal = _InvoiceSumTotalLTL
            End If

            PropertyHasChanged("InvoiceSumTotal")

        End Sub


        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _Content & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Content & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _GUID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Content
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "dokumento turinys (trumpas aprašas)"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Account", "koresponduojanti sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("Person", "dokumento kontrahentas", _
                "", Validation.RuleSeverity.Information))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("DocumentNumber", "dokumento numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.DateAfterLastClosing, _
                New CommonValidation.DateAfterLastClosingRuleArgs("Date", "OldDate"))

            ValidationRules.AddRule(AddressOf AccountVatValidation, New Validation.RuleArgs("AccountVat"))
            ValidationRules.AddRule(AddressOf SumValidation, New Validation.RuleArgs("Sum"))
            ValidationRules.AddRule(AddressOf AccountCurrencyRateChangeEffectValidation, _
                New Validation.RuleArgs("AccountCurrencyRateChangeEffect"))

            ValidationRules.AddDependantProperty("Income", "AccountVat", False)
            ValidationRules.AddDependantProperty("InvoiceID", "AccountVat", False)
            ValidationRules.AddDependantProperty("VatRate", "AccountVat", False)
            ValidationRules.AddDependantProperty("InvoiceSumTotal", "Sum", False)
            ValidationRules.AddDependantProperty("CurrencyRateChangeEffect", _
                "AccountCurrencyRateChangeEffect", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property AccountVat is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountVatValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As AdvanceReportItem = DirectCast(target, AdvanceReportItem)

            If (ValObj._InvoiceID > 0 OrElse Not CRound(ValObj._VatRate) > 0) _
                AndAlso ValObj._AccountVat > 0 Then

                e.Description = "PVM apskaitos sąskaita nurodoma tik eilutei, " _
                    & "nesusietai su sąskaita faktūra, ir nustačius didesnį už nulį PVM tarifą."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf Not ValObj._InvoiceID > 0 AndAlso CRound(ValObj._VatRate) > 0 _
                AndAlso Not ValObj._AccountVat > 0 Then

                e.Description = "Nenurodyta PVM (sąnaudų/atskaitos) apskaitos sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property Sum is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function SumValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As AdvanceReportItem = DirectCast(target, AdvanceReportItem)

            If Not CRound(ValObj._Sum) > 0 Then

                e.Description = "Nenurodyta dokumento suma."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf ValObj._InvoiceID > 0 AndAlso CRound(ValObj._Sum) _
                <> CRound(ValObj._InvoiceSumTotal) Then

                e.Description = "Dokumento suma nesutampa su priskirtos " _
                    & "sąskaitos faktūros suma."
                e.Severity = Validation.RuleSeverity.Warning
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property AccountCurrencyRateChangeEffect is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountCurrencyRateChangeEffectValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As AdvanceReportItem = DirectCast(target, AdvanceReportItem)

            If CRound(ValObj._CurrencyRateChangeEffect) > 0 AndAlso _
                Not ValObj._AccountCurrencyRateChangeEffect > 0 Then

                e.Description = "Nenurodyta valiutos kurso pasikeitimo įtakos sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property CurrencyRateChangeEffect is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function CurrencyRateChangeEffectValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As AdvanceReportItem = DirectCast(target, AdvanceReportItem)

            If ValObj.Parent Is Nothing Then Return True

            If (Not ValObj._InvoiceID > 0 OrElse ValObj._InvoiceCurrencyCode Is Nothing _
                OrElse String.IsNullOrEmpty(ValObj._InvoiceCurrencyCode.Trim) _
                OrElse ValObj._InvoiceCurrencyCode.Trim.ToUpper = GetCurrentCompany.BaseCurrency) _
                AndAlso (DirectCast(ValObj.Parent, AdvanceReportItemList).CurrencyCode Is Nothing _
                OrElse String.IsNullOrEmpty(DirectCast(ValObj.Parent, AdvanceReportItemList).CurrencyCode.Trim) _
                OrElse DirectCast(ValObj.Parent, AdvanceReportItemList).CurrencyCode.Trim.ToUpper _
                = GetCurrentCompany.BaseCurrency) AndAlso CRound(ValObj._CurrencyRateChangeEffect) > 0 Then

                e.Description = "Valiutos kurso pasikeitimo įtaka galima tik valiutinėse operacijose."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

        End Sub

#End Region

#Region " Factory Methods "

        Friend Shared Function NewAdvanceReportItem() As AdvanceReportItem
            Dim result As New AdvanceReportItem
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function NewAdvanceReportItem(ByVal InvoiceToAdd As ActiveReports.InvoiceInfoItem, _
            ByVal InvoicePersonInfo As PersonInfo, ByVal CurrencyCode As String, _
            ByVal CurrencyRate As Double) As AdvanceReportItem
            Return New AdvanceReportItem(InvoiceToAdd, InvoicePersonInfo, CurrencyCode, CurrencyRate)
        End Function

        Friend Shared Function GetAdvanceReportItem(ByVal dr As DataRow, _
            ByVal pCurrencyRate As Double, ByVal pCurrencyCode As String, _
            ByVal nFinancialDataCanChange As Boolean) As AdvanceReportItem
            Return New AdvanceReportItem(dr, pCurrencyRate, pCurrencyCode, nFinancialDataCanChange)
        End Function


        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub


        Private Sub New(ByVal InvoiceToAdd As ActiveReports.InvoiceInfoItem, _
            ByVal InvoicePersonInfo As PersonInfo, ByVal CurrencyCode As String, _
            ByVal CurrencyRate As Double)
            MarkAsChild()
            Create(InvoiceToAdd, InvoicePersonInfo, CurrencyCode, CurrencyRate)
        End Sub

        Private Sub New(ByVal dr As DataRow, ByVal pCurrencyRate As Double, _
            ByVal pCurrencyCode As String, ByVal nFinancialDataCanChange As Boolean)
            MarkAsChild()
            Fetch(dr, pCurrencyRate, pCurrencyCode, nFinancialDataCanChange)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Create(ByVal InvoiceToAdd As ActiveReports.InvoiceInfoItem, _
            ByVal InvoicePersonInfo As PersonInfo, ByVal CurrencyCode As String, _
            ByVal CurrencyRate As Double)

            If InvoiceToAdd Is Nothing OrElse Not InvoiceToAdd.ID > 0 Then
                Throw New Exception("Klaida. Nepateikta informacija apie sąskaitą faktūrą.")
            ElseIf InvoicePersonInfo Is Nothing OrElse Not InvoicePersonInfo.ID > 0 Then
                Throw New Exception("Klaida. Nepateikta informacija apie sąskaitos faktūros kontrahentą.")
            ElseIf InvoicePersonInfo.ID <> InvoiceToAdd.PersonID Then
                Throw New Exception("Klaida. Pateikta informacija apie sąskaitos faktūros " _
                    & "kontrahentą nesutampa su sąskaitos faktūros duomenimis.")
            End If

            _Account = InvoiceToAdd.PersonAccount
            _Content = InvoiceToAdd.Content
            _Date = InvoiceToAdd.Date
            _DocumentNumber = InvoiceToAdd.Number
            _InvoiceContent = InvoiceToAdd.Content
            _InvoiceCurrencyCode = InvoiceToAdd.CurrencyCode
            _InvoiceCurrencyRate = InvoiceToAdd.CurrencyRate
            _InvoiceID = InvoiceToAdd.ID
            _InvoiceIsMade = (InvoiceToAdd.Type = ActiveReports.InvoiceInfoType.InvoiceMade)
            If _InvoiceIsMade Then
                _InvoiceDateAndNumber = "Išrašyta SF " & InvoiceToAdd.Date.ToShortDateString _
                    & " Nr. " & InvoiceToAdd.Number
                _Income = True
            Else
                _InvoiceDateAndNumber = "Gauta SF " & InvoiceToAdd.Date.ToShortDateString _
                    & " Nr. " & InvoiceToAdd.Number
                _Income = False
            End If
            _InvoiceSumLTL = InvoiceToAdd.SumLTL
            _InvoiceSumOriginal = InvoiceToAdd.Sum
            _InvoiceSumTotalLTL = InvoiceToAdd.TotalSumLTL
            _InvoiceSumTotalOriginal = InvoiceToAdd.TotalSum
            _InvoiceSumVatLTL = InvoiceToAdd.SumVatLTL
            _InvoiceSumVatOriginal = InvoiceToAdd.SumVat
            _Person = InvoicePersonInfo

            If CurrencyCode.Trim.ToUpper = _InvoiceCurrencyCode.Trim.ToUpper Then
                _InvoiceSumTotal = _InvoiceSumTotalOriginal
            ElseIf CRound(CurrencyRate, 6) > 0 Then
                _InvoiceSumTotal = CRound(_InvoiceSumTotalLTL / CRound(CurrencyRate, 6))
            Else
                _InvoiceSumTotal = _InvoiceSumTotalLTL
            End If

            _Sum = _InvoiceSumTotal
            _SumTotal = _InvoiceSumTotal
            _SumCorrectionLTL = Convert.ToInt32(CRound(_InvoiceSumTotalLTL - _
                CRound(_Sum * CurrencyRate)) * 100)
            _SumLTL = CRound(CRound(_Sum * CurrencyRate) + _SumCorrectionLTL / 100)
            _SumTotalLTL = _SumLTL

            ValidationRules.CheckRules()

        End Sub

        Private Sub Fetch(ByVal dr As DataRow, ByVal pCurrencyRate As Double, _
            ByVal pCurrencyCode As String, ByVal nFinancialDataCanChange As Boolean)

            _ID = CIntSafe(dr.Item(0), 0)
            _Date = CDateSafe(dr.Item(1), Today)
            _OldDate = _Date
            _DocumentNumber = CStrSafe(dr.Item(2)).Trim
            _Content = CStrSafe(dr.Item(3)).Trim
            _Income = (ConvertEnumDatabaseStringCode(Of BookEntryType)(CStrSafe(dr.Item(4))) _
                = BookEntryType.Kreditas)
            _Sum = CDblSafe(dr.Item(5), 2, 0)
            _VatRate = CDblSafe(dr.Item(6), 2, 0)
            _SumVat = CDblSafe(dr.Item(7), 2, 0)
            _SumVatCorrection = Convert.ToInt32(Math.Floor(CRound(_SumVat - CRound(_Sum * _VatRate / 100)) * 100))
            _SumTotal = CRound(_Sum + _SumVat)
            _SumLTL = CDblSafe(dr.Item(8), 2, 0)
            _SumCorrectionLTL = Convert.ToInt32(Math.Floor(CRound(_SumLTL - CRound(_Sum * pCurrencyRate)) * 100))
            _SumVatLTL = CDblSafe(dr.Item(9), 2, 0)
            _SumVatCorrectionLTL = Convert.ToInt32(Math.Floor(CRound(_SumVatLTL - CRound(_SumVat * pCurrencyRate)) * 100))
            _SumTotalLTL = CRound(_SumLTL + _SumVatLTL)
            _CurrencyRateChangeEffect = CDblSafe(dr.Item(10), 2, 0)
            _AccountCurrencyRateChangeEffect = CLongSafe(dr.Item(11), 0)
            _InvoiceID = CIntSafe(dr.Item(12), 0)
            _InvoiceContent = CStrSafe(dr.Item(13)).Trim
            If _InvoiceID > 0 Then
                _InvoiceIsMade = ConvertDbBoolean(CIntSafe(dr.Item(14), 0))
                If _InvoiceIsMade Then
                    _InvoiceDateAndNumber = "Išrašyta SF " & CDateSafe(dr.Item(15), Today).ToShortDateString _
                        & " Nr. " & CStrSafe(dr.Item(16)).Trim
                Else
                    _InvoiceDateAndNumber = "Gauta SF " & CDateSafe(dr.Item(15), Today).ToShortDateString _
                        & " Nr. " & CStrSafe(dr.Item(16)).Trim
                End If
            End If

            _InvoiceCurrencyCode = CStrSafe(dr.Item(17)).Trim
            _InvoiceCurrencyRate = CDblSafe(dr.Item(18), 6, 0)
            _InvoiceSumOriginal = CDblSafe(dr.Item(19), 2, 0)
            _InvoiceSumVatOriginal = CDblSafe(dr.Item(20), 2, 0)
            _InvoiceSumLTL = CDblSafe(dr.Item(21), 2, 0)
            _InvoiceSumVatLTL = CDblSafe(dr.Item(22), 2, 0)
            _InvoiceSumTotalOriginal = CRound(_InvoiceSumOriginal + _InvoiceSumVatOriginal, 2)
            _InvoiceSumTotalLTL = CRound(_InvoiceSumLTL + _InvoiceSumVatLTL, 2)
            _Account = CLongSafe(dr.Item(23), 0)
            _AccountVat = CLongSafe(dr.Item(24), 0)

            If _InvoiceID > 0 Then
                If pCurrencyCode.Trim.ToUpper = _InvoiceCurrencyCode.Trim.ToUpper Then
                    _InvoiceSumTotal = _InvoiceSumTotalOriginal
                ElseIf CRound(pCurrencyRate, 6) > 0 Then
                    _InvoiceSumTotal = CRound(_InvoiceSumTotalLTL / CRound(pCurrencyRate, 6))
                Else
                    _InvoiceSumTotal = _InvoiceSumTotalLTL
                End If
            End If

            _Person = PersonInfo.GetPersonInfo(dr, 25)

            _FinancialDataCanChange = nFinancialDataCanChange

            MarkOld()
            ValidationRules.CheckRules()

        End Sub

        Friend Sub Insert(ByVal parent As AdvanceReport)

            Dim myComm As New SQLCommand("InsertAdvanceReportItem")
            AddWithParamsGeneral(myComm)
            AddWithParamsFinancial(myComm)
            myComm.AddParam("?AA", parent.ID)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As AdvanceReport)

            Dim myComm As SQLCommand
            If parent.ChronologicValidator.FinancialDataCanChange Then
                myComm = New SQLCommand("UpdateAdvanceReportItem")
                AddWithParamsFinancial(myComm)
            Else
                myComm = New SQLCommand("UpdateAdvanceReportItemNonFinancial")
            End If
            myComm.AddParam("?CD", _ID)
            AddWithParamsGeneral(myComm)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteAdvanceReportItem")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub

        Private Sub AddWithParamsGeneral(ByRef myComm As SQLCommand)

            myComm.AddParam("?AB", _DocumentNumber.Trim)
            myComm.AddParam("?AC", _Content.Trim)
            myComm.AddParam("?AK", _Date.Date)
            If _Person Is Nothing OrElse Not _Person.ID > 0 Then
                myComm.AddParam("?AL", 0)
            Else
                myComm.AddParam("?AL", _Person.ID)
            End If

        End Sub

        Private Sub AddWithParamsFinancial(ByRef myComm As SQLCommand)

            If _Income Then
                myComm.AddParam("?AD", ConvertEnumDatabaseStringCode(BookEntryType.Kreditas))
            Else
                myComm.AddParam("?AD", ConvertEnumDatabaseStringCode(BookEntryType.Debetas))
            End If
            myComm.AddParam("?AE", CRound(_Sum))
            myComm.AddParam("?AF", CRound(_VatRate))
            myComm.AddParam("?AG", CRound(_SumVat))
            myComm.AddParam("?AH", CRound(_SumLTL))
            myComm.AddParam("?AI", CRound(_SumVatLTL))
            myComm.AddParam("?AJ", _InvoiceID)
            myComm.AddParam("?AM", _Account)
            myComm.AddParam("?AN", _AccountVat)
            myComm.AddParam("?AO", CRound(_CurrencyRateChangeEffect, 2))
            myComm.AddParam("?AP", _AccountCurrencyRateChangeEffect)

        End Sub

#End Region

    End Class

End Namespace