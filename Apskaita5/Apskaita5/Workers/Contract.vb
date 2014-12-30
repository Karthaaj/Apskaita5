Namespace Workers

    <Serializable()> _
    Public Class Contract
        Inherits BusinessBase(Of Contract)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _ChronologicValidator As ContractChronologicValidator
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _ExtraPayID As Integer = 0
        Private _TerminationID As Integer = 0
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

        Private _Date As Date = Today
        Private _Serial As String = ""
        Private _Number As Integer = 0
        Private _Content As String = ""
        Private _Position As String = ""
        Private _Wage As Double = 0
        Private _WageType As WageType = Workers.WageType.Position
        Private _HumanReadableWageType As String = ConvertEnumHumanReadable(Workers.WageType.Position)
        Private _IsTerminated As Boolean = False
        Private _TerminationDate As Date = Today
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

        Public ReadOnly Property ChronologicValidator() As IChronologicValidator
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

        Public ReadOnly Property TerminationID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TerminationID
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

        Public Property Serial() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Serial.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                If Not IsNew Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If value Is Nothing Then value = ""
                    If _Serial.Trim <> value.Trim Then
                        _Serial = value.Trim
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property Number() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                If Not IsNew Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _Number <> value Then
                        _Number = value
                        PropertyHasChanged()
                    End If
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
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

        Public Property IsTerminated() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsTerminated
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                If Not _ChronologicValidator.TerminationCanBeCanceled Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _IsTerminated <> value Then
                        _IsTerminated = value
                        PropertyHasChanged()
                    End If
                End If
            End Set
        End Property

        Public Property TerminationDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TerminationDate
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Date)
                If Not _IsTerminated OrElse Not _ChronologicValidator.TerminationCanBeCanceled Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
                    If _TerminationDate.Date <> value.Date Then
                        _TerminationDate = value.Date
                        PropertyHasChanged()
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
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
                If Not _ChronologicValidator.FinancialDataCanChange Then
                    PropertyHasChanged()
                Else
                    CanWriteProperty(True)
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
                Return (Not String.IsNullOrEmpty(_Serial.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Content.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Position.Trim))
            End Get
        End Property



        Public Overrides Function Save() As Contract

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbcrlf & Me.GetAllBrokenRules)
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
            Return "Darbo sutartis " & _Date.ToShortDateString & " Nr. " & _Serial & _Number.ToString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Number", "darbo sutarties numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Position", "pareigos", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Wage", "darbo užmokestis"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AnnualHoliday", "kasmetinės atostogos"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("WorkLoad", "krūvis (40 val. darbo " _
                & "savaitė = 1; 20 val. darbo savaitė = 0,5 ir t.t.)"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologicValidator"))

            ValidationRules.AddRule(AddressOf TerminationDateValidation, _
                New Validation.RuleArgs("TerminationDate"))

            ValidationRules.AddDependantProperty("IsTerminated", "TerminationDate", False)
            ValidationRules.AddDependantProperty("Date", "TerminationDate", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property TerminationDate is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function TerminationDateValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As Contract = DirectCast(target, Contract)

            If Not ValObj.IsNew AndAlso Not ValObj._IsTerminated AndAlso _
                Not ValObj._ChronologicValidator.TerminationCanBeCanceled Then
                e.Description = ValObj._ChronologicValidator.TerminationCanBeCanceledExplanation
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf ValObj._IsTerminated AndAlso ValObj._Date.Date > ValObj._TerminationDate.Date Then
                e.Description = "Darbo sutarties nutraukimo data negali būti ankstesnė nei " & _
                    "jos sudarymo data."
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

        Public Shared Function NewContract(ByVal PersonID As Integer) As Contract
            Dim result As Contract = DataPortal.Create(Of Contract)(New Criteria(PersonID))
            result.MarkNew()
            Return result
        End Function

        Public Shared Function GetContract(ByVal nID As Integer) As Contract
            Return DataPortal.Fetch(Of Contract)(New Criteria(nID))
        End Function

        Public Shared Function GetContract(ByVal ContractSerial As String, _
            ByVal ContractNumber As Integer) As Contract
            Return DataPortal.Fetch(Of Contract)(New Criteria(ContractSerial, ContractNumber))
        End Function

        Public Shared Sub DeleteContract(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub

        Public Shared Sub DeleteContract(ByVal ContractSerial As String, ByVal ContractNumber As Integer)
            DataPortal.Delete(New Criteria(ContractSerial, ContractNumber))
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
            Public Sub New(ByVal nID As Integer)
                _ID = nID
            End Sub
            Public Sub New(ByVal nContractSerial As String, ByVal nContractNumber As Integer)
                _ContractSerial = nContractSerial
                _ContractNumber = nContractNumber
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            If Not criteria.ID > 0 Then Throw New Exception("Klaida. Nenurodytas darbuotojo ID.")

            Dim myComm As New SQLCommand("FetchPersonInfoByID")
            myComm.AddParam("?CD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Asmuo (darbuotojas), kurio ID='" & criteria.ID & "', nerastas.")

                Dim dr As DataRow = myData.Rows(0)

                If Not CIntSafe(dr.Item(19), 0) > 0 Then Throw New Exception( _
                    "Klaida. Asmuo, kurio ID='" & criteria.ID & "', nėra darbuotojas.")

                _PersonID = criteria.ID
                _PersonName = CStrSafe(dr.Item(1)).Trim
                _PersonCode = CStrSafe(dr.Item(2)).Trim
                _PersonAddress = CStrSafe(dr.Item(3)).Trim
                _PersonSodraCode = CStrSafe(dr.Item(10)).Trim

            End Using

            _ChronologicValidator = ContractChronologicValidator.NewContractChronologicValidator()

            ValidationRules.CheckRules()

            MarkNew()

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As SQLCommand
            If criteria.ID > 0 Then
                myComm = New SQLCommand("FetchContract")
                myComm.AddParam("?CD", criteria.ID)
            Else
                myComm = New SQLCommand("FetchContractBySerialNumber")
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

                For Each dr As DataRow In myData.Rows

                    Select Case ConvertEnumDatabaseStringCode(Of WorkerStatusType)(CStrSafe(dr.Item(1)))

                        Case WorkerStatusType.Employed

                            _ID = CIntSafe(dr.Item(0), 0)
                            _Date = CDateSafe(dr.Item(2), Date.MinValue)
                            _Content = CStrSafe(dr.Item(5)).Trim
                            _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(6), Date.UtcNow), _
                                DateTimeKind.Utc).ToLocalTime
                            _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(7), Date.UtcNow), _
                                DateTimeKind.Utc).ToLocalTime
                            _Serial = CStrSafe(dr.Item(8)).Trim
                            _Number = CIntSafe(dr.Item(9), 0)
                            _PersonID = CIntSafe(dr.Item(10), 0)
                            _PersonName = CStrSafe(dr.Item(11)).Trim
                            _PersonCode = CStrSafe(dr.Item(12)).Trim
                            _PersonAddress = CStrSafe(dr.Item(13)).Trim
                            _PersonSodraCode = CStrSafe(dr.Item(14)).Trim

                        Case WorkerStatusType.ExtraPay

                            _ExtraPayID = CIntSafe(dr.Item(0), 0)
                            _ExtraPay = CDblSafe(dr.Item(3), 0)

                        Case WorkerStatusType.Fired

                            _TerminationID = CIntSafe(dr.Item(0), 0)
                            _IsTerminated = True
                            _TerminationDate = CDateSafe(dr.Item(2), Date.MinValue)

                        Case WorkerStatusType.Holiday

                            _AnnualHolidayID = CIntSafe(dr.Item(0), 0)
                            _AnnualHoliday = CIntSafe(dr.Item(3), 0)

                        Case WorkerStatusType.HolidayCorrection

                            _HolidayCorrectionID = CIntSafe(dr.Item(0), 0)
                            _HolidayCorrection = CDblSafe(dr.Item(3), 4, 0)

                        Case WorkerStatusType.NPD

                            _NpdID = CIntSafe(dr.Item(0), 0)
                            _NPD = CDblSafe(dr.Item(3), 2, 0)

                        Case WorkerStatusType.PNPD

                            _PnpdID = CIntSafe(dr.Item(0), 0)
                            _PNPD = CDblSafe(dr.Item(3), 2, 0)

                        Case WorkerStatusType.Wage

                            _WageID = CIntSafe(dr.Item(0), 0)
                            _Wage = CDblSafe(dr.Item(3), 2, 0)
                            _WageType = ConvertEnumDatabaseStringCode(Of WageType)(CStrSafe(dr.Item(4)))
                            _HumanReadableWageType = ConvertEnumHumanReadable(_WageType)

                        Case WorkerStatusType.WorkLoad

                            _WorkLoadID = CIntSafe(dr.Item(0), 0)
                            _WorkLoad = CDblSafe(dr.Item(3), 4, 0)

                        Case WorkerStatusType.Position

                            _PositionID = CIntSafe(dr.Item(0), 0)
                            _Position = CStrSafe(dr.Item(4))

                    End Select

                Next

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

            If Not IsNew Then

                _ChronologicValidator = ContractChronologicValidator.GetContractChronologicValidator(_ID)
                ValidationRules.CheckRules()
                If Not IsValid Then Throw New Exception("Sutarties duomenyse yra klaidų: " _
                    & vbCrLf & BrokenRulesCollection.ToString(Validation.RuleSeverity.Error))

                CheckIfUpdateDateChanged()

            End If

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If IsNew Then _InsertDate = _UpdateDate

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            UpdateParam(WorkerStatusType.Employed)
            UpdateParam(WorkerStatusType.ExtraPay)
            UpdateParam(WorkerStatusType.Holiday)
            UpdateParam(WorkerStatusType.HolidayCorrection)
            UpdateParam(WorkerStatusType.NPD)
            UpdateParam(WorkerStatusType.PNPD)
            UpdateParam(WorkerStatusType.Position)
            UpdateParam(WorkerStatusType.Wage)
            UpdateParam(WorkerStatusType.WorkLoad)
            UpdateParam(WorkerStatusType.Fired)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

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

            Dim myComm As SQLCommand

            If DirectCast(criteria, Criteria).ID > 0 Then

                myComm = New SQLCommand("FetchContractSerialNumber")
                myComm.AddParam("?CD", DirectCast(criteria, Criteria).ID)

                Using myData As DataTable = myComm.Fetch

                    If Not myData.Rows.Count > 0 Then Throw New Exception( _
                        "Klaida. Objektas, kurio ID='" & DirectCast(criteria, Criteria).ID.ToString & "', nerastas.")

                    _Serial = CStrSafe(myData.Rows(0).Item(0))
                    _Number = CIntSafe(myData.Rows(0).Item(1), 0)
                    _ID = DirectCast(criteria, Criteria).ID

                End Using

            Else

                myComm = New SQLCommand("FetchContractBySerialNumber")
                myComm.AddParam("?DN", DirectCast(criteria, Criteria).ContractNumber)
                myComm.AddParam("?DS", DirectCast(criteria, Criteria).ContractSerial)

                Using myData As DataTable = myComm.Fetch

                    If Not myData.Rows.Count > 0 Then Throw New Exception( _
                        "Klaida. Objektas, kurio ID='" & DirectCast(criteria, Criteria).ContractSerial _
                            & DirectCast(criteria, Criteria).ContractNumber.ToString & "', nerastas.")

                    _Serial = DirectCast(criteria, Criteria).ContractSerial
                    _Number = DirectCast(criteria, Criteria).ContractNumber
                    _ID = 0

                    For Each dr As DataRow In myData.Rows
                        If ConvertEnumDatabaseStringCode(Of WorkerStatusType)(CStrSafe(dr.Item(1))) _
                            = WorkerStatusType.Employed Then
                            _ID = CIntSafe(dr.Item(0), 0)
                            Exit For
                        End If
                    Next

                    If Not _ID > 0 Then Throw New Exception( _
                        "Klaida. Objektas, kurio ID='" & DirectCast(criteria, Criteria).ContractSerial _
                            & DirectCast(criteria, Criteria).ContractNumber.ToString & "', nerastas.")

                End Using

            End If

            myComm = New SQLCommand("CheckIfWorkerStatusCanBeDeleted")
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID='" & _ID.ToString & "', nerastas.")

                Dim dr As DataRow = myData.Rows(0)

                Dim exceptionString As String = ""

                If CDateSafe(dr.Item(1), Date.MaxValue) <> Date.MaxValue Then _
                    exceptionString = AddWithNewLine(exceptionString, _
                    "Paskutinis darbo užmokesčio žiniaraštis - " _
                    & CDateSafe(dr.Item(1), Date.MaxValue).ToString("yyyy-MM-dd") & ".", False)
                If CDateSafe(dr.Item(2), Date.MaxValue) <> Date.MaxValue Then _
                    exceptionString = AddWithNewLine(exceptionString, _
                    "Paskutinis avanso žiniaraštis - " _
                    & CDateSafe(dr.Item(2), Date.MaxValue).ToString("yyyy-MM-dd") & ".", False)
                If CDateSafe(dr.Item(3), Date.MaxValue) <> Date.MaxValue Then _
                    exceptionString = AddWithNewLine(exceptionString, _
                    "Paskutinis darbo laiko žiniaraštis - " _
                    & CDateSafe(dr.Item(3), Date.MaxValue).ToString("yyyy-MM-dd") & ".", False)

                If Not String.IsNullOrEmpty(exceptionString.Trim) Then Throw New Exception( _
                    "Klaida. Šiai darbo sutarčiai yra sudaryti žiniaraščiai:" & vbCrLf & exceptionString)

            End Using

            myComm = New SQLCommand("DeleteContract")
            myComm.AddParam("?DS", _Serial.Trim)
            myComm.AddParam("?DN", _Number)

            myComm.Execute()

            MarkNew()

        End Sub


        Private Sub UpdateParam(ByVal ParamType As WorkerStatusType)

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

                If ParamType = WorkerStatusType.Fired AndAlso _
                    Not _ChronologicValidator.TerminationCanBeCanceled Then Exit Sub

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

            If ParamType = WorkerStatusType.Fired Then
                myComm.AddParam("?DT", _TerminationDate.Date)
            Else
                myComm.AddParam("?DT", _Date.Date)
            End If
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

            If ParamType = WorkerStatusType.Employed Then
                myComm.AddParam("?CN", _Content.Trim)
            Else
                myComm.AddParam("?CN", "")
            End If

        End Sub

        Private Function GetParamIDByType(ByVal ParamType As WorkerStatusType) As Integer
            Select Case ParamType
                Case WorkerStatusType.Employed
                    Return _ID
                Case WorkerStatusType.ExtraPay
                    Return _ExtraPayID
                Case WorkerStatusType.Fired
                    Return _TerminationID
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
                    _ID = ParamID
                Case WorkerStatusType.ExtraPay
                    _ExtraPayID = ParamID
                Case WorkerStatusType.Fired
                    _TerminationID = ParamID
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

            If (ParamType = WorkerStatusType.ExtraPay AndAlso Not CRound(_ExtraPay, 2) > 0) _
                OrElse (ParamType = WorkerStatusType.Fired AndAlso Not _IsTerminated) _
                OrElse (ParamType = WorkerStatusType.HolidayCorrection AndAlso CRound(_HolidayCorrection, 4) = 0) _
                OrElse (ParamType = WorkerStatusType.NPD AndAlso Not CRound(_NPD, 2) > 0) _
                OrElse (ParamType = WorkerStatusType.PNPD AndAlso Not CRound(_PNPD, 2) > 0) _
                OrElse (ParamType = WorkerStatusType.Position AndAlso String.IsNullOrEmpty(_Position.Trim)) Then _
                Return False

            Return True

        End Function

        Private Sub CheckIfContractSerialNumberUnique()

            Dim myComm As New SQLCommand("CheckIfContractSerialNumberUnique")
            myComm.AddParam("?DS", _Serial.Trim)
            myComm.AddParam("?DN", _Number)
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                    Throw New Exception("Klaida. Tokia serija ir numeris jau yra.")
            End Using

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfContractUpdateDateChanged")
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