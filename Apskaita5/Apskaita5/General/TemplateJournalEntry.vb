Namespace General

    <Serializable()> _
    Public Class TemplateJournalEntry
        Inherits BusinessBase(Of TemplateJournalEntry)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _Content As String = ""
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _DebetList As TemplateBookEntryList
        Private _CreditList As TemplateBookEntryList


        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _DebetListSortedList As Csla.SortedBindingList(Of TemplateBookEntry) = Nothing
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _CreditListSortedList As Csla.SortedBindingList(Of TemplateBookEntry) = Nothing

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

        Public ReadOnly Property DebetList() As TemplateBookEntryList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DebetList
            End Get
        End Property

        Public ReadOnly Property CreditList() As TemplateBookEntryList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CreditList
            End Get
        End Property

        Public ReadOnly Property DebetListSorted() As Csla.SortedBindingList(Of TemplateBookEntry)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _DebetListSortedList Is Nothing Then _DebetListSortedList = _
                    New Csla.SortedBindingList(Of TemplateBookEntry)(_DebetList)
                Return _DebetListSortedList
            End Get
        End Property

        Public ReadOnly Property CreditListSorted() As Csla.SortedBindingList(Of TemplateBookEntry)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _CreditListSortedList Is Nothing Then _CreditListSortedList = _
                    New Csla.SortedBindingList(Of TemplateBookEntry)(_CreditList)
                Return _CreditListSortedList
            End Get
        End Property

        Public ReadOnly Property IsDirtyEnough() As Boolean _
        Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_Name.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Content.Trim)) _
                    OrElse _DebetList.Count > 0 OrElse _CreditList.Count > 0
            End Get
        End Property


        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _DebetList.IsDirty OrElse _CreditList.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _DebetList.IsValid AndAlso _CreditList.IsValid _
                    AndAlso _DebetList.Count > 0 AndAlso _CreditList.Count > 0
            End Get
        End Property



        Public Overrides Function Save() As TemplateJournalEntry

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " _
                & vbCrLf & Me.GetAllBrokenRules)

            Dim result As TemplateJournalEntry = MyBase.Save
            HelperLists.TemplateJournalEntryInfoList.InvalidateCache()
            Return result

        End Function

        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            If Not _DebetList.IsValid Then result = AddWithNewLine(result, _
                _DebetList.GetAllBrokenRules, False)
            If Not _CreditList.IsValid Then result = AddWithNewLine(result, _
                _CreditList.GetAllBrokenRules, False)
            If Not _DebetList.Count > 0 Then result = AddWithNewLine(result, _
                "Nenurodyta nė viena debetuojama sąskaita.", False)
            If Not _CreditList.Count > 0 Then result = AddWithNewLine(result, _
                "Nenurodyta nė viena kredituojama sąskaita.", False)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = ""
            If Not MyBase.BrokenRulesCollection.WarningCount > 0 Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning), False)
            result = AddWithNewLine(result, _DebetList.GetAllWarnings, False)
            result = AddWithNewLine(result, _CreditList.GetAllWarnings, False)
            Return result
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 Then Return ""
            Return _Name
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "operacijos šablono pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos turinys"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("General.JournalEntryTemplate2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntryTemplate2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntryTemplate1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntryTemplate3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntryTemplate3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewTemplateJournalEntry() As TemplateJournalEntry

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Dim result As New TemplateJournalEntry
            result._DebetList = TemplateBookEntryList.NewTemplateBookEntryList(BookEntryType.Debetas)
            result._CreditList = TemplateBookEntryList.NewTemplateBookEntryList(BookEntryType.Kreditas)

            result.ValidationRules.CheckRules()

            Return result

        End Function

        Public Shared Function GetTemplateJournalEntry(ByVal nID As Integer) As TemplateJournalEntry
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")
            Return DataPortal.Fetch(Of TemplateJournalEntry)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteTemplateJournalEntry(ByVal id As Integer)
            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka sąskaitos duomenų ištrynimui.")
            DataPortal.Delete(New Criteria(id))
            HelperLists.TemplateJournalEntryInfoList.InvalidateCache()
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

            Dim myComm As New SQLCommand("SelectTemplateJournalEntry")
            myComm.AddParam("?BD", criteria.ID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID='" & criteria.ID & "', nerastas.)")

                Dim dr As DataRow = myData.Rows(0)

                _ID = criteria.ID
                _Name = CStrSafe(dr.Item(0)).Trim
                _Content = CStrSafe(dr.Item(1)).Trim
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(2), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(3), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime

            End Using

            myComm = New SQLCommand("SelectTemplateBookEntryList")
            myComm.AddParam("?BD", criteria.ID)

            Using myData As DataTable = myComm.Fetch
                _DebetList = TemplateBookEntryList.GetTemplateBookEntryList(myData, BookEntryType.Debetas)
                _CreditList = TemplateBookEntryList.GetTemplateBookEntryList(myData, BookEntryType.Kreditas)
            End Using

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            CheckIfNameUnique()

            Dim myComm As New SQLCommand("TemplateJournalEntryInsert")
            AddWithParams(myComm)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            DebetList.Update(Me)
            CreditList.Update(Me)

            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            CheckIfNameUnique()

            CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("TemplateJournalEntryUpdate")
            AddWithParams(myComm)
            myComm.AddParam("?BD", _ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            DebetList.Update(Me)
            CreditList.Update(Me)
            DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?PV", _Name.Trim)
            myComm.AddParam("?TR", _Content.Trim)

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

            Dim myComm As New SQLCommand("TemplateJournalEntryDelete")
            myComm.AddParam("?BD", DirectCast(criteria, Criteria).ID)

            DatabaseAccess.TransactionBegin()

            myComm.Execute()

            myComm = New SQLCommand("TemplateBookEntriesDelete")
            myComm.AddParam("?BD", DirectCast(criteria, Criteria).ID)
            myComm.Execute()

            DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub


        Private Sub CheckIfNameUnique()

            Dim myComm As New SQLCommand("TemplateJournalEntryExists")
            myComm.AddParam("?TD", _ID)
            myComm.AddParam("?NM", _Name.ToLower.Trim)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 AndAlso CIntSafe(myData.Rows(0).Item(0)) > 0 Then _
                    Throw New Exception("Klaida. Bendrojo žurnalo šablonas tokiu pavadinimu jau egzistuoja.")
            End Using

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfTemplateJournalEntryUpdateDateChanged")
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