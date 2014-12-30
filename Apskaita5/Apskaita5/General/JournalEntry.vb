Namespace General

    <Serializable()> _
    Public Class JournalEntry
        Inherits BusinessBase(Of JournalEntry)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _Date As Date = Today
        Private _DocNumber As String = ""
        Private _Content As String = ""
        Private _Person As PersonInfo = Nothing
        Private _DocType As DocumentType = DocumentType.None
        Private _DocTypeHumanReadable As String = ""
        Private WithEvents _DebetList As BookEntryList
        Private WithEvents _CreditList As BookEntryList
        Private _DebetSum As Double = 0
        Private _CreditSum As Double = 0
        Private _OldDate As Date = Today
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _ChronologyValidator As IChronologicValidator = Nothing

        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _DebetListSortedList As Csla.SortedBindingList(Of BookEntry) = Nothing
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _CreditListSortedList As Csla.SortedBindingList(Of BookEntry) = Nothing

        ''' <summary>
        ''' Gets an ID of the JournalEntry object (assigned by DB AUTO_INCREMENT).
        ''' Friend SET method is used when the object is child 
        ''' and is used for generating/fetching other documents JuornalEntry objects.
        ''' </summary>
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

        Public ReadOnly Property ChronologyValidator() As IChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologyValidator
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a date of the Journal Entry.
        ''' </summary>
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

        ''' <summary>
        ''' Gets or sets a number of the document associated with the Journal Entry.
        ''' </summary>
        Public Property DocNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocNumber.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _DocNumber.Trim <> value.Trim Then
                    _DocNumber = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a content/description of the the Journal Entry.
        ''' </summary>
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

        ''' <summary>
        ''' Gets or sets a person associated with the Journal Entry.
        ''' </summary>
        Public Property Person() As PersonInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Person
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As PersonInfo)
                CanWriteProperty(True)
                If Not (_Person Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _Person Is Nothing AndAlso Not value Is Nothing AndAlso _Person = value) Then
                    _Person = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets a DocumentType of the document associated with the Journal Entry (as enum).
        ''' </summary>
        Public ReadOnly Property DocType() As DocumentType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocType
            End Get
        End Property

        ''' <summary>
        ''' Gets a DocumentType of the document associated with the Journal Entry 
        ''' (as human readable string).
        ''' </summary>
        Public ReadOnly Property DocTypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DocTypeHumanReadable.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets a BookEntryList of type Debet in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property DebetList() As BookEntryList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DebetList
            End Get
        End Property

        ''' <summary>
        ''' Gets a BookEntryList of type Credit in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property CreditList() As BookEntryList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CreditList
            End Get
        End Property

        ''' <summary>
        ''' Gets a sortable BookEntryList of type Debet in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property DebetListSorted() As Csla.SortedBindingList(Of BookEntry)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _DebetListSortedList Is Nothing Then _DebetListSortedList = _
                    New Csla.SortedBindingList(Of BookEntry)(_DebetList)
                Return _DebetListSortedList
            End Get
        End Property

        ''' <summary>
        ''' Gets a sortable BookEntryList of type Credit in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property CreditListSorted() As Csla.SortedBindingList(Of BookEntry)
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _CreditListSortedList Is Nothing Then _CreditListSortedList = _
                    New Csla.SortedBindingList(Of BookEntry)(_CreditList)
                Return _CreditListSortedList
            End Get
        End Property

        ''' <summary>
        ''' Gets a total sum of ammounts/values of all the BookEntryList of type Debet in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property DebetSum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DebetSum)
            End Get
        End Property

        ''' <summary>
        ''' Gets a total sum of ammounts/values of all the BookEntryList of type Credit in the JuornalEntry. 
        ''' </summary>
        Public ReadOnly Property CreditSum() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CreditSum)
            End Get
        End Property

        ''' <summary>
        ''' Gets an original date of the JuornalEntry, i.e. befor user changes. 
        ''' </summary>
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
                Return (Not String.IsNullOrEmpty(_DocNumber.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Content.Trim) _
                    OrElse _DebetList.Count > 0 OrElse _CreditList.Count > 0)
            End Get
        End Property


        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _DebetList.IsDirty OrElse _CreditList.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _DebetList.IsValid AndAlso _CreditList.IsValid
            End Get
        End Property


        Public Overrides Function Save() As JournalEntry

            If _DocType <> DocumentType.None Then Throw New Exception("Klaida. " & _
                "Negalima tiesiogiai išsaugoti registruojant dokumentą padaryto įrašo bendrajame žurnale.")

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Private Sub DebetList_Changed(ByVal sender As Object, _
        ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _DebetList.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

            _DebetSum = _DebetList.GetSum
            PropertyHasChanged("DebetSum")

        End Sub

        Private Sub CreditList_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _CreditList.ListChanged

            If SuspendChildListChangedEvents Then Exit Sub

            _CreditSum = _CreditList.GetSum
            PropertyHasChanged("CreditSum")

        End Sub


        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As JournalEntry = DirectCast(MyBase.GetClone(), JournalEntry)
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
        ''' Helper method. Takes care of DebetList and CreditList loosing their handlers. See GetClone method.
        ''' </summary>
        Friend Sub RestoreChildListsHandles()

            Try
                RemoveHandler _DebetList.ListChanged, AddressOf DebetList_Changed
                RemoveHandler _CreditList.ListChanged, AddressOf CreditList_Changed
            Catch ex As Exception
            End Try
            AddHandler _DebetList.ListChanged, AddressOf DebetList_Changed
            AddHandler _CreditList.ListChanged, AddressOf CreditList_Changed

        End Sub


        ''' <summary>
        ''' Indicates if the JournalEntry can be deleted as standalone document, 
        ''' i.e. is not a part of some complex document, e.g. invoice. 
        ''' </summary>
        Friend Shared Function DirectDeletionIsPossible(ByVal AssociatedDocumentType As DocumentType, _
            ByVal ThrowOnNotPossible As Boolean) As Boolean

            Dim result As Boolean = True
            Dim ExceptionMessage As String = ""

            If AssociatedDocumentType = DocumentType.AdvanceReport Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant avanso apyskaitą. Jį leidžiama ištrinti tik dokumentų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.Amortization Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant amortizacijos operaciją su ilgalaikiu turtu. Jį leidžiama " & _
                    "ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.GoodsProduction Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant prekių gamybos operaciją. Jį leidžiama " & _
                    "ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.GoodsRevalue Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant prekių pervertinimo operaciją. Jį leidžiama " & _
                    "ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.GoodsWriteOff Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant prekių nurašymo operaciją. Jį leidžiama " & _
                    "ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.ImprestSheet Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant avanso žiniaraštį. Jį leidžiama ištrinti tik darbuotojų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.InvoiceMade Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant išrašytą sąskaitą faktūrą. Jį leidžiama ištrinti tik dokumentų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.InvoiceReceived Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant gautą sąskaitą faktūrą. Jį leidžiama ištrinti tik dokumentų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.LongTermAssetDiscard Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant ilgalaikio turto nurašymo operaciją. Jį leidžiama " & _
                    "ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.TillIncomeOrder Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant kasos pajamų orderį. Jį leidžiama ištrinti tik dokumentų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.TillSpendingOrder Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant kasos išlaidų orderį. Jį leidžiama ištrinti tik dokumentų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.GoodsInternalTransfer Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant prekių vidinio judėjimo operaciją. Jį leidžiama " & _
                    "ištrinti tik prekių modulyje."

            ElseIf AssociatedDocumentType = DocumentType.WageSheet Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant darbo užmokesčio žiniaraštį. Jį leidžiama ištrinti tik darbuotojų modulyje."

            ElseIf AssociatedDocumentType = DocumentType.LongTermAssetAccountChange Then
                result = False
                ExceptionMessage = "Klaida. Šis bendrojo žurnalo įrašas buvo padarytas " & _
                    "registruojant ilgalaikio turto apskaitos sąsk. pakeitimą. " _
                    & "Jį leidžiama ištrinti tik turto modulyje."

            ElseIf AssociatedDocumentType = DocumentType.BankOperation _
                OrElse AssociatedDocumentType = DocumentType.ClosingEntries _
                OrElse AssociatedDocumentType = DocumentType.None _
                OrElse AssociatedDocumentType = DocumentType.TransferOfBalance Then
                result = True

            Else
                Throw New Exception("Klaida. Neatpažintas dokumento tipas '" _
                    & AssociatedDocumentType.ToString & "'.")

            End If

            If Not result AndAlso ThrowOnNotPossible Then Throw New Exception(ExceptionMessage)

            Return result

        End Function

        ''' <summary>
        ''' Replaces DebetList with the new debet list.
        ''' </summary>
        ''' <param name="NewDebetList">New debet list.</param>
        ''' <remarks>Used in till order objects to avoid redundant updates.</remarks>
        Friend Sub SetDebetList(ByVal NewDebetList As BookEntryList)

            If NewDebetList Is Nothing Then Throw New ArgumentNullException( _
                "Klaida. Metodui JournalEntry.SetDebetList nenurodytas korespondencijų sąrašas.")
            If NewDebetList.Type <> BookEntryType.Debetas Then Throw New ArgumentOutOfRangeException( _
                "Klaida. Metodui JournalEntry.SetDebetList nurodomas korespondencijų sąrašas " _
                & "turi būti debeto tipo.")

            _DebetList = NewDebetList

        End Sub

        ''' <summary>
        ''' Replaces CreditList with the new credit list.
        ''' </summary>
        ''' <param name="NewCreditList">New credit list.</param>
        ''' <remarks>Used in till order objects to avoid redundant updates.</remarks>
        Friend Sub SetCreditList(ByVal NewCreditList As BookEntryList)

            If NewCreditList Is Nothing Then Throw New ArgumentNullException( _
                "Klaida. Metodui JournalEntry.SetCreditList nenurodytas korespondencijų sąrašas.")
            If NewCreditList.Type <> BookEntryType.Kreditas Then Throw New ArgumentOutOfRangeException( _
                "Klaida. Metodui JournalEntry.SetCreditList nurodomas korespondencijų sąrašas " _
                & "turi būti kredito tipo.")

            _CreditList = NewCreditList

        End Sub

        ''' <summary>
        ''' Gets a list of debet and credit book entries formated as e.g. "D 2711, K 443". 
        ''' </summary>
        Public Function GetCorrespondentionsString() As String

            Dim resultList As New List(Of String)
            For Each item As BookEntry In _DebetList
                resultList.Add("D " & item.Account.ToString)
            Next
            For Each item As BookEntry In _CreditList
                resultList.Add("K " & item.Account.ToString)
            Next

            Dim result As String = String.Join(", ", resultList.ToArray)

            If result.Trim.Length > 254 Then result = result.Trim.Substring(0, 249) & "<...>"

            Return result

        End Function

        ''' <summary>
        ''' Loads JournalEntryTemplate data (content and corespondences) to JournalEntry.
        ''' Clears current corespondences. In case of an old JournalEntry 
        ''' cleared corespondences are moved to DeletedList.
        ''' </summary>
        ''' <param name="EntryTemplate">JournalEntryTemplate object containing the data to load.</param>
        Public Sub LoadJournalEntryFromTemplate( _
            ByVal EntryTemplate As HelperLists.TemplateJournalEntryInfo)

            Content = EntryTemplate.Content
            _DebetList.LoadBookEntryListFromTemplate(EntryTemplate.DebetListString, True)
            _CreditList.LoadBookEntryListFromTemplate(EntryTemplate.CreditListString, True)

        End Sub

        Public Function GetAllBrokenRules() As String
            Dim result As String = ""
            If Not MyBase.IsValid Then result = AddWithNewLine(result, _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error), False)
            If Not _DebetList.IsValid Then result = AddWithNewLine(result, _
                _DebetList.GetAllBrokenRules, False)
            If Not _CreditList.IsValid Then result = AddWithNewLine(result, _
                _CreditList.GetAllBrokenRules, False)
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
            If Not _ID > 0 Then
                Return ""
            Else
                Return _Date.ToShortDateString & " Nr. " & _DocNumber & " - " & _Content
            End If
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("DocNumber", "pagrindžiančio dokumento numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos turinys"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("Person", "susietas kontrahentas", _
                "ID", Validation.RuleSeverity.Warning))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("DebetSum", "debetuojama suma"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("CreditSum", "kredituojama suma"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologyValidator"))

            ValidationRules.AddRule(AddressOf DebetEqualsCreditValidation, New Validation.RuleArgs("DebetSum"))

            ValidationRules.AddDependantProperty("CreditSum", "DebetSum", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that Debet BookEntries.GetSum = Credit BookEntries.GetSum.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function DebetEqualsCreditValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As JournalEntry = DirectCast(target, JournalEntry)

            If Not CRound(ValObj._DebetSum) > 0 OrElse Not CRound(ValObj._CreditSum) > 0 Then Return True

            If CRound(ValObj._DebetSum) <> CRound(ValObj._CreditSum) Then
                e.Description = "Debeto korespondencijų suma nelygi kredito korespondencijų sumai."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("General.JournalEntry2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntry1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntry2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntry3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("General.JournalEntry3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewJournalEntry() As JournalEntry
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims įvesti.")
            Return New JournalEntry(DocumentType.None)
        End Function

        Friend Shared Function NewJournalEntryChild(ByVal AssociatedDocumentType As DocumentType) As JournalEntry
            Return New JournalEntry(AssociatedDocumentType)
        End Function

        Public Shared Function GetJournalEntry(ByVal nID As Integer) As JournalEntry

            Return DataPortal.Fetch(Of JournalEntry)(New Criteria(nID))

        End Function

        Friend Shared Function GetJournalEntryChild(ByVal JournalEntryID As Integer, _
            ByVal ExpectedType As DocumentType) As JournalEntry

            Dim result As JournalEntry = New JournalEntry(JournalEntryID)

            If result._DocType <> ExpectedType Then Throw New Exception( _
                "Klaida. Bendrojo žurnalo įrašo ID=" & JournalEntryID.ToString _
                & " dokumento tipas yra '" & result._DocTypeHumanReadable _
                & "', o tikėtąsi tipo '" & ConvertEnumHumanReadable(ExpectedType) & "'.")

            Return result

        End Function

        Public Shared Sub DeleteJournalEntry(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub


        Private Sub New()
            ' require use of factory methods
        End Sub


        Private Sub New(ByVal AssociatedDocumentType As DocumentType)
            If AssociatedDocumentType <> DocumentType.None Then MarkAsChild()
            Create(AssociatedDocumentType)
        End Sub

        Private Sub New(ByVal JournalEntryID As Integer)
            MarkAsChild()
            DoFetch(JournalEntryID)
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


        Private Sub Create(ByVal AssociatedDocumentType As DocumentType)

            _DocType = AssociatedDocumentType
            _DocTypeHumanReadable = ConvertEnumHumanReadable(AssociatedDocumentType)
            _DebetList = BookEntryList.NewBookEntryList(BookEntryType.Debetas)
            _CreditList = BookEntryList.NewBookEntryList(BookEntryType.Kreditas)
            _ChronologyValidator = SimpleChronologicValidator.NewSimpleChronologicValidator( _
                "bendrojo žurnalo operacija")

            ValidationRules.CheckRules()

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims gauti.")
            DoFetch(criteria.ID)
        End Sub

        Private Sub DoFetch(ByVal JournalEntryID As Integer)

            Dim myComm As New SQLCommand("JournalEntryFetch")
            myComm.AddParam("?BD", JournalEntryID)

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception("Klaida. Objektas, kurio ID='" _
                    & JournalEntryID.ToString & "', nerastas.)")

                Dim dr As DataRow = myData.Rows(0)

                _ID = JournalEntryID
                _Date = CDateSafe(dr.Item(0), Today)
                _OldDate = _Date
                _DocNumber = CStrSafe(dr.Item(1)).Trim
                _Content = CStrSafe(dr.Item(2)).Trim
                _DocType = ConvertEnumDatabaseStringCode(Of DocumentType)(CStrSafe(dr.Item(3)).Trim)
                _DocTypeHumanReadable = ConvertEnumHumanReadable(_DocType)
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(4), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(5), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _Person = HelperLists.PersonInfo.GetPersonInfo(dr, 6)

            End Using

            _ChronologyValidator = SimpleChronologicValidator.GetSimpleChronologicValidator(_ID)

            myComm = New SQLCommand("BookEntriesFetch")
            myComm.AddParam("?BD", JournalEntryID)

            Using myData As DataTable = myComm.Fetch
                _DebetList = BookEntryList.GetBookEntryList(myData, BookEntryType.Debetas, _
                    _ChronologyValidator.FinancialDataCanChange, _
                    _ChronologyValidator.FinancialDataCanChangeExplanation)
                _CreditList = BookEntryList.GetBookEntryList(myData, BookEntryType.Kreditas, _
                    _ChronologyValidator.FinancialDataCanChange, _
                    _ChronologyValidator.FinancialDataCanChangeExplanation)
            End Using

            _DebetSum = _DebetList.GetSum
            _CreditSum = _CreditList.GetSum

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Friend Function SaveServerSide() As JournalEntry
            Dim result As JournalEntry = Me.Clone
            If result.IsNew Then
                result.DoInsert()
            Else
                result.DoUpdate()
            End If
            Return result
        End Function

        Protected Overrides Sub DataPortal_Insert()
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            DoInsert()
        End Sub

        Private Sub DoInsert()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            Dim myComm As New SQLCommand("JournalEntryInsert")
            AddWithParams(myComm)

            myComm.Execute()

            _ID = Convert.ToInt32(myComm.LastInsertID)

            _DebetList.Update(Me)
            _CreditList.Update(Me)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub

        Protected Overrides Sub DataPortal_Update()
            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")
            CheckIfUpdateDateChanged()
            DoUpdate()
        End Sub

        Private Sub DoUpdate()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            Dim myComm As New SQLCommand("JournalEntryUpdate")
            AddWithParams(myComm)
            myComm.AddParam("?BD", _ID)

            myComm.Execute()

            If _ChronologyValidator.FinancialDataCanChange Then
                _DebetList.Update(Me)
                _CreditList.Update(Me)
            End If

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            MarkOld()

        End Sub


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pašalinti.")

            Dim IndirectRelations As IndirectRelationInfoList = IndirectRelationInfoList. _
                GetIndirectRelationInfoListServerSide(DirectCast(criteria, Criteria).ID)
            IndirectRelations.CheckIfJournalEntryCanBeDeleted(New DocumentType() _
                {DocumentType.None, DocumentType.ClosingEntries})

            DoDelete(DirectCast(criteria, Criteria).ID)

            ' Last closing date is part of CompanyInfo object in GlobalContext
            If IndirectRelations.DocType = DocumentType.ClosingEntries Then _
                ApskaitaObjects.Settings.CompanyInfo.LoadCompanyInfoToGlobalContext("", "")

        End Sub

        Friend Shared Sub DoDelete(ByVal JournalEntryID As Integer)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            Dim myComm As New SQLCommand("JournalEntryDelete")
            myComm.AddParam("?BD", JournalEntryID)
            myComm.Execute()

            Dim myComm2 As New SQLCommand("BookEntryClear")
            myComm2.AddParam("?BD", JournalEntryID)
            myComm2.Execute()

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub

        Private Sub AddWithParams(ByRef myComm As SQLCommand)

            myComm.AddParam("?AA", _Date.Date)
            myComm.AddParam("?AB", _DocNumber.Trim)
            myComm.AddParam("?AC", _Content.Trim)
            myComm.AddParam("?AD", ConvertEnumDatabaseStringCode(_DocType))
            If Not _Person Is Nothing AndAlso _Person.ID > 0 Then
                myComm.AddParam("?AE", _Person.ID)
            Else
                myComm.AddParam("?AE", 0)
            End If
            myComm.AddParam("?AF", Me.GetCorrespondentionsString)

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?AG", _UpdateDate.ToUniversalTime)

        End Sub


        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfJournalEntryUpdateDateChanged")
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