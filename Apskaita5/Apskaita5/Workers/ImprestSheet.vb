Namespace Workers

    <Serializable()> _
    Public Class ImprestSheet
        Inherits BusinessBase(Of ImprestSheet)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _ChronologicValidator As SheetChronologicValidator
        Private _Number As Integer = 0
        Private _Year As Integer = Today.Year
        Private _Month As Integer = Today.Month
        Private _Date As Date = Today
        Private _TotalSum As Double = 0
        Private _OldDate As Date = Today
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _Items As ImprestItemList


        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _ItemsSortedList As Csla.SortedBindingList(Of ImprestItem) = Nothing

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

        Public Property Number() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Number
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If value < 0 Then value = 0
                If _Number <> value Then
                    _Number = value
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

        Public ReadOnly Property Items() As ImprestItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Items
            End Get
        End Property

        Public ReadOnly Property ItemsSorted() As Csla.SortedBindingList(Of ImprestItem)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _ItemsSortedList Is Nothing Then _ItemsSortedList = _
                    New Csla.SortedBindingList(Of ImprestItem)(_Items)
                Return _ItemsSortedList
            End Get
        End Property

        Public ReadOnly Property TotalSum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalSum)
            End Get
        End Property

        Public ReadOnly Property OldDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldDate
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return _Items.GetIsDirtyEnough(Me)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _Items.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _Items.IsValid
            End Get
        End Property


        Public Overrides Function Save() As ImprestSheet

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Private Sub Items_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _Items.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

            _TotalSum = CRound(_Items.GetTotalSum)
            PropertyHasChanged("TotalSum")

        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As ImprestSheet = DirectCast(MyBase.GetClone(), ImprestSheet)
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
                RemoveHandler _Items.ListChanged, AddressOf Items_Changed
            Catch ex As Exception
            End Try
            AddHandler _Items.ListChanged, AddressOf Items_Changed
        End Sub


        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            If Not _Items.IsValid Then result = AddWithNewLine(result, _
                _Items.GetAllBrokenRules, False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Not MyBase.BrokenRulesCollection.WarningCount > 0 Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning), False)
            result = AddWithNewLine(result, _Items.GetAllWarnings, False)
            Return result
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Date.ToShortDateString
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Number", "avanso žiniaraščio numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("TotalSum", "bendra avanso žiniaraščio suma" _
                & "(nepasirinkta nė viena eilutė arba nenurodyta pasirinktų eilučių sumos)"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologicValidator"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Workers.ImprestSheet2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.ImprestSheet2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.ImprestSheet1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.ImprestSheet3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.ImprestSheet3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewImprestSheet(ByVal nYear As Integer, ByVal nMonth As Integer) As ImprestSheet

            Dim result As ImprestSheet = DataPortal.Fetch(Of ImprestSheet)(New Criteria(nYear, nMonth))
            result.MarkNew()
            Return result

        End Function

        Public Shared Function GetImprestSheet(ByVal nID As Integer) As ImprestSheet
            Return DataPortal.Fetch(Of ImprestSheet)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteImprestSheet(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private mId As Integer
            Private _FetchNew As Boolean
            Private _Year As Integer
            Private _Month As Integer
            Public ReadOnly Property Id() As Integer
                Get
                    Return mId
                End Get
            End Property
            Public ReadOnly Property FetchNew() As Boolean
                Get
                    Return _FetchNew
                End Get
            End Property
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
            Public Sub New(ByVal id As Integer)
                mId = id
                _FetchNew = False
                _Year = 0
                _Month = 0
            End Sub
            Public Sub New(ByVal nYear As Integer, ByVal nMonth As Integer)
                mId = 0
                _FetchNew = True
                _Year = nYear
                _Month = nMonth
            End Sub
        End Class


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If criteria.FetchNew Then
                If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                    "Klaida. Jūsų teisių nepakanka duomenims įvesti.")
                Create(criteria.Year, criteria.Month)
            Else
                If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                    "Klaida. Jūsų teisių nepakanka duomenims gauti.")
                Fetch(criteria.Id)
            End If

        End Sub

        Private Sub Create(ByVal nYear As Integer, ByVal nMonth As Integer)

            Dim myComm As New SQLCommand("FetchNewImprestSheet")
            myComm.AddParam("?DT", New Date(nYear, nMonth, Date.DaysInMonth(nYear, nMonth)))
            myComm.AddParam("?YR", nYear)
            myComm.AddParam("?MN", nMonth)

            Using myData As DataTable = myComm.Fetch
                _Items = ImprestItemList.GetImprestItemList(myData, True)
            End Using

            _Year = nYear
            _Month = nMonth
            _Date = New Date(_Year, _Month, 15).Date
            _OldDate = _Date.Date

            _ChronologicValidator = SheetChronologicValidator.NewSheetChronologicValidator( _
                DocumentType.ImprestSheet, _Year, _Month)

            MarkNew()

            ValidationRules.CheckRules()

        End Sub

        Private Sub Fetch(ByVal nID As Integer)

            Dim myComm As New SQLCommand("FetchImprestSheetGeneralData")
            myComm.AddParam("?NR", nID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Nerasti žiniaraščio, kurio ID=" & nID.ToString & ", duomenys.")

                Dim dr As DataRow = myData.Rows(0)

                _ID = nID
                _Number = CIntSafe(dr.Item(0), 0)
                _Date = CDateSafe(dr.Item(1), Today)
                _Year = CIntSafe(dr.Item(2), 0)
                _Month = CIntSafe(dr.Item(3), 0)
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(4), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(5), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime

                _OldDate = _Date.Date

            End Using

            _ChronologicValidator = SheetChronologicValidator.GetSheetChronologicValidator( _
                DocumentType.ImprestSheet, _ID)

            myComm = New SQLCommand("FetchImprestSheetDetails")
            myComm.AddParam("?NR", nID)

            Using myData As DataTable = myComm.Fetch
                _Items = ImprestItemList.GetImprestItemList(myData, _ChronologicValidator.FinancialDataCanChange)
            End Using

            _TotalSum = _Items.GetTotalSum

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            If _Date.Date <= GetCurrentCompany.LastClosingDate.Date Then Throw New Exception( _
                "Klaida. Neleidžiama koreguoti operacijų po uždarymo (" _
                & GetCurrentCompany.LastClosingDate & ").")

            Dim JE As General.JournalEntry = GetJournalEntry()

            DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()

            _ID = JE.ID

            Dim myComm As New SQLCommand("InsertImprestSheet")
            AddWithParams(myComm)
            myComm.AddParam("?YR", _Year)
            myComm.AddParam("?MN", _Month)

            myComm.Execute()

            _Items.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            Dim JE As General.JournalEntry = GetJournalEntry()

            CheckIfUpdateDateChanged()

            DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide()

            Dim myComm As New SQLCommand("UpdateImprestSheet")
            AddWithParams(myComm)

            myComm.Execute()

            If _ChronologicValidator.FinancialDataCanChange Then _Items.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AD", _ID)
            myComm.AddParam("?NR", _Number)
            myComm.AddParam("?DT", _Date)

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?UD", _UpdateDate.ToUniversalTime)

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            SheetChronologicValidator.CheckIfCanDelete(DocumentType.ImprestSheet, _
                DirectCast(criteria, Criteria).Id)

            IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted(DirectCast(criteria, Criteria).Id, DocumentType.ImprestSheet)

            DatabaseAccess.TransactionBegin()

            General.JournalEntry.DoDelete(DirectCast(criteria, Criteria).Id)

            Dim myComm As New SQLCommand("DeleteImprestSheetGeneral")
            myComm.AddParam("?NR", DirectCast(criteria, Criteria).Id)
            myComm.Execute()

            myComm = New SQLCommand("DeleteImprestSheetDetails")
            myComm.AddParam("?NR", DirectCast(criteria, Criteria).Id)
            myComm.Execute()

            DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub


        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry

            If IsNew Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.ImprestSheet)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_ID, DocumentType.ImprestSheet)
            End If

            result.Content = "Avansų darbuotojams žiniaraštis už " & _Year.ToString & " m. " & _
                _Month.ToString & " mėn."
            result.Date = _Date.Date
            result.DocNumber = "Av-" & _Number.ToString

            _TotalSum = _Items.GetTotalSum ' just in case

            If IsNew Then
                Dim DebetEntry As General.BookEntry = General.BookEntry.NewBookEntry()
                DebetEntry.Amount = _TotalSum
                DebetEntry.Account = GetCurrentCompany.Accounts.GetAccount( _
                    General.DefaultAccountType.WageImprestPayable)
                Dim CreditEntry As General.BookEntry = General.BookEntry.NewBookEntry()
                CreditEntry.Amount = _TotalSum
                CreditEntry.Account = GetCurrentCompany.Accounts.GetAccount( _
                     General.DefaultAccountType.WagePayable)
                result.CreditList.Add(CreditEntry)
                result.DebetList.Add(DebetEntry)
            ElseIf _ChronologicValidator.FinancialDataCanChange Then
                result.DebetList(0).Amount = _TotalSum
                result.CreditList(0).Amount = _TotalSum
            End If

            Return result

        End Function

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfImprestSheetUpdateDateChanged")
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