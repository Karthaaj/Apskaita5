Namespace Goods

    <Serializable()> _
    Public Class GoodsComplexOperationPriceCut
        Inherits BusinessBase(Of GoodsComplexOperationPriceCut)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _JournalEntryID As Integer = 0
        Private _OperationalLimit As ComplexChronologicValidator = Nothing
        Private _DocumentNumber As String = ""
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _Description As String = ""
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _Items As GoodsPriceCutItemList

        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _ItemsSortedList As Csla.SortedBindingList(Of GoodsPriceCutItem) = Nothing


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

        Public Property [Date]() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Date
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Date)
                CanWriteProperty(True)
                If Not _OperationalLimit.FinancialDataCanChange Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _Date.Date <> value.Date Then
                    _Date = value.Date
                    PropertyHasChanged()
                    _Items.ResetValuesInWarehouse()
                End If
            End Set
        End Property

        Public ReadOnly Property OldDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldDate
            End Get
        End Property

        Public Property Description() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Description.Trim
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If value Is Nothing Then value = ""
                If _Description.Trim <> value.Trim Then
                    _Description = value.Trim
                    PropertyHasChanged()
                End If
            End Set
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

        Public ReadOnly Property Items() As GoodsPriceCutItemList
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
                    OrElse Not String.IsNullOrEmpty(_Description.Trim) _
                    OrElse _Items.Count > 0)
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


        Public Sub AddNewPriceCutItem(ByVal item As GoodsPriceCutItem)

            If _Items.ContainsGoods(item.GoodsInfo.ID) Then Throw New Exception( _
                "Klaida. Eilutė prekei '" & item.GoodsName & "' jau yra.")
            If Not _OperationalLimit.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Finansiniai operacijos duomenys negali būti keičiami:" _
                & vbCrLf & _OperationalLimit.FinancialDataCanChangeExplanation)

            _Items.Add(item)

            ValidationRules.CheckRules()

        End Sub

        Public Sub RefreshValuesInWarehouse(ByVal values As GoodsPriceInWarehouseItemList)
            If Not _OperationalLimit.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Finansiniai operacijos duomenys negali būti keičiami:" _
                & vbCrLf & _OperationalLimit.FinancialDataCanChangeExplanation)
            _Items.RefreshValuesInWarehouse(values)
        End Sub

        Public Function GetGoodsPriceInWarehouseParam() As GoodsPriceInWarehouseParam()
            Return _Items.GetGoodsPriceInWarehouseParams
        End Function


        Public Overrides Function Save() As GoodsComplexOperationPriceCut

            If IsNew AndAlso Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            If Not IsNew AndAlso Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            If Not _Items.Count > 0 Then Throw New Exception("Klaida. Neįvesta nė viena eilutė.")

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf & Me.GetAllBrokenRules)
            Return MyBase.Save

        End Function

        

        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return "Goods.GoodsComplexOperationPriceCut"
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("DocumentNumber", "dokumento numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Description", "operacijos aprašymas"))

            ValidationRules.AddRule(AddressOf DateValidation, New Validation.RuleArgs("Date"))

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property Date is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function DateValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsComplexOperationPriceCut = DirectCast(target, GoodsComplexOperationPriceCut)

            If Not ValObj._OperationalLimit Is Nothing AndAlso _
                Not ValObj._OperationalLimit.ValidateOperationDate(ValObj._Date, _
                e.Description, e.Severity) Then Return False

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Goods.GoodsOperationPriceCut2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationPriceCut1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationPriceCut2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationPriceCut3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationPriceCut3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewGoodsComplexOperationPriceCut() As GoodsComplexOperationPriceCut
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            Dim result As New GoodsComplexOperationPriceCut
            result._Items = GoodsPriceCutItemList.NewGoodsPriceCutItemList
            Dim BaseValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                NewSimpleChronologicValidator(ConvertEnumHumanReadable(GoodsComplexOperationType.BulkPriceCut))
            result._OperationalLimit = ComplexChronologicValidator.NewComplexChronologicValidator( _
                ConvertEnumHumanReadable(GoodsComplexOperationType.BulkPriceCut), BaseValidator, Nothing)
            result.ValidationRules.CheckRules()
            Return result
        End Function

        Public Shared Function GetGoodsComplexOperationPriceCut(ByVal nID As Integer) As GoodsComplexOperationPriceCut
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")
            Return DataPortal.Fetch(Of GoodsComplexOperationPriceCut)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteGoodsComplexOperationPriceCut(ByVal id As Integer)
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

            Dim obj As ComplexOperationPersistenceObject = ComplexOperationPersistenceObject. _
                GetComplexOperationPersistenceObject(criteria.ID, True)

            If obj.OperationType <> GoodsComplexOperationType.BulkPriceCut Then _
                Throw New Exception("Klaida. Kompleksinė operacija, kurios ID=" _
                & obj.ID.ToString & ", yra ne prekių nukainojimas, o " _
                & ConvertEnumHumanReadable(obj.OperationType))

            _ID = obj.ID
            _JournalEntryID = obj.JournalEntryID
            _DocumentNumber = obj.DocNo
            _Date = obj.OperationDate
            _OldDate = _Date
            _Description = obj.Content
            _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate

            Dim BaseValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                GetSimpleChronologicValidator(_ID, _Date, ConvertEnumHumanReadable( _
                GoodsComplexOperationType.BulkPriceCut))

            Dim myComm As New SQLCommand("FetchGoodsComplexOperationPriceCut")
            myComm.AddParam("?CD", criteria.ID)

            Using myData As DataTable = myComm.Fetch
                Using LimitationsDataSource As DataTable = OperationalLimitList. _
                    GetDataSourceForComplexOperation(_ID, _Date)
                    _Items = GoodsPriceCutItemList.GetGoodsPriceCutItemList(myData, _
                        LimitationsDataSource, BaseValidator)
                End Using
            End Using

            _OperationalLimit = ComplexChronologicValidator.GetComplexChronologicValidator( _
                _ID, _Date, ConvertEnumHumanReadable(GoodsOperationType.PriceCut), _
                BaseValidator, _Items.GetLimitations())

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()
            CheckIfCanUpdate()
            DoSave()
        End Sub

        Protected Overrides Sub DataPortal_Update()
            ComplexOperationPersistenceObject.CheckIfUpdateDateChanged(_ID, _UpdateDate)
            CheckIfCanUpdate()
            DoSave()
        End Sub

        Private Sub DoSave()

            Dim obj As ComplexOperationPersistenceObject = GetPersistenceObj()

            Dim JE As General.JournalEntry = GetJournalEntry()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            JE = JE.SaveServerSide
            If IsNew Then
                _JournalEntryID = JE.ID
                obj.JournalEntryID = _JournalEntryID
            End If

            If IsNew Then
                _ID = obj.Save(True, True, True)
            Else
                obj.Save(_OperationalLimit.FinancialDataCanChange, True, True)
            End If

            _Items.Update(Me)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            If IsNew Then _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate
            _OldDate = _Date

            MarkOld()

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
            obj.Warehouse = Nothing
            obj.SecondaryWarehouse = Nothing
            obj.OperationType = GoodsComplexOperationType.BulkPriceCut
            obj.Content = _Description
            obj.DocNo = _DocumentNumber
            obj.JournalEntryID = _JournalEntryID
            obj.OperationDate = _Date

            Return obj

        End Function


        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            Dim OperationToDelete As GoodsComplexOperationPriceCut = New GoodsComplexOperationPriceCut
            OperationToDelete.DataPortal_Fetch(DirectCast(criteria, Criteria))

            If Not OperationToDelete._OperationalLimit.FinancialDataCanChange Then _
                Throw New Exception("Klaida. Negalima ištrinti prekių " _
                    & "nukainojimo urmu operacijos:" & vbCrLf & OperationToDelete. _
                    _OperationalLimit.FinancialDataCanChangeExplanation)

            IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted( _
                OperationToDelete.JournalEntryID, DocumentType.GoodsRevalue)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            ComplexOperationPersistenceObject.DeleteOperations(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.Delete(DirectCast(criteria, Criteria).ID)

            General.JournalEntry.DoDelete(OperationToDelete.JournalEntryID)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub


        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry
            If IsNew OrElse Not _JournalEntryID > 0 Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.GoodsRevalue)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, DocumentType.GoodsRevalue)
            End If

            result.Content = _Description
            result.Date = _Date
            result.DocNumber = _DocumentNumber

            If _OperationalLimit.FinancialDataCanChange Then

                Dim FullBookEntryList As BookEntryInternalList = BookEntryInternalList. _
                NewBookEntryInternalList(BookEntryType.Debetas)

                For Each i As GoodsPriceCutItem In _Items
                    FullBookEntryList.AddRange(i.GetTotalBookEntryList())
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

        Private Sub CheckIfCanUpdate()

            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Prekių vidinio judėjimo operacijoje " _
                & "yra klaidų: " & BrokenRulesCollection.ToString)

            Dim exceptionText As String = ""

            If IsNew Then
                exceptionText = AddWithNewLine(exceptionText, _Items.CheckIfCanUpdate( _
                    Nothing, False, _Date, _OperationalLimit.BaseValidator), False)
            Else
                Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _OldDate)
                    exceptionText = AddWithNewLine(exceptionText, _Items.CheckIfCanUpdate( _
                        myData, False, _Date, _OperationalLimit.BaseValidator), False)
                End Using
            End If

            If Not String.IsNullOrEmpty(exceptionText.Trim.Trim) Then _
                Throw New Exception(exceptionText.Trim)

        End Sub

        Private Sub ReloadLimitations()

            Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _Date)
                _Items.ReloadLimitations(myData, _OperationalLimit.BaseValidator)
            End Using

            _OperationalLimit = ComplexChronologicValidator.GetComplexChronologicValidator( _
                _ID, _OperationalLimit.CurrentOperationDate, ConvertEnumHumanReadable( _
                GoodsOperationType.PriceCut), _OperationalLimit.BaseValidator, _
                _Items.GetLimitations())

            ValidationRules.CheckRules()

        End Sub

#End Region

    End Class

End Namespace