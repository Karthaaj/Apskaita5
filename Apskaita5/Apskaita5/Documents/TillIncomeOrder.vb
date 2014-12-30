Imports ApskaitaObjects.HelperLists
Namespace Documents

    <Serializable()> _
    Public Class TillIncomeOrder
        Inherits BusinessBase(Of TillIncomeOrder)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _ChronologicValidator As SimpleChronologicValidator
        Private _Account As CashAccountInfo = Nothing
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _DocumentSerial As String = ""
        Private _DocumentNumber As Integer = 0
        Private _AddDateToNumberOptionWasUsed As Boolean = False
        Private _FullDocumentNumber As String = ""
        Private _Payer As PersonInfo = Nothing
        Private _IsUnderPayRoll As Boolean = False
        Private _Content As String = ""
        Private _CurrencyRateInAccount As Double = 1
        Private _Sum As Double = 0
        Private _SumLTL As Double = 0
        Private _SumCorespondences As Double = 0
        Private _AccountCurrencyRateChangeImpact As Long = 0
        Private _CurrencyRateChangeImpact As Double = 0
        Private _PayersRepresentative As String = ""
        Private _AttachmentsDescription As String = ""
        Private _AdditionalContent As String = ""
        Private _AdvanceReportID As Integer = 0
        Private _AdvanceReportDescription As String = ""
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _BookEntryItems As General.BookEntryList

        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _BookEntryItemsSortedList As Csla.SortedBindingList(Of General.BookEntry) = Nothing


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property ChronologicValidator() As SimpleChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologicValidator
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

        Public Property Account() As HelperLists.CashAccountInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Account
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As CashAccountInfo)

                CanWriteProperty(True)

                If Not _ChronologicValidator.FinancialDataCanChange Then Exit Property

                If Not (_Account Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Account Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _Account = value) Then

                    Dim OldCurrencyCode, NewCurrencyCode As String
                    If _Account Is Nothing OrElse Not _Account.ID > 0 Then
                        OldCurrencyCode = GetCurrentCompany.BaseCurrency
                    Else
                        OldCurrencyCode = GetCurrencySafe(_Account.CurrencyCode, GetCurrentCompany.BaseCurrency)
                    End If
                    If value Is Nothing OrElse Not value.ID > 0 Then
                        NewCurrencyCode = GetCurrentCompany.BaseCurrency
                    Else
                        NewCurrencyCode = GetCurrencySafe(value.CurrencyCode, GetCurrentCompany.BaseCurrency)
                    End If

                    _Account = value

                    PropertyHasChanged()

                    If Not CurrenciesEquals(OldCurrencyCode, NewCurrencyCode, GetCurrentCompany.BaseCurrency) Then

                        If CurrenciesEquals(NewCurrencyCode, GetCurrentCompany.BaseCurrency, GetCurrentCompany.BaseCurrency) Then
                            _CurrencyRateInAccount = 1
                        Else
                            _CurrencyRateInAccount = 0
                        End If

                        PropertyHasChanged("AccountCurrency")
                        PropertyHasChanged("CurrencyRateInAccount")

                        Recalculate(True)

                    End If

                End If
            End Set
        End Property

        Public ReadOnly Property AccountCurrency() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If Not _Account Is Nothing AndAlso _Account.ID > 0 Then Return _Account.CurrencyCode
                Return GetCurrentCompany.BaseCurrency
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
                    If _AddDateToNumberOptionWasUsed AndAlso _DocumentNumber > 0 Then
                        _FullDocumentNumber = GetFullDocumentNumber()
                        PropertyHasChanged("FullDocumentNumber")
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property OldDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldDate
            End Get
        End Property

        Public Property DocumentSerial() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocumentSerial.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _DocumentSerial.Trim.ToUpper <> value.Trim.ToUpper Then
                    _DocumentSerial = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property DocumentNumber() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocumentNumber
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If value < 0 Then value = 0
                If _DocumentNumber <> value Then
                    _DocumentNumber = value
                    PropertyHasChanged()
                    _FullDocumentNumber = GetFullDocumentNumber()
                    PropertyHasChanged("FullDocumentNumber")
                End If
            End Set
        End Property

        Public ReadOnly Property AddDateToNumberOptionWasUsed() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AddDateToNumberOptionWasUsed
            End Get
        End Property

        Public ReadOnly Property FullDocumentNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _FullDocumentNumber.Trim
            End Get
        End Property

        Public Property Payer() As PersonInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Payer
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As PersonInfo)
                CanWriteProperty(True)
                If Not (_Payer Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Payer Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _Payer = value) Then
                    _Payer = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property IsUnderPayRoll() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsUnderPayRoll
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _IsUnderPayRoll <> value Then
                    _IsUnderPayRoll = value
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

        Public Property CurrencyRateInAccount() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrencyRateInAccount, 6)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)

                If Not _ChronologicValidator.FinancialDataCanChange Then Exit Property

                If _Account Is Nothing OrElse Not _Account.ID > 0 OrElse _
                    CurrenciesEquals(_Account.CurrencyCode, GetCurrentCompany.BaseCurrency, GetCurrentCompany.BaseCurrency) Then Exit Property

                If CRound(_CurrencyRateInAccount, 6) <> CRound(value, 6) Then

                    _CurrencyRateInAccount = CRound(value, 6)
                    PropertyHasChanged()

                    Recalculate(True)

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
                If Not _ChronologicValidator.FinancialDataCanChange Then Exit Property
                If CRound(_Sum) <> CRound(value) Then
                    _Sum = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public ReadOnly Property SumLTL() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumLTL)
            End Get
        End Property

        Public ReadOnly Property SumCorespondences() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_SumCorespondences)
            End Get
        End Property

        Public Property AccountCurrencyRateChangeImpact() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountCurrencyRateChangeImpact
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If Not _ChronologicValidator.FinancialDataCanChange Then Exit Property
                If _AccountCurrencyRateChangeImpact <> value Then
                    _AccountCurrencyRateChangeImpact = value
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property CurrencyRateChangeImpact() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrencyRateChangeImpact)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If Not _ChronologicValidator.FinancialDataCanChange Then Exit Property
                If CRound(_CurrencyRateChangeImpact) <> CRound(value) Then
                    _CurrencyRateChangeImpact = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property PayersRepresentative() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PayersRepresentative.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _PayersRepresentative.Trim <> value.Trim Then
                    _PayersRepresentative = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AttachmentsDescription() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AttachmentsDescription.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _AttachmentsDescription.Trim <> value.Trim Then
                    _AttachmentsDescription = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AdditionalContent() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AdditionalContent.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _AdditionalContent.Trim <> value.Trim Then
                    _AdditionalContent = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property AdvanceReportID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AdvanceReportID
            End Get
        End Property

        Public ReadOnly Property AdvanceReportDescription() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AdvanceReportDescription.Trim
            End Get
        End Property

        Public ReadOnly Property BookEntryItems() As General.BookEntryList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _BookEntryItems
            End Get
        End Property

        Public ReadOnly Property BookEntryItemsSorted() As Csla.SortedBindingList(Of General.BookEntry)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _BookEntryItemsSortedList Is Nothing Then _BookEntryItemsSortedList = _
                    New Csla.SortedBindingList(Of General.BookEntry)(_BookEntryItems)
                Return _BookEntryItemsSortedList
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_DocumentSerial.Trim) _
                OrElse Not String.IsNullOrEmpty(_FullDocumentNumber.Trim) _
                OrElse Not String.IsNullOrEmpty(_Content.Trim) _
                OrElse CRound(_Sum) > 0 OrElse _BookEntryItems.Count > 0)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _BookEntryItems.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _BookEntryItems.IsValid
            End Get
        End Property



        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
            If Not _BookEntryItems.IsValid Then result = AddWithNewLine(result, _
                _BookEntryItems.GetAllBrokenRules, False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If MyBase.BrokenRulesCollection.WarningCount > 0 Then _
                result = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
            result = AddWithNewLine(result, _BookEntryItems.GetAllWarnings, False)
            Return result
        End Function


        Public Overrides Function Save() As TillIncomeOrder

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf _
                & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Private Sub BookEntryItems_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _BookEntryItems.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

            _SumCorespondences = _BookEntryItems.GetSum
            PropertyHasChanged("SumCorespondences")

            If Me.AccountCurrency.Trim.ToUpper = GetCurrentCompany.BaseCurrency Then
                _Sum = _SumCorespondences
                _SumLTL = _SumCorespondences
                PropertyHasChanged("Sum")
                PropertyHasChanged("SumLTL")
            End If

        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As TillIncomeOrder = DirectCast(MyBase.GetClone(), TillIncomeOrder)
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
        ''' Helper method. Takes care of BookEntryItems loosing its handler. See GetClone method.
        ''' </summary>
        Friend Sub RestoreChildListsHandles()
            Try
                RemoveHandler _BookEntryItems.ListChanged, AddressOf BookEntryItems_Changed
            Catch ex As Exception
            End Try
            AddHandler _BookEntryItems.ListChanged, AddressOf BookEntryItems_Changed
        End Sub


        Private Sub Recalculate(ByVal RaisePropertyChangedEvents As Boolean)

            _SumLTL = CRound(_Sum * _CurrencyRateInAccount)

            If RaisePropertyChangedEvents Then
                PropertyHasChanged("SumLTL")
            End If

        End Sub

        Private Function GetFullDocumentNumber() As String
            If _AddDateToNumberOptionWasUsed AndAlso _DocumentNumber > 0 Then
                Return _Date.Year.ToString & GetMinLengthString( _
                    _Date.Month.ToString, 2, "0"c) & GetMinLengthString( _
                    _Date.Day.ToString, 2, "0"c) & "-" & _DocumentNumber
            ElseIf _DocumentNumber > 0 Then
                Return _DocumentNumber.ToString
            Else
                Return ""
            End If
        End Function


        Public Sub LoadAdvanceReport(ByVal nAdvanceReportInfo As ActiveReports.AdvanceReportInfo)

            If Not CRound(nAdvanceReportInfo.IncomeSumTotal - nAdvanceReportInfo.ExpensesSumTotal) > 0 Then _
                Throw New Exception("Klaida. Prie kasos pajamų orderio gali būti pridedama " _
                & "tik tokia avanso apyskaita, kurioje surinktų pajamų suma didesnė už " _
                & "patirtų išlaidų sumą.")

            _AdvanceReportID = nAdvanceReportInfo.ID
            _AdvanceReportDescription = nAdvanceReportInfo.ToString

            PropertyHasChanged("AdvanceReportID")
            PropertyHasChanged("AdvanceReportDescription")

            If _ChronologicValidator.FinancialDataCanChange Then

                If CurrenciesEquals(Me.AccountCurrency, GetCurrentCompany.BaseCurrency, GetCurrentCompany.BaseCurrency) Then
                    _Sum = CRound(nAdvanceReportInfo.IncomeSumTotalLTL - nAdvanceReportInfo.ExpensesSumTotalLTL)
                ElseIf CurrenciesEquals(Me.AccountCurrency, nAdvanceReportInfo.CurrencyCode, GetCurrentCompany.BaseCurrency) Then
                    _Sum = CRound(nAdvanceReportInfo.IncomeSumTotal - nAdvanceReportInfo.ExpensesSumTotal)
                Else
                    _Sum = CRound(CRound(nAdvanceReportInfo.IncomeSumTotalLTL _
                        - nAdvanceReportInfo.ExpensesSumTotalLTL) * _CurrencyRateInAccount)
                End If

                PropertyHasChanged("Sum")
                Recalculate(True)

            End If

        End Sub

        Public Sub ClearAdvanceReport()
            _AdvanceReportID = 0
            _AdvanceReportDescription = ""
            PropertyHasChanged("AdvanceReportID")
            PropertyHasChanged("AdvanceReportDescription")
        End Sub


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Content
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf AccountValidation, New Validation.RuleArgs("Account"))
            ValidationRules.AddRule(AddressOf PayerValidation, New Validation.RuleArgs("Payer"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologicValidator"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("DocumentNumber", "orderio numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos su lėšomis aprašas (turinys)"))
            ValidationRules.AddRule(AddressOf SumLTLValidation, New Validation.RuleArgs("SumLTL"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Sum", "operacijos suma"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("CurrencyRateInAccount", "sąskaitos valiutos kursas"))
            ValidationRules.AddRule(AddressOf AccountCurrencyRateChangeImpactValidation, _
                New Validation.RuleArgs("AccountCurrencyRateChangeImpact"))
            ValidationRules.AddRule(AddressOf AdvanceReportDescriptionValidation, _
                New Validation.RuleArgs("AdvanceReportDescription"))

            ValidationRules.AddDependantProperty("IsUnderPayRoll", "Payer", False)
            ValidationRules.AddDependantProperty("CurrencyRateChangeImpact", "SumLTL", False)
            ValidationRules.AddDependantProperty("SumCorespondences", "SumLTL", False)
            ValidationRules.AddDependantProperty("CurrencyRateChangeImpact", "AccountCurrencyRateChangeImpact", False)
            ValidationRules.AddDependantProperty("IsUnderPayRoll", "AdvanceReportDescription", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that a valid account is set.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As TillIncomeOrder = DirectCast(target, TillIncomeOrder)

            If ValObj._Account Is Nothing OrElse Not ValObj._Account.ID > 0 Then

                e.Description = "Nenurodyta sąskaita, kurioje atliekama operacija."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf ValObj._Account.Type = CashAccountType.BankAccount OrElse _
                ValObj._Account.Type = CashAccountType.PseudoBankAccount Then

                e.Description = "Pasirinktos sąskaitos tipas yra banko sąskaita, " _
                    & "joje vienintelis galimas operacijos tipas yra banko pavedimas, " _
                    & "o šios operacijos tipas yra kasos išlaidų orderis."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that a Payer is set unless a pay roll is attached.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function PayerValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As TillIncomeOrder = DirectCast(target, TillIncomeOrder)

            If Not ValObj._IsUnderPayRoll AndAlso (ValObj._Payer Is Nothing _
                OrElse Not ValObj._Payer.ID > 0) Then

                e.Description = "Nenurodytas lėšų mokėtojas."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf ValObj._IsUnderPayRoll AndAlso Not (ValObj._Payer Is Nothing _
                OrElse Not ValObj._Payer.ID > 0) Then

                e.Description = "Gaunant mokėjimą pagal žiniaraštį, mokantis asmuo nenurodomas."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that a sum of order is provided.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function SumLTLValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As TillIncomeOrder = DirectCast(target, TillIncomeOrder)

            If Not CRound(ValObj._SumLTL) > 0 Then

                e.Description = "Nenurodyta lėšų suma."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf CRound(ValObj._SumLTL) <> CRound(ValObj._SumCorespondences + _
                ValObj._CurrencyRateChangeImpact) Then

                e.Description = "Gaunama lėšų suma bazine valiuta turi būti lygi korespondencijų sumai " _
                    & "plius valiutos kurso pasikeitimo įtaka."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that a valid currency rate change impact account is set when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountCurrencyRateChangeImpactValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As TillIncomeOrder = DirectCast(target, TillIncomeOrder)

            If Not ValObj._AccountCurrencyRateChangeImpact > 0 AndAlso CRound(ValObj._CurrencyRateChangeImpact) <> 0 Then

                e.Description = "Nenurodyta valiutos kurso pasikeitimo įtakos apskaitos sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that a pay roll based order wouldn't have an advance reports attached.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AdvanceReportDescriptionValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As TillIncomeOrder = DirectCast(target, TillIncomeOrder)

            If ValObj._IsUnderPayRoll AndAlso ValObj._AdvanceReportID > 0 Then

                e.Description = "Lėšų gavimas pagal žiniaraštį negali turėti susietos avanso apyskaitos."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Documents.TillIncomeOrder2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.TillIncomeOrder1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.TillIncomeOrder2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.TillIncomeOrder3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Documents.TillIncomeOrder3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewTillIncomeOrder() As TillIncomeOrder

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim result As New TillIncomeOrder
            result._BookEntryItems = General.BookEntryList.NewBookEntryList(BookEntryType.Kreditas)
            result._AddDateToNumberOptionWasUsed = GetCurrentCompany.AddDateToTillIncomeOrderNumber
            result._ChronologicValidator = SimpleChronologicValidator.NewSimpleChronologicValidator("kasos pajamų orderis")
            result.ValidationRules.CheckRules()
            Return result

        End Function

        Public Shared Function GetTillIncomeOrder(ByVal nID As Integer) As TillIncomeOrder
            Return DataPortal.Fetch(Of TillIncomeOrder)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteTillIncomeOrder(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
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

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As New SQLCommand("FetchTillIncomeOrder")
            myComm.AddParam("?BD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Lėšų operacija, kurios ID='" & criteria.ID & "', nerasta.)")

                _ID = criteria.ID

                Dim dr As DataRow = myData.Rows(0)

                _Date = CDateSafe(dr.Item(0), Today)
                _OldDate = _Date
                ' _FullDocumentNumber = CStrSafe(dr.Item(1)).Trim
                _Content = CStrSafe(dr.Item(2)).Trim
                _DocumentSerial = CStrSafe(dr.Item(3)).Trim
                _DocumentNumber = CIntSafe(dr.Item(4), 0)
                _AddDateToNumberOptionWasUsed = ConvertDbBoolean(CIntSafe(dr.Item(5), 0))
                _FullDocumentNumber = GetFullDocumentNumber()
                _CurrencyRateInAccount = CDblSafe(dr.Item(6), 6, 0)
                _Sum = CDblSafe(dr.Item(7), 2, 0)
                _SumLTL = CDblSafe(dr.Item(8), 2, 0)
                _IsUnderPayRoll = ConvertDbBoolean(CIntSafe(dr.Item(9), 0))
                _AccountCurrencyRateChangeImpact = CLongSafe(dr.Item(10), 0)
                _CurrencyRateChangeImpact = CDblSafe(dr.Item(11), 2, 0)
                _PayersRepresentative = CStrSafe(dr.Item(12)).Trim
                _AdditionalContent = CStrSafe(dr.Item(13)).Trim
                _AttachmentsDescription = CStrSafe(dr.Item(14)).Trim
                ' _DocumentState = CStrSafe(dr.Item(15)).Trim
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(16), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(17), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _AdvanceReportID = CIntSafe(dr.Item(18), 0)
                _AdvanceReportDescription = CStrSafe(dr.Item(19)).Trim & " Nr. " & CStrSafe(dr.Item(20)).Trim _
                    & " : " & GetLimitedLengthString(CStrSafe(dr.Item(21)).Trim, 100)
                _Account = CashAccountInfo.GetCashAccountInfo(dr, 22)
                _Payer = PersonInfo.GetPersonInfo(dr, 36)

            End Using

            _ChronologicValidator = SimpleChronologicValidator.GetSimpleChronologicValidator( _
                _ID, _Date, "kasos pajamų orderis")

            myComm = New SQLCommand("BookEntriesFetch")
            myComm.AddParam("?BD", _ID)

            Using myData As DataTable = myComm.Fetch
                _BookEntryItems = General.BookEntryList.GetBookEntryList(myData, _
                    BookEntryType.Kreditas, _ChronologicValidator.FinancialDataCanChange, _
                    _ChronologicValidator.FinancialDataCanChangeExplanation, _AccountCurrencyRateChangeImpact)
            End Using

            _SumCorespondences = _BookEntryItems.GetSum

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            If _Date.Date <= GetCurrentCompany.LastClosingDate.Date Then Throw New Exception( _
                "Klaida. Neleidžiama koreguoti operacijų po uždarymo (" _
                & GetCurrentCompany.LastClosingDate & ").")

            CheckIfNumberUnique()

            If _AccountCurrencyRateChangeImpact > 0 AndAlso _
                _BookEntryItems.GetSumInAccount(_AccountCurrencyRateChangeImpact) > 0 Then _
                _CurrencyRateChangeImpact = CRound(_CurrencyRateChangeImpact _
                    + _BookEntryItems.GetSumInAccount(_AccountCurrencyRateChangeImpact))

            Dim JE As General.JournalEntry = GetJournalEntry()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()

            _ID = JE.ID
            _InsertDate = JE.InsertDate
            _UpdateDate = JE.UpdateDate

            Dim myComm As New SQLCommand("InsertTillIncomeOrder")
            AddWithParamsGeneral(myComm)
            AddWithParamsFinancial(myComm)
            myComm.AddParam("?AB", ConvertDbBoolean(_AddDateToNumberOptionWasUsed))

            myComm.Execute()

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            _ChronologicValidator = SimpleChronologicValidator.GetSimpleChronologicValidator( _
                _ID, _ChronologicValidator.CurrentOperationDate, "kasos pajamų orderis")
            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Dokumente yra klaidų:" & vbCrLf & Me.GetAllBrokenRules)

            CheckIfNumberUnique()

            If _AccountCurrencyRateChangeImpact > 0 AndAlso _
                _BookEntryItems.GetSumInAccount(_AccountCurrencyRateChangeImpact) > 0 Then _
                _CurrencyRateChangeImpact = CRound(_CurrencyRateChangeImpact _
                    + _BookEntryItems.GetSumInAccount(_AccountCurrencyRateChangeImpact))

            Dim JE As General.JournalEntry = GetJournalEntry()

            Dim myComm As SQLCommand
            If _ChronologicValidator.FinancialDataCanChange Then
                myComm = New SQLCommand("UpdateTillIncomeOrder")
                AddWithParamsFinancial(myComm)
            Else
                myComm = New SQLCommand("UpdateTillIncomeOrderNonFinancial")
            End If
            AddWithParamsGeneral(myComm)

            DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()
            _UpdateDate = JE.UpdateDate

            myComm.Execute()

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted(DirectCast(criteria, Criteria).ID, _
                DocumentType.TillIncomeOrder)

            Dim myComm As New SQLCommand("DeleteTillIncomeOrder")
            myComm.AddParam("?BD", DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionBegin()

            General.JournalEntry.DoDelete(DirectCast(criteria, Criteria).ID)

            myComm.Execute()

            DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub


        Private Sub AddWithParamsGeneral(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _ID)
            myComm.AddParam("?AD", _DocumentNumber)
            myComm.AddParam("?AE", _DocumentSerial.Trim)
            myComm.AddParam("?AF", _PayersRepresentative.Trim)
            myComm.AddParam("?AG", _AttachmentsDescription.Trim)
            myComm.AddParam("?AH", _AdditionalContent.Trim)
            If Not _IsUnderPayRoll AndAlso _AdvanceReportID > 0 Then
                myComm.AddParam("?AI", _AdvanceReportID)
            Else
                myComm.AddParam("?AI", 0)
            End If
            myComm.AddParam("?AM", ConvertDbBoolean(_IsUnderPayRoll))
            myComm.AddParam("?AQ", "") ' DocumentState

        End Sub

        Private Sub AddWithParamsFinancial(ByRef myComm As SQLCommand)
            myComm.AddParam("?AC", _Account.ID)
            myComm.AddParam("?AJ", CRound(_CurrencyRateInAccount, 6))
            myComm.AddParam("?AK", CRound(_Sum))
            myComm.AddParam("?AL", CRound(_SumLTL))
            If _AccountCurrencyRateChangeImpact > 0 AndAlso CRound(_CurrencyRateChangeImpact) <> 0 Then
                myComm.AddParam("?AN", _AccountCurrencyRateChangeImpact)
                myComm.AddParam("?AO", CRound(_CurrencyRateChangeImpact))
            Else
                myComm.AddParam("?AN", 0)
                myComm.AddParam("?AO", 0)
            End If
        End Sub

        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry
            If IsNew Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.TillIncomeOrder)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_ID, DocumentType.TillIncomeOrder)
                If result.UpdateDate <> _UpdateDate Then Throw New Exception( _
                    "Klaida. Dokumento atnaujinimo data pasikeitė. Teigtina, kad kitas " _
                    & "vartotojas redagavo šį objektą.")
            End If

            result.Content = _Content
            result.Date = _Date
            result.DocNumber = _DocumentSerial & _FullDocumentNumber
            If _IsUnderPayRoll Then
                result.Person = Nothing
            Else
                result.Person = _Payer
            End If

            If _ChronologicValidator.FinancialDataCanChange Then

                Dim FullBookEntryList As BookEntryInternalList = BookEntryInternalList. _
                NewBookEntryInternalList(BookEntryType.Debetas)

                FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                    BookEntryType.Debetas, _Account.Account, CRound(_SumLTL), Nothing))

                If CRound(_CurrencyRateChangeImpact) > 0 AndAlso _AccountCurrencyRateChangeImpact > 0 Then
                    FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                        BookEntryType.Kreditas, _AccountCurrencyRateChangeImpact, _
                        CRound(_CurrencyRateChangeImpact), Nothing))
                ElseIf CRound(_CurrencyRateChangeImpact) < 0 AndAlso _AccountCurrencyRateChangeImpact > 0 Then
                    FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                        BookEntryType.Debetas, _AccountCurrencyRateChangeImpact, _
                        CRound(-_CurrencyRateChangeImpact), Nothing))
                Else
                    _AccountCurrencyRateChangeImpact = 0
                End If

                For Each i As General.BookEntry In _BookEntryItems

                    If i.Account <> _AccountCurrencyRateChangeImpact Then _
                        FullBookEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                        BookEntryType.Kreditas, i.Account, i.Amount, i.Person))

                Next

                FullBookEntryList.Aggregate()

                result.DebetList.Clear()
                result.CreditList.Clear()

                result.DebetList.LoadBookEntryListFromInternalList(FullBookEntryList, False)
                result.CreditList.LoadBookEntryListFromInternalList(FullBookEntryList, False)

            End If

            If Not result.IsValid Then Throw New Exception("Klaida. Nepavyko generuoti " _
                & "bendrojo žurnalo įrašo.")

            Return result

        End Function

        Private Sub CheckIfNumberUnique()

            Dim myComm As SQLCommand
            If _AddDateToNumberOptionWasUsed Then
                myComm = New SQLCommand("CheckIfTillIncomeOrderNumberUniqueWithDate")
                myComm.AddParam("?ND", _Date)
            Else
                myComm = New SQLCommand("CheckIfTillIncomeOrderNumberUnique")
            End If
            myComm.AddParam("?SR", _DocumentSerial.Trim.ToUpper)
            myComm.AddParam("?NM", _DocumentNumber)
            myComm.AddParam("?IN", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Kasos pajamų orderis su tokia serija ir numeriu jau yra.")
            End Using

        End Sub

#End Region

    End Class

End Namespace