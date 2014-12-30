Namespace Goods

    <Serializable()> _
    Public Class GoodsComplexOperationProduction
        Inherits BusinessBase(Of GoodsComplexOperationProduction)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _JournalEntryID As Integer = 0
        Private _ChronologyValidatorDiscard As ComplexChronologicValidator = Nothing
        Private _Date As Date = Today
        Private _OldDate As Date = Today
        Private _DocumentNumber As String = ""
        Private _Content As String = ""
        Private _WarehouseForProduction As WarehouseInfo = Nothing
        Private _WarehouseForComponents As WarehouseInfo = Nothing
        Private _OldWarehouseForProductionID As Integer = 0
        Private _OldWarehouseForComponentsID As Integer = 0
        Private _Acquisition As GoodsOperationAcquisition = Nothing
        Private _CalculationIsPerUnit As Double = 0
        Private _Amount As Double = 0
        Private _UnitValue As Double = 0
        Private _TotalValue As Double = 0
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private WithEvents _ComponentItems As GoodsComponentItemList
        Private WithEvents _CostsItems As GoodsProductionCostItemList


        Private SuspendChildListChangedEvents As Boolean = False
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _ComponentItemsSortedList As Csla.SortedBindingList(Of GoodsComponentItem) = Nothing
        ' used to implement automatic sort in datagridview
        <NotUndoable()> _
        <NonSerialized()> _
        Private _CostsItemsSortedList As Csla.SortedBindingList(Of GoodsProductionCostItem) = Nothing

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

        Public ReadOnly Property ChronologyValidatorAcquisition() As OperationalLimitList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Acquisition.OperationLimitations
            End Get
        End Property

        Public ReadOnly Property ChronologyValidatorDiscard() As ComplexChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologyValidatorDiscard
            End Get
        End Property

        Public ReadOnly Property GoodsInfo() As GoodsSummary
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Acquisition.GoodsInfo
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
                    _ComponentItems.SetDate(value.Date)
                    _Acquisition.SetDate(value.Date)
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

        Public Property WarehouseForProduction() As WarehouseInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseForProduction
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WarehouseInfo)
                CanWriteProperty(True)

                If WarehouseForProductionIsReadOnly Then Exit Property
                
                If Not (_WarehouseForProduction Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _WarehouseForProduction Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _WarehouseForProduction.ID = value.ID) Then

                    _WarehouseForProduction = value
                    PropertyHasChanged()

                    UpdateItemsWithAcquisitionWarehouse(value)

                End If

            End Set
        End Property

        Public Property WarehouseForComponents() As WarehouseInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _WarehouseForComponents
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As WarehouseInfo)
                CanWriteProperty(True)
                If WarehouseForComponentsIsReadOnly Then Exit Property
                If Not (_WarehouseForComponents Is Nothing AndAlso value Is Nothing) _
                    AndAlso Not (Not _WarehouseForComponents Is Nothing AndAlso Not value Is Nothing _
                    AndAlso _WarehouseForComponents.ID = value.ID) Then
                    _WarehouseForComponents = value
                    PropertyHasChanged()
                    UpdateItemsWithDiscardWarehouse(value)
                End If
            End Set
        End Property

        Public ReadOnly Property OldWarehouseForProductionID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldWarehouseForProductionID
            End Get
        End Property

        Public ReadOnly Property OldWarehouseForComponentsID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _OldWarehouseForComponentsID
            End Get
        End Property

        Friend ReadOnly Property Acquisition() As GoodsOperationAcquisition
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Acquisition
            End Get
        End Property

        Public ReadOnly Property CalculationIsPerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CalculationIsPerUnit, 6)
            End Get
        End Property

        Public Property Amount() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Amount, 6)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)

                If AmountIsReadOnly Then Exit Property

                If CRound(_Amount, 6) <> CRound(value, 6) Then

                    _Amount = CRound(value, 6)
                    PropertyHasChanged()

                    _Acquisition.Ammount = _Amount

                    If CRound(_Amount, 6) > 0 Then
                        _UnitValue = CRound(_TotalValue / _Amount, 6)
                    Else
                        _UnitValue = 0
                    End If
                    PropertyHasChanged("UnitValue")

                End If

            End Set
        End Property

        Public ReadOnly Property UnitValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_UnitValue, 6)
            End Get
        End Property

        Public ReadOnly Property TotalValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalValue)
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

        Public ReadOnly Property ComponentItems() As GoodsComponentItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ComponentItems
            End Get
        End Property

        Public ReadOnly Property CostsItems() As GoodsProductionCostItemList
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CostsItems
            End Get
        End Property


        Public ReadOnly Property WarehouseForProductionIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _Acquisition.OperationLimitations.FinancialDataCanChange OrElse _
                    Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property WarehouseForComponentsIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _ChronologyValidatorDiscard.FinancialDataCanChange OrElse _
                    Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property AmountIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _Acquisition.OperationLimitations.FinancialDataCanChange OrElse _
                    Not _ChronologyValidatorDiscard.FinancialDataCanChange OrElse _
                    Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean _
            Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Not IsNew Then Return IsDirty
                Return (Not String.IsNullOrEmpty(_DocumentNumber.Trim) _
                    OrElse Not String.IsNullOrEmpty(_Content.Trim) _
                    OrElse _ComponentItems.Count > 0)
            End Get
        End Property

        Public Overrides ReadOnly Property IsDirty() As Boolean
            Get
                Return MyBase.IsDirty OrElse _ComponentItems.IsDirty OrElse _CostsItems.IsDirty
            End Get
        End Property

        Public Overrides ReadOnly Property IsValid() As Boolean
            Get
                Return MyBase.IsValid AndAlso _ComponentItems.IsValid AndAlso _CostsItems.IsValid
            End Get
        End Property



        Public Function GetAllBrokenRules() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Error).Trim
            result = AddWithNewLine(result, _ComponentItems.GetAllBrokenRules, False)
            result = AddWithNewLine(result, _CostsItems.GetAllBrokenRules, False)

            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = Me.BrokenRulesCollection.ToString(Validation.RuleSeverity.Warning).Trim
            result = AddWithNewLine(result, _ComponentItems.GetAllWarnings, False)
            result = AddWithNewLine(result, _CostsItems.GetAllWarnings, False)


            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function


        Public Function GetCostsParams() As GoodsCostParam()
            Dim result As New List(Of GoodsCostParam)
            If _WarehouseForComponents Is Nothing OrElse Not _WarehouseForComponents.ID > 0 Then _
                Return result.ToArray
            For Each i As GoodsComponentItem In _ComponentItems
                result.Add(GoodsCostParam.GetGoodsCostParam(i.GoodsInfo.ID, _WarehouseForComponents.ID, _
                    i.Amount, i.Discard.ID, 0, i.GoodsInfo.ValuationMethod, _Date))
            Next
            Return result.ToArray
        End Function

        Public Sub ReloadCostInfo(ByVal list As GoodsCostItemList)

            If list Is Nothing Then Throw New ArgumentNullException("Klaida. " _
                & "Metodui GoodsComplexOperationProduction.ReloadCostInfo " _
                & "nenurodytas (null) GoodsCostItemList parametras.")

            If Not _Acquisition.OperationLimitations.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Žaliavų bendra savikaina negali keistis: " & vbCrLf _
                & _Acquisition.OperationLimitations.FinancialDataCanChangeExplanation)

            If Not _ChronologyValidatorDiscard.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Žaliavų savikaina negali keistis: " & vbCrLf _
                & _ChronologyValidatorDiscard.FinancialDataCanChangeExplanation)

            If Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Operacijos finansiniai duomenys negali keistis: " & vbCrLf _
                & _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChangeExplanation)

            _ComponentItems.ReloadCostInfo(list)

        End Sub

        Public Sub AddNewComponentItem(ByVal item As GoodsComponentItem)

            If Not _Acquisition.OperationLimitations.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Žaliavų bendra savikaina negali keistis: " & vbCrLf _
                & _Acquisition.OperationLimitations.FinancialDataCanChangeExplanation)

            If Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Operacijos finansiniai duomenys negali keistis: " & vbCrLf _
                & _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChangeExplanation)

            If _ComponentItems.ContainsGood(item.GoodsInfo.ID) Then Throw New Exception( _
                "Klaida. Eilutė žaliavai '" & item.GoodsName & "' jau yra.")

            item.SetDate(_Date)
            item.SetWarehouse(_WarehouseForComponents)

            _ComponentItems.Add(item)

            _ChronologyValidatorDiscard.MergeNewValidationItem(item.Discard.OperationLimitations)

            PropertyHasChanged("Date")

        End Sub

        Public Sub RecalculateForProductionAmount()

            If Not CRound(_Amount, 6) > 0 Then Exit Sub

            If Not _Acquisition.OperationLimitations.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Žaliavų bendra savikaina negali keistis: " & vbCrLf _
                & _Acquisition.OperationLimitations.FinancialDataCanChangeExplanation)

            If Not _ChronologyValidatorDiscard.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Žaliavų savikaina negali keistis: " & vbCrLf _
                & _ChronologyValidatorDiscard.FinancialDataCanChangeExplanation)

            If Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Operacijos finansiniai duomenys negali keistis: " & vbCrLf _
                & _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChangeExplanation)

            _ComponentItems.RecalculateForProductionAmount(_Amount)
            _CostsItems.RecalculateForProductionAmount(_Amount)

        End Sub


        Private Sub UpdateItemsWithAcquisitionWarehouse(ByVal value As WarehouseInfo)
            _Acquisition.Warehouse = value
            PropertyHasChanged("Date")
        End Sub

        Private Sub UpdateItemsWithDiscardWarehouse(ByVal value As WarehouseInfo)
            _ComponentItems.SetWarehouse(value)
            _ChronologyValidatorDiscard.ReloadValidationItems(_ComponentItems.GetLimitations())
            PropertyHasChanged("Date")
        End Sub


        Public Overrides Function Save() As GoodsComplexOperationProduction

            If IsNew AndAlso Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            If Not IsNew AndAlso Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka duomenims pakeisti.")

            Me.ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & vbCrLf & Me.GetAllBrokenRules)

            Return MyBase.Save

        End Function


        Private Sub CalculateSum(ByVal RaisePropertyHasChanged As Boolean)

            Dim result As Double = 0
            For Each c As GoodsComponentItem In _ComponentItems
                result = CRound(result + c.TotalCost)
            Next
            For Each c As GoodsProductionCostItem In _CostsItems
                result = CRound(result + c.TotalCost)
            Next

            _TotalValue = result
            If CRound(_Amount, 6) > 0 Then
                _UnitValue = CRound(_TotalValue / _Amount, 6)
            Else
                _UnitValue = 0
            End If

            If RaisePropertyHasChanged Then
                PropertyHasChanged("TotalValue")
                PropertyHasChanged("UnitValue")
            End If

        End Sub

        Private Sub ComponentItems_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _ComponentItems.ListChanged
            If SuspendChildListChangedEvents Then Exit Sub
            CalculateSum(True)
            If e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted Then
                _ChronologyValidatorDiscard.ReloadValidationItems(_ComponentItems.GetLimitations)
                ValidationRules.CheckRules()
                PropertyHasChanged("Date")
            End If

        End Sub

        Private Sub CostsItems_Changed(ByVal sender As Object, _
            ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _CostsItems.ListChanged
            If SuspendChildListChangedEvents Then Exit Sub
            CalculateSum(True)
        End Sub

        ''' <summary>
        ''' Helper method. Takes care of child lists loosing their handlers.
        ''' </summary>
        Protected Overrides Function GetClone() As Object
            Dim result As GoodsComplexOperationProduction = DirectCast(MyBase.GetClone(), _
                GoodsComplexOperationProduction)
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
                RemoveHandler _ComponentItems.ListChanged, AddressOf ComponentItems_Changed
                RemoveHandler _CostsItems.ListChanged, AddressOf CostsItems_Changed
            Catch ex As Exception
            End Try
            AddHandler _ComponentItems.ListChanged, AddressOf ComponentItems_Changed
            AddHandler _CostsItems.ListChanged, AddressOf CostsItems_Changed
        End Sub


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            Return "Goods.GoodsComplexOperationProduction"
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("DocumentNumber", "dokumento numeris"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos aprašymas"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("WarehouseForProduction", _
                "sandėlis, produkcijos pajamavimui", "ID"))
            ValidationRules.AddRule(AddressOf CommonValidation.InfoObjectRequired, _
                New CommonValidation.InfoObjectRequiredRuleArgs("WarehouseForComponents", _
                "žaliavų/detalių sandėlis", "ID"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Amount", "pagamintas kiekis"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologyValidatorAcquisition"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologyValidatorDiscard"))

            ValidationRules.AddRule(AddressOf TotalValueValidation, New Validation.RuleArgs("TotalValue"))

        End Sub

        ''' <summary>
        ''' Rule ensuring that the value of property TotalValue is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function TotalValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As GoodsComplexOperationProduction = DirectCast(target, GoodsComplexOperationProduction)

            If Not CRound(ValObj._TotalValue) > 0 AndAlso Not ValObj._ComponentItems. _
                ContainsCalculatedAtRuntimeValueGoods Then

                e.Description = "Nėra nei žaliavų/detalių, nei gamybos sąnaudų, iš kurių būtų " _
                    & "galima būtų formuoti pagamintos produkcijos savikainą."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Goods.GoodsOperationProduction2")
        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationProduction1")
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationProduction2")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationProduction3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Goods.GoodsOperationProduction3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewGoodsComplexOperationProduction(ByVal GoodsID As Integer) As GoodsComplexOperationProduction
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            Return DataPortal.Create(Of GoodsComplexOperationProduction)(New Criteria(GoodsID))
        End Function

        Public Shared Function NewGoodsComplexOperationProduction(ByVal GoodsID As Integer, _
            ByVal CalculationID As Integer) As GoodsComplexOperationProduction
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujiems duomenims įvesti.")
            Return DataPortal.Create(Of GoodsComplexOperationProduction)(New Criteria(GoodsID, CalculationID))
        End Function

        Public Shared Function GetGoodsComplexOperationProduction(ByVal nID As Integer) _
            As GoodsComplexOperationProduction
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiems duomenims gauti.")
            Return DataPortal.Fetch(Of GoodsComplexOperationProduction)(New Criteria(nID))
        End Function

        Public Shared Sub DeleteGoodsComplexOperationProduction(ByVal id As Integer)
            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka sąskaitos duomenų ištrynimui.")
            DataPortal.Delete(New Criteria(id))
        End Sub


        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _ID As Integer = 0
            Private _CalculationID As Integer = 0
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property CalculationID() As Integer
                Get
                    Return _CalculationID
                End Get
            End Property
            Public Sub New(ByVal nID As Integer)
                _ID = nID
            End Sub
            Public Sub New(ByVal nID As Integer, ByVal nCalculationID As Integer)
                _ID = nID
                _CalculationID = nCalculationID
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)

            Dim BaseValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                NewSimpleChronologicValidator(ConvertEnumHumanReadable(DocumentType.GoodsProduction))

            If criteria.CalculationID > 0 Then

                Dim calculation As ProductionCalculation = ProductionCalculation. _
                    GetProductionCalculationInternal(criteria.CalculationID)
                _ComponentItems = GoodsComponentItemList.NewGoodsComponentItemList( _
                    calculation.ComponentList, calculation.Amount, BaseValidator)
                _CostsItems = GoodsProductionCostItemList.NewGoodsProductionCostItemList( _
                    calculation.CostList, calculation.Amount)

                _Acquisition = GoodsOperationAcquisition.NewGoodsOperationAcquisitionChild( _
                    calculation.Goods.ID)
                _WarehouseForProduction = _Acquisition.Warehouse

                _CalculationIsPerUnit = calculation.Amount
                _Amount = calculation.Amount

                CalculateSum(False)

            Else

                _ComponentItems = GoodsComponentItemList.NewGoodsComponentItemList
                _CostsItems = GoodsProductionCostItemList.NewGoodsProductionCostItemList

                _Acquisition = GoodsOperationAcquisition.NewGoodsOperationAcquisitionChild(criteria.ID)
                _WarehouseForProduction = _Acquisition.Warehouse

            End If

            _ChronologyValidatorDiscard = ComplexChronologicValidator. _
                NewComplexChronologicValidator(ConvertEnumHumanReadable(DocumentType.GoodsProduction), _
                BaseValidator, _ComponentItems.GetLimitations)

            ValidationRules.CheckRules()

            MarkNew()

        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            Dim obj As ComplexOperationPersistenceObject = ComplexOperationPersistenceObject. _
                GetComplexOperationPersistenceObject(criteria.ID, True)

            Fetch(obj)

        End Sub

        Private Sub Fetch(ByVal obj As ComplexOperationPersistenceObject)

            If obj.OperationType <> GoodsComplexOperationType.Production Then _
                Throw New Exception("Klaida. Kompleksinė operacija, kurios ID=" _
                & obj.ID.ToString & ", yra ne gamyba, o " _
                & ConvertEnumHumanReadable(obj.OperationType))

            _Content = obj.Content
            _DocumentNumber = obj.DocNo
            _ID = obj.ID
            _InsertDate = obj.InsertDate
            _JournalEntryID = obj.JournalEntryID
            _Date = obj.OperationDate
            _WarehouseForProduction = obj.SecondaryWarehouse
            _UpdateDate = obj.UpdateDate
            _WarehouseForComponents = obj.Warehouse

            _OldWarehouseForProductionID = obj.SecondaryWarehouse.ID
            _OldWarehouseForComponentsID = obj.Warehouse.ID
            _OldDate = _Date

            Dim BaseValidator As SimpleChronologicValidator = SimpleChronologicValidator. _
                GetSimpleChronologicValidator(_ID, _Date, ConvertEnumHumanReadable(DocumentType.GoodsProduction))

            Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _Date)
                Dim objList As List(Of OperationPersistenceObject) = _
                    OperationPersistenceObject.GetOperationPersistenceObjectList(_ID)
                _ComponentItems = GoodsComponentItemList.GetGoodsComponentItemList(objList, BaseValidator, myData)
                For Each p As OperationPersistenceObject In objList
                    If p.OperationType = GoodsOperationType.Acquisition Then
                        _Acquisition = GoodsOperationAcquisition.GetGoodsOperationAcquisitionChild(p, myData)
                        Exit For
                    End If
                Next
            End Using

            _Amount = _Acquisition.Ammount
            _TotalValue = _Acquisition.TotalCost
            _UnitValue = _Acquisition.UnitCost

            Dim myComm As New SQLCommand("BookEntriesFetch")
            myComm.AddParam("?BD", _JournalEntryID)

            Using myData As DataTable = myComm.Fetch

                Dim invertedComponentEntryList As BookEntryInternalList = _
                    _ComponentItems.GetBookEntryInternalList(True)

                Dim costEntryList As BookEntryInternalList = BookEntryInternalList. _
                    NewBookEntryInternalList(BookEntryType.Kreditas)

                For Each dr As DataRow In myData.Rows

                    If ConvertEnumDatabaseStringCode(Of BookEntryType)(CStrSafe(dr.Item(1))) _
                        = BookEntryType.Kreditas Then

                        costEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                            BookEntryType.Kreditas, CLongSafe(dr.Item(2), 0), _
                            CDblSafe(dr.Item(3), 2, 0), Nothing))

                    ElseIf CDblSafe(dr.Item(3), 2, 0) < CRound(_TotalValue, 2) Then

                        costEntryList.Add(BookEntryInternal.NewBookEntryInternal( _
                            BookEntryType.Kreditas, CLongSafe(dr.Item(2), 0), _
                            CRound(_TotalValue - CDblSafe(dr.Item(3), 2, 0)), Nothing))

                    End If

                Next

                costEntryList.AddRange(invertedComponentEntryList)

                costEntryList.Aggregate()

                _CostsItems = GoodsProductionCostItemList.GetGoodsProductionCostItemList( _
                    costEntryList, _Amount, BaseValidator.FinancialDataCanChange)

            End Using

            _ChronologyValidatorDiscard = ComplexChronologicValidator.GetComplexChronologicValidator( _
                _ID, _Date, ConvertEnumHumanReadable(DocumentType.GoodsProduction), BaseValidator, _
                _ComponentItems.GetLimitations)

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            _ComponentItems.PrepareOperationConsignments()

            CheckIfCanUpdate()

            _Acquisition.GetConsignment()

            DoSave()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            _ComponentItems.PrepareOperationConsignments()

            CheckIfCanUpdate()

            _Acquisition.GetConsignment()

            ComplexOperationPersistenceObject.CheckIfUpdateDateChanged(_ID, _UpdateDate)

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
                _ID = obj.Save(_Acquisition.OperationLimitations.FinancialDataCanChange _
                    AndAlso _ChronologyValidatorDiscard.FinancialDataCanChange, _
                    _Acquisition.OperationLimitations.FinancialDataCanChange, _
                    _Acquisition.OperationLimitations.FinancialDataCanChange _
                    AndAlso _ChronologyValidatorDiscard.FinancialDataCanChange)
            Else
                obj.Save(_Acquisition.OperationLimitations.FinancialDataCanChange _
                    AndAlso _ChronologyValidatorDiscard.FinancialDataCanChange, _
                    _Acquisition.OperationLimitations.FinancialDataCanChange, _
                    _Acquisition.OperationLimitations.FinancialDataCanChange _
                    AndAlso _ChronologyValidatorDiscard.FinancialDataCanChange)
            End If

            _Acquisition.SaveChild(_JournalEntryID, _ID, _DocumentNumber, _Content, _
                True, Not _ChronologyValidatorDiscard.BaseValidator.FinancialDataCanChange)

            _ComponentItems.Update(Me)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            If IsNew Then _InsertDate = obj.InsertDate
            _UpdateDate = obj.UpdateDate
            _OldDate = _Date
            _OldWarehouseForProductionID = _WarehouseForProduction.ID
            _OldWarehouseForComponentsID = _WarehouseForComponents.ID

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
            obj.GoodsID = _Acquisition.GoodsInfo.ID
            obj.OperationType = GoodsComplexOperationType.Production
            obj.Content = _Content
            obj.DocNo = _DocumentNumber
            obj.JournalEntryID = _JournalEntryID
            obj.OperationDate = _Date
            obj.SecondaryWarehouse = _WarehouseForProduction
            obj.Warehouse = _WarehouseForComponents

            Return obj

        End Function



        Protected Overrides Sub DataPortal_DeleteSelf()
            DataPortal_Delete(New Criteria(_ID))
        End Sub

        Protected Overrides Sub DataPortal_Delete(ByVal criteria As Object)

            Dim OperationToDelete As GoodsComplexOperationProduction = _
                New GoodsComplexOperationProduction
            OperationToDelete.DataPortal_Fetch(DirectCast(criteria, Criteria))

            If Not OperationToDelete._ChronologyValidatorDiscard.FinancialDataCanChange Then _
                Throw New Exception("Klaida. Negalima ištrinti prekių " _
                    & "gamybos operacijos:" & vbCrLf & OperationToDelete. _
                    _ChronologyValidatorDiscard.FinancialDataCanChangeExplanation)
            If Not OperationToDelete._Acquisition.OperationLimitations.FinancialDataCanChange Then _
                Throw New Exception("Klaida. Negalima ištrinti prekių " _
                    & "vidinio judėjimo operacijos:" & vbCrLf & OperationToDelete. _
                    _Acquisition.OperationLimitations.FinancialDataCanChangeExplanation)

            IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted( _
                OperationToDelete.JournalEntryID, DocumentType.GoodsProduction)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            ComplexOperationPersistenceObject.DeleteConsignmentDiscards(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.DeleteConsignments(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.DeleteOperations(DirectCast(criteria, Criteria).ID)

            ComplexOperationPersistenceObject.Delete(DirectCast(criteria, Criteria).ID)

            General.JournalEntry.DoDelete(OperationToDelete.JournalEntryID)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            MarkNew()

        End Sub


        Private Function GetJournalEntry() As General.JournalEntry

            Dim result As General.JournalEntry
            If IsNew Then
                result = General.JournalEntry.NewJournalEntryChild(DocumentType.GoodsProduction)
            Else
                result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, DocumentType.GoodsProduction)
            End If

            result.Content = _Content
            result.Date = _Date
            result.DocNumber = _DocumentNumber

            If _ChronologyValidatorDiscard.FinancialDataCanChange AndAlso _Acquisition. _
                OperationLimitations.FinancialDataCanChange Then

                Dim FullBookEntryList As BookEntryInternalList = BookEntryInternalList. _
                    NewBookEntryInternalList(BookEntryType.Kreditas)

                FullBookEntryList.AddRange(_ComponentItems.GetBookEntryInternalList(False))
                FullBookEntryList.AddRange(_CostsItems.GetBookEntryInternalList)
                FullBookEntryList.AddRange(_Acquisition.GetTotalBookEntryList)

                FullBookEntryList.Aggregate()

                result.DebetList.Clear()
                result.CreditList.Clear()

                result.DebetList.LoadBookEntryListFromInternalList(FullBookEntryList, False)
                result.CreditList.LoadBookEntryListFromInternalList(FullBookEntryList, False)

            End If

            If Not result.IsValid Then Throw New Exception("Klaida. Nepavyko generuoti " _
                & "bendrojo žurnalo įrašo: " & result.GetAllBrokenRules)

            Return result

        End Function

        Private Sub CheckIfCanUpdate()

            _ComponentItems.SetValues(Me)

            CalculateSum(False)

            _Acquisition.Ammount = _Amount
            _Acquisition.UnitCost = _UnitValue
            _Acquisition.TotalCost = _TotalValue
            _Acquisition.SetDate(_Date)

            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Prekių gamybos judėjimo operacijoje " _
                & "yra klaidų: " & BrokenRulesCollection.ToString)

            Dim exceptionText As String = ""

            If IsNew Then
                exceptionText = AddWithNewLine(exceptionText, _ComponentItems.CheckIfCanUpdate( _
                    Nothing, _ChronologyValidatorDiscard.BaseValidator, False), False)
            Else
                Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _OldDate)
                    exceptionText = AddWithNewLine(exceptionText, _ComponentItems.CheckIfCanUpdate( _
                        myData, _ChronologyValidatorDiscard.BaseValidator, False), False)
                End Using
            End If

            If Not String.IsNullOrEmpty(exceptionText.Trim.Trim) Then _
                Throw New Exception(exceptionText.Trim)

        End Sub

        Private Sub ReloadLimitations()

            Using myData As DataTable = OperationalLimitList.GetDataSourceForComplexOperation(_ID, _Date)
                _ComponentItems.ReloadLimitations(myData, _ChronologyValidatorDiscard.BaseValidator)
            End Using

            _ChronologyValidatorDiscard.ReloadValidationItems(_JournalEntryID, _Date, _
                _ComponentItems.GetLimitations())

            ValidationRules.CheckRules()

        End Sub

#End Region

    End Class

End Namespace