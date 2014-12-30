Namespace Goods

    <Serializable()> _
Public Class GoodsOperationDiscard
        Inherits BusinessBase(Of GoodsOperationDiscard)

#Region " Business Methods "

        Private _GUID As Guid = Guid.NewGuid
        Private _ID As Integer = 0
        Private _JournalEntryID As Integer = 0
        Private _JournalEntryType As DocumentType = DocumentType.GoodsWriteOff
        Private _GoodsInfo As GoodsSummary = Nothing
        Private _OperationLimitations As OperationalLimitList = Nothing
        Private _ComplexOperationID As Integer = 0
        Private _ComplexOperationType As GoodsComplexOperationType = GoodsComplexOperationType.InternalTransfer
        Private _ComplexOperationHumanReadable As String = ""
        Private _Warehouse As WarehouseInfo = Nothing
        Private _OldWarehouseID As Integer = 0
        Private _DocumentNumber As String = ""
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _Description As String = ""
        Private _Ammount As Double = 0
        Private _UnitCost As Double = 0
        Private _TotalCost As Double = 0
        Private _AccountGoodsDiscardCosts As Long = 0
        Private _NormativeUnitValue As Double = 0
        Private _NormativeTotalValue As Double = 0

        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now


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

        Public ReadOnly Property JournalEntryType() As DocumentType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _JournalEntryType
            End Get
        End Property

        Public ReadOnly Property GoodsInfo() As GoodsSummary
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _GoodsInfo
            End Get
        End Property

        Public ReadOnly Property OperationLimitations() As OperationalLimitList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OperationLimitations
            End Get
        End Property

        Public Property Warehouse() As WarehouseInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Warehouse
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WarehouseInfo)
                CanWriteProperty(True)
                If WarehouseIsReadOnly Then Exit Property
                If Not (_Warehouse Is Nothing AndAlso value Is Nothing) AndAlso _
                    (value Is Nothing OrElse _Warehouse Is Nothing OrElse _Warehouse.ID <> value.ID) Then
                    _Warehouse = value
                    _OperationLimitations.SetWarehouse(_Warehouse)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property OldWarehouseID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldWarehouseID
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
                If DocumentNumberIsReadOnly Then Exit Property
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
                If DateIsReadOnly Then Exit Property
                If _Date.Date <> value.Date Then
                    _Date = value.Date
                    PropertyHasChanged()
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
                If DescriptionIsReadOnly Then Exit Property
                If value Is Nothing Then value = ""
                If _Description.Trim <> value.Trim Then
                    _Description = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property Ammount() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Ammount, 6)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If AmmountIsReadOnly Then Exit Property
                If CRound(_Ammount, 6) <> CRound(value, 6) Then
                    _Ammount = CRound(value)
                    PropertyHasChanged()
                    _UnitCost = 0
                    _TotalCost = 0
                    PropertyHasChanged("UnitCost")
                    PropertyHasChanged("TotalCost")
                End If
            End Set
        End Property

        Public Property AccountGoodsDiscardCosts() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountGoodsDiscardCosts
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If AccountGoodsDiscardCostsIsReadOnly Then Exit Property
                If _AccountGoodsDiscardCosts <> value Then
                    _AccountGoodsDiscardCosts = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property UnitCost() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_UnitCost, 6)
            End Get
        End Property

        Public ReadOnly Property TotalCost() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalCost)
            End Get
        End Property

        Public ReadOnly Property NormativeUnitValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_NormativeUnitValue, 6)
            End Get
        End Property

        Public ReadOnly Property NormativeTotalValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_NormativeTotalValue)
            End Get
        End Property

        Public ReadOnly Property ComplexOperationID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ComplexOperationID
            End Get
        End Property

        Public ReadOnly Property ComplexOperationType() As GoodsComplexOperationType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ComplexOperationType
            End Get
        End Property

        Public ReadOnly Property ComplexOperationHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ComplexOperationHumanReadable.Trim
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


        Public ReadOnly Property WarehouseIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _OperationLimitations.FinancialDataCanChange _
                    OrElse (Not Me.IsChild AndAlso Not Me.IsNew AndAlso _ComplexOperationID > 0)
            End Get
        End Property

        Public ReadOnly Property DocumentNumberIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not Me.IsChild AndAlso Not Me.IsNew AndAlso _ComplexOperationID > 0
            End Get
        End Property

        Public ReadOnly Property DateIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Me.IsChild OrElse (Not Me.IsNew AndAlso _ComplexOperationID > 0)
            End Get
        End Property

        Public ReadOnly Property DescriptionIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not Me.IsChild AndAlso Not Me.IsNew AndAlso _ComplexOperationID > 0
            End Get
        End Property

        Public ReadOnly Property AmmountIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _OperationLimitations.FinancialDataCanChange _
                    OrElse (Not Me.IsChild AndAlso Not Me.IsNew AndAlso _ComplexOperationID > 0)
            End Get
        End Property

        Public ReadOnly Property AccountGoodsDiscardCostsIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _OperationLimitations.FinancialDataCanChange _
                    OrElse (Not Me.IsChild AndAlso Not Me.IsNew AndAlso _ComplexOperationID > 0)
            End Get
        End Property


        Public Sub ReloadCostInfo(ByVal CostInfo As GoodsCostItem)

            If _GoodsInfo.AccountingMethod = GoodsAccountingMethod.Periodic Then Throw New Exception( _
                "Klaida. Šiai prekei taikoma periodinė apskaita. Jos partijų vertės neskaičiuojamos.")
            If CostInfo.GoodsID <> _GoodsInfo.ID Then Throw New Exception( _
                "Klaida. Nesutampa prekių ID ir prekių partijos vertės ID.")
            If Not _Warehouse Is Nothing AndAlso CostInfo.WarehouseID <> _Warehouse.ID Then _
                Throw New Exception("Klaida. Nesutampa prekių ir prekių partijos sandėliai.")
            If CostInfo.Amount <> CRound(_Ammount, 6) Then _
                Throw New Exception("Klaida. Nesutampa nurodytas prekių kiekis " & _
                "ir prekių kiekis, kuriam paskaičiuota vertė.")
            If CostInfo.ValuationMethod <> _GoodsInfo.ValuationMethod Then Throw New Exception( _
                "Klaida. Nesutampa prekių ir prekių partijos vertinimo metodas.")
            If Not _OperationLimitations.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Operacijos su preke """ & _GoodsInfo.Name _
                & """ finansiniai duomenys, įskaitant savikainą, negali būti keičiami. Priežastis:" _
                & vbCrLf & _OperationLimitations.FinancialDataCanChangeExplanation)

            _UnitCost = CostInfo.UnitCosts
            _TotalCost = CostInfo.TotalCosts
            PropertyHasChanged("UnitCost")
            PropertyHasChanged("TotalCost")

        End Sub

        Friend Sub SetDate(ByVal value As Date)
            If Not IsChild Then Throw New InvalidOperationException( _
                "Klaida. Metodas SetDate gali būti naudojamas tik child objektui.")
            If _Date.Date <> value.Date Then
                _Date = value.Date
                PropertyHasChanged("Date")
            End If
        End Sub

        Friend Sub SetNormativeCosts(ByVal nNormativeUnitValue As Double, ByVal nNormativeTotalValue As Double)

            If Not IsChild Then Throw New InvalidOperationException( _
                "Klaida. Metodas SetDate gali būti naudojamas tik child objektui.")

            If Not _OperationLimitations.FinancialDataCanChange Then Exit Sub

            If CRound(_NormativeUnitValue, 6) <> CRound(nNormativeUnitValue, 6) Then
                _NormativeUnitValue = CRound(nNormativeUnitValue, 6)
                PropertyHasChanged("NormativeUnitValue")
            End If

            If CRound(_NormativeTotalValue) <> CRound(nNormativeTotalValue) Then
                _NormativeTotalValue = CRound(nNormativeTotalValue)
                PropertyHasChanged("NormativeTotalValue")
            End If

        End Sub


        Public Overrides Function Save() As GoodsOperationDiscard

            If (Me.IsNew AndAlso Not CanAddObject()) OrElse (Not Me.IsNew AndAlso Not CanEditObject()) Then _
                Throw New System.Security.SecurityException("Klaida. Jūsų teisių nepakanka šiai operacijai.")
            If IsChild OrElse _ComplexOperationID > 0 Then Throw New InvalidOperationException( _
                "Klaida. Ši operacija yra sudedamoji kito dokumento dalis ir negali būti keičiama atskirai.")

            Me.ValidationRules.CheckRules()
            If Not Me.IsValid Then Throw New Exception("Prekių operacijos duomenyse yra klaidų: " & _
                Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error))

            Return MyBase.Save()

        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _GUID
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Ammount", "nurašomas kiekis"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("Warehouse", "sandėlis", "ID"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "OperationLimitations"))

            ValidationRules.AddRule(AddressOf UnitCostValidation, New Validation.RuleArgs("UnitCost"))
            ValidationRules.AddRule(AddressOf TotalCostValidation, New Validation.RuleArgs("TotalCost"))
            ValidationRules.AddRule(AddressOf AccountGoodsDiscardCostsValidation, _
                New Validation.RuleArgs("AccountGoodsDiscardCosts"))

            ValidationRules.AddDependantProperty("Warehouse", "Date", False)

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property UnitCost is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function UnitCostValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsOperationDiscard = DirectCast(target, GoodsOperationDiscard)

            If ValObj._GoodsInfo.AccountingMethod = GoodsAccountingMethod.Persistent AndAlso _
                Not ValObj._UnitCost > 0 Then
                e.Description = "Nenurodyta vieneto savikaina."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property TotalCost is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function TotalCostValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsOperationDiscard = DirectCast(target, GoodsOperationDiscard)

            If ValObj._GoodsInfo.AccountingMethod = GoodsAccountingMethod.Persistent AndAlso _
                Not ValObj._TotalCost > 0 Then
                e.Description = "Nenurodyta bendra savikaina."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the value of property AccountGoodsDiscardCosts is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountGoodsDiscardCostsValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsOperationDiscard = DirectCast(target, GoodsOperationDiscard)

            If ValObj._GoodsInfo.AccountingMethod = GoodsAccountingMethod.Persistent AndAlso _
                Not ValObj._AccountGoodsDiscardCosts > 0 Then
                e.Description = "Nenurodyta savikainos nurašymo sąskaita."
                e.Severity = Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Goods.GoodsOperationDiscard2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationDiscard2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationDiscard1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationDiscard3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationDiscard3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewGoodsOperationDiscard(ByVal GoodsID As Integer) As GoodsOperationDiscard
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            Return DataPortal.Create(Of GoodsOperationDiscard)(New Criteria(GoodsID))
        End Function

        Friend Shared Function NewGoodsOperationDiscardChild(ByVal summary As GoodsSummary, _
            ByVal parentValidator As IChronologicValidator) As GoodsOperationDiscard
            Return New GoodsOperationDiscard(summary, parentValidator)
        End Function

        Friend Shared Function NewGoodsOperationDiscardChild(ByVal GoodsID As Integer, _
            ByVal parentValidator As IChronologicValidator) As GoodsOperationDiscard
            Return New GoodsOperationDiscard(GoodsID, parentValidator)
        End Function

        Public Shared Function GetGoodsOperationDiscard(ByVal id As Integer) As GoodsOperationDiscard
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")
            Return DataPortal.Fetch(Of GoodsOperationDiscard)(New Criteria(id))
        End Function

        Friend Shared Function GetGoodsOperationDiscardChild(ByVal obj As OperationPersistenceObject, _
            ByVal parentValidator As IChronologicValidator, ByVal LimitationsDataSource As DataTable) As GoodsOperationDiscard
            Return New GoodsOperationDiscard(obj, parentValidator, LimitationsDataSource)
        End Function

        Public Shared Sub DeleteGoodsOperationDiscard(ByVal id As Integer)
            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenų ištrynimui.")
            DataPortal.Delete(New Criteria(id))
        End Sub

        Friend Sub DeleteGoodsOperationDiscardChild()
            If Not IsChild Then Throw New InvalidOperationException( _
                "Method DeleteGoodsOperationDiscardChild is only applicable to child object.")
            DoDelete(True)
        End Sub



        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal summary As GoodsSummary, ByVal parentValidator As IChronologicValidator)
            ' require use of factory methods
            MarkAsChild()
            Create(summary, parentValidator)
        End Sub

        Private Sub New(ByVal GoodsID As Integer, ByVal parentValidator As IChronologicValidator)
            ' require use of factory methods
            MarkAsChild()
            Create(GoodsID, parentValidator)
        End Sub

        Private Sub New(ByVal obj As OperationPersistenceObject, _
            ByVal parentValidator As IChronologicValidator, _
            ByVal LimitationsDataSource As DataTable)
            ' require use of factory methods
            MarkAsChild()
            Fetch(obj, parentValidator, LimitationsDataSource)
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private mId As Integer
            Public ReadOnly Property Id() As Integer
                Get
                    Return mId
                End Get
            End Property
            Public Sub New(ByVal id As Integer)
                mId = id
            End Sub
        End Class

        <NonSerialized(), NotUndoable()> _
        Private DiscardList As ConsignmentDiscardPersistenceObjectList = Nothing

        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
            Create(criteria.Id, Nothing)
        End Sub

        Private Sub Create(ByVal nGoodsID As Integer, ByVal parentValidator As IChronologicValidator)
            Create(GoodsSummary.GetGoodsSummary(nGoodsID), parentValidator)
        End Sub

        Private Sub Create(ByVal summary As GoodsSummary, ByVal parentValidator As IChronologicValidator)

            _GoodsInfo = summary
            _Warehouse = _GoodsInfo.DefaultWarehouse
            Dim wid As Integer = 0
            If Not _Warehouse Is Nothing AndAlso _Warehouse.ID > 0 Then wid = _Warehouse.ID
            _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                summary.ID, 0, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                _GoodsInfo.AccountingMethod, _GoodsInfo.Name, Today, wid, parentValidator)

            MarkNew()

            ValidationRules.CheckRules()

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
            Fetch(criteria.Id)
        End Sub

        Private Sub Fetch(ByVal nOperationID As Integer)
            Dim obj As OperationPersistenceObject = OperationPersistenceObject. _
                GetOperationPersistenceObject(nOperationID, True)
            Fetch(obj, Nothing, Nothing)
        End Sub

        Private Sub Fetch(ByVal obj As OperationPersistenceObject, _
            ByVal parentValidator As IChronologicValidator, _
            ByVal LimitationsDataSource As DataTable)

            If obj.OperationType <> GoodsOperationType.Discard Then Throw New Exception( _
                "Operacijos, kurios ID=" & obj.ID.ToString & " tipas yra ne " & _
                "nurašymas, o " & ConvertEnumHumanReadable(obj.OperationType) & ".")

            _ID = obj.ID
            _ComplexOperationID = obj.ComplexOperationID
            _ComplexOperationType = obj.ComplexOperationType
            _ComplexOperationHumanReadable = obj.ComplexOperationHumanReadable
            _Date = obj.OperationDate
            _OldDate = _Date
            _Description = obj.Content
            _Ammount = -obj.Amount
            _UnitCost = obj.UnitValue
            _TotalCost = -obj.TotalValue
            _Warehouse = obj.Warehouse
            _OldWarehouseID = Warehouse.ID
            _JournalEntryID = obj.JournalEntryID
            If _JournalEntryID > 0 Then _JournalEntryType = obj.JournalEntryType
            _DocumentNumber = obj.DocNo
            _AccountGoodsDiscardCosts = obj.AccountOperation
            _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate
            _GoodsInfo = obj.GoodsInfo

            If LimitationsDataSource Is Nothing Then
                _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                    obj.GoodsID, obj.ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                    _GoodsInfo.AccountingMethod, _GoodsInfo.Name, obj.OperationDate, _
                    obj.WarehouseID, parentValidator)
            Else
                _OperationLimitations = OperationalLimitList.GetOperationalLimitList(obj.GoodsID, _
                    obj.ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                    _GoodsInfo.AccountingMethod, _GoodsInfo.Name, obj.OperationDate, _
                    obj.WarehouseID, parentValidator, LimitationsDataSource)
            End If

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            CheckIfCanUpdate(Nothing, Nothing, True)

            GetDiscardList()

            DoSave(True)

            _OperationLimitations = OperationalLimitList.GetOperationalLimitList(_GoodsInfo.ID, _
                _ID, GoodsOperationType.Acquisition, _GoodsInfo.ValuationMethod, _
                _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, Nothing)

        End Sub

        Protected Overrides Sub DataPortal_Update()

            CheckIfCanUpdate(Nothing, Nothing, True)

            OperationPersistenceObject.CheckIfUpdateDateChanged(_ID, _UpdateDate)

            GetDiscardList()

            DoSave(True)

            _OperationLimitations = OperationalLimitList.GetOperationalLimitList(_GoodsInfo.ID, _
                _ID, GoodsOperationType.Acquisition, _GoodsInfo.ValuationMethod, _
                _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, Nothing)

        End Sub

        Friend Sub SaveChild(ByVal ParentJournalEntryID As Integer, _
            ByVal ParentComplexOperationID As Integer, ByVal ParentDate As Date, _
            ByVal ParentDescription As String, ByVal ParentDocNo As String)
            _Date = ParentDate
            _JournalEntryID = ParentJournalEntryID
            _Description = ParentDescription
            _DocumentNumber = ParentDocNo
            If ParentComplexOperationID > 0 Then _ComplexOperationID = ParentComplexOperationID
            DoSave(False)
        End Sub

        Private Sub DoSave(ByVal SaveJournalEntry As Boolean)

            If DiscardList Is Nothing AndAlso _GoodsInfo.AccountingMethod <> GoodsAccountingMethod.Periodic Then _
                Throw New InvalidOperationException("Klaida. Prieš išsaugant GoodsOperationDiscard " _
                & "būtina iškviesti GetDiscardList metodą.")

            Dim obj As OperationPersistenceObject = GetPersistenceObj()

            Dim JE As General.JournalEntry = Nothing
            If SaveJournalEntry AndAlso _GoodsInfo.AccountingMethod <> _
                GoodsAccountingMethod.Periodic Then JE = GetJournalEntryForDocument()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            If SaveJournalEntry AndAlso _GoodsInfo.AccountingMethod <> GoodsAccountingMethod.Periodic _
                AndAlso Not JE Is Nothing Then
                JE = JE.SaveServerSide()
                If IsNew Then
                    _JournalEntryID = JE.ID
                    obj.JournalEntryID = _JournalEntryID
                End If
            End If

            If IsNew Then
                _ID = obj.Save(_OperationLimitations.FinancialDataCanChange)
            Else
                obj.Save(_OperationLimitations.FinancialDataCanChange)
            End If

            If _OperationLimitations.FinancialDataCanChange AndAlso _
                _GoodsInfo.AccountingMethod <> GoodsAccountingMethod.Periodic Then
                DiscardList.Update(_ID)
            End If

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            If IsNew Then _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate
            _OldDate = _Date
            _OldWarehouseID = _Warehouse.ID

            MarkOld()

        End Sub

        Private Function GetPersistenceObj() As OperationPersistenceObject

            Dim obj As OperationPersistenceObject
            If IsNew Then
                obj = OperationPersistenceObject.NewOperationPersistenceObject
            Else
                obj = OperationPersistenceObject.GetOperationPersistenceObject(_ID, False)
            End If

            If _GoodsInfo.AccountingMethod = GoodsAccountingMethod.Persistent Then

                obj.UnitValue = _UnitCost
                obj.AccountGeneral = -_TotalCost
                obj.AccountOperationValue = _TotalCost
                obj.AmountInWarehouse = -_Ammount

            Else

                obj.UnitValue = _NormativeUnitValue
                obj.AccountOperationValue = -_NormativeTotalValue
                obj.AmountInWarehouse = 0

            End If

            obj.AmountInPurchases = 0
            obj.AccountOperation = _AccountGoodsDiscardCosts
            obj.Amount = -_Ammount
            obj.Content = _Description
            obj.DocNo = _DocumentNumber
            obj.GoodsID = _GoodsInfo.ID
            obj.JournalEntryID = _JournalEntryID
            obj.OperationDate = _Date
            obj.OperationType = GoodsOperationType.Discard
            obj.TotalValue = -_TotalCost
            obj.Warehouse = _Warehouse
            obj.WarehouseID = _Warehouse.ID
            obj.ComplexOperationID = _ComplexOperationID

            Return obj

        End Function

        Private Function GetJournalEntryForDocument() As General.JournalEntry

            If _GoodsInfo.AccountingMethod = GoodsAccountingMethod.Periodic Then _
                Throw New InvalidOperationException("Klaida. Bendrojo žurnalo įrašas " _
                & "negali būti sukurtas prekių nurašymui taikant periodinį apskaitos metodą.")

            Dim result As General.JournalEntry = Nothing

            If IsNew Then

                If _Date.Date <= GetCurrentCompany.LastClosingDate.Date Then Throw New Exception( _
                    "Klaida. Neleidžiama koreguoti operacijų po uždarymo (" _
                    & GetCurrentCompany.LastClosingDate & ").")

                result = General.JournalEntry.NewJournalEntryChild(DocumentType.GoodsWriteOff)

            Else

                result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, _
                    DocumentType.GoodsWriteOff)

            End If

            result.Date = _Date.Date
            result.Person = Nothing
            result.Content = _Description & " (prekių nurašymas)"
            result.DocNumber = _DocumentNumber

            Dim CommonBookEntryList As BookEntryInternalList = GetTotalBookEntryList()

            result.DebetList.LoadBookEntryListFromInternalList(CommonBookEntryList, False)
            result.CreditList.LoadBookEntryListFromInternalList(CommonBookEntryList, False)

            If Not result.IsValid Then Throw New Exception("Klaida. Nekorektiškai generuotas " _
                & "bendrojo žurnalo įrašas: " & result.GetAllBrokenRules)

            Return result

        End Function



        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)

            Dim OperationToDelete As GoodsOperationDiscard = New GoodsOperationDiscard
            OperationToDelete.DataPortal_Fetch(criteria)

            If OperationToDelete.ComplexOperationID > 0 Then Throw New Exception( _
                "Klaida. Ši operacija yra sudedamoji dalis kompleksinės operacijos - " _
                & ConvertEnumHumanReadable(OperationToDelete.ComplexOperationType) _
                & ". Ją pašalinti galima tik kartu su komplesine operacija.")

            If OperationToDelete.JournalEntryType = DocumentType.InvoiceMade Then _
                Throw New Exception("Klaida. Ši operacija yra sudedamoji dalis išrašytos " _
                & "sąskaitos faktūros. Ją pašalinti galima tik kartu su sąskaita faktūra.")

            If OperationToDelete.JournalEntryType = DocumentType.InvoiceReceived Then _
                Throw New Exception("Klaida. Ši operacija yra sudedamoji dalis gautos " _
                & "sąskaitos faktūros. Ją pašalinti galima tik kartu su sąskaita faktūra.")

            If Not OperationToDelete._OperationLimitations.FinancialDataCanChange Then _
                Throw New Exception("Klaida. " & OperationToDelete._OperationLimitations. _
                FinancialDataCanChangeExplanation)

            If OperationToDelete.JournalEntryID > 0 Then IndirectRelationInfoList. _
                CheckIfJournalEntryCanBeDeleted(OperationToDelete.JournalEntryID, _
                DocumentType.GoodsWriteOff)

            OperationToDelete.DoDelete(False)

        End Sub

        Private Sub DoDelete(ByVal OperationIsChild As Boolean)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            OperationPersistenceObject.DeleteConsignmentDiscards(_ID)

            OperationPersistenceObject.Delete(_ID)

            If Not OperationIsChild AndAlso _JournalEntryID > 0 Then _
                General.JournalEntry.DoDelete(_JournalEntryID)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub


        Friend Function CheckIfCanDelete(ByVal LimitationsDataSource As DataTable, _
            ByVal parentValidator As IChronologicValidator, ByVal ThrowOnInvalid As Boolean) As String

            If IsNew Then Return ""

            If LimitationsDataSource Is Nothing Then
                _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                    _GoodsInfo.ID, _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                    _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, parentValidator)
            Else
                _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                    _GoodsInfo.ID, _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                    _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, _
                    parentValidator, LimitationsDataSource)
            End If

            If Not _OperationLimitations.FinancialDataCanChange Then
                If ThrowOnInvalid Then
                    Throw New Exception("Klaida. Negalima ištrinti prekių '" & _
                        _OperationLimitations.CurrentGoodsName & "' nurašymo operacijos:" _
                        & vbCrLf & _OperationLimitations.FinancialDataCanChangeExplanation)
                Else
                    Return "Klaida. Negalima ištrinti prekių '" & _
                        _OperationLimitations.CurrentGoodsName & "' nurašymo operacijos:" _
                        & vbCrLf & _OperationLimitations.FinancialDataCanChangeExplanation
                End If
            End If

            Return ""

        End Function

        Friend Function CheckIfCanUpdate(ByVal LimitationsDataSource As DataTable, _
            ByVal parentValidator As IChronologicValidator, ByVal ThrowOnInvalid As Boolean) As String

            If IsNew Then

                _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                    _GoodsInfo.ID, _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                    _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _Date, _Warehouse.ID, parentValidator)

            Else

                If LimitationsDataSource Is Nothing Then
                    _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                        _GoodsInfo.ID, _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                        _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, parentValidator)
                Else
                    _OperationLimitations = OperationalLimitList.GetOperationalLimitList( _
                        _GoodsInfo.ID, _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                        _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, _
                        parentValidator, LimitationsDataSource)
                End If

            End If

            ValidationRules.CheckRules()
            If Not IsValid Then
                If ThrowOnInvalid Then
                    Throw New Exception("Prekių '" & _GoodsInfo.Name _
                    & "' nurašymo operacijoje yra klaidų: " & BrokenRulesCollection.ToString)
                Else
                    Return "Prekių '" & _GoodsInfo.Name _
                        & "' nurašymo operacijoje yra klaidų: " & BrokenRulesCollection.ToString
                End If
            End If

            Return ""

        End Function

        Friend Function GetTotalBookEntryList() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
               BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            If _GoodsInfo.AccountingMethod = GoodsAccountingMethod.Periodic Then Return result

            Dim GoodsAccountBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
            GoodsAccountBookEntry.Account = _Warehouse.WarehouseAccount
            GoodsAccountBookEntry.Ammount = _TotalCost

            result.Add(GoodsAccountBookEntry)

            Dim CostsAccountBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
            CostsAccountBookEntry.Account = _AccountGoodsDiscardCosts
            CostsAccountBookEntry.Ammount = _TotalCost

            result.Add(CostsAccountBookEntry)

            Return result

        End Function

        Friend Sub GetDiscardList()

            If _GoodsInfo.AccountingMethod = GoodsAccountingMethod.Periodic Then Exit Sub

            Dim consignements As ConsignmentPersistenceObjectList = _
                ConsignmentPersistenceObjectList.GetConsignmentPersistenceObjectList( _
                _GoodsInfo.ID, _Warehouse.ID, _ID, 0, (_GoodsInfo.ValuationMethod = GoodsValuationMethod.LIFO))

            consignements.RemoveLateEntries(_Date)

            Dim discards As ConsignmentDiscardPersistenceObjectList = _
                ConsignmentDiscardPersistenceObjectList.NewConsignmentDiscardPersistenceObjectList( _
                consignements, _Ammount, _GoodsInfo.Name)

            If Not IsNew Then

                Dim curDiscards As ConsignmentDiscardPersistenceObjectList = _
                    ConsignmentDiscardPersistenceObjectList. _
                    GetConsignmentDiscardPersistenceObjectList(_ID)

                curDiscards.MergeChangedList(discards)

                DiscardList = curDiscards

            Else

                DiscardList = discards

            End If

            _TotalCost = DiscardList.GetTotalValue
            _Ammount = DiscardList.GetTotalAmount
            If Not CRound(_Ammount, 6) > 0 Then Throw New Exception( _
                "Klaida. Kiekis negali būti lygus nuliui metode SetCostValues.")
            _UnitCost = CRound(_TotalCost / _Ammount, 6)

        End Sub

        Friend Sub ReloadLimitations(ByVal LimitationsDataSource As DataTable, _
            ByVal parentValidator As IChronologicValidator)

            If LimitationsDataSource Is Nothing Then Throw New ArgumentNullException( _
                "Klaida. Metodui GoodsOperationTransfer.ReloadLimitations " _
                & "nenurodytas LimitationsDataSource parametras.")
            If IsNew Then Throw New InvalidOperationException("Klaida. " _
                & "Metodas GoodsOperationTransfer.ReloadLimitations gali " _
                & "būti taikomas tik Old objektams.")

            _OperationLimitations = OperationalLimitList.GetOperationalLimitList(_GoodsInfo.ID, _
                _ID, GoodsOperationType.Discard, _GoodsInfo.ValuationMethod, _
                _GoodsInfo.AccountingMethod, _GoodsInfo.Name, _OldDate, _OldWarehouseID, _
                parentValidator, LimitationsDataSource)

            ValidationRules.CheckRules()

        End Sub

#End Region

    End Class

End Namespace