Namespace Workers

    <Serializable()> _
    Public Class WorkTimeClass
        Inherits BusinessBase(Of WorkTimeClass)
        Implements IGetErrorForListItem

#Region " Business Methods "

        Private _Guid As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _Code As String = ""
        Private _Name As String = ""
        Private _Type As WorkTimeType = WorkTimeType.OtherIncluded
        Private _TypeHumanReadable As String = ConvertEnumHumanReadable(WorkTimeType.OtherIncluded)
        Private _InclusionPercentage As Double = 100
        Private _SpecialWageShemaApplicable As Boolean = False
        Private _SpecialWageShema As String = ""
        Private _WithoutWorkHours As Boolean = False
        Private _AlreadyIncludedInGeneralTime As Boolean = True
        Private _IsInUse As Boolean = False


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public Property Code() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Code.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Code.Trim <> value.Trim Then
                    _Code = value.Trim
                    PropertyHasChanged()
                End If
            End Set
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

        Public Property [Type]() As WorkTimeType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WorkTimeType)
                CanWriteProperty(True)
                If _Type <> value AndAlso Not _IsInUse Then
                    _Type = value
                    _TypeHumanReadable = ConvertEnumHumanReadable(_Type)
                    PropertyHasChanged()
                    PropertyHasChanged("TypeHumanReadable")
                End If
            End Set
        End Property

        Public Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _TypeHumanReadable.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Type <> ConvertEnumHumanReadable(Of WorkTimeType)(value.Trim) AndAlso Not _IsInUse Then
                    _Type = ConvertEnumHumanReadable(Of WorkTimeType)(value.Trim)
                    _TypeHumanReadable = ConvertEnumHumanReadable(_Type)
                    PropertyHasChanged()
                    PropertyHasChanged("Type")
                End If
            End Set
        End Property

        Public Property InclusionPercentage() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_InclusionPercentage)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_InclusionPercentage) <> CRound(value) AndAlso Not _IsInUse Then
                    _InclusionPercentage = CRound(value)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property SpecialWageShemaApplicable() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SpecialWageShemaApplicable
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _SpecialWageShemaApplicable <> value Then
                    _SpecialWageShemaApplicable = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property SpecialWageShema() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SpecialWageShema.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _SpecialWageShema.Trim <> value.Trim Then
                    _SpecialWageShema = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property WithoutWorkHours() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WithoutWorkHours
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _WithoutWorkHours <> value Then
                    _WithoutWorkHours = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AlreadyIncludedInGeneralTime() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AlreadyIncludedInGeneralTime
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _AlreadyIncludedInGeneralTime <> value Then
                    _AlreadyIncludedInGeneralTime = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property IsInUse() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsInUse
            End Get
        End Property



        Public Function GetErrorString() As String _
            Implements IGetErrorForListItem.GetErrorString
            If IsValid Then Return ""
            Return "Klaida (-os) eilutėje '" & _Name & "': " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error)
        End Function

        Public Function GetWarningString() As String _
            Implements IGetErrorForListItem.GetWarningString
            If BrokenRulesCollection.WarningCount < 1 Then Return ""
            Return "Eilutėje '" & _Name & "' gali būti klaida: " _
                & Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _Guid
        End Function

        Public Overrides Function ToString() As String
            Return _Code & " - " & _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Code", "(ne)darbo kodas"))
            ValidationRules.AddRule(AddressOf CommonValidation.ValueUniqueInCollection, _
                New CommonValidation.SimpleRuleArgs("Code", "(ne)darbo kodas"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "(ne)darbo pavadinimas"))
            ValidationRules.AddRule(AddressOf InclusionPercentageValidation, _
                New Validation.RuleArgs("InclusionPercentage"))
            ValidationRules.AddRule(AddressOf SpecialWageShemaValidation, _
                New Validation.RuleArgs("SpecialWageShema"))
            ValidationRules.AddRule(AddressOf WithoutWorkHoursValidation, _
                New Validation.RuleArgs("WithoutWorkHours"))
            ValidationRules.AddRule(AddressOf SpecialWageShemaApplicableValidation, _
                New Validation.RuleArgs("SpecialWageShemaApplicable"))

            ValidationRules.AddDependantProperty("Type", "InclusionPercentage", False)
            ValidationRules.AddDependantProperty("Type", "SpecialWageShemaApplicable", False)
            ValidationRules.AddDependantProperty("SpecialWageShemaApplicable", "SpecialWageShema", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property InclusionPercentage is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function InclusionPercentageValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As WorkTimeClass = DirectCast(target, WorkTimeClass)

            If ValObj._Type <> WorkTimeType.OtherIncluded AndAlso CRound(ValObj._InclusionPercentage) <> 0 _
                AndAlso CRound(ValObj._InclusionPercentage) <> 100 Then
                e.Description = "Klaida. Įtraukimo į darbo laiką procentas gali būti " _
                    & "nurodomas tik tipui 'Kitas įskaitomas į darbo laiką'"
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf (ValObj._Type = WorkTimeType.NightWork OrElse _
                ValObj._Type = WorkTimeType.OvertimeWork OrElse _
                ValObj._Type = WorkTimeType.PublicHolidaysAndRestDayWork OrElse _
                ValObj._Type = WorkTimeType.UnusualWork OrElse _
                ValObj._Type = WorkTimeType.DownTime) AndAlso _
                CRound(ValObj._InclusionPercentage) <> 100 Then
                e.Description = "Klaida. Įtraukimo į darbo laiką procentas pasirinktam " _
                    & "tipui turi būti lygus 100."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf (ValObj._Type = WorkTimeType.OtherExcluded OrElse _
                ValObj._Type = WorkTimeType.Truancy OrElse _
                ValObj._Type = WorkTimeType.AnnualHolidays OrElse _
                ValObj._Type = WorkTimeType.OtherHolidays OrElse _
                ValObj._Type = WorkTimeType.SickDays) AndAlso _
                CRound(ValObj._InclusionPercentage) <> 0 Then
                e.Description = "Klaida. Įtraukimo į darbo laiką procentas pasirinktam " _
                    & "tipui turi būti lygus 0."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf ValObj._Type = WorkTimeType.OtherIncluded AndAlso _
                Not CRound(ValObj._InclusionPercentage) > 0 Then
                e.Description = "Klaida. Nenurodytas įtraukimo į darbo laiką procentas."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            ElseIf ValObj._Type = WorkTimeType.OtherIncluded AndAlso _
                CRound(ValObj._InclusionPercentage) > 100 Then
                e.Description = "Klaida. Įtraukimo į darbo laiką procentas negali būti didesnis nei 100."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property SpecialWageShemaApplicable is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function SpecialWageShemaApplicableValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As WorkTimeClass = DirectCast(target, WorkTimeClass)

            If ValObj._SpecialWageShemaApplicable AndAlso _
                (ValObj._Type = WorkTimeType.AnnualHolidays OrElse _
                ValObj._Type = WorkTimeType.NightWork OrElse _
                ValObj._Type = WorkTimeType.OvertimeWork OrElse _
                ValObj._Type = WorkTimeType.PublicHolidaysAndRestDayWork OrElse _
                ValObj._Type = WorkTimeType.SickDays OrElse _
                ValObj._Type = WorkTimeType.Truancy OrElse _
                ValObj._Type = WorkTimeType.UnusualWork) Then

                e.Description = "Pasirinktam tipui speciali apmokėjimo schema negalima."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf Not ValObj._SpecialWageShemaApplicable AndAlso _
                ValObj._Type = WorkTimeType.DownTime Then

                e.Description = "Prastovos tipui turi būti nustatyta apmokėjimo schema."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property SpecialWageShema is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function SpecialWageShemaValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As WorkTimeClass = DirectCast(target, WorkTimeClass)

            If ValObj._SpecialWageShemaApplicable AndAlso (ValObj._SpecialWageShema Is Nothing _
                OrElse String.IsNullOrEmpty(ValObj._SpecialWageShema.Trim)) Then

                e.Description = "Nenurodyta apmokėjimo schemos formulė (galimi kintamieji: " _
                    & "VAL - dirbtos šios kategorijos valandos, DIN - dirbtos šios kategorijos dienos, " _
                    & "DUV - apskaičiuotas valandinis DU, DUD - apskaičiuotas dienos DU, " _
                    & "VDD - dienos VDU, VDV - valandos VDU)."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf ValObj._SpecialWageShemaApplicable Then

                Dim FS As New FormulaSolver
                FS.Param("VAL") = 8
                FS.Param("DIN") = 1
                FS.Param("DUV") = 6
                FS.Param("DUD") = 32
                FS.Param("VDD") = 32.32
                FS.Param("VDV") = 5.99
                If Not FS.Solved(ValObj._SpecialWageShema.Trim, New Double) Then
                    e.Description = "Klaidingai įvesta formulė. Matematinės išraiškos klaida. " _
                        & "(galimi kintamieji: VAL - dirbtos šios kategorijos valandos, " _
                        & "DIN - dirbtos šios kategorijos dienos, DUV - apskaičiuotas valandinis " _
                        & "DU, DUD - apskaičiuotas dienos DU, VDD - dienos VDU, VDV - valandos VDU)."
                    e.Severity = Validation.RuleSeverity.Error
                    Return False
                End If

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property WithoutWorkHours is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function WithoutWorkHoursValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As WorkTimeClass = DirectCast(target, WorkTimeClass)

            If ValObj._Type <> WorkTimeType.AnnualHolidays AndAlso _
                ValObj._Type <> WorkTimeType.OtherExcluded AndAlso _
                ValObj._Type <> WorkTimeType.OtherHolidays AndAlso _
                ValObj._Type <> WorkTimeType.SickDays AndAlso ValObj._WithoutWorkHours Then

                e.Description = "Pasirinktai kategorijai turi būti skaičiuojamos valandos."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            ElseIf (ValObj._Type = WorkTimeType.AnnualHolidays OrElse _
                ValObj._Type = WorkTimeType.OtherHolidays OrElse _
                ValObj._Type = WorkTimeType.SickDays) AndAlso Not ValObj._WithoutWorkHours Then

                e.Description = "Pasirinktai kategorijai negali būti skaičiuojamos valandos."
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

        Friend Shared Function NewWorkTimeClass() As WorkTimeClass
            Dim result As New WorkTimeClass
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Friend Shared Function NewWorkTimeClass(ByVal line As String()) As WorkTimeClass
            Return New WorkTimeClass(line)
        End Function

        Friend Shared Function GetWorkTimeClass(ByVal dr As DataRow) As WorkTimeClass
            Return New WorkTimeClass(dr)
        End Function

        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
        End Sub

        Private Sub New(ByVal dr As DataRow)
            MarkAsChild()
            Fetch(dr)
        End Sub

        Private Sub New(ByVal line As String())
            MarkAsChild()
            FetchDefault(line)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Fetch(ByVal dr As DataRow)

            _ID = CIntSafe(dr.item(0), 0)
            _Code = CStrSafe(dr.Item(1)).Trim
            _Name = CStrSafe(dr.Item(2)).Trim
            _InclusionPercentage = CDblSafe(dr.Item(3), 2, 0)
            _Type = ConvertEnumDatabaseCode(Of WorkTimeType)(CIntSafe(dr.Item(4), 0))
            _TypeHumanReadable = ConvertEnumHumanReadable(_Type)
            _SpecialWageShemaApplicable = ConvertDbBoolean(CIntSafe(dr.Item(5), 0))
            _SpecialWageShema = CStrSafe(dr.Item(6)).Trim
            _WithoutWorkHours = ConvertDbBoolean(CIntSafe(dr.Item(7), 0))
            _AlreadyIncludedInGeneralTime = ConvertDbBoolean(CIntSafe(dr.Item(8), 0))
            _IsInUse = ConvertDbBoolean(CIntSafe(dr.Item(9), 0))

            MarkOld()

        End Sub

        Private Sub FetchDefault(ByVal line As String())

            _Name = CStrSafe(line(0)).Trim
            _Code = CStrSafe(line(1)).Trim
            _Type = ConvertEnumDatabaseCode(Of WorkTimeType)(CIntSafe(line(2), 0))
            _TypeHumanReadable = ConvertEnumHumanReadable(_Type)
            _InclusionPercentage = CDblSafe(line(3), 2, 0)
            _WithoutWorkHours = ConvertDbBoolean(CIntSafe(line(4), 0))
            _AlreadyIncludedInGeneralTime = ConvertDbBoolean(CIntSafe(line(5), 0))
            _SpecialWageShemaApplicable = ConvertDbBoolean(CIntSafe(line(6), 0))
            If line.Length > 7 Then _SpecialWageShema = CStrSafe(line(7)).Trim

            ValidationRules.CheckRules()

        End Sub


        Friend Sub Insert(ByVal parent As WorkTimeClassList)

            Dim myComm As New SQLCommand("InsertWorkTimeClass")
            AddWithParams(myComm)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Friend Sub Update(ByVal parent As WorkTimeClassList)

            Dim myComm As SQLCommand
            If _IsInUse Then
                myComm = New SQLCommand("UpdateWorkTimeClassLimited")
            Else
                myComm = New SQLCommand("UpdateWorkTimeClass")
            End If
            myComm.AddParam("?CD", _ID)
            AddWithParams(myComm)

            myComm.Execute()

            MarkOld()

        End Sub

        Friend Sub DeleteSelf()

            Dim myComm As New SQLCommand("DeleteWorkTimeClass")
            myComm.AddParam("?CD", _ID)

            myComm.Execute()

            MarkNew()

        End Sub


        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _Code.Trim)
            myComm.AddParam("?AB", _Name.Trim)
            If Not _IsInUse Then
                myComm.AddParam("?AC", CRound(_InclusionPercentage))
                myComm.AddParam("?AD", ConvertEnumDatabaseCode(_Type))
                myComm.AddParam("?AE", ConvertDbBoolean(_SpecialWageShemaApplicable))
                myComm.AddParam("?AG", ConvertDbBoolean(_WithoutWorkHours))
                myComm.AddParam("?AH", ConvertDbBoolean(_AlreadyIncludedInGeneralTime))
            End If
            myComm.AddParam("?AF", _SpecialWageShema.Trim)

        End Sub

        Friend Sub UpdateIsInUse(ByVal ThrowOnInUse As Boolean)

            If IsNew Then Exit Sub

            Dim myComm As New SQLCommand("CheckIfWorkTimeClassIsInUse")
            myComm.AddParam("?TD", _ID)

            Using myData As DataTable = myComm.Fetch
                _IsInUse = (myData.Rows.Count > 0 AndAlso ConvertDbBoolean(CIntSafe(myData.Rows(0).Item(0), 0)))
            End Using

            If ThrowOnInUse AndAlso _IsInUse Then Throw New Exception( _
                "Klaida. Darbo laiko klasė '" & Me.ToString _
                    & "' naudojama darbo laiko apskaitos žiniaraščiuose. Jos pašalinti neleidžiama.")

        End Sub

#End Region

    End Class

End Namespace