Namespace Goods

    <Serializable()> _
    Public Class GoodsComplexOperationInventorization
        Inherits BusinessBase(Of GoodsComplexOperationInventorization)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _JournalEntryID As Integer = 0
        Private _OperationalLimit As ComplexChronologicValidator = Nothing
        Private _Date As Date = Today
        Private _DocumentNumber As String = ""
        Private _Content As String = ""
        Private _WarehouseID As Integer = 0
        Private _WarehouseName As String = ""
        Private _WarehouseAccount As Long = 0
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _Items As GoodsInventorizationItemList


        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _ItemsSortedList As Csla.SortedBindingList(Of GoodsInventorizationItem) = Nothing

        Public ReadOnly Property ID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property JournalEntryID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _JournalEntryID
            End Get
        End Property

        Public ReadOnly Property OperationalLimit() As ComplexChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OperationalLimit
            End Get
        End Property

        Public ReadOnly Property [Date]() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Date
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

        Public ReadOnly Property WarehouseID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseID
            End Get
        End Property

        Public ReadOnly Property WarehouseName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseName.Trim
            End Get
        End Property

        Public ReadOnly Property WarehouseAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseAccount
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

        Public ReadOnly Property Items() As GoodsInventorizationItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Items
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_DocumentNumber.Trim) _
                OrElse Not String.IsNullOrEmpty(_Content.Trim))
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



        Public Function GetAllBrokenRules() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error).Trim
            result = AddWithNewLine(result, _Items.GetAllBrokenRules, False)

            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning).Trim
            result = AddWithNewLine(result, _Items.GetAllWarnings(), False)


            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function


        Public Overrides Function Save() As GoodsComplexOperationInventorization

            If IsNew AndAlso Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            If Not IsNew AndAlso Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            If Not _Items.Count > 0 Then Throw New Exception("Klaida. Neįvesta nė viena eilutė.")

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbcrlf & Me.GetAllBrokenRules)
            Return MyBase.Save

        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return "Goods.GoodsComplexOperationInventorization"
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("DocumentNumber", "dokumento numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos aprašymas"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "OperationalLimit"))

        End Sub

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Goods.GoodsOperationInventorization2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationInventorization1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationInventorization2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationInventorization3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationInventorization3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewGoodsComplexOperationInventorization(ByVal OperationDate As Date, _
            ByVal OperationWarehouseID As Integer) As GoodsComplexOperationInventorization

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")

            Return DataPortal.Create(Of GoodsComplexOperationInventorization) _
                (New Criteria(OperationDate, OperationWarehouseID))

        End Function

        Public Shared Function GetGoodsComplexOperationInventorization(ByVal nID As Integer) As GoodsComplexOperationInventorization

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")

            Return DataPortal.Fetch(Of GoodsComplexOperationInventorization)(New Criteria(nID))

        End Function

        Public Shared Sub DeleteGoodsComplexOperationInventorization(ByVal id As Integer)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenų ištrynimui.")

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
            Private _OperationDate As Date
            Private _OperationWarehouseID As Integer
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property OperationDate() As Date
                Get
                    Return _OperationDate
                End Get
            End Property
            Public ReadOnly Property OperationWarehouseID() As Integer
                Get
                    Return _OperationWarehouseID
                End Get
            End Property
            Public Sub New(ByVal nID As Integer)
                _ID = nID
                _OperationDate = Today
                _OperationWarehouseID = 0
            End Sub
            Public Sub New(ByVal nOperationDate As Date, ByVal nWarehouseID As Integer)
                _ID = 0
                _OperationDate = nOperationDate
                _OperationWarehouseID = nWarehouseID
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

            Dim LastInventorizationDate As Date = GetLastInventorizationDate(criteria.OperationWarehouseID)

            If LastInventorizationDate.Date >= criteria.OperationDate.Date Then _
                Throw New Exception("Klaida. Nauja inventorizacija negali būti anksčiau nei praėjusi - " _
                & LastInventorizationDate.ToString("yyyy-MM-dd") & ".")

            _Date = criteria.OperationDate

            Dim myComm As New SQLCommand("CreateGoodsComplexOperationInventorizationItems")
            myComm.AddParam("?WD", criteria.OperationWarehouseID)
            If LastInventorizationDate = Date.MinValue Then
                myComm.AddParam("?DF", New Date(1970, 1, 1))
            Else
                myComm.AddParam("?DF", LastInventorizationDate.Date)
            End If
            myComm.AddParam("?DT", criteria.OperationDate.Date)

            Using myData As DataTable = myComm.Fetch

                Using LimitationsDataSource As DataTable = OperationalLimitList. _
                    GetDataSourceForNewInventorization(criteria.OperationWarehouseID)

                    Dim consignmentDictionary As Dictionary(Of Integer, ConsignmentPersistenceObjectList) _
                        = ConsignmentPersistenceObjectList.GetConsignmentPersistenceObjectList( _
                        criteria.OperationWarehouseID, -1)

                    _Items = GoodsInventorizationItemList.NewGoodsInventorizationItemList( _
                        myData, consignmentDictionary, LimitationsDataSource, criteria.OperationDate, _
                        criteria.OperationWarehouseID)

                End Using

            End Using

            Dim BaseValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                NewSimpleChronologicValidator(ConvertEnumHumanReadable(GoodsOperationType.Inventorization))


            _OperationalLimit = ComplexChronologicValidator.GetComplexChronologicValidator( _
                 _ID, _Date, ConvertEnumHumanReadable(GoodsOperationType.Inventorization), _
                 BaseValidator, _Items.GetLimitations())

            MarkNew()

            ValidationRules.CheckRules()

        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            Dim obj As ComplexOperationPersistenceObject = ComplexOperationPersistenceObject. _
                GetComplexOperationPersistenceObject(criteria.ID, True)

            _ID = obj.ID
            _JournalEntryID = obj.JournalEntryID
            _Date = obj.OperationDate
            _DocumentNumber = obj.DocNo
            _Content = obj.Content
            _WarehouseID = obj.WarehouseID
            _WarehouseName = obj.Warehouse.Name
            _WarehouseAccount = obj.Warehouse.WarehouseAccount
            _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate

            Dim LastInventorizationDate As Date = GetLastInventorizationDate(_WarehouseID)

            Dim myComm As New SQLCommand("FetchGoodsComplexOperationInventorizationItems")
            myComm.AddParam("?WD", _WarehouseID)
            If LastInventorizationDate = Date.MinValue Then
                myComm.AddParam("?DF", New Date(1970, 1, 1))
            Else
                myComm.AddParam("?DF", LastInventorizationDate.Date)
            End If
            myComm.AddParam("?DT", _Date.Date)
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch

                Using LimitationsDataSource As DataTable = OperationalLimitList. _
                    GetDataSourceForComplexOperation(_ID, _Date)

                    Dim consignmentDictionary As Dictionary(Of Integer, ConsignmentPersistenceObjectList) _
                        = ConsignmentPersistenceObjectList.GetConsignmentPersistenceObjectList( _
                        _WarehouseID, _ID)

                    _Items = GoodsInventorizationItemList.GetGoodsInventorizationItemList(myData, _
                        consignmentDictionary, LimitationsDataSource, _Date, _WarehouseID)

                End Using

            End Using

            Dim parentValidator As IChronologicValidator
            If _JournalEntryID > 0 Then
                parentValidator = SimpleChronologicValidator.GetSimpleChronologicValidator(_JournalEntryID)
            Else
                parentValidator = EmptyChronologicValidator.NewEmptyChronologicValidator( _
                    ConvertEnumHumanReadable(GoodsOperationType.Inventorization))
            End If

            _OperationalLimit = ComplexChronologicValidator.GetComplexChronologicValidator( _
                 _ID, _Date, ConvertEnumHumanReadable(GoodsOperationType.Inventorization), _
                 parentValidator, _Items.GetLimitations())

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            PrepareOperationConsignments()

            CheckIfCanUpdate()

            DoSave()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            ComplexOperationPersistenceObject.CheckIfUpdateDateChanged(_ID, _UpdateDate)

            PrepareOperationConsignments()

            CheckIfCanUpdate()

            DoSave()

        End Sub

        Private Sub DoSave()

            Dim obj As ComplexOperationPersistenceObject = GetPersistenceObj()

            Dim JE As General.JournalEntry = GetJournalEntry()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            If Not JE Is Nothing Then
                JE = JE.SaveServerSide
                If IsNew Then
                    _JournalEntryID = JE.ID
                    obj.JournalEntryID = _JournalEntryID
                End If
            ElseIf JE Is Nothing AndAlso Not IsNew AndAlso _JournalEntryID > 0 Then
                General.JournalEntry.DoDelete(_JournalEntryID)
                _JournalEntryID = 0
                obj.JournalEntryID = 0
            End If

            If IsNew Then
                _ID = obj.Save(_OperationalLimit.FinancialDataCanChange, _
                    _OperationalLimit.FinancialDataCanChange, False)
            Else
                obj.Save(_OperationalLimit.FinancialDataCanChange, _
                    _OperationalLimit.FinancialDataCanChange, False)
            End If

            _Items.Update(Me)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            If IsNew Then _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate

            MarkOld()

            ReloadLimitations()

        End Sub

        Private Function GetPersistenceObj() As ComplexOperationPersistenceObject

            Dim obj As ComplexOperationPersistenceObject
            If IsNew Then
                obj = ComplexOperationPersistenceObject.NewComplexOperationPersistenceObject
            Else
                obj = ComplexOperationPersistenceObject.GetComplexOperationPersistenceObject(_ID, False)
            End If

            obj.AccountOperation = 0
            obj.GoodsID = 0
            obj.WarehouseID = _WarehouseID
            obj.SecondaryWarehouse = Nothing
            obj.OperationType = GoodsComplexOperationType.Inventorization
            obj.Content = _Content
            obj.DocNo = _DocumentNumber
            obj.JournalEntryID = _JournalEntryID
            obj.OperationDate = _Date

            Return obj

        End Function


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            Dim OperationToDelete As GoodsComplexOperationInventorization _
                = New GoodsComplexOperationInventorization
            OperationToDelete.DataPortal_Fetch(DirectCast(criteria, Criteria))

            If Not OperationToDelete._OperationalLimit.FinancialDataCanChange Then _
                Throw New Exception("Klaida. Negalima ištrinti prekių " _
                    & "inventorizacijos operacijos:" & vbCrLf & OperationToDelete. _
                    _OperationalLimit.FinancialDataCanChangeExplanation)

            If OperationToDelete.JournalEntryID > 0 Then IndirectRelationInfoList. _
                CheckIfJournalEntryCanBeDeleted(OperationToDelete.JournalEntryID, _
                DocumentType.GoodsInventorization)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            ComplexOperationPersistenceObject.DeleteConsignmentDiscards(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.DeleteConsignments(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.DeleteOperations(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.Delete(DirectCast(criteria, Criteria).ID)

            If OperationToDelete.JournalEntryID > 0 Then General.JournalEntry. _
                DoDelete(OperationToDelete.JournalEntryID)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub


        Private Function GetLastInventorizationDate(ByVal OperationWarehouseID As Integer) As Date

            Dim myComm As New SQLCommand("FetchLastInventorizationDate")
            myComm.AddParam("?WD", OperationWarehouseID)
            myComm.AddParam("?CD", _ID)

            Dim LastInventorizationDate As Date = Date.MinValue

            Using myData As DataTable = myComm.Fetch

                If Not myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Sandėlis, kurio ID=" & OperationWarehouseID.ToString & ", nerastas.")

                _WarehouseID = CIntSafe(myData.Rows(0).Item(0), 0)
                _WarehouseName = CStrSafe(myData.Rows(0).Item(1))
                _WarehouseAccount = CLongSafe(myData.Rows(0).Item(2), 0)

                LastInventorizationDate = CDateSafe(myData.Rows(0).Item(3), Date.MinValue)

            End Using

            Return LastInventorizationDate

        End Function

        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry
            If IsNew OrElse Not _JournalEntryID > 0 Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.GoodsInventorization)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, DocumentType.GoodsInventorization)
            End If

            result.Content = _Content
            result.Date = _Date
            result.DocNumber = _DocumentNumber

            If _OperationalLimit.BaseValidator.FinancialDataCanChange Then

                Dim FullBookEntryList As BookEntryInternalList = BookEntryInternalList. _
                    NewBookEntryInternalList(BookEntryType.Debetas)

                For Each i As GoodsInventorizationItem In _Items
                    FullBookEntryList.AddRange(i.GetTotalBookEntryList(Me))
                Next

                FullBookEntryList.Aggregate()

                If Not FullBookEntryList.Count > 0 Then Return Nothing

                result.DebetList.Clear()
                result.CreditList.Clear()

                result.DebetList.LoadBookEntryListFromInternalList(FullBookEntryList, False)
                result.CreditList.LoadBookEntryListFromInternalList(FullBookEntryList, False)

            ElseIf result.IsNew OrElse Not result.ID > 0 Then

                Return Nothing

            End If

            If Not result.IsValid Then Throw New Exception("Klaida. Nepavyko generuoti " _
                & "bendrojo žurnalo įrašo.")

            Return result

        End Function

        Private Sub CheckIfCanUpdate()

            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Prekių inventorizacijos operacijoje " _
                & "yra klaidų: " & BrokenRulesCollection.ToString)

            Dim exceptionText As String = ""

            If IsNew Then
                exceptionText = AddWithNewLine(exceptionText, _Items.CheckIfCanUpdate( _
                    Nothing, _Date, _WarehouseID, False), False)
            Else
                Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _Date)
                    exceptionText = AddWithNewLine(exceptionText, _Items.CheckIfCanUpdate( _
                        myData, _Date, _WarehouseID, False), False)
                End Using
            End If

            If Not String.IsNullOrEmpty(exceptionText.Trim.Trim) Then _
                Throw New Exception(exceptionText.Trim)

        End Sub

        Private Sub ReloadLimitations()

            Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _Date)
                _Items.ReloadLimitations(myData, _Date, _WarehouseID)
            End Using

            _OperationalLimit = ComplexChronologicValidator.GetComplexChronologicValidator( _
                _ID, _OperationalLimit.CurrentOperationDate, ConvertEnumHumanReadable( _
                GoodsOperationType.Inventorization), _OperationalLimit.BaseValidator, _Items.GetLimitations())

            ValidationRules.CheckRules()

        End Sub

        Private Sub PrepareOperationConsignments()
            _Items.PrepareOperationConsignments(Me)
        End Sub

#End Region

    End Class

End Namespace