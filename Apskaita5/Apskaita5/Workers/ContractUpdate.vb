Namespace Workers

    <Serializable()> _
    Public Class ContractUpdate
        Inherits BusinessBase(Of ContractUpdate)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _ChronologicValidator As ContractChronologicValidator
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _ExtraPayID As Integer = 0
        Private _AnnualHolidayID As Integer = 0
        Private _HolidayCorrectionID As Integer = 0
        Private _NpdID As Integer = 0
        Private _PnpdID As Integer = 0
        Private _WageID As Integer = 0
        Private _WorkLoadID As Integer = 0
        Private _PositionID As Integer = 0

        Private _PersonID As Integer = 0
        Private _PersonName As String = ""
        Private _PersonCode As String = ""
        Private _PersonSodraCode As String = ""
        Private _PersonAddress As String = ""
        Private _Serial As String = ""
        Private _Number As Integer = 0

        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _Content As String = ""
        Private _PositionChanged As Boolean = False
        Private _WageChanged As Boolean = False
        Private _ExtraPayChanged As Boolean = False
        Private _AnnualHolidayChanged As Boolean = False
        Private _HolidayCorrectionChanged As Boolean = False
        Private _NpdChanged As Boolean = False
        Private _PnpdChanged As Boolean = False
        Private _WorkLoadChanged As Boolean = False
        Private _Position As String = ""
        Private _Wage As Double = 0
        Private _WageType As WageType = Workers.WageType.Position
        Private _HumanReadableWageType As String = ConvertEnumHumanReadable(Workers.WageType.Position)
        Private _ExtraPay As Double = 0
        Private _AnnualHoliday As Integer = 28
        Private _HolidayCorrection As Double = 0
        Private _NPD As Double = 0
        Private _PNPD As Double = 0
        Private _WorkLoad As Double = 1


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property ChronologicValidator() As ContractChronologicValidator
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

        Public ReadOnly Property ExtraPayID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ExtraPayID
            End Get
        End Property

        Public ReadOnly Property AnnualHolidayID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AnnualHolidayID
            End Get
        End Property

        Public ReadOnly Property HolidayCorrectionID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _HolidayCorrectionID
            End Get
        End Property

        Public ReadOnly Property NpdID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _NpdID
            End Get
        End Property

        Public ReadOnly Property PnpdID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PnpdID
            End Get
        End Property

        Public ReadOnly Property WageID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WageID
            End Get
        End Property

        Public ReadOnly Property WorkLoadID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WorkLoadID
            End Get
        End Property

        Public ReadOnly Property PositionID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PositionID
            End Get
        End Property


        Public ReadOnly Property PersonID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonID
            End Get
        End Property

        Public ReadOnly Property PersonName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonName.Trim
            End Get
        End Property

        Public ReadOnly Property PersonCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonCode.Trim
            End Get
        End Property

        Public ReadOnly Property PersonSodraCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonSodraCode.Trim
            End Get
        End Property

        Public ReadOnly Property PersonAddress() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PersonAddress.Trim
            End Get
        End Property

        Public ReadOnly Property Serial() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Serial.Trim
            End Get
        End Property

        Public ReadOnly Property Number() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number
            End Get
        End Property

        Public ReadOnly Property OldDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldDate
            End Get
        End Property


        Public ReadOnly Property HolidayCompensationPayed() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _ChronologicValidator.TerminationCanBeCanceled
            End Get
        End Property

        Public ReadOnly Property ContractDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologicValidator.ContractDate
            End Get
        End Property

        Public ReadOnly Property ContractTerminationDate() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _ChronologicValidator.ContractTerminationDate = Date.MaxValue Then Return ""
                Return _ChronologicValidator.ContractTerminationDate.ToString("yyyy-MM-dd")
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

        Public Property PositionChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PositionChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _PositionChanged <> value Then
                    _PositionChanged = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property WageChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WageChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _WageChanged <> value Then
                        _WageChanged = value
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property ExtraPayChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ExtraPayChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _ExtraPayChanged <> value Then
                        _ExtraPayChanged = value
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property AnnualHolidayChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AnnualHolidayChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _AnnualHolidayChanged <> value Then
                    _AnnualHolidayChanged = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property HolidayCorrectionChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _HolidayCorrectionChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _HolidayCorrectionChanged <> value Then
                    _HolidayCorrectionChanged = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property NpdChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _NpdChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _NpdChanged <> value Then
                        _NpdChanged = value
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property PnpdChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PnpdChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _PnpdChanged <> value Then
                        _PnpdChanged = value
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property WorkLoadChanged() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WorkLoadChanged
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _WorkLoadChanged <> value Then
                    _WorkLoadChanged = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Position() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Position.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Position.Trim <> value.Trim Then
                    _Position = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Wage() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Wage)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value < 0 Then value = 0
                    If CRound(_Wage) <> CRound(value) Then
                        _Wage = CRound(value)
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property WageType() As WageType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WageType
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WageType)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _WageType <> value Then
                        _WageType = value
                        _HumanReadableWageType = ConvertEnumHumanReadable(_WageType)
                        PropertyHasChanged()
                        PropertyHasChanged("HumanReadableWageType")
                    End If
                End If
            End Set
        End Property

        Public Property HumanReadableWageType() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _HumanReadableWageType.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value Is Nothing Then value = ""
                    If ConvertEnumHumanReadable(Of WageType)(value) <> _WageType Then
                        _WageType = ConvertEnumHumanReadable(Of WageType)(value)
                        _HumanReadableWageType = ConvertEnumHumanReadable(_WageType)
                        PropertyHasChanged()
                        PropertyHasChanged("WageType")
                    End If
                End If
            End Set
        End Property

        Public Property ExtraPay() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ExtraPay)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value < 0 Then value = 0
                    If CRound(_ExtraPay) <> CRound(value) Then
                        _ExtraPay = CRound(value)
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property AnnualHoliday() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AnnualHoliday
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If value < 0 Then value = 0
                If _AnnualHoliday <> value Then
                    _AnnualHoliday = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property HolidayCorrection() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_HolidayCorrection, 4)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_HolidayCorrection, 4) <> CRound(value, 4) Then
                    _HolidayCorrection = CRound(value, 4)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property NPD() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_NPD)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value < 0 Then value = 0
                    If CRound(_NPD) <> CRound(value) Then
                        _NPD = CRound(value)
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property PNPD() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_PNPD)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                If Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value < 0 Then value = 0
                    If CRound(_PNPD) <> CRound(value) Then
                        _PNPD = CRound(value)
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property WorkLoad() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_WorkLoad, 4)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If value < 0 Then value = 0
                If CRound(_WorkLoad, 4) <> CRound(value, 4) Then
                    _WorkLoad = CRound(value, 4)
                    PropertyHasChanged()
                End If
            End Set
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_Content.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Position.Trim))
            End Get
        End Property


        Public Overrides Function Save() As ContractUpdate

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf & Me.GetAllBrokenRules)
            Return MyBase.Save

        End Function


        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Not MyBase.BrokenRulesCollection.WarningCount > 0 Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning), False)
            Return result
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return "Darbo sutarties " & Me._ChronologicValidator.ContractDate.ToString("yyyy-MM-dd") _
                & " Nr. " & _Serial & _Number.ToString & " pakeitimas"
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologicValidator"))

            ValidationRules.AddRule(AddressOf PositionValidation, New Validation.RuleArgs("Position"))
            ValidationRules.AddRule(AddressOf WageValidation, New Validation.RuleArgs("Wage"))
            ValidationRules.AddRule(AddressOf AnnualHolidayValidation, New Validation.RuleArgs("AnnualHoliday"))
            ValidationRules.AddRule(AddressOf HolidayCorrectionValidation, New Validation.RuleArgs("HolidayCorrection"))
            ValidationRules.AddRule(AddressOf WorkLoadValidation, New Validation.RuleArgs("WorkLoad"))
            ValidationRules.AddRule(AddressOf ContentValidation, New Validation.RuleArgs("Content"))

            ValidationRules.AddDependantProperty("PositionChanged", "Position", False)
            ValidationRules.AddDependantProperty("WageChanged", "Wage", False)
            ValidationRules.AddDependantProperty("AnnualHolidayChanged", "AnnualHoliday", False)
            ValidationRules.AddDependantProperty("HolidayCorrectionChanged", "HolidayCorrection", False)
            ValidationRules.AddDependantProperty("WorkLoadChanged", "WorkLoad", False)
            ValidationRules.AddDependantProperty("PositionChanged", "Content", False)
            ValidationRules.AddDependantProperty("WageChanged", "Content", False)
            ValidationRules.AddDependantProperty("AnnualHolidayChanged", "Content", False)
            ValidationRules.AddDependantProperty("HolidayCorrectionChanged", "Content", False)
            ValidationRules.AddDependantProperty("WorkLoadChanged", "Content", False)
            ValidationRules.AddDependantProperty("ExtraPayChanged", "Content", False)
            ValidationRules.AddDependantProperty("NpdChanged", "Content", False)
            ValidationRules.AddDependantProperty("PnpdChanged", "Content", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property Position is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function PositionValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If ValObj._PositionChanged AndAlso (ValObj._Position Is Nothing OrElse _
                String.IsNullOrEmpty(ValObj._Position.Trim)) Then
                e.Description = "Nenurodytas pareigybės pavadinimas."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property Wage is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function WageValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If ValObj._WageChanged AndAlso Not CRound(ValObj._Wage) > 0 Then
                e.Description = "Nenurodytas darbo užmokestis."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property AnnualHoliday is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AnnualHolidayValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If ValObj._AnnualHolidayChanged AndAlso Not ValObj._AnnualHoliday > 0 Then
                e.Description = "Nenurodyta kasmetinių atostogų norma."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property HolidayCorrection is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function HolidayCorrectionValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If ValObj._HolidayCorrectionChanged AndAlso CRound(ValObj._HolidayCorrection, 4) = 0 Then
                e.Description = "Nenurodytas atostogų korekcijos dydis."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property WorkLoad is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function WorkLoadValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If ValObj._WorkLoadChanged AndAlso Not CRound(ValObj._WorkLoad, 4) > 0 Then
                e.Description = "Nenurodytas krūvis (40 val. darbo savaitė = 1; " _
                    & "20 val. darbo savaitė = 0,5 ir t.t.)"
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that at least one status item is selected to be updated.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ContentValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As ContractUpdate = DirectCast(target, ContractUpdate)

            If Not ValObj._AnnualHolidayChanged AndAlso Not ValObj._ExtraPayChanged _
                AndAlso Not ValObj._HolidayCorrectionChanged AndAlso Not ValObj._NpdChanged _
                AndAlso Not ValObj._PnpdChanged AndAlso Not ValObj._PositionChanged _
                AndAlso Not ValObj._WageChanged AndAlso Not ValObj._WorkLoadChanged Then
                e.Description = "Nepasirinktas nė vienas keistinas darbo sutarties parametras."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Workers.Contract2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.Contract1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.Contract2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.Contract3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.Contract3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewContractUpdate(ByVal ContractSerial As String, _
            ByVal ContractNumber As Integer) As ContractUpdate

            Dim result As ContractUpdate = DataPortal.Create(Of ContractUpdate) _
                (New Criteria(ContractSerial, ContractNumber, Today))
            result.MarkNew()
            Return result

        End Function

        Public Shared Function GetContractUpdate(ByVal nID As Integer) As ContractUpdate
            Return DataPortal.Fetch(Of ContractUpdate)(New Criteria(nID))
        End Function

        Public Shared Function GetContractUpdate(ByVal ContractSerial As String, _
            ByVal ContractNumber As Integer, ByVal ContractDate As Date) As ContractUpdate
            Return DataPortal.Fetch(Of ContractUpdate)(New Criteria(ContractSerial, _
                ContractNumber, ContractDate))
        End Function

        Public Shared Sub DeleteContractUpdate(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub

        Public Shared Sub DeleteContractUpdate(ByVal ContractSerial As String, _
            ByVal ContractNumber As Integer, ByVal ContractDate As Date)
            DataPortal.Delete(New Criteria(ContractSerial, ContractNumber, ContractDate))
        End Sub

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _ID As Integer = 0
            Private _ContractSerial As String = ""
            Private _ContractNumber As Integer = 0
            Private _ContractDate As Date = Today
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property ContractSerial() As String
                Get
                    Return _ContractSerial
                End Get
            End Property
            Public ReadOnly Property ContractNumber() As Integer
                Get
                    Return _ContractNumber
                End Get
            End Property
            Public ReadOnly Property ContractDate() As Date
                Get
                    Return _ContractDate
                End Get
            End Property
            Public Sub New(ByVal nID As Integer)
                _ID = nID
            End Sub
            Public Sub New(ByVal nContractSerial As String, ByVal nContractNumber As Integer, _
                ByVal nContractDate As Date)
                _ContractSerial = nContractSerial
                _ContractNumber = nContractNumber
                _ContractDate = nContractDate
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim myComm As SQLCommand
            If criteria.ID > 0 Then
                myComm = New SQLCommand("CreateNewContractUpdate")
                myComm.AddParam("?CD", criteria.ID)
            Else
                myComm = New SQLCommand("CreateNewContractUpdateBySerialNumber")
                myComm.AddParam("?DS", criteria.ContractSerial.Trim)
                myComm.AddParam("?DN", criteria.ContractNumber)
            End If

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then
                    If criteria.ID > 0 Then
                        Throw New Exception("Klaida. Objektas, kurio ID='" & criteria.ID & "', nerastas.")
                    Else
                        Throw New Exception("Klaida. Objektas, kurio serija ir numeris yra '" _
                            & criteria.ContractSerial & criteria.ContractNumber.ToString & "', nerastas.")
                    End If
                End If

                Dim dr As DataRow = myData.Rows(0)

                _Serial = CStrSafe(dr.Item(0)).Trim
                _Number = CIntSafe(dr.Item(1), 0)
                _PersonID = CIntSafe(dr.Item(2), 0)
                _PersonName = CStrSafe(dr.Item(3)).Trim
                _PersonCode = CStrSafe(dr.Item(4)).Trim
                _PersonAddress = CStrSafe(dr.Item(5)).Trim
                _PersonSodraCode = CStrSafe(dr.Item(6)).Trim

            End Using

            _ChronologicValidator = ContractChronologicValidator.NewContractChronologicValidator(_Serial, _Number)

            MarkNew()

            ValidationRules.CheckRules()

        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As SQLCommand
            If criteria.ID > 0 Then
                myComm = New SQLCommand("FetchContractUpdate")
                myComm.AddParam("?CD", criteria.ID)
            Else
                myComm = New SQLCommand("FetchContractUpdateBySerialNumber")
                myComm.AddParam("?DS", criteria.ContractSerial.Trim)
                myComm.AddParam("?DN", criteria.ContractNumber)
                myComm.AddParam("?DT", criteria.ContractDate)
            End If

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then
                    If criteria.ID > 0 Then
                        Throw New Exception("Klaida. Objektas, kurio ID='" & criteria.ID & "', nerastas.")
                    Else
                        Throw New Exception("Klaida. Objektas, kurio serija ir numeris yra '" _
                            & criteria.ContractSerial & criteria.ContractNumber.ToString & "', " _
                            & " data " & criteria.ContractDate.ToShortDateString & ", - nerastas.")
                    End If
                End If

                Dim GeneralDataAcquired As Boolean = False
                Dim curID As Integer = Integer.MaxValue
                Dim curInsertDate As DateTime = Date.MaxValue

                For Each dr As DataRow In myData.Rows

                    If CIntSafe(dr.Item(0), 0) < curID Then curID = CIntSafe(dr.Item(0), 0)
                    If DateTime.SpecifyKind(CDateTimeSafe(dr.Item(6), Date.UtcNow), _
                        DateTimeKind.Utc).ToLocalTime < curInsertDate Then curInsertDate = _
                        DateTime.SpecifyKind(CDateTimeSafe(dr.Item(6), Date.UtcNow), _
                        DateTimeKind.Utc).ToLocalTime

                    If Not GeneralDataAcquired Then

                        _Date = CDateSafe(dr.Item(2), Date.MinValue)
                        _OldDate = _Date
                        _Content = CStrSafe(dr.Item(5)).Trim
                        _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(7), Date.UtcNow), _
                            DateTimeKind.Utc).ToLocalTime
                        _Serial = CStrSafe(dr.Item(8)).Trim
                        _Number = CIntSafe(dr.Item(9), 0)
                        _PersonID = CIntSafe(dr.Item(10), 0)
                        _PersonName = CStrSafe(dr.Item(11)).Trim
                        _PersonCode = CStrSafe(dr.Item(12)).Trim
                        _PersonAddress = CStrSafe(dr.Item(13)).Trim
                        _PersonSodraCode = CStrSafe(dr.Item(14)).Trim

                        GeneralDataAcquired = True

                    End If

                    Select Case ConvertEnumDatabaseStringCode(Of WorkerStatusType)(CStrSafe(dr.Item(1)))

                        Case WorkerStatusType.Employed

                            Throw New Exception("Pasirinktas objektas yra darbo sutartis, o ne darbo sutarties pakeitimas.")

                        Case WorkerStatusType.ExtraPay

                            _ExtraPayID = CIntSafe(dr.Item(0), 0)
                            _ExtraPay = CDblSafe(dr.Item(3), 0)
                            _ExtraPayChanged = True

                        Case WorkerStatusType.Fired

                            Throw New Exception("Pasirinktas objektas yra darbo sutartis, o ne darbo sutarties pakeitimas.")

                        Case WorkerStatusType.Holiday

                            _AnnualHolidayID = CIntSafe(dr.Item(0), 0)
                            _AnnualHoliday = CIntSafe(dr.Item(3), 0)
                            _AnnualHolidayChanged = True

                        Case WorkerStatusType.HolidayCorrection

                            _HolidayCorrectionID = CIntSafe(dr.Item(0), 0)
                            _HolidayCorrection = CDblSafe(dr.Item(3), 4, 0)
                            _HolidayCorrectionChanged = True

                        Case WorkerStatusType.NPD

                            _NpdID = CIntSafe(dr.Item(0), 0)
                            _NPD = CDblSafe(dr.Item(3), 2, 0)
                            _NpdChanged = True

                        Case WorkerStatusType.PNPD

                            _PnpdID = CIntSafe(dr.Item(0), 0)
                            _PNPD = CDblSafe(dr.Item(3), 2, 0)
                            _PnpdChanged = True

                        Case WorkerStatusType.Wage

                            _WageID = CIntSafe(dr.Item(0), 0)
                            _Wage = CDblSafe(dr.Item(3), 2, 0)
                            _WageType = ConvertEnumDatabaseStringCode(Of WageType)(CStrSafe(dr.Item(4)))
                            _HumanReadableWageType = ConvertEnumHumanReadable(_WageType)
                            _WageChanged = True

                        Case WorkerStatusType.WorkLoad

                            _WorkLoadID = CIntSafe(dr.Item(0), 0)
                            _WorkLoad = CDblSafe(dr.Item(3), 4, 0)
                            _WorkLoadChanged = True

                        Case WorkerStatusType.Position

                            _PositionID = CIntSafe(dr.Item(0), 0)
                            _Position = CStrSafe(dr.Item(4))
                            _PositionChanged = True

                    End Select

                Next

                _InsertDate = curInsertDate
                _ID = curID

            End Using

            _ChronologicValidator = ContractChronologicValidator.GetContractChronologicValidator(_ID)

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            DoSave()
        End Sub

        Protected Overrides Sub DataPortal_Update()
            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")
            DoSave()
        End Sub

        Private Sub DoSave()

            CheckIfContractSerialNumberUnique()

            If IsNew Then
                _ChronologicValidator = ContractChronologicValidator.NewContractChronologicValidator(_Serial, _Number)
            Else
                _ChronologicValidator = ContractChronologicValidator.GetContractChronologicValidator(_ID)
            End If

            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Sutarties pakeitimo duomenyse yra klaidų: " _
                & vbCrLf & BrokenRulesCollection.ToString(Validation.RuleSeverity.Error))

            If Not IsNew Then CheckIfUpdateDateChanged()

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If IsNew Then _InsertDate = _UpdateDate

            DatabaseAccess.TransactionBegin()

            UpdateParam(WorkerStatusType.ExtraPay)
            UpdateParam(WorkerStatusType.Holiday)
            UpdateParam(WorkerStatusType.HolidayCorrection)
            UpdateParam(WorkerStatusType.NPD)
            UpdateParam(WorkerStatusType.PNPD)
            UpdateParam(WorkerStatusType.Position)
            UpdateParam(WorkerStatusType.Wage)
            UpdateParam(WorkerStatusType.WorkLoad)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            _Serial = DirectCast(criteria, Criteria).ContractSerial
            _Number = DirectCast(criteria, Criteria).ContractNumber
            _OldDate = DirectCast(criteria, Criteria).ContractDate

            Dim myComm As SQLCommand

            If DirectCast(criteria, Criteria).ID > 0 Then

                myComm = New SQLCommand("FetchContractUpdateSerialNumber")
                myComm.AddParam("?CD", DirectCast(criteria, Criteria).ID)

                Using myData As DataTable = myComm.Fetch

                    If Not myData.Rows.Count > 0 Then Throw New Exception( _
                        "Klaida. Objektas, kurio ID='" & DirectCast(criteria, Criteria).ID & "', nerastas.")

                    _Serial = CStrSafe(myData.Rows(0).Item(0))
                    _Number = CIntSafe(myData.Rows(0).Item(1), 0)
                    _OldDate = CDateSafe(myData.Rows(0).Item(2), Date.MinValue)

                    If Not _Number > 0 OrElse _OldDate = Date.MinValue Then Throw New Exception( _
                        "Klaida. Objektas, kurio ID='" & DirectCast(criteria, Criteria).ID & "', nerastas.")

                End Using

            End If

            myComm = New SQLCommand("CheckIfContractUpdateCanBeDeleted")
            myComm.AddParam("?DN", _Number)
            myComm.AddParam("?DS", _Serial)
            myComm.AddParam("?DT", _OldDate)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 OrElse Not CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                    Throw New Exception("Klaida. Objektas, kurio serija ir numeris yra '" & _Serial _
                    & _Number.ToString & "', " & " data " & _OldDate.ToShortDateString & ", - nerastas.")
                If CIntSafe(myData.Rows(0).Item(1), 0) > 0 Then Throw New Exception( _
                    "Klaida. Objektas, kurio serija ir numeris yra '" & _Serial _
                    & _Number.ToString & "', " & " data " & _OldDate.ToShortDateString _
                    & ", - ne darbo sutarties pakeitimas, o darbo sutartis (jos nutraukimas).")
                If CDateSafe(myData.Rows(0).Item(2), Date.MinValue) <> Date.MinValue Then _
                    Throw New Exception("Klaida. Pagal šį darbo sutarties pakeitimą buvo " _
                    & "skaičiuojamas darbo užmokestis (" & CDateSafe(myData.Rows(0).Item(2), _
                    Date.MinValue).ToShortDateString & " žiniaraštis).")

            End Using

            myComm = New SQLCommand("DeleteContractUpdate")
            myComm.AddParam("?DS", _Serial.Trim)
            myComm.AddParam("?DN", _Number)
            myComm.AddParam("?DT", _OldDate)

            myComm.Execute()

            MarkNew()

        End Sub


        Private Sub UpdateParam(ByVal ParamType As WorkerStatusType)

            ' do not update fields, that are out of scope
            If ParamType = WorkerStatusType.Employed OrElse ParamType = WorkerStatusType.Fired Then Exit Sub

            Dim ParamID As Integer = GetParamIDByType(ParamType)

            If IsNew OrElse Not ParamID > 0 Then

                ' do not insert empty values and fields, that are limited by business logic
                If Not ParamValueHasStatusItem(ParamType) OrElse IsParamValueReadonly(ParamType) Then Exit Sub

                Dim myComm As New SQLCommand("InsertWorkerStatus")
                AddWithParams(myComm, ParamType)
                myComm.AddParam("?PD", _PersonID)
                myComm.AddParam("?NM", _Number)
                myComm.AddParam("?SR", _Serial.Trim)
                myComm.AddParam("?TP", ConvertEnumDatabaseStringCode(ParamType))

                myComm.Execute()

                ParamID = Convert.ToInt32(myComm.LastInsertID)
                SetParamIDByType(ParamType, ParamID)

            Else

                ' remove empty values
                If Not IsParamValueReadonly(ParamType) AndAlso Not ParamValueHasStatusItem(ParamType) Then

                    Dim myComm As New SQLCommand("DeleteWorkerStatus")
                    myComm.AddParam("?SD", ParamID)

                    myComm.Execute()

                    SetParamIDByType(ParamType, 0)

                Else

                    If IsParamValueReadonly(ParamType) Then

                        Dim myComm As New SQLCommand("UpdateWorkerStatusWithoutValue")
                        AddWithParams(myComm, ParamType)
                        myComm.AddParam("?SD", ParamID)

                        myComm.Execute()

                    Else

                        Dim myComm As New SQLCommand("UpdateWorkerStatus")
                        AddWithParams(myComm, ParamType)
                        myComm.AddParam("?SD", ParamID)

                        myComm.Execute()

                    End If

                End If

            End If

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand, ByVal ParamType As WorkerStatusType)

            myComm.AddParam("?DT", _Date.Date)
            myComm.AddParam("?UD", _UpdateDate.ToUniversalTime)

            Select Case ParamType
                Case WorkerStatusType.ExtraPay
                    myComm.AddParam("?VL", CRound(_ExtraPay, 2))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.Holiday
                    myComm.AddParam("?VL", CRound(_AnnualHoliday, 2))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.HolidayCorrection
                    myComm.AddParam("?VL", CRound(_HolidayCorrection, 4))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.NPD
                    myComm.AddParam("?VL", CRound(_NPD, 2))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.PNPD
                    myComm.AddParam("?VL", CRound(_PNPD, 2))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.Wage
                    myComm.AddParam("?VL", CRound(_Wage, 2))
                    myComm.AddParam("?OP", ConvertEnumDatabaseStringCode(_WageType))
                Case WorkerStatusType.WorkLoad
                    myComm.AddParam("?VL", CRound(_WorkLoad, 4))
                    myComm.AddParam("?OP", "")
                Case WorkerStatusType.Position
                    myComm.AddParam("?VL", 0, GetType(Double))
                    myComm.AddParam("?OP", _Position.Trim)
                Case Else
                    myComm.AddParam("?VL", 0, GetType(Double))
                    myComm.AddParam("?OP", "")
            End Select

            myComm.AddParam("?CN", _Content.Trim)

        End Sub

        Private Function GetParamIDByType(ByVal ParamType As WorkerStatusType) As Integer
            Select Case ParamType
                Case WorkerStatusType.Employed
                    Return 0
                Case WorkerStatusType.ExtraPay
                    Return _ExtraPayID
                Case WorkerStatusType.Fired
                    Return 0
                Case WorkerStatusType.Holiday
                    Return _AnnualHolidayID
                Case WorkerStatusType.HolidayCorrection
                    Return _HolidayCorrectionID
                Case WorkerStatusType.NPD
                    Return _NpdID
                Case WorkerStatusType.PNPD
                    Return _PnpdID
                Case WorkerStatusType.Position
                    Return _PositionID
                Case WorkerStatusType.Wage
                    Return _WageID
                Case WorkerStatusType.WorkLoad
                    Return _WorkLoadID
                Case Else
                    Return 0
            End Select
        End Function

        Private Sub SetParamIDByType(ByVal ParamType As WorkerStatusType, ByVal ParamID As Integer)
            Select Case ParamType
                Case WorkerStatusType.Employed

                Case WorkerStatusType.ExtraPay
                    _ExtraPayID = ParamID
                Case WorkerStatusType.Fired

                Case WorkerStatusType.Holiday
                    _AnnualHolidayID = ParamID
                Case WorkerStatusType.HolidayCorrection
                    _HolidayCorrectionID = ParamID
                Case WorkerStatusType.NPD
                    _NpdID = ParamID
                Case WorkerStatusType.PNPD
                    _PnpdID = ParamID
                Case WorkerStatusType.Position
                    _PositionID = ParamID
                Case WorkerStatusType.Wage
                    _WageID = ParamID
                Case WorkerStatusType.WorkLoad
                    _WorkLoadID = ParamID
            End Select
        End Sub

        Private Function IsParamValueReadonly(ByVal ParamType As WorkerStatusType) As Boolean
            Return Not IsNew AndAlso Not _ChronologicValidator.FinancialDataCanChange AndAlso _
                (ParamType = WorkerStatusType.ExtraPay OrElse ParamType = WorkerStatusType.NPD _
                OrElse ParamType = WorkerStatusType.PNPD OrElse ParamType = WorkerStatusType.Wage)
        End Function

        Private Function ParamValueHasStatusItem(ByVal ParamType As WorkerStatusType) As Boolean

            If (ParamType = WorkerStatusType.ExtraPay AndAlso Not _ExtraPayChanged) _
                OrElse (ParamType = WorkerStatusType.Fired OrElse ParamType = WorkerStatusType.Employed) _
                OrElse (ParamType = WorkerStatusType.HolidayCorrection AndAlso CRound(_HolidayCorrection, 4) = 0) _
                OrElse (ParamType = WorkerStatusType.HolidayCorrection AndAlso Not _HolidayCorrectionChanged) _
                OrElse (ParamType = WorkerStatusType.NPD AndAlso Not _NpdChanged) _
                OrElse (ParamType = WorkerStatusType.PNPD AndAlso Not _PnpdChanged) _
                OrElse (ParamType = WorkerStatusType.Position AndAlso Not _PositionChanged) _
                OrElse (ParamType = WorkerStatusType.Holiday AndAlso Not _AnnualHolidayChanged) _
                OrElse (ParamType = WorkerStatusType.Wage AndAlso Not _WageChanged) _
                OrElse (ParamType = WorkerStatusType.WorkLoad AndAlso Not _WorkLoadChanged) Then _
                Return False

            Return True

        End Function

        Private Sub CheckIfContractSerialNumberUnique()

            If Not IsNew AndAlso _Date.Date = _OldDate.Date Then Exit Sub

            Dim myComm As New SQLCommand("CheckIfContractUpdateSerialNumberUnique")
            myComm.AddParam("?DS", _Serial.Trim)
            myComm.AddParam("?DN", _Number)
            myComm.AddParam("?DT", _Date.Date)
            If IsNew Then
                myComm.AddParam("?OD", Today.AddYears(100))
            Else
                myComm.AddParam("?OD", _OldDate.Date)
            End If

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                    Throw New Exception("Klaida. Darbo sutarties pakeitimas tokiai dienai jau yra.")
            End Using

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfContractUpdateUpdateDateChanged")
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 OrElse CDateTimeSafe(myData.Rows(0).Item(0), _
                    Date.MinValue) = Date.MinValue Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID=" & _ID.ToString & ", nerastas.")
                If DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> _UpdateDate Then Throw New Exception( _
                    "Klaida. Dokumento atnaujinimo data pasikeitė. Teigtina, kad kitas " _
                    & "vartotojas redagavo šį objektą.")
            End Using

        End Sub

#End Region

    End Class

End Namespace