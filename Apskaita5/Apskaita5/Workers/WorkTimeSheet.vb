Namespace Workers

    <Serializable()> _
    Public Class WorkTimeSheet
        Inherits BusinessBase(Of WorkTimeSheet)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _Date As Date = Today
        Private _Number As String = ""
        Private _SubDivision As String = ""
        Private _Year As Integer = Today.Year
        Private _Month As Integer = Today.Month
        Private _SignedByPosition As String = ""
        Private _SignedByName As String = ""
        Private _PreparedByPosition As String = ""
        Private _PreparedByName As String = ""
        Private _WorkersCount As Integer = 0
        Private _TotalWorkTime As Double = 0
        Private _DefaultRestTimeClass As WorkTimeClassInfo = Nothing
        Private _DefaultPublicHolidayTimeClass As WorkTimeClassInfo = Nothing
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _GeneralItemList As WorkTimeItemList
        Private WithEvents _SpecialItemList As SpecialWorkTimeItemList

        Private SuspendChildListChangedEvents As Boolean = False


        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
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

        Public ReadOnly Property DefaultRestTimeClass() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultRestTimeClass
            End Get
        End Property

        Public ReadOnly Property DefaultPublicHolidayTimeClass() As WorkTimeClassInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultPublicHolidayTimeClass
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

        Public Property Number() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Number.Trim <> value.Trim Then
                    _Number = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property SubDivision() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SubDivision.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _SubDivision.Trim <> value.Trim Then
                    _SubDivision = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property Year() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Year
            End Get
        End Property

        Public ReadOnly Property Month() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Month
            End Get
        End Property

        Public Property SignedByPosition() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SignedByPosition.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _SignedByPosition.Trim <> value.Trim Then
                    _SignedByPosition = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property SignedByName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SignedByName.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _SignedByName.Trim <> value.Trim Then
                    _SignedByName = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property PreparedByPosition() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PreparedByPosition.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _PreparedByPosition.Trim <> value.Trim Then
                    _PreparedByPosition = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property PreparedByName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _PreparedByName.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _PreparedByName.Trim <> value.Trim Then
                    _PreparedByName = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property GeneralItemList() As WorkTimeItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _GeneralItemList
            End Get
        End Property

        Public ReadOnly Property SpecialItemList() As SpecialWorkTimeItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _SpecialItemList
            End Get
        End Property

        Public ReadOnly Property WorkersCount() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WorkersCount
            End Get
        End Property

        Public ReadOnly Property TotalWorkTime() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalWorkTime, ROUNDWORKTIME)
            End Get
        End Property

        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_Number.Trim) _
                    OrElse Not String.IsNullOrEmpty(_SubDivision.Trim) _
                    OrElse Not String.IsNullOrEmpty(_SignedByPosition.Trim) _
                    OrElse Not String.IsNullOrEmpty(_SignedByName.Trim) _
                    OrElse Not String.IsNullOrEmpty(_PreparedByPosition.Trim) _
                    OrElse Not String.IsNullOrEmpty(_PreparedByName.Trim) _
                    OrElse _SpecialItemList.Count > 0)
            End Get
        End Property



        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _GeneralItemList.IsDirty OrElse _SpecialItemList.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _GeneralItemList.IsValid AndAlso _SpecialItemList.IsValid
            End Get
        End Property


        Public Overrides Function Save() As WorkTimeSheet

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " _
                & vbCrLf & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            If Not _GeneralItemList.IsValid Then result = AddWithNewLine(result, _
                _GeneralItemList.GetAllBrokenRules, False)
            If Not _SpecialItemList.IsValid Then result = AddWithNewLine(result, _
                _SpecialItemList.GetAllBrokenRules, False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Not MyBase.BrokenRulesCollection.WarningCount > 0 Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning), False)
            result = AddWithNewLine(result, _GeneralItemList.GetAllWarnings, False)
            result = AddWithNewLine(result, _SpecialItemList.GetAllWarnings, False)
            Return result
        End Function


        Private Sub GeneralItemList_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _GeneralItemList.ListChanged
            If SuspendChildListChangedEvents Then Exit Sub
            RecalculateGeneralTotals(True)
        End Sub

        Private Sub RecalculateGeneralTotals(ByVal RaisePropertyHasChanged As Boolean)

            _WorkersCount = 0
            _TotalWorkTime = 0

            For Each item As WorkTimeItem In _GeneralItemList
                If item.IsChecked Then _WorkersCount += 1
                If item.IsChecked Then _TotalWorkTime = CRound(_TotalWorkTime _
                    + item.TotalHours, ROUNDWORKTIME)
            Next

            If RaisePropertyHasChanged Then
                PropertyHasChanged("WorkersCount")
                PropertyHasChanged("TotalWorkTime")
            End If

        End Sub

        Private Sub SpecialItemList_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _SpecialItemList.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As WorkTimeSheet = DirectCast(MyBase.GetClone(), WorkTimeSheet)
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
        ''' Helper method. Takes care of TaskTimeSpans loosing its handler. See GetClone method.
        ''' </summary>
        Friend Sub RestoreChildListsHandles()
            Try
                RemoveHandler _GeneralItemList.ListChanged, AddressOf GeneralItemList_Changed
                RemoveHandler _SpecialItemList.ListChanged, AddressOf SpecialItemList_Changed
            Catch ex As Exception
            End Try
            AddHandler _GeneralItemList.ListChanged, AddressOf GeneralItemList_Changed
            AddHandler _SpecialItemList.ListChanged, AddressOf SpecialItemList_Changed
        End Sub


        Public Function GetTotalHours() As Double

            Dim result As Double = 0

            For Each item As WorkTimeItem In _GeneralItemList
                result = CRound(result + item.TotalHours, ROUNDWORKTIME)
            Next

            Return result

        End Function

        Public Function GetTotalDays() As Integer

            Dim result As Integer = 0

            For Each item As WorkTimeItem In _GeneralItemList
                result = result + item.TotalDays
            Next

            Return result

        End Function

        Public Function GetTotalHoursByType(ByVal TimeClass As WorkTimeType) As Double

            Dim result As Double = 0

            For Each item As SpecialWorkTimeItem In _SpecialItemList
                If item.Type.Type = TimeClass Then result = CRound(result + item.TotalHours, ROUNDWORKTIME)
            Next

            Return result

        End Function

        Public Function GetTotalAbsenceHours() As Double

            Dim result As Double = 0

            For Each item As SpecialWorkTimeItem In _SpecialItemList
                If item.Type.Type = WorkTimeType.AnnualHolidays OrElse _
                    item.Type.Type = WorkTimeType.DownTime OrElse _
                    item.Type.Type = WorkTimeType.OtherExcluded OrElse _
                    item.Type.Type = WorkTimeType.OtherHolidays OrElse _
                    item.Type.Type = WorkTimeType.SickDays OrElse _
                    item.Type.Type = WorkTimeType.Truancy Then result = _
                        CRound(result + item.TotalHours, ROUNDWORKTIME)
            Next

            Return result

        End Function

        Public Function GetTotalAbsenceDays() As Integer

            Dim result As Integer = 0

            For Each item As WorkTimeItem In _GeneralItemList
                result = result + item.GetTotalAbsenceDays(_DefaultRestTimeClass, _
                    _DefaultPublicHolidayTimeClass)
            Next

            Return result

        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return "Darbo laiko apskaitos žiniaraštis už " & _Year.ToString _
                & " m. " & _Month.ToString & " mėn."
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("SignedByPosition", _
                "pasirašančio darbuotojo pareigos", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("SignedByName", _
                "pasirašančio darbuotojo vardas, pavardė", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("PreparedByPosition", _
                "parengusio darbuotojo pareigos", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("PreparedByName", _
                "parengusio darbuotojo vardas, pavardė", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("WorkersCount", "nė vienas darbuotojas"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Workers.WorkTimeSheet2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WorkTimeSheet1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WorkTimeSheet2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WorkTimeSheet3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.WorkTimeSheet3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewWorkTimeSheet(ByVal ForYear As Integer, _
            ByVal ForMonth As Integer, ByVal RestDayInfo As WorkTimeClassInfo, _
            ByVal PublicHolidaysInfo As WorkTimeClassInfo) As WorkTimeSheet

            If Not ForYear > 0 OrElse Not ForMonth > 0 Then Throw New Exception( _
                "Klaida. Nenurodyta mėnuo ir (ar) metai.")
            If ForYear < 1950 Then Throw New Exception("Klaida. Metai negali būti ankstesni už 1950.")
            If ForYear > 2100 Then Throw New Exception("Klaida. Metai negali būti vėlesni kaip 2100.")
            If RestDayInfo Is Nothing OrElse Not RestDayInfo.ID > 0 Then Throw New Exception( _
                "Klaida. Nenurodytas darbo ir poilsio laiko tipas paprastoms poilsio dienoms.")
            If PublicHolidaysInfo Is Nothing OrElse Not PublicHolidaysInfo.ID > 0 Then Throw New Exception( _
                "Klaida. Nenurodytas darbo ir poilsio laiko tipas švenčių dienoms.")
            If ForMonth > 12 Then ForMonth = 12

            Dim result As WorkTimeSheet = DataPortal.Create(Of WorkTimeSheet) _
                (New CreateCriteria(ForYear, ForMonth, RestDayInfo, PublicHolidaysInfo))
            result.MarkNew()
            Return result

        End Function

        Public Shared Function GetWorkTimeSheet(ByVal nID As Integer) As WorkTimeSheet
            Return DataPortal.Fetch(Of WorkTimeSheet)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteWorkTimeSheet(ByVal id As Integer)
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

        <Serializable()> _
        Private Class CreateCriteria
            Private _Year As Integer
            Private _Month As Integer
            Private _RestDayInfo As WorkTimeClassInfo
            Private _PublicHolidaysInfo As WorkTimeClassInfo
            Public ReadOnly Property Year() As Integer
                Get
                    Return _Year
                End Get
            End Property
            Public ReadOnly Property Month() As Integer
                Get
                    Return _Month
                End Get
            End Property
            Public ReadOnly Property RestDayInfo() As WorkTimeClassInfo
                Get
                    Return _RestDayInfo
                End Get
            End Property
            Public ReadOnly Property PublicHolidaysInfo() As WorkTimeClassInfo
                Get
                    Return _PublicHolidaysInfo
                End Get
            End Property
            Public Sub New(ByVal nYear As Integer, ByVal nMonth As Integer, _
                ByVal nRestDayInfo As WorkTimeClassInfo, ByVal nPublicHolidaysInfo As WorkTimeClassInfo)
                _Year = nYear
                _Month = nMonth
                _RestDayInfo = nRestDayInfo
                _PublicHolidaysInfo = nPublicHolidaysInfo
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As CreateCriteria)

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            _Year = criteria.Year
            _Month = criteria.Month
            _DefaultRestTimeClass = criteria.RestDayInfo
            _DefaultPublicHolidayTimeClass = criteria.PublicHolidaysInfo
            _Date = New Date(criteria.Year, criteria.Month, _
                Date.DaysInMonth(criteria.Year, criteria.Month))

            _GeneralItemList = WorkTimeItemList.NewWorkTimeItemList(Me, _
                criteria.RestDayInfo, criteria.PublicHolidaysInfo)
            _SpecialItemList = SpecialWorkTimeItemList.NewSpecialWorkTimeItemList(Me)

            MarkNew()

            RecalculateGeneralTotals(False)

            ValidationRules.CheckRules()

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")

            Dim myComm As New SQLCommand("FetchWorkTimeSheet")
            myComm.AddParam("?PD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID='" & criteria.ID & "', nerastas.)")

                Dim dr As DataRow = myData.Rows(0)

                _Date = CDateSafe(dr.Item(0), Today)
                _Number = CStrSafe(dr.Item(1)).Trim
                _SubDivision = CStrSafe(dr.Item(2)).Trim
                _Year = CIntSafe(dr.Item(3), 0)
                _Month = CIntSafe(dr.Item(4), 0)
                _SignedByPosition = CStrSafe(dr.Item(5)).Trim
                _SignedByName = CStrSafe(dr.Item(6)).Trim
                _PreparedByPosition = CStrSafe(dr.Item(7)).Trim
                _PreparedByName = CStrSafe(dr.Item(8)).Trim
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(9), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(10), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _DefaultRestTimeClass = WorkTimeClassInfo.GetWorkTimeClassInfo(dr, 11)
                _DefaultPublicHolidayTimeClass = WorkTimeClassInfo.GetWorkTimeClassInfo(dr, 20)

                _ID = criteria.ID

            End Using

            _GeneralItemList = WorkTimeItemList.GetWorkTimeItemList(Me)
            _SpecialItemList = SpecialWorkTimeItemList.GetSpecialWorkTimeItemList(Me)

            RecalculateGeneralTotals(False)

            ValidationRules.CheckRules()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim myComm As New SQLCommand("InsertWorkTimeSheet")
            AddWithParams(myComm)
            myComm.AddParam("?AD", _Year)
            myComm.AddParam("?AE", _Month)
            myComm.AddParam("?AK", _DefaultRestTimeClass.ID)
            myComm.AddParam("?AL", _DefaultPublicHolidayTimeClass.ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            GeneralItemList.Update(Me)
            SpecialItemList.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("UpdateWorkTimeSheet")
            AddWithParams(myComm)
            myComm.AddParam("?CD", _ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            GeneralItemList.Update(Me)
            SpecialItemList.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            Dim myComm As New SQLCommand("DeleteDayWorkTimeForWorkTimeSheet")
            myComm.AddParam("?PD", DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            myComm = New SQLCommand("DeleteWorkTimeItemList")
            myComm.AddParam("?PD", DirectCast(criteria, Criteria).ID)

            myComm.Execute()

            myComm = New SQLCommand("DeleteSpecialWorkTimeItemList")
            myComm.AddParam("?CD", DirectCast(criteria, Criteria).ID)

            myComm.Execute()

            myComm = New SQLCommand("DeleteWorkTimeSheet")
            myComm.AddParam("?CD", DirectCast(criteria, Criteria).ID)

            myComm.Execute()

            DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub


        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _Date.Date)
            myComm.AddParam("?AB", _Number.Trim)
            myComm.AddParam("?AC", _SubDivision.Trim)
            myComm.AddParam("?AF", _SignedByPosition.Trim)
            myComm.AddParam("?AG", _SignedByName.Trim)
            myComm.AddParam("?AH", _PreparedByPosition.Trim)
            myComm.AddParam("?AI", _PreparedByName.Trim)

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?AJ", _UpdateDate.ToUniversalTime)
            
        End Sub


        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfWorkTimeSheetUpdateDateChanged")
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