Namespace Assets

    <Serializable()> _
Public Class LongTermAssetOperation
        Inherits BusinessBase(Of LongTermAssetOperation)

#Region " Business Methods "

        Private _AssetID As Integer
        Private _AssetName As String
        Private _AssetMeasureUnit As String
        Private _AccumulatedAmortizationBeforeAquisition As Double
        Private _AccumulatedAmortizationBeforeAquisitionRevaluedPortion As Double
        Private _AssetAcquiredAccount As Long
        Private _AssetContraryAccount As Long
        Private _AssetValueDecreaseAccount As Long
        Private _AssetValueIncreaseAccount As Long
        Private _AssetValueIncreaseAmortizationAccount As Long
        Private _AssetDateAcquired As Date
        Private _AssetAquisitionOpID As Integer
        Private _AssetLiquidationValue As Double

        Private _ChronologyValidator As OperationChronologicValidator
        Private _IsInvoiceBound As Boolean = False

        Private _CurrentAcquisitionAccountValue As Double
        Private _CurrentAcquisitionAccountValuePerUnit As Double
        Private _CurrentAmortizationAccountValue As Double
        Private _CurrentAmortizationAccountValuePerUnit As Double
        Private _CurrentValueDecreaseAccountValue As Double
        Private _CurrentValueDecreaseAccountValuePerUnit As Double
        Private _CurrentValueIncreaseAccountValue As Double
        Private _CurrentValueIncreaseAccountValuePerUnit As Double
        Private _CurrentValueIncreaseAmortizationAccountValue As Double
        Private _CurrentValueIncreaseAmortizationAccountValuePerUnit As Double
        Private _CurrentAssetAmmount As Integer
        Private _CurrentAssetValue As Double
        Private _CurrentAssetValuePerUnit As Double
        Private _CurrentAssetValueRevaluedPortion As Double
        Private _CurrentAssetValueRevaluedPortionPerUnit As Double

        Private _CurrentUsageTermMonths As Integer
        Private _CurrentAmortizationPeriod As Integer
        Private _CurrentUsageStatus As Boolean

        Private _ID As Integer = -1
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _IsComplexAct As Boolean = False
        Private _Type As LtaOperationType = LtaOperationType.AmortizationPeriod
        Private _AccountChangeType As LtaAccountChangeType = LtaAccountChangeType.AcquisitionAccount
        Private _Date As Date = Today.Date
        Private _OldDate As Date = Today.Date
        Private _Content As String = ""
        Private _AccountCorresponding As Long = 0
        Private _ActNumber As Integer = 0
        Private _JournalEntryID As Integer = -1
        Private _JournalEntryType As DocumentType = DocumentType.None
        Private _JournalEntryContent As String = ""
        Private _JournalEntryDocNumber As String = ""
        Private _UnitValueChange As Double = 0
        Private _TotalValueChange As Double = 0
        Private _AmmountChange As Integer = 0
        Private _RevaluedPortionUnitValueChange As Double = 0
        Private _RevaluedPortionTotalValueChange As Double = 0
        Private _NewAmortizationPeriod As Integer
        Private _AmortizationCalculations As String = ""
        Private _AmortizationCalculatedForMonths As Integer = 0

        Private _AfterOperationAcquisitionAccountValue As Double = 0
        Private _AfterOperationAcquisitionAccountValuePerUnit As Double = 0
        Private _AfterOperationAmortizationAccountValue As Double = 0
        Private _AfterOperationAmortizationAccountValuePerUnit As Double = 0
        Private _AfterOperationValueDecreaseAccountValue As Double = 0
        Private _AfterOperationValueDecreaseAccountValuePerUnit As Double = 0
        Private _AfterOperationValueIncreaseAccountValue As Double = 0
        Private _AfterOperationValueIncreaseAccountValuePerUnit As Double = 0
        Private _AfterOperationValueIncreaseAmortizationAccountValue As Double = 0
        Private _AfterOperationValueIncreaseAmortizationAccountValuePerUnit As Double = 0
        Private _AfterOperationAssetAmmount As Integer = 0
        Private _AfterOperationAssetValue As Double = 0
        Private _AfterOperationAssetValuePerUnit As Double = 0
        Private _AfterOperationAssetValueRevaluedPortion As Double = 0
        Private _AfterOperationAssetValueRevaluedPortionPerUnit As Double = 0


        Public ReadOnly Property AssetID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetID
            End Get
        End Property

        Public ReadOnly Property AssetName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetName
            End Get
        End Property

        Public ReadOnly Property AssetMeasureUnit() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetMeasureUnit
            End Get
        End Property

        Public ReadOnly Property AccumulatedAmortizationBeforeAquisition() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AccumulatedAmortizationBeforeAquisition)
            End Get
        End Property

        Public ReadOnly Property AccumulatedAmortizationBeforeAquisitionRevaluedPortion() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AccumulatedAmortizationBeforeAquisitionRevaluedPortion)
            End Get
        End Property

        Public ReadOnly Property AssetAcquiredAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetAcquiredAccount
            End Get
        End Property

        Public ReadOnly Property AssetContraryAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetContraryAccount
            End Get
        End Property

        Public ReadOnly Property AssetValueDecreaseAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetValueDecreaseAccount
            End Get
        End Property

        Public ReadOnly Property AssetValueIncreaseAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetValueIncreaseAccount
            End Get
        End Property

        Public ReadOnly Property AssetValueIncreaseAmortizationAccount() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetValueIncreaseAmortizationAccount
            End Get
        End Property

        Public ReadOnly Property AssetDateAcquired() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetDateAcquired
            End Get
        End Property

        Public ReadOnly Property AssetAquisitionOpID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetAquisitionOpID
            End Get
        End Property

        Public ReadOnly Property AssetLiquidationValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AssetLiquidationValue
            End Get
        End Property


        Public ReadOnly Property ChronologyValidator() As OperationChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologyValidator
            End Get
        End Property

        Public ReadOnly Property IsInvoiceBound() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsInvoiceBound
            End Get
        End Property


        Public ReadOnly Property CurrentAcquisitionAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAcquisitionAccountValue)
            End Get
        End Property

        Public ReadOnly Property CurrentAcquisitionAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentAmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAmortizationAccountValue)
            End Get
        End Property

        Public ReadOnly Property CurrentAmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentValueDecreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueDecreaseAccountValue)
            End Get
        End Property

        Public ReadOnly Property CurrentValueDecreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentValueIncreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueIncreaseAccountValue)
            End Get
        End Property

        Public ReadOnly Property CurrentValueIncreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentValueIncreaseAmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueIncreaseAmortizationAccountValue)
            End Get
        End Property

        Public ReadOnly Property CurrentValueIncreaseAmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentAssetAmmount() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrentAssetAmmount
            End Get
        End Property

        Public ReadOnly Property CurrentAssetValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAssetValue)
            End Get
        End Property

        Public ReadOnly Property CurrentAssetValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAssetValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property CurrentAssetValueRevaluedPortion() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAssetValueRevaluedPortion)
            End Get
        End Property

        Public ReadOnly Property CurrentAssetValueRevaluedPortionPerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            End Get
        End Property



        Public ReadOnly Property CurrentUsageTermMonths() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrentUsageTermMonths
            End Get
        End Property

        Public ReadOnly Property CurrentAmortizationPeriod() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrentAmortizationPeriod
            End Get
        End Property

        Public ReadOnly Property CurrentUsageStatus() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrentUsageStatus
            End Get
        End Property


        Public ReadOnly Property TypeHumanReadableIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not IsNew OrElse IsChild OrElse _IsInvoiceBound
            End Get
        End Property

        Public ReadOnly Property AccountChangeTypeHumanReadableIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not IsNew OrElse (Not IsChild AndAlso _IsInvoiceBound) OrElse _Type <> LtaOperationType.AccountChange
            End Get
        End Property

        Public ReadOnly Property DateIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not IsChild AndAlso _IsInvoiceBound
            End Get
        End Property

        Public ReadOnly Property ContentIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not IsChild AndAlso _IsInvoiceBound
            End Get
        End Property

        Public ReadOnly Property AccountCorrespondingIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse Not _ChronologyValidator.FinancialDataCanChange
            End Get
        End Property

        Public ReadOnly Property ActNumberIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse (_Type <> LtaOperationType.Discard _
                    AndAlso _Type <> LtaOperationType.UsingStart AndAlso _Type <> LtaOperationType.UsingEnd)
            End Get
        End Property

        Public ReadOnly Property UnitValueChangeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.AcquisitionValueIncrease AndAlso _
                    _Type <> LtaOperationType.Amortization AndAlso _Type <> LtaOperationType.ValueChange)
            End Get
        End Property

        Public ReadOnly Property TotalValueChangeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type = LtaOperationType.AccountChange OrElse _
                    _Type = LtaOperationType.AmortizationPeriod OrElse _
                    _Type = LtaOperationType.UsingEnd OrElse _Type = LtaOperationType.UsingStart)
            End Get
        End Property

        Public ReadOnly Property AmmountChangeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                   (_Type <> LtaOperationType.Discard AndAlso _Type <> LtaOperationType.Transfer)
            End Get
        End Property

        Public ReadOnly Property RevaluedPortionUnitValueChangeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.Amortization AndAlso _
                    _Type <> LtaOperationType.ValueChange)
            End Get
        End Property

        Public ReadOnly Property RevaluedPortionTotalValueChangeIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.Amortization AndAlso _Type <> LtaOperationType.Discard _
                    AndAlso _Type <> LtaOperationType.Transfer AndAlso _Type <> LtaOperationType.ValueChange)
            End Get
        End Property

        Public ReadOnly Property NewAmortizationPeriodIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _ChronologyValidator.FinancialDataCanChange _
                    OrElse _Type <> LtaOperationType.AmortizationPeriod
            End Get
        End Property

        Public ReadOnly Property AmortizationCalculationsIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _ChronologyValidator.FinancialDataCanChange _
                    OrElse _Type <> LtaOperationType.Amortization
            End Get
        End Property

        Public ReadOnly Property AfterOperationAssetValueIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type = LtaOperationType.AccountChange OrElse _
                    _Type = LtaOperationType.AmortizationPeriod OrElse _
                    _Type = LtaOperationType.UsingEnd OrElse _Type = LtaOperationType.UsingStart)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAssetValuePerUnitIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.AcquisitionValueIncrease AndAlso _
                    _Type <> LtaOperationType.Amortization AndAlso _Type <> LtaOperationType.ValueChange)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAssetValueRevaluedPortionIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.Amortization AndAlso _Type <> LtaOperationType.Discard _
                    AndAlso _Type <> LtaOperationType.Transfer AndAlso _Type <> LtaOperationType.ValueChange)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAssetValueRevaluedPortionPerUnitIsReadOnly() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return (Not IsChild AndAlso _IsInvoiceBound) OrElse _
                    Not _ChronologyValidator.FinancialDataCanChange OrElse _
                    (_Type <> LtaOperationType.Amortization AndAlso _
                    _Type <> LtaOperationType.ValueChange)
            End Get
        End Property


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

        Public ReadOnly Property IsComplexAct() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsComplexAct
            End Get
        End Property

        Public Property [Type]() As LTAOperationType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Type
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As LTAOperationType)
                CanWriteProperty(True)
                If TypeHumanReadableIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _Type <> value Then
                    _Type = value
                    PropertyHasChanged()
                    PropertyHasChanged("TypeHumanReadable")
                    OnTypeChanged()
                End If
            End Set
        End Property

        Public Property TypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return ConvertEnumHumanReadable(_Type)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If String.IsNullOrEmpty(value) Then value = ""
                If TypeHumanReadableIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If ConvertEnumHumanReadable(Of LtaOperationType)(value.Trim) <> _Type Then
                    Me.Type = ConvertEnumHumanReadable(Of LtaOperationType)(value.Trim)
                End If
            End Set
        End Property

        Public Property AccountChangeType() As LtaAccountChangeType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountChangeType
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As LtaAccountChangeType)
                CanWriteProperty(True)
                If AccountChangeTypeHumanReadableIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _AccountChangeType <> value Then
                    _AccountChangeType = value
                    PropertyHasChanged()
                    PropertyHasChanged("AccountChangeTypeHumanReadable")
                    _ChronologyValidator.ReconfigureForType(_Type, _AccountChangeType)
                End If
            End Set
        End Property

        Public Property AccountChangeTypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return ConvertEnumHumanReadable(_AccountChangeType)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If String.IsNullOrEmpty(value) Then value = ""
                If AccountChangeTypeHumanReadableIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If ConvertEnumHumanReadable(Of LtaAccountChangeType)(value.Trim) <> _AccountChangeType Then
                    _AccountChangeType = ConvertEnumHumanReadable(Of LtaAccountChangeType)(value.Trim)
                    PropertyHasChanged()
                    PropertyHasChanged("AccountChangeType")
                    _ChronologyValidator.ReconfigureForType(_Type, _AccountChangeType)
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
                If DateIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _Date.Date <> value.Date Then
                    _Date = value
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

        Public Property Content() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Content
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If ContentIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _Content.Trim <> value.Trim Then
                    _Content = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountCorresponding() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountCorresponding
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If AccountCorrespondingIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _AccountCorresponding <> value Then
                    _AccountCorresponding = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property ActNumber() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ActNumber
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If ActNumberIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _ActNumber <> value Then
                    _ActNumber = value
                    PropertyHasChanged()
                End If
            End Set
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

        Public ReadOnly Property JournalEntryContent() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _JournalEntryContent
            End Get
        End Property

        Public ReadOnly Property JournalEntryDocNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _JournalEntryDocNumber.Trim
            End Get
        End Property

        Public Property UnitValueChange() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_UnitValueChange, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If UnitValueChangeIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If CRound(_UnitValueChange, ROUNDUNITASSET) <> CRound(value, ROUNDUNITASSET) Then

                    _UnitValueChange = CRound(value, ROUNDUNITASSET)
                    _TotalValueChange = CRound(_UnitValueChange * _CurrentAssetAmmount, 2)
                    PropertyHasChanged()
                    PropertyHasChanged("TotalValueChange")

                    If _Type = LtaOperationType.ValueChange Then
                        _RevaluedPortionTotalValueChange = CRound(value * _CurrentAssetAmmount)
                        _RevaluedPortionUnitValueChange = CRound(value, ROUNDUNITASSET)
                        PropertyHasChanged("RevaluedPortionUnitValueChange")
                        PropertyHasChanged("RevaluedPortionTotalValueChange")
                    End If

                    Recalculate(True)

                End If
            End Set
        End Property

        Public Property TotalValueChange() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_TotalValueChange)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If TotalValueChangeIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If CRound(_TotalValueChange) <> CRound(value) Then

                    _TotalValueChange = CRound(value)
                    PropertyHasChanged()

                    If _Type = LtaOperationType.ValueChange Then
                        _RevaluedPortionTotalValueChange = CRound(value)
                        PropertyHasChanged("RevaluedPortionTotalValueChange")
                    End If

                    Recalculate(True)

                End If
            End Set
        End Property

        Public Property AmmountChange() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AmmountChange
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If AmmountChangeIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _AmmountChange <> value Then

                    _AmmountChange = value

                    If (_CurrentAssetAmmount - _AmmountChange) < 1 Then
                        _TotalValueChange = -CRound(_CurrentAssetValue)
                        _RevaluedPortionTotalValueChange = -CRound(_CurrentAssetValueRevaluedPortion)
                    Else
                        _TotalValueChange = -CRound(_AmmountChange * _CurrentAssetValuePerUnit)
                        _RevaluedPortionTotalValueChange = -CRound( _
                            _CurrentAssetValueRevaluedPortionPerUnit * _AmmountChange)
                    End If

                    PropertyHasChanged()
                    PropertyHasChanged("TotalValueChange")
                    PropertyHasChanged("RevaluedPortionTotalValueChange")

                    Recalculate(True)

                End If
            End Set
        End Property

        Public Property RevaluedPortionUnitValueChange() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_RevaluedPortionUnitValueChange, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If RevaluedPortionUnitValueChangeIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If CRound(_RevaluedPortionUnitValueChange, ROUNDUNITASSET) <> CRound(value, ROUNDUNITASSET) Then

                    _RevaluedPortionUnitValueChange = CRound(value, ROUNDUNITASSET)

                    PropertyHasChanged()

                    If _Type = LtaOperationType.ValueChange Then
                        _UnitValueChange = CRound(value, ROUNDUNITASSET)
                        PropertyHasChanged("UnitValueChange")
                    End If

                    Recalculate(True)

                End If
            End Set
        End Property

        Public Property RevaluedPortionTotalValueChange() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _RevaluedPortionTotalValueChange
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If RevaluedPortionTotalValueChangeIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If CRound(_RevaluedPortionTotalValueChange) <> CRound(value) Then

                    _RevaluedPortionTotalValueChange = CRound(value)

                    PropertyHasChanged()

                    If _Type = LtaOperationType.ValueChange Then
                        _TotalValueChange = CRound(value)
                        PropertyHasChanged("TotalValueChange")
                    End If

                    Recalculate(True)

                End If
            End Set
        End Property

        Public Property NewAmortizationPeriod() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _NewAmortizationPeriod
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If NewAmortizationPeriodIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _NewAmortizationPeriod <> value Then
                    _NewAmortizationPeriod = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AmortizationCalculations() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AmortizationCalculations
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If AmortizationCalculationsIsReadOnly Then
                    PropertyHasChanged()
                    Exit Property
                End If
                If _AmortizationCalculations.Trim <> value.Trim Then
                    _AmortizationCalculations = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property AmortizationCalculatedForMonths() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AmortizationCalculatedForMonths
            End Get
        End Property


        Public ReadOnly Property AfterOperationAcquisitionAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAcquisitionAccountValue)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAcquisitionAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAmortizationAccountValue)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueDecreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueDecreaseAccountValue)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueDecreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueIncreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueIncreaseAccountValue)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueIncreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueIncreaseAmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueIncreaseAmortizationAccountValue)
            End Get
        End Property

        Public ReadOnly Property AfterOperationValueIncreaseAmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public ReadOnly Property AfterOperationAssetAmmount() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AfterOperationAssetAmmount
            End Get
        End Property

        Public Property AfterOperationAssetValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAssetValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If AfterOperationAssetValueIsReadOnly Then
                    Exit Property
                End If
                If CRound(_CurrentAssetValue + _TotalValueChange) <> CRound(value) Then
                    TotalValueChange = CRound(value - _CurrentAssetValue)
                End If
            End Set
        End Property

        Public Property AfterOperationAssetValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAssetValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If AfterOperationAssetValuePerUnitIsReadOnly Then
                    Exit Property
                End If
                If CRound(_CurrentAssetValuePerUnit + _UnitValueChange, ROUNDUNITASSET) <> CRound(value) Then
                    UnitValueChange = CRound(value - _CurrentAssetValuePerUnit, ROUNDUNITASSET)
                End If
            End Set
        End Property

        Public Property AfterOperationAssetValueRevaluedPortion() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAssetValueRevaluedPortion)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If AfterOperationAssetValueRevaluedPortionIsReadOnly Then
                    Exit Property
                End If
                If CRound(_CurrentAssetValueRevaluedPortion + _RevaluedPortionTotalValueChange) <> CRound(value) Then

                    RevaluedPortionTotalValueChange = CRound(value - _CurrentAssetValueRevaluedPortion)

                End If
            End Set
        End Property

        Public Property AfterOperationAssetValueRevaluedPortionPerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AfterOperationAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If AfterOperationAssetValueRevaluedPortionPerUnitIsReadOnly Then
                    Exit Property
                End If
                If CRound(_CurrentAssetValueRevaluedPortionPerUnit + _RevaluedPortionUnitValueChange, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) Then

                    RevaluedPortionUnitValueChange = CRound(value _
                        - _CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)

                End If
            End Set
        End Property



        Public Sub SetAttachedJournalEntry(ByVal JournalEntryInfo As ActiveReports.JournalEntryInfo)
            SetAttachedJournalEntry(JournalEntryInfo.Id, JournalEntryInfo.Content, _
                JournalEntryInfo.DocType, JournalEntryInfo.DocNumber)
        End Sub

        Public Sub SetAttachedJournalEntry(ByVal nJournalEntryID As Integer, _
            ByVal nJournalEntryContent As String, ByVal nJournalEntryType As DocumentType, _
            ByVal nJournalEntryDocNumber As String)

            If Me.IsChild Then Throw New Exception("Klaida. Objektas yra sąskaitos faktūros dalis.")

            If _Type <> LtaOperationType.AcquisitionValueIncrease AndAlso _
                _Type <> LtaOperationType.Transfer AndAlso _Type <> LtaOperationType.ValueChange Then _
                Throw New Exception("Klaida. Susietas bendrojo žurnalo įrašas gali būti " _
                & "priskiriamas tik įsigijimo savikainos padidinimo, pervertinimo ir perleidimo operacijoms.")

            If Not nJournalEntryID > 0 Then
                _JournalEntryType = DocumentType.None
                _JournalEntryContent = ""
                _JournalEntryID = -1
                _JournalEntryDocNumber = ""
                PropertyHasChanged("JournalEntryID")
                PropertyHasChanged("JournalEntryContent")
                PropertyHasChanged("JournalEntryType")
                PropertyHasChanged("JournalEntryDocNumber")
                Exit Sub
            End If

            If nJournalEntryID = _AssetAquisitionOpID Then Throw New Exception( _
                "Nurodyta bendrojo žurnalo operacija (dokumentas) yra " _
                & "šio turto įsigijimo pagrindas. Ta pati operacija negali pagrįsti ir " _
                & "turto įsigijimo, ir kitos operacijos su tuo turtu.")

            If (_Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.AcquisitionValueIncrease) AndAlso _
                (nJournalEntryType = DocumentType.InvoiceMade OrElse _
                nJournalEntryType = DocumentType.InvoiceReceived) Then _
                Throw New Exception("Turto perleidimo arba įsigijimo vertės pokyčio operacijas, " _
                    & "pagrįstas sąskaitomis - faktūromis, galima tvarkyti tik išrašant arba " _
                    & "keičiant sąskaitas - faktūras.")

            _JournalEntryType = nJournalEntryType
            _JournalEntryContent = nJournalEntryContent
            _JournalEntryID = nJournalEntryID
            _JournalEntryDocNumber = nJournalEntryDocNumber
            PropertyHasChanged("JournalEntryID")
            PropertyHasChanged("JournalEntryContent")
            PropertyHasChanged("JournalEntryType")
            PropertyHasChanged("JournalEntryDocNumber")

        End Sub


        ''' <summary>
        ''' Calculates amortization for an amortization operation and updates properties.
        ''' </summary>
        ''' <remarks>Uses LongTermAssetAmortizationCalculation.GetLongTermAssetAmortizationCalculation
        ''' call synchronously which can be a lengthy operation. For asynchronous call use 
        ''' SetAmortizationCalculation instead. </remarks>
        Public Sub CalculateAmortization()

            If _Type <> LtaOperationType.Amortization Then Throw New Exception( _
                "Klaida. Amortizaciją leidžiama skaičiuoti, tik jei operacijos tipas yra amortizacija.")
            If Not _ChronologyValidator.FinancialDataCanChange Then Throw New Exception( _
                _ChronologyValidator.FinancialDataCanChangeExplanation)

            Dim AmortizationCalculation As LongTermAssetAmortizationCalculation
            AmortizationCalculation = LongTermAssetAmortizationCalculation. _
                GetLongTermAssetAmortizationCalculation(_AssetID, _ID, _Date)

            SetAmortizationCalculation(AmortizationCalculation)

        End Sub

        ''' <summary>
        ''' Updates properties with the AmortizationCalculation data.
        ''' </summary>
        Public Sub SetAmortizationCalculation(ByVal AmortizationCalculation _
            As LongTermAssetAmortizationCalculation)

            If _Type <> LtaOperationType.Amortization Then Throw New Exception( _
                "Klaida. Amortizaciją leidžiama skaičiuoti, tik jei operacijos tipas yra amortizacija.")

            If Not _ChronologyValidator.FinancialDataCanChange Then Throw New Exception( _
                _ChronologyValidator.FinancialDataCanChangeExplanation)

            _UnitValueChange = -AmortizationCalculation.AmmortizationValuePerUnit
            _TotalValueChange = -AmortizationCalculation.AmmortizationValue
            _RevaluedPortionTotalValueChange = -AmortizationCalculation.AmmortizationValueRevaluedPortion
            _RevaluedPortionUnitValueChange = -AmortizationCalculation.AmmortizationValuePerUnitRevaluedPortion
            _AmortizationCalculatedForMonths = AmortizationCalculation.AmortizationCalculatedForMonths
            _AmortizationCalculations = AmortizationCalculation.CalculationDescription

            PropertyHasChanged("UnitValueChange")
            PropertyHasChanged("TotalValueChange")
            PropertyHasChanged("RevaluedPortionTotalValueChange")
            PropertyHasChanged("RevaluedPortionUnitValueChange")
            PropertyHasChanged("AmortizationCalculatedForMonths")
            PropertyHasChanged("AmortizationCalculations")
            Recalculate(True)

        End Sub


        Private Sub OnTypeChanged()

            _AmmountChange = 0
            _UnitValueChange = 0
            _TotalValueChange = 0
            _RevaluedPortionUnitValueChange = 0
            _RevaluedPortionTotalValueChange = 0
            _AmortizationCalculations = ""
            _AccountCorresponding = 0
            _ActNumber = 0
            If _Type = LtaOperationType.AmortizationPeriod Then
                _NewAmortizationPeriod = _CurrentAmortizationPeriod
            Else
                _NewAmortizationPeriod = 0
            End If
            _JournalEntryID = -1
            _JournalEntryType = DocumentType.None
            _JournalEntryContent = ""
            _JournalEntryDocNumber = ""

            PropertyHasChanged("JournalEntryContent")
            PropertyHasChanged("JournalEntryID")
            PropertyHasChanged("JournalEntryDocNumber")
            PropertyHasChanged("AmortizationCalculations")
            PropertyHasChanged("AmmountChange")
            PropertyHasChanged("UnitValueChange")
            PropertyHasChanged("TotalValueChange")
            PropertyHasChanged("Ammount")
            PropertyHasChanged("UnitValue")
            PropertyHasChanged("TotalValue")
            PropertyHasChanged("RevaluedPortionUnitValueChange")
            PropertyHasChanged("RevaluedPortionTotalValueChange")
            PropertyHasChanged("AccountCorresponding")
            PropertyHasChanged("ActNumber")
            PropertyHasChanged("NewAmortizationPeriod")

            Recalculate(True)

            _ChronologyValidator.ReconfigureForType(_Type, _AccountChangeType)

            ValidationRules.CheckRules()

        End Sub

        Private Sub Recalculate(ByVal RaisePropertyChanged As Boolean)

            If _Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.Discard Then

                If _AmmountChange < 1 Then
                    _AfterOperationAcquisitionAccountValue = CRound(_CurrentAcquisitionAccountValue)
                ElseIf _AmmountChange = _CurrentAssetAmmount Then
                    _AfterOperationAcquisitionAccountValue = 0
                Else
                    _AfterOperationAcquisitionAccountValue = CRound(_CurrentAcquisitionAccountValue _
                        - CRound(_AmmountChange * _CurrentAcquisitionAccountValuePerUnit))
                End If

            ElseIf _Type = LtaOperationType.AcquisitionValueIncrease Then

                _AfterOperationAcquisitionAccountValue = CRound(_CurrentAcquisitionAccountValue + _
                    _TotalValueChange - _RevaluedPortionTotalValueChange)

            Else

                _AfterOperationAcquisitionAccountValue = CRound(_CurrentAcquisitionAccountValue)

            End If

            If _Type = LtaOperationType.AcquisitionValueIncrease Then

                _AfterOperationAcquisitionAccountValuePerUnit = CRound(_CurrentAcquisitionAccountValuePerUnit _
                    + _UnitValueChange - _RevaluedPortionUnitValueChange, ROUNDUNITASSET)

            Else

                _AfterOperationAcquisitionAccountValuePerUnit = _
                    CRound(_CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.Discard Then

                If _AmmountChange < 1 Then
                    _AfterOperationAmortizationAccountValue = CRound(_CurrentAmortizationAccountValue)
                ElseIf _AmmountChange = _CurrentAssetAmmount Then
                    _AfterOperationAmortizationAccountValue = 0
                Else
                    _AfterOperationAmortizationAccountValue = CRound(_CurrentAmortizationAccountValue _
                        - CRound(_AmmountChange * _CurrentAmortizationAccountValuePerUnit))
                End If

            ElseIf _Type = LtaOperationType.Amortization Then

                _AfterOperationAmortizationAccountValue = CRound(_CurrentAmortizationAccountValue - _
                    (_TotalValueChange - _RevaluedPortionTotalValueChange))

            Else

                _AfterOperationAmortizationAccountValue = CRound(_CurrentAmortizationAccountValue)

            End If

            If _Type = LtaOperationType.Amortization Then

                _AfterOperationAmortizationAccountValuePerUnit = CRound(_CurrentAmortizationAccountValuePerUnit _
                    - (_UnitValueChange - _RevaluedPortionUnitValueChange), ROUNDUNITASSET)

            Else

                _AfterOperationAmortizationAccountValuePerUnit = _
                    CRound(_CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.Discard Then

                If _AmmountChange < 1 OrElse Not _CurrentValueDecreaseAccountValue > 0 Then
                    _AfterOperationValueDecreaseAccountValue = CRound(_CurrentValueDecreaseAccountValue)
                ElseIf _AmmountChange = _CurrentAssetAmmount Then
                    _AfterOperationValueDecreaseAccountValue = 0
                Else
                    _AfterOperationValueDecreaseAccountValue = CRound(_CurrentValueDecreaseAccountValue _
                        - CRound(_AmmountChange * _CurrentValueDecreaseAccountValuePerUnit))
                End If

            ElseIf _Type = LtaOperationType.ValueChange Then

                If CRound(_CurrentValueIncreaseAccountValue _
                    - _CurrentValueIncreaseAmortizationAccountValue _
                    - _CurrentValueDecreaseAccountValue + _RevaluedPortionTotalValueChange) >= 0 Then

                    _AfterOperationValueDecreaseAccountValue = 0

                Else

                    _AfterOperationValueDecreaseAccountValue = CRound(_CurrentValueDecreaseAccountValue _
                        - _CurrentValueIncreaseAccountValue _
                        + _CurrentValueIncreaseAmortizationAccountValue _
                        - _RevaluedPortionTotalValueChange)

                End If

            Else

                _AfterOperationValueDecreaseAccountValue = CRound(_CurrentValueDecreaseAccountValue)

            End If

            If _Type = LtaOperationType.ValueChange Then

                If CRound(_CurrentValueIncreaseAccountValuePerUnit _
                    - _CurrentValueIncreaseAmortizationAccountValuePerUnit _
                    - _CurrentValueDecreaseAccountValuePerUnit _
                    + _RevaluedPortionUnitValueChange, ROUNDUNITASSET) >= 0 Then

                    _AfterOperationValueDecreaseAccountValuePerUnit = 0

                Else

                    _AfterOperationValueDecreaseAccountValuePerUnit = _
                        CRound(_CurrentValueDecreaseAccountValuePerUnit _
                        - _CurrentValueIncreaseAccountValuePerUnit _
                        + _CurrentValueIncreaseAmortizationAccountValuePerUnit _
                        - _RevaluedPortionUnitValueChange, ROUNDUNITASSET)

                End If

            Else

                _AfterOperationValueDecreaseAccountValuePerUnit = _
                    CRound(_CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.Discard Then

                If _AmmountChange < 1 OrElse Not _CurrentValueIncreaseAccountValue > 0 Then
                    _AfterOperationValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAccountValue)
                ElseIf _AmmountChange = _CurrentAssetAmmount Then
                    _AfterOperationValueIncreaseAccountValue = 0
                Else
                    _AfterOperationValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAccountValue _
                        - CRound(_AmmountChange * _CurrentValueIncreaseAccountValuePerUnit))
                End If

            ElseIf _Type = LtaOperationType.ValueChange Then

                If CRound(_CurrentValueIncreaseAccountValue _
                    - _CurrentValueIncreaseAmortizationAccountValue _
                    - _CurrentValueDecreaseAccountValue + _RevaluedPortionTotalValueChange) <= 0 Then

                    _AfterOperationValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAmortizationAccountValue)

                Else

                    _AfterOperationValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAccountValue _
                        - _CurrentValueDecreaseAccountValue _
                        + _RevaluedPortionTotalValueChange)

                End If

            Else

                _AfterOperationValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAccountValue)

            End If

            If _Type = LtaOperationType.ValueChange Then

                If CRound(_CurrentValueIncreaseAccountValuePerUnit _
                    - _CurrentValueIncreaseAmortizationAccountValuePerUnit _
                    - _CurrentValueDecreaseAccountValuePerUnit _
                    + _RevaluedPortionUnitValueChange, ROUNDUNITASSET) <= 0 Then

                    _AfterOperationValueIncreaseAccountValuePerUnit = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)

                Else

                    _AfterOperationValueIncreaseAccountValuePerUnit = _
                        CRound(_CurrentValueIncreaseAccountValuePerUnit _
                        - _CurrentValueDecreaseAccountValuePerUnit _
                        + _RevaluedPortionUnitValueChange, ROUNDUNITASSET)

                End If

            Else

                _AfterOperationValueIncreaseAccountValuePerUnit = _
                    CRound(_CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Transfer OrElse _Type = LtaOperationType.Discard Then

                If _AmmountChange < 1 Then
                    _AfterOperationValueIncreaseAmortizationAccountValue = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValue)
                ElseIf _AmmountChange = _CurrentAssetAmmount Then
                    _AfterOperationValueIncreaseAmortizationAccountValue = 0
                Else
                    _AfterOperationValueIncreaseAmortizationAccountValue = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValue _
                        - CRound(_AmmountChange * _CurrentValueIncreaseAmortizationAccountValuePerUnit))
                End If

            ElseIf _Type = LtaOperationType.Amortization Then

                _AfterOperationValueIncreaseAmortizationAccountValue = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValue _
                    - _RevaluedPortionTotalValueChange)

            Else

                _AfterOperationValueIncreaseAmortizationAccountValue = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValue)

            End If

            If _Type = LtaOperationType.Amortization Then

                _AfterOperationValueIncreaseAmortizationAccountValuePerUnit = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit _
                    - _RevaluedPortionUnitValueChange, ROUNDUNITASSET)

            Else

                _AfterOperationValueIncreaseAmortizationAccountValuePerUnit = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Discard OrElse _Type = LtaOperationType.Transfer Then
                _AfterOperationAssetAmmount = _CurrentAssetAmmount - _AmmountChange
            Else
                _AfterOperationAssetAmmount = _CurrentAssetAmmount
            End If

            If _Type <> LtaOperationType.AccountChange _
                AndAlso _Type <> LtaOperationType.AmortizationPeriod _
                AndAlso _Type <> LtaOperationType.UsingEnd _
                AndAlso _Type <> LtaOperationType.UsingStart Then

                _AfterOperationAssetValue = CRound(_CurrentAssetValue + _TotalValueChange)

            Else

                _AfterOperationAssetValue = CRound(_CurrentAssetValue)

            End If

            If _Type = LtaOperationType.AcquisitionValueIncrease _
                OrElse _Type = LtaOperationType.Amortization _
                OrElse _Type = LtaOperationType.ValueChange Then

                _AfterOperationAssetValuePerUnit = CRound(_CurrentAssetValuePerUnit _
                    + _UnitValueChange, ROUNDUNITASSET)

            Else

                _AfterOperationAssetValuePerUnit = CRound(_CurrentAssetValuePerUnit, ROUNDUNITASSET)

            End If

            If _Type = LtaOperationType.Amortization _
                OrElse _Type = LtaOperationType.Discard _
                OrElse _Type = LtaOperationType.Transfer _
                OrElse _Type = LtaOperationType.ValueChange Then

                _AfterOperationAssetValueRevaluedPortion = CRound(_CurrentAssetValueRevaluedPortion _
                    + _RevaluedPortionTotalValueChange)

            Else

                _AfterOperationAssetValueRevaluedPortion = CRound(_CurrentAssetValueRevaluedPortion)

            End If

            If _Type = LtaOperationType.Amortization OrElse _Type = LtaOperationType.ValueChange Then

                _AfterOperationAssetValueRevaluedPortionPerUnit = _
                    CRound(_CurrentAssetValueRevaluedPortionPerUnit + _
                    _RevaluedPortionUnitValueChange, ROUNDUNITASSET)

            Else

                _AfterOperationAssetValueRevaluedPortionPerUnit = _
                    CRound(_CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET)

            End If

            If RaisePropertyChanged Then
                PropertyHasChanged("AfterOperationAcquisitionAccountValue")
                PropertyHasChanged("AfterOperationAcquisitionAccountValuePerUnit")
                PropertyHasChanged("AfterOperationAmortizationAccountValue")
                PropertyHasChanged("AfterOperationAmortizationAccountValuePerUnit")
                PropertyHasChanged("AfterOperationValueDecreaseAccountValue")
                PropertyHasChanged("AfterOperationValueDecreaseAccountValuePerUnit")
                PropertyHasChanged("AfterOperationValueIncreaseAccountValue")
                PropertyHasChanged("AfterOperationValueIncreaseAccountValuePerUnit")
                PropertyHasChanged("AfterOperationValueIncreaseAmortizationAccountValue")
                PropertyHasChanged("AfterOperationValueIncreaseAmortizationAccountValuePerUnit")
                PropertyHasChanged("AfterOperationAssetAmmount")
                PropertyHasChanged("AfterOperationAssetValue")
                PropertyHasChanged("AfterOperationAssetValuePerUnit")
                PropertyHasChanged("AfterOperationAssetValueRevaluedPortion")
                PropertyHasChanged("AfterOperationAssetValueRevaluedPortionPerUnit")
            End If

        End Sub


        Friend Sub SetDate(ByVal value As Date)
            If _Date.Date <> value.Date Then
                _Date = value.Date
                PropertyHasChanged("Date")
            End If
        End Sub


        Public Overrides Function Save() As LongTermAssetOperation

            If _Type = LtaOperationType.Transfer AndAlso (_JournalEntryType = DocumentType.InvoiceMade _
                OrElse _JournalEntryType = DocumentType.InvoiceReceived) Then Throw New Exception( _
                "Turto perleidimo operacijas, pagrįstas sąskaitomis - faktūromis, " & _
                "galima tvarkyti tik išrašant arba keičiant sąskaitas - faktūras.")

            ValidationRules.CheckRules()
            If Not Me.IsValid Then Throw New Exception( _
                "Duomenyse yra klaidų: " & vbCrLf & Me.BrokenRulesCollection.ToString)

            Return MyBase.Save()
        End Function

        Friend Sub ForceValidationRulesRecheck()
            ValidationRules.CheckRules()
        End Sub

        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf AccountCorrespondingValidation, "AccountCorresponding")
            ValidationRules.AddRule(AddressOf ActNumberValidation, "ActNumber")
            ValidationRules.AddRule(AddressOf JournalEntryIdValidation, "JournalEntryID")
            ValidationRules.AddRule(AddressOf UnitValueValidation, "UnitValueChange")
            ValidationRules.AddRule(AddressOf TotalValueValidation, "TotalValueChange")
            ValidationRules.AddRule(AddressOf RevaluedPortionUnitValueValidation, "RevaluedPortionUnitValueChange")
            ValidationRules.AddRule(AddressOf RevaluedPortionTotalValueValidation, "RevaluedPortionTotalValueChange")
            ValidationRules.AddRule(AddressOf AmountValidation, "AmmountChange")
            ValidationRules.AddRule(AddressOf AmortizationPeriodValidation, "NewAmortizationPeriod")
            ValidationRules.AddRule(AddressOf OperationTypeValidation, "TypeHumanReadable")
            ValidationRules.AddRule(AddressOf AmortizationCalculatedForMonthsValidation, _
                "AmortizationCalculatedForMonths")

            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("Date", "ChronologyValidator"))

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Content", "operacijos pagrindas (aprašas)"))
            ValidationRules.AddRule(AddressOf CommonValidation.LimitingFactor, _
                New CommonValidation.LimitingFactorRuleArgs("ID", _
                "Ši operacija su turtu yra registruota per suvestinį operacijų su " & _
                "turtu dokumentą (aktą). Jos duomenis galima koreguoti tik koreguojant " & _
                "atitinkamą suvestinį dokumentą (aktą).", "IsComplexAct", _
                Validation.RuleSeverity.Error))

        End Sub

        ''' <summary>
        ''' Rule ensuring that the corresponding account is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AccountCorrespondingValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj._Type = LtaOperationType.AccountChange OrElse _
                ValObj._Type = LtaOperationType.Amortization OrElse _
                ValObj._Type = LtaOperationType.Discard Then

                If Not ValObj._AccountCorresponding > 0 Then

                    If ValObj.Type = LtaOperationType.AccountChange Then
                        e.Description = "Nepasirinkta nauja sąskaita."
                    Else
                        e.Description = "Nepasirinkta koresponduojanti sąnaudų sąskaita."
                    End If

                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                End If

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the act number is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ActNumberValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If (ValObj._Type = LtaOperationType.Discard OrElse ValObj._Type = LtaOperationType.UsingEnd OrElse _
                ValObj._Type = LtaOperationType.UsingStart) AndAlso Not ValObj._ActNumber > 0 Then

                e.Description = "Nenurodytas akto numeris."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that the related general ledger entry is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function JournalEntryIdValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj.IsChild Then Return True

            If ValObj._Type = LtaOperationType.ValueChange OrElse _
                ValObj._Type = LtaOperationType.AcquisitionValueIncrease OrElse _
                ValObj._Type = LtaOperationType.Transfer Then

                If Not ValObj._JournalEntryID > 0 Then
                    e.Description = "Nenurodyta pagrindžianti bendrojo žurnalo operacija (dokumentas)."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf ValObj._JournalEntryID = ValObj._AssetAquisitionOpID Then
                    e.Description = "Nurodyta bendrojo žurnalo operacija (dokumentas) yra turto " _
                    & "šio turto įsigijimo pagrindas. Ta pati operacija negali pagrįsti ir " _
                    & "turto įsigijimo, ir kitos operacijos su tuo turtu."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                End If

            End If

            If ValObj._Type = LtaOperationType.Transfer AndAlso _
                (ValObj._JournalEntryType = DocumentType.InvoiceMade OrElse _
                ValObj._JournalEntryType = DocumentType.InvoiceReceived) Then

                e.Description = "Turto perleidimo operacijas, pagrįstas sąskaitomis - faktūromis, " & _
                    "galima tvarkyti tik išrašant arba keičiant sąskaitas - faktūras."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the unit value is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function UnitValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj._Type = LtaOperationType.AcquisitionValueIncrease _
                OrElse ValObj._Type = LtaOperationType.Amortization OrElse _
                ValObj._Type = LtaOperationType.ValueChange Then

                If CRound(ValObj._UnitValueChange, ROUNDUNITASSET) = 0 Then
                    e.Description = "Nenurodytas turto vieneto vertės pokytis."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf Not CRound(ValObj._UnitValueChange, ROUNDUNITASSET) < 0 AndAlso _
                    ValObj._Type = LtaOperationType.Amortization Then
                    e.Description = "Vieneto vertės pokytis po amortizacijos negali būti teigiamas."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf Not CRound(ValObj._UnitValueChange, ROUNDUNITASSET) > 0 AndAlso _
                    ValObj._Type = LtaOperationType.AcquisitionValueIncrease Then
                    e.Description = "Vieneto vertės pokytis po įsigijimo savikainos padidinimo negali būti neigiamas."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf CRound(ValObj._AfterOperationAssetValuePerUnit, ROUNDUNITASSET) _
                    < CRound(ValObj._AssetLiquidationValue, ROUNDUNITASSET) Then
                    e.Description = "Turto vieneto vertė negali būti sumažinta žemiau likvidacinės."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                End If

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the total value is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function TotalValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If (ValObj.Type = LtaOperationType.AcquisitionValueIncrease OrElse _
                ValObj.Type = LtaOperationType.Amortization OrElse _
                ValObj.Type = LtaOperationType.Discard OrElse ValObj._Type = LtaOperationType.Transfer _
                OrElse ValObj._Type = LtaOperationType.ValueChange) Then

                If CRound(ValObj._TotalValueChange) = 0 Then

                    e.Description = "Nenurodytas bendras turto vertės pokytis."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf Not CRound(ValObj._TotalValueChange) < 0 AndAlso _
                    (ValObj._Type = LtaOperationType.Amortization _
                    OrElse ValObj._Type = LtaOperationType.Discard _
                    OrElse ValObj._Type = LtaOperationType.Transfer) Then

                    e.Description = "Vertės pokytis po amortizacijos, perleidimo arba nurašymo negali būti teigiamas."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf Not CRound(ValObj._TotalValueChange) > 0 AndAlso _
                    ValObj._Type = LtaOperationType.AcquisitionValueIncrease Then

                    e.Description = "Vertės pokytis po įsigijimo savikainos padidinimo negali būti neigiamas."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                End If

                Dim nAfterOperationValue As Double = ValObj.AfterOperationAssetValue

                If (ValObj.Type = LtaOperationType.AcquisitionValueIncrease OrElse _
                    ValObj.Type = LtaOperationType.Amortization OrElse _
                    ValObj._Type = LtaOperationType.ValueChange) _
                    AndAlso CRound(nAfterOperationValue) < _
                    CRound(ValObj._AssetLiquidationValue * ValObj.CurrentAssetAmmount) Then

                    e.Description = "Turto vertė po operacijos negali sumažėti mažiau likvidacinės."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                End If

                If (ValObj.Type = LtaOperationType.Discard OrElse ValObj._Type = LtaOperationType.Transfer) _
                    AndAlso CRound(nAfterOperationValue) < 0 Then

                    e.Description = "Turto vertė po operacijos negali sumažėti mažiau nulio."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                End If

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the revalued portion unit value is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function RevaluedPortionUnitValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If (ValObj._Type = LtaOperationType.Amortization AndAlso _
                CRound(ValObj._CurrentAssetValueRevaluedPortionPerUnit, ROUNDUNITASSET) <> 0) _
                OrElse ValObj._Type = LtaOperationType.ValueChange Then

                If CRound(ValObj._RevaluedPortionUnitValueChange, ROUNDUNITASSET) = 0 Then
                    e.Description = "Nenurodytas turto perkainotos dalies vieneto vertės pokytis."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf CRound(ValObj._AfterOperationAssetValuePerUnit, ROUNDUNITASSET) _
                    < CRound(ValObj._AssetLiquidationValue, ROUNDUNITASSET) Then
                    e.Description = "Turto vieneto vertė negali būti sumažinta žemiau likvidacinės."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf ValObj._Type = LtaOperationType.Amortization AndAlso _
                    Not CRound(ValObj._RevaluedPortionUnitValueChange, ROUNDUNITASSET) < 0 Then
                    e.Description = "Turto vieneto perkainotos dalies vertės pokytis po amortizacijos negali būti teigiamas."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                End If

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that the revalued portion total value is provided when necessary.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function RevaluedPortionTotalValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ((ValObj._Type = LtaOperationType.Amortization _
                OrElse ValObj._Type = LtaOperationType.Discard _
                OrElse ValObj._Type = LtaOperationType.Transfer) AndAlso _
                CRound(ValObj._CurrentAssetValueRevaluedPortion) <> 0) _
                OrElse ValObj._Type = LtaOperationType.ValueChange Then

                If CRound(ValObj._RevaluedPortionTotalValueChange) = 0 Then

                    e.Description = "Nenurodytas bendras turto pervertintos dalies vertės pokytis."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf Not CRound(ValObj._RevaluedPortionTotalValueChange) < 0 AndAlso _
                    ValObj._Type = LtaOperationType.Amortization Then

                    e.Description = "Bendras turto pervertintos dalies vertės pokytis po " _
                        & "amortizacijos negali būti teigiamas." _
                    & " (" & ValObj._AssetName & ")"
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf (ValObj._Type = LtaOperationType.Discard _
                    OrElse ValObj._Type = LtaOperationType.Transfer) AndAlso _
                    ((CRound(ValObj._CurrentAssetValueRevaluedPortion) > 0 AndAlso _
                    CRound(ValObj._RevaluedPortionTotalValueChange) > 0) OrElse _
                    (CRound(ValObj._CurrentAssetValueRevaluedPortion) < 0 AndAlso _
                    CRound(ValObj._RevaluedPortionTotalValueChange) < 0)) Then

                    e.Description = "Bendras turto pervertintos dalies vertės pokytis po " _
                        & "nurašymo arba perleidimo negali būti teigiamas." _
                    & " (" & ValObj._AssetName & ")"
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf (ValObj._Type = LtaOperationType.Amortization _
                    OrElse ValObj._Type = LtaOperationType.ValueChange) _
                    AndAlso ValObj.AfterOperationAssetValue < CRound(ValObj._AssetLiquidationValue _
                    * ValObj._CurrentAssetAmmount) Then

                    e.Description = "Turto vertė po operacijos negali sumažėti mažiau likvidacinės vertės."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                ElseIf (ValObj._Type = LtaOperationType.Transfer _
                    OrElse ValObj._Type = LtaOperationType.Discard) _
                    AndAlso ValObj.AfterOperationAssetValue < 0 Then

                    e.Description = "Turto vertė po operacijos negali sumažėti mažiau nulio."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False

                End If

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that an operation can only be added or updated if current ammount
        ''' of the asset > 0.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AmountValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj.IsNew AndAlso Not ValObj._CurrentAssetAmmount > 0 Then

                e.Description = "Visas ilgalaikis turtas yra perleistas (nurašytas) ." & _
                    "Operacijų su juo įvesti ar taisyti nėra leidžiama."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False

            End If

            If (ValObj._Type = LtaOperationType.Discard OrElse ValObj.Type = LtaOperationType.Transfer) Then

                If Not ValObj._AmmountChange > 0 Then
                    e.Description = "Nenurodytas IT kiekis."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                ElseIf ValObj._AmmountChange > ValObj._CurrentAssetAmmount Then
                    e.Description = "Nepakanka IT kiekio."
                    e.Severity = Csla.Validation.RuleSeverity.Error
                    Return False
                End If

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that a new amortization period is valid.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AmortizationPeriodValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj._Type = LtaOperationType.AmortizationPeriod AndAlso Not ValObj._NewAmortizationPeriod > 0 Then
                e.Description = "Nenurodytas naujas amortizacijos laikotarpis."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False
            End If

            If ValObj._Type = LtaOperationType.AmortizationPeriod AndAlso _
                ValObj._NewAmortizationPeriod < Math.Floor(ValObj._CurrentUsageTermMonths / 12) Then
                e.Description = "Naujas amortizacijos laikotarpis negali būti trumpesnis nei sukaupta amortizacija."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring that an operation type is valid (for usage operation types).
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function OperationTypeValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj._Type = LtaOperationType.UsingStart AndAlso ValObj._CurrentUsageStatus Then
                e.Description = "Turtas jau perduotas naudojimui."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False
            End If

            If ValObj._Type = LtaOperationType.UsingEnd AndAlso Not ValObj._CurrentUsageStatus Then
                e.Description = "Turtas ir taip nėra naudojamas."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring that amortization can only be calculated for a month or more.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AmortizationCalculatedForMonthsValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAssetOperation = DirectCast(target, LongTermAssetOperation)

            If ValObj._Type = LtaOperationType.Amortization AndAlso _
                Not ValObj._AmortizationCalculatedForMonths > 0 Then
                e.Description = "Amortizacija negali būti paskaičiuota mažesniam nei mėnesio laikotarpiui."
                e.Severity = Csla.Validation.RuleSeverity.Error
                Return False
            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Assets.LongTermAssetOperation2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAssetOperation2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAssetOperation1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAssetOperation3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAssetOperation3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function NewLongTermAssetOperation(ByVal AssetId As Integer) As LongTermAssetOperation
            Return DataPortal.Create(Of LongTermAssetOperation) _
                (New Criteria(AssetId, LtaOperationType.Discard))
        End Function

        Friend Shared Function NewLongTermAssetOperationChild(ByVal AssetId As Integer, _
            ByVal nNewOperationType As LtaOperationType, ByVal parentValidator As IChronologicValidator) As LongTermAssetOperation
            Return New LongTermAssetOperation(AssetId, nNewOperationType, parentValidator)
        End Function

        Public Shared Function GetLongTermAssetOperation(ByVal id As Integer, _
            ByVal nFetchByJournalEntryID As Boolean) As LongTermAssetOperation
            Return DataPortal.Fetch(Of LongTermAssetOperation)(New Criteria(id, nFetchByJournalEntryID))
        End Function

        Friend Shared Function GetLongTermAssetOperationChild(ByVal id As Integer, _
            ByVal parentValidator As IChronologicValidator) As LongTermAssetOperation
            Return New LongTermAssetOperation(id, parentValidator)
        End Function

        Public Shared Sub DeleteLongTermAssetOperation(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub

        Friend Shared Sub DeleteLongTermAssetOperationChild(ByVal id As Integer)
            DoDelete(id)
        End Sub


        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal AssetId As Integer, ByVal nNewOperationType As LtaOperationType, _
            ByVal parentValidator As IChronologicValidator)
            MarkAsChild()
            Create(AssetId, nNewOperationType, parentValidator)
        End Sub

        Private Sub New(ByVal id As Integer, ByVal parentValidator As IChronologicValidator)
            MarkAsChild()
            Fetch(id, parentValidator)
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private mId As Integer
            Private _FetchByJournalEntryID As Boolean
            Private _NewOperationType As LtaOperationType
            Public ReadOnly Property Id() As Integer
                Get
                    Return mId
                End Get
            End Property
            Public ReadOnly Property FetchByJournalEntryID() As Boolean
                Get
                    Return _FetchByJournalEntryID
                End Get
            End Property
            Public ReadOnly Property NewOperationType() As LtaOperationType
                Get
                    Return _NewOperationType
                End Get
            End Property
            Public Sub New(ByVal id As Integer)
                mId = id
                _FetchByJournalEntryID = False
                _NewOperationType = LtaOperationType.Discard
            End Sub
            Public Sub New(ByVal id As Integer, ByVal nFetchByJournalEntryID As Boolean)
                mId = id
                _FetchByJournalEntryID = nFetchByJournalEntryID
                _NewOperationType = LtaOperationType.Discard
            End Sub
            Public Sub New(ByVal Assetid As Integer, ByVal nNewOperationType As LtaOperationType)
                mId = Assetid
                _FetchByJournalEntryID = False
                _NewOperationType = nNewOperationType
            End Sub
        End Class


        Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Jūsų teisių nepakanka naujos IT operacijos įtraukimui.")
            Create(criteria.Id, criteria.NewOperationType, Nothing)
        End Sub

        Private Sub Create(ByVal AssetId As Integer, ByVal nNewOperationType As LtaOperationType, _
            ByVal parentValidator As IChronologicValidator)
            _Type = nNewOperationType
            GetOperationBackgroundInfo(AssetId, -1, Today.AddYears(100))
            Recalculate(False)
            _ChronologyValidator = OperationChronologicValidator.NewOperationChronologicValidator( _
                _AssetID, _AssetName, _Type, _AccountChangeType, _AssetDateAcquired, parentValidator)
            ValidationRules.CheckRules()
        End Sub


        Private Sub GetOperationBackgroundInfo(ByVal nAssetId As Integer, _
            ByVal nOperationID As Integer, ByVal nOperationDate As Date)

            Dim myComm As New SQLCommand("FetchLongTermAssetOperationBackgroundInfo")
            myComm.AddParam("?TD", nAssetId)
            myComm.AddParam("?OD", nOperationID)
            myComm.AddParam("?DT", nOperationDate)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception( _
                    "Klaida. Turto objektas, kurio ID=" & nOperationID.ToString & ", nerastas.")

                _AssetID = nAssetId

                Dim dr As DataRow = myData.Rows(0)

                _AssetName = CStrSafe(dr.Item(0)).Trim
                _AssetMeasureUnit = CStrSafe(dr.Item(1)).Trim
                _AssetAcquiredAccount = CLongSafe(dr.Item(2))
                _AssetContraryAccount = CLongSafe(dr.Item(3))
                _AssetValueDecreaseAccount = CLongSafe(dr.Item(4), 0)
                _AssetValueIncreaseAccount = CLongSafe(dr.Item(5))
                _AssetValueIncreaseAmortizationAccount = CLongSafe(dr.Item(6), 0)
                _AssetDateAcquired = CDate(dr.Item(7))
                _AssetAquisitionOpID = CIntSafe(dr.Item(8), 0)
                _AssetLiquidationValue = CDblSafe(dr.Item(9), 2, 0)
                _CurrentUsageStatus = ConvertDbBoolean(CIntSafe(dr.Item(10), 0))
                _CurrentAssetAmmount = CIntSafe(dr.Item(11), 0)
                _CurrentAcquisitionAccountValuePerUnit = CDblSafe(dr.Item(12), ROUNDUNITASSET, 0)
                _CurrentAcquisitionAccountValue = CDblSafe(dr.Item(13), 2, 0)
                If CDbl(dr.Item(14)) < 0 Then
                    _CurrentValueDecreaseAccountValuePerUnit = -CDblSafe(dr.Item(14), ROUNDUNITASSET, 0)
                    _CurrentValueIncreaseAccountValuePerUnit = 0
                ElseIf CDbl(dr.Item(14)) > 0 Then
                    _CurrentValueDecreaseAccountValuePerUnit = 0
                    _CurrentValueIncreaseAccountValuePerUnit = CDblSafe(dr.Item(14), ROUNDUNITASSET, 0)
                Else
                    _CurrentValueDecreaseAccountValuePerUnit = 0
                    _CurrentValueIncreaseAccountValuePerUnit = 0
                End If
                If CDbl(dr.Item(15)) < 0 Then
                    _CurrentValueDecreaseAccountValue = -CDblSafe(dr.Item(15), 2, 0)
                    _CurrentValueIncreaseAccountValue = 0
                ElseIf CDbl(dr.Item(15)) > 0 Then
                    _CurrentValueDecreaseAccountValue = 0
                    _CurrentValueIncreaseAccountValue = CDblSafe(dr.Item(15), 2, 0)
                Else
                    _CurrentValueDecreaseAccountValue = 0
                    _CurrentValueIncreaseAccountValue = 0
                End If
                _CurrentAmortizationAccountValuePerUnit = CDblSafe(dr.Item(16), ROUNDUNITASSET, 0)
                _CurrentAmortizationAccountValue = CDblSafe(dr.Item(17), 2, 0)
                _CurrentValueIncreaseAmortizationAccountValuePerUnit = CDblSafe(dr.Item(18), ROUNDUNITASSET, 0)
                _CurrentValueIncreaseAmortizationAccountValue = CDblSafe(dr.Item(19), 2, 0)
                _CurrentAmortizationPeriod = CIntSafe(dr.Item(20), 0)
                _CurrentUsageTermMonths = CIntSafe(dr.Item(21), 0)

                _CurrentValueIncreaseAccountValue = CRound(_CurrentValueIncreaseAccountValue + _
                    _CurrentValueIncreaseAmortizationAccountValue)
                _CurrentValueIncreaseAccountValuePerUnit = CRound(_CurrentValueIncreaseAccountValuePerUnit + _
                    _CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)

                _CurrentAssetAmmount = _CurrentAssetAmmount + CIntSafe(dr.Item(22), 0)
                _CurrentAcquisitionAccountValue = _
                    CRound(_CurrentAcquisitionAccountValue + CDblSafe(dr.Item(27), 2, 0))
                _CurrentAcquisitionAccountValuePerUnit = _
                    CRound(_CurrentAcquisitionAccountValuePerUnit + _
                    CDblSafe(dr.Item(28), ROUNDUNITASSET, 0), ROUNDUNITASSET)
                _CurrentAmortizationAccountValue = _
                    CRound(_CurrentAmortizationAccountValue + CDblSafe(dr.Item(29), 2, 0))
                _CurrentAmortizationAccountValuePerUnit = _
                    CRound(_CurrentAmortizationAccountValuePerUnit _
                    + CDblSafe(dr.Item(30), ROUNDUNITASSET, 0), ROUNDUNITASSET)
                _CurrentValueDecreaseAccountValue = CRound(_CurrentValueDecreaseAccountValue + _
                    CDblSafe(dr.Item(31), 2, 0))
                _CurrentValueDecreaseAccountValuePerUnit = _
                    CRound(_CurrentValueDecreaseAccountValuePerUnit _
                    + CDblSafe(dr.Item(32), ROUNDUNITASSET, 0), ROUNDUNITASSET)
                _CurrentValueIncreaseAccountValue = _
                    CRound(_CurrentValueIncreaseAccountValue + CDblSafe(dr.Item(33), 2, 0))
                _CurrentValueIncreaseAccountValuePerUnit = _
                    CRound(_CurrentValueIncreaseAccountValuePerUnit _
                    + CDblSafe(dr.Item(34), ROUNDUNITASSET, 0), ROUNDUNITASSET)
                _CurrentValueIncreaseAmortizationAccountValue = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValue + CDblSafe(dr.Item(35), 2, 0))
                _CurrentValueIncreaseAmortizationAccountValuePerUnit = _
                    CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit _
                    + CDblSafe(dr.Item(36), ROUNDUNITASSET, 0), ROUNDUNITASSET)
                _CurrentUsageTermMonths = _CurrentUsageTermMonths + CIntSafe(dr.Item(37), 0)
                If (Math.Floor(CIntSafe(dr.Item(38), 0) / 2) <> Math.Ceiling(CIntSafe(dr.Item(38), 0) / 2)) Then _
                    _CurrentUsageStatus = Not _CurrentUsageStatus

            End Using

            _CurrentAssetValue = CRound(_CurrentAcquisitionAccountValue - _
                _CurrentAmortizationAccountValue - _CurrentValueDecreaseAccountValue + _
                _CurrentValueIncreaseAccountValue - _CurrentValueIncreaseAmortizationAccountValue)
            _CurrentAssetValuePerUnit = CRound(_CurrentAcquisitionAccountValuePerUnit - _
                _CurrentAmortizationAccountValuePerUnit - _CurrentValueDecreaseAccountValuePerUnit + _
                _CurrentValueIncreaseAccountValuePerUnit - _
                _CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            If _CurrentValueDecreaseAccountValue > 0 Then
                _CurrentAssetValueRevaluedPortion = -CRound(_CurrentValueDecreaseAccountValue)
                _CurrentAssetValueRevaluedPortionPerUnit = _
                    -CRound(_CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            ElseIf _CurrentValueIncreaseAccountValue > 0 Then
                _CurrentAssetValueRevaluedPortion = CRound(_CurrentValueIncreaseAccountValue _
                    - _CurrentValueIncreaseAmortizationAccountValue)
                _CurrentAssetValueRevaluedPortionPerUnit = CRound(_CurrentValueIncreaseAccountValuePerUnit _
                    - _CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET)
            Else
                _CurrentAssetValueRevaluedPortion = 0
                _CurrentAssetValueRevaluedPortionPerUnit = 0
            End If

        End Sub


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Jūsų teisių nepakanka šiems duomenims gauti.")

            Dim myComm As SQLCommand
            Dim IdToFetch As Integer = criteria.Id

            If criteria.FetchByJournalEntryID Then

                myComm = New SQLCommand("FetchLongTermAssetOperationIdByJournalEntryId")
                myComm.AddParam("?JD", criteria.Id)

                Using myData As DataTable = myComm.Fetch

                    If myData.Rows.Count < 1 OrElse Not CIntSafe(myData.Rows(0).Item(0), 0) > 0 Then _
                        Throw New Exception("Klaida. Nerasti turto operacijos duomenys pagal BŽ ID=" & _
                        criteria.Id.ToString & ".")

                    IdToFetch = CIntSafe(myData.Rows(0).Item(0), 0)

                End Using

            End If

            Fetch(IdToFetch, Nothing)

        End Sub

        Private Sub Fetch(ByVal OperationID As Integer, ByVal parentValidator As IChronologicValidator)

            Dim myComm As New SQLCommand("FetchLongTermAssetOperation")
            myComm.AddParam("?OD", OperationID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception( _
                    "Klaida. Nerasti turto operacijos duomenys pagal BŽ ID=" & OperationID.ToString & ".")

                Dim dr As DataRow = myData.Rows(0)

                _AssetID = CIntSafe(dr.Item(0), 0)
                _Type = ConvertEnumDatabaseStringCode(Of LtaOperationType)(CStrSafe(dr.Item(1)))
                If IsDBNull(dr.Item(2)) OrElse String.IsNullOrEmpty(dr.Item(2).ToString) Then
                    _AccountChangeType = LtaAccountChangeType.AcquisitionAccount
                Else
                    _AccountChangeType = ConvertEnumDatabaseStringCode(Of LtaAccountChangeType) _
                        (CStrSafe(dr.Item(2)))
                End If
                _Date = CDate(dr.Item(3))
                _OldDate = _Date
                _JournalEntryID = CIntSafe(dr.Item(4), 0)
                _JournalEntryDocNumber = CStrSafe(dr.Item(5))
                _JournalEntryContent = CStrSafe(dr.Item(6))
                _JournalEntryType = ConvertEnumDatabaseStringCode(Of DocumentType)(CStrSafe(dr.Item(7)))
                _IsComplexAct = ConvertDbBoolean(CIntSafe(dr.Item(8), 0))
                _Content = CStrSafe(dr.Item(9))
                _AccountCorresponding = CLongSafe(dr.Item(10), 0)
                _ActNumber = CIntSafe(dr.Item(11), 0)
                _UnitValueChange = CDblSafe(dr.Item(12), ROUNDUNITASSET, 0)
                _AmmountChange = CIntSafe(dr.Item(13), 0)
                _TotalValueChange = CDblSafe(dr.Item(14), 2, 0)
                _NewAmortizationPeriod = CIntSafe(dr.Item(15), 0)
                _AmortizationCalculations = CStrSafe(dr.Item(16))
                _RevaluedPortionUnitValueChange = CDblSafe(dr.Item(17), ROUNDUNITASSET, 0)
                _RevaluedPortionTotalValueChange = CDblSafe(dr.Item(18), 2, 0)
                _AmortizationCalculatedForMonths = CIntSafe(dr.Item(19), 0)
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(20), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(21), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _IsInvoiceBound = (_JournalEntryType = DocumentType.InvoiceMade OrElse _
                    _JournalEntryType = DocumentType.InvoiceReceived)

                _ID = OperationID

            End Using

            GetOperationBackgroundInfo(_AssetID, _ID, _Date)

            Recalculate(False)

            _ChronologyValidator = OperationChronologicValidator. _
                GetOperationChronologicValidator(_AssetID, _AssetName, _Type, _
                _AccountChangeType, _AssetDateAcquired, _ID, _Date, parentValidator)

            MarkOld()

            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Jūsų teisių nepakanka šiems duomenims išsaugoti.")

            CheckAllRulesChild(Nothing)

            Dim JE As General.JournalEntry = GetJournalEntryForDocument()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            If Not JE Is Nothing Then
                JE = JE.SaveServerSide()
                _JournalEntryID = JE.ID
                _JournalEntryType = JE.DocType
                _JournalEntryContent = JE.Content
                _JournalEntryDocNumber = JE.DocNumber
            End If

            DoSave(False)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            ValidationRules.CheckRules()

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Jūsų teisių nepakanka šiems duomenims pekeisti.")

            CheckAllRulesChild(Nothing)

            Dim JE As General.JournalEntry = GetJournalEntryForDocument()

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            If Not JE Is Nothing Then
                JE = JE.SaveServerSide()
                _JournalEntryContent = JE.Content
                _JournalEntryDocNumber = JE.DocNumber
            End If

            DoSave(False)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

            ValidationRules.CheckRules()

        End Sub

        ''' <summary>
        ''' Does a save operation from server side. Doesn't check for critical rules 
        ''' (fetch or programatical error within transaction crashes program).
        ''' Critical rules CheckIfInventoryNumberUnique, CheckIfLimitingOperationsExists, 
        ''' CheckIfDateIsValid needs to be invoked before starting a transaction.
        ''' </summary>
        Friend Sub SaveServerSide(ByVal parentID As Integer, ByVal FinancialDataReadOnly As Boolean)

            _JournalEntryID = parentID

            DoSave(FinancialDataReadOnly)

        End Sub

        Private Sub DoSave(ByVal FinancialDataReadOnly As Boolean)

            Dim myComm As SQLCommand
            If IsNew Then
                myComm = New SQLCommand("InsertLongTermAssetOperation")
                myComm.AddParam("?TD", _AssetID)
                myComm.AddParam("?OT", ConvertEnumDatabaseStringCode(_Type))
                If _Type = LtaOperationType.AccountChange Then
                    myComm.AddParam("?AT", ConvertEnumDatabaseStringCode(_AccountChangeType))
                Else
                    myComm.AddParam("?AT", "")
                End If
                myComm.AddParam("?CA", ConvertDbBoolean(_IsComplexAct))
                AddParamsFinancial(myComm)
            ElseIf _ChronologyValidator.FinancialDataCanChange AndAlso Not FinancialDataReadOnly Then
                myComm = New SQLCommand("UpdateLongTermAssetOperation")
                myComm.AddParam("?OD", _ID)
                AddParamsFinancial(myComm)
            Else
                myComm = New SQLCommand("UpdateLongTermAssetOperationLimited")
                myComm.AddParam("?OD", _ID)
            End If

            AddParamsGeneral(myComm)

            myComm.Execute()

            If IsNew Then _ID = Convert.ToInt32(myComm.LastInsertID)

            _OldDate = _Date

            MarkOld()

        End Sub

        Private Sub AddParamsGeneral(ByRef myComm As SQLCommand)

            myComm.AddParam("?DT", _Date.Date)
            If _Type <> LtaOperationType.AmortizationPeriod AndAlso _
                _Type <> LtaOperationType.UsingEnd AndAlso _Type <> LtaOperationType.UsingStart Then
                myComm.AddParam("?JD", _JournalEntryID)
            Else
                myComm.AddParam("?JD", 0)
            End If
            myComm.AddParam("?CN", _Content.Trim)
            If _Type = LtaOperationType.Discard OrElse _Type = LtaOperationType.UsingEnd _
                OrElse _Type = LtaOperationType.UsingStart Then
                myComm.AddParam("?NM", _ActNumber)
            Else
                myComm.AddParam("?NM", 0)
            End If

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?UD", _UpdateDate.ToUniversalTime)

        End Sub

        Private Sub AddParamsFinancial(ByRef myComm As SQLCommand)

            If _Type = LtaOperationType.AccountChange OrElse _Type = LtaOperationType.Amortization _
                OrElse _Type = LtaOperationType.Discard Then
                myComm.AddParam("?AC", _AccountCorresponding)
            Else
                myComm.AddParam("?AC", 0)
            End If
            If _Type = LtaOperationType.AcquisitionValueIncrease OrElse _Type = LtaOperationType.Amortization _
                            OrElse _Type = LtaOperationType.ValueChange Then
                myComm.AddParam("?UV", CRound(_UnitValueChange, ROUNDUNITASSET))
            Else
                myComm.AddParam("?UV", 0)
            End If
            If _Type = LtaOperationType.Discard OrElse _Type = LtaOperationType.Transfer Then
                myComm.AddParam("?AM", _AmmountChange)
            Else
                myComm.AddParam("?AM", 0)
            End If
            If _Type <> LtaOperationType.AccountChange AndAlso _Type <> LtaOperationType.AmortizationPeriod _
                AndAlso _Type <> LtaOperationType.UsingEnd AndAlso _Type <> LtaOperationType.UsingStart Then
                myComm.AddParam("?TV", CRound(_TotalValueChange))
            Else
                myComm.AddParam("?TV", 0)
            End If
            If _Type = LtaOperationType.AmortizationPeriod Then
                myComm.AddParam("?AP", _NewAmortizationPeriod)
            Else
                myComm.AddParam("?AP", 0)
            End If
            If _Type = LtaOperationType.Amortization Then
                myComm.AddParam("?CL", _AmortizationCalculations.Trim)
                myComm.AddParam("?UT", _AmortizationCalculatedForMonths)
            Else
                myComm.AddParam("?CL", "")
                myComm.AddParam("?UT", 0)
            End If
            If _Type = LtaOperationType.Amortization OrElse _Type = LtaOperationType.ValueChange Then
                myComm.AddParam("?RU", CRound(_RevaluedPortionUnitValueChange, ROUNDUNITASSET))
            Else
                myComm.AddParam("?RU", 0)
            End If
            If _Type = LtaOperationType.Amortization OrElse _Type = LtaOperationType.ValueChange _
                OrElse _Type = LtaOperationType.Discard OrElse _Type = LtaOperationType.Transfer Then
                myComm.AddParam("?RT", CRound(_RevaluedPortionTotalValueChange))
            Else
                myComm.AddParam("?RT", 0)
            End If

            myComm.AddParam("?DA", CRound(AfterOperationAcquisitionAccountValue _
                - _CurrentAcquisitionAccountValue))
            myComm.AddParam("?DB", CRound(AfterOperationAcquisitionAccountValuePerUnit _
                - _CurrentAcquisitionAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?DC", CRound(AfterOperationAmortizationAccountValue _
                - _CurrentAmortizationAccountValue))
            myComm.AddParam("?DE", CRound(AfterOperationAmortizationAccountValuePerUnit _
                - _CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?DF", CRound(AfterOperationValueDecreaseAccountValue _
                - _CurrentValueDecreaseAccountValue))
            myComm.AddParam("?DG", CRound(AfterOperationValueDecreaseAccountValuePerUnit _
                - _CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?DH", CRound(AfterOperationValueIncreaseAccountValue _
                - _CurrentValueIncreaseAccountValue))
            myComm.AddParam("?DI", CRound(AfterOperationValueIncreaseAccountValuePerUnit _
                - _CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?DJ", CRound(AfterOperationValueIncreaseAmortizationAccountValue _
                - _CurrentValueIncreaseAmortizationAccountValue))
            myComm.AddParam("?DK", CRound(AfterOperationValueIncreaseAmortizationAccountValuePerUnit _
                - _CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET))

        End Sub


        Private Function GetJournalEntryForDocument() As General.JournalEntry

            If _Type <> LtaOperationType.AccountChange AndAlso _
                _Type <> LtaOperationType.Amortization AndAlso _
                _Type <> LtaOperationType.Discard Then Return Nothing

            Dim result As General.JournalEntry = Nothing

            If IsNew Then
                If _Date.Date <= GetCurrentCompany.LastClosingDate.Date Then Throw New Exception( _
                    "Klaida. Neleidžiama koreguoti operacijų po uždarymo (" _
                    & GetCurrentCompany.LastClosingDate & ").")
                If _Type = LtaOperationType.AccountChange Then
                    result = General.JournalEntry.NewJournalEntryChild( _
                        DocumentType.LongTermAssetAccountChange)
                ElseIf _Type = LtaOperationType.Amortization Then
                    result = General.JournalEntry.NewJournalEntryChild(DocumentType.Amortization)
                ElseIf _Type = LtaOperationType.Discard Then
                    result = General.JournalEntry.NewJournalEntryChild( _
                        DocumentType.LongTermAssetDiscard)
                End If
            Else
                If _OldDate.Date <= GetCurrentCompany.LastClosingDate.Date OrElse _
                    _Date.Date <= GetCurrentCompany.LastClosingDate.Date Then Throw New Exception( _
                    "Klaida. Neleidžiama koreguoti operacijų po uždarymo (" _
                    & GetCurrentCompany.LastClosingDate & ").")
                If _Type = LtaOperationType.AccountChange Then
                    result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, _
                        DocumentType.LongTermAssetAccountChange)
                ElseIf _Type = LtaOperationType.Amortization Then
                    result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, _
                        DocumentType.Amortization)
                ElseIf _Type = LtaOperationType.Discard Then
                    result = General.JournalEntry.GetJournalEntryChild(_JournalEntryID, _
                        DocumentType.LongTermAssetDiscard)
                End If
            End If

            result.Date = _Date.Date
            result.Person = Nothing
            Dim CommonBookEntryList As BookEntryInternalList = _
                BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)
            If _Type = LtaOperationType.AccountChange Then
                result.Content = _Content & "(IT apskaitos sąsk. pakeitimas)"
                result.DocNumber = "nėra"
                CommonBookEntryList = GetTotalBookEntryListForAccountChange()

            ElseIf _Type = LtaOperationType.Amortization Then
                result.Content = _Content & " (IT amortizacijos priskaičiavimas)"
                result.DocNumber = "nėra"
                CommonBookEntryList = GetTotalBookEntryListForAmmortization()

            ElseIf _Type = LtaOperationType.Discard Then
                result.Content = _Content & "(IT nurašymas)"
                result.DocNumber = _ActNumber.ToString
                CommonBookEntryList = GetTotalBookEntryListForDiscard()

            End If

            CommonBookEntryList.Aggregate()

            result.DebetList.LoadBookEntryListFromInternalList(CommonBookEntryList, False)
            result.CreditList.LoadBookEntryListFromInternalList(CommonBookEntryList, False)

            If Not result.IsValid Then Throw New Exception("Klaida. Nekorektiškai generuotas " _
                & "bendrojo žurnalo įrašas: " & result.GetAllBrokenRules)

            Return result

        End Function


        Private Function GetTotalBookEntryListForAccountChange() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
                BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            Dim DebetBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
            Dim CreditBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)

            Dim BookEntriesPending As Boolean = False

            If _AccountChangeType = LtaAccountChangeType.AcquisitionAccount Then

                DebetBookEntry.Account = _AccountCorresponding
                DebetBookEntry.Ammount = CRound(_CurrentAcquisitionAccountValue)
                CreditBookEntry.Account = _AssetAcquiredAccount
                CreditBookEntry.Ammount = CRound(_CurrentAcquisitionAccountValue)
                BookEntriesPending = True

            ElseIf _AccountChangeType = LtaAccountChangeType.ValueDecreaseAccount _
                AndAlso CRound(_CurrentValueDecreaseAccountValue) > 0 Then

                DebetBookEntry.Account = _AssetValueDecreaseAccount
                DebetBookEntry.Ammount = CRound(_CurrentValueDecreaseAccountValue)
                CreditBookEntry.Account = _AccountCorresponding
                CreditBookEntry.Ammount = CRound(_CurrentValueDecreaseAccountValue)
                BookEntriesPending = True

            ElseIf _AccountChangeType = LtaAccountChangeType.ValueIncreaseAccount _
                AndAlso CRound(_CurrentValueIncreaseAccountValue) > 0 Then

                CreditBookEntry.Account = _AssetValueIncreaseAccount
                CreditBookEntry.Ammount = CRound(_CurrentValueIncreaseAccountValue)
                DebetBookEntry.Account = _AccountCorresponding
                DebetBookEntry.Ammount = CRound(_CurrentValueIncreaseAccountValue)
                BookEntriesPending = True

            ElseIf _AccountChangeType = LtaAccountChangeType.AmortizationAccount _
                AndAlso CRound(_CurrentAmortizationAccountValue) > 0 Then

                DebetBookEntry.Account = _AssetContraryAccount
                DebetBookEntry.Ammount = CRound(_CurrentAmortizationAccountValue)
                CreditBookEntry.Account = _AccountCorresponding
                CreditBookEntry.Ammount = CRound(_CurrentAmortizationAccountValue)
                BookEntriesPending = True

            ElseIf _AccountChangeType = LtaAccountChangeType.ValueIncreaseAmortizationAccount _
                AndAlso CRound(_CurrentValueIncreaseAmortizationAccountValue) > 0 Then

                DebetBookEntry.Account = _AssetValueIncreaseAmortizationAccount
                DebetBookEntry.Ammount = CRound(_CurrentValueIncreaseAmortizationAccountValue)
                CreditBookEntry.Account = _AccountCorresponding
                CreditBookEntry.Ammount = CRound(_CurrentValueIncreaseAmortizationAccountValue)
                BookEntriesPending = True

            End If

            If BookEntriesPending Then
                result.Add(DebetBookEntry)
                result.Add(CreditBookEntry)
            End If

            Return result

        End Function

        Private Function GetTotalBookEntryListForAmmortization() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
                BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            If CRound(-_TotalValueChange) > 0 Then

                Dim DebetBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                DebetBookEntry.Account = _AccountCorresponding
                DebetBookEntry.Ammount = CRound(-_TotalValueChange)
                result.Add(DebetBookEntry)

            End If

            If CRound(-_TotalValueChange) > 0 Then

                If CRound(-_TotalValueChange + _RevaluedPortionTotalValueChange) > 0 Then
                    Dim CreditBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    CreditBookEntry.Account = _AssetContraryAccount
                    CreditBookEntry.Ammount = CRound(-_TotalValueChange + _RevaluedPortionTotalValueChange)
                    result.Add(CreditBookEntry)
                End If
                If CRound(-_RevaluedPortionTotalValueChange) > 0 Then
                    Dim CreditBookEntrySecondary As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    CreditBookEntrySecondary.Account = _AssetValueIncreaseAmortizationAccount
                    CreditBookEntrySecondary.Ammount = CRound(-_RevaluedPortionTotalValueChange)
                    result.Add(CreditBookEntrySecondary)
                End If

            End If

            Return result

        End Function

        Private Function GetTotalBookEntryListForDiscard() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
                BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            Dim DebetSum As Double = 0
            Dim CreditSum As Double = 0

            If _CurrentAssetAmmount = _AmmountChange Then

                If CRound(_CurrentAmortizationAccountValue) > 0 Then
                    Dim AmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    AmortizationAccountBookEntry.Account = _AssetContraryAccount
                    AmortizationAccountBookEntry.Ammount = CRound(Me._CurrentAmortizationAccountValue)
                    result.Add(AmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentAmortizationAccountValue)
                End If

                If CRound(Me._CurrentValueDecreaseAccountValue) > 0 Then
                    Dim ValueDecreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueDecreaseAccountBookEntry.Account = Me._AssetValueDecreaseAccount
                    ValueDecreaseAccountBookEntry.Ammount = CRound(_CurrentValueDecreaseAccountValue)
                    result.Add(ValueDecreaseAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentValueDecreaseAccountValue)
                End If

                If CRound(Me._CurrentValueIncreaseAmortizationAccountValue) > 0 Then
                    Dim ValueIncreaseAmmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueIncreaseAmmortizationAccountBookEntry.Account = _
                        _AssetValueIncreaseAmortizationAccount
                    ValueIncreaseAmmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValue)
                    result.Add(ValueIncreaseAmmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentValueIncreaseAmortizationAccountValue)
                End If

                Dim AcquisitionAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AcquisitionAccountBookEntry.Account = _AssetAcquiredAccount
                AcquisitionAccountBookEntry.Ammount = CRound(_CurrentAcquisitionAccountValue)
                result.Add(AcquisitionAccountBookEntry)
                CreditSum = CRound(CreditSum + _CurrentAcquisitionAccountValue)

                If CRound(Me._CurrentValueIncreaseAccountValue) > 0 Then
                    Dim ValueIncreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    ValueIncreaseAccountBookEntry.Account = Me._AssetValueIncreaseAccount
                    ValueIncreaseAccountBookEntry.Ammount = CRound(_CurrentValueIncreaseAccountValue)
                    result.Add(ValueIncreaseAccountBookEntry)
                    CreditSum = CRound(CreditSum + _CurrentValueIncreaseAccountValue)
                End If

            Else

                If CRound(_CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim AmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    AmortizationAccountBookEntry.Account = _AssetContraryAccount
                    AmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentAmortizationAccountValuePerUnit * _AmmountChange)
                    result.Add(AmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + AmortizationAccountBookEntry.Ammount)
                End If

                If CRound(_CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueDecreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueDecreaseAccountBookEntry.Account = _AssetValueDecreaseAccount
                    ValueDecreaseAccountBookEntry.Ammount = _
                        CRound(_CurrentValueDecreaseAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueDecreaseAccountBookEntry)
                    DebetSum = CRound(DebetSum + ValueDecreaseAccountBookEntry.Ammount)
                End If

                If CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueIncreaseAmmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueIncreaseAmmortizationAccountBookEntry.Account = _
                        _AssetValueIncreaseAmortizationAccount
                    ValueIncreaseAmmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueIncreaseAmmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + ValueIncreaseAmmortizationAccountBookEntry.Ammount)
                End If

                Dim AcquisitionAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AcquisitionAccountBookEntry.Account = _AssetAcquiredAccount
                AcquisitionAccountBookEntry.Ammount = _
                    CRound(_CurrentAcquisitionAccountValuePerUnit * _AmmountChange)
                result.Add(AcquisitionAccountBookEntry)
                CreditSum = CRound(CreditSum + AcquisitionAccountBookEntry.Ammount)

                If CRound(_CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueIncreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    ValueIncreaseAccountBookEntry.Account = _AssetValueIncreaseAccount
                    ValueIncreaseAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueIncreaseAccountBookEntry)
                    CreditSum = CRound(CreditSum + ValueIncreaseAccountBookEntry.Ammount)
                End If

            End If

            Dim AccountCorrespondingBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
            AccountCorrespondingBookEntry.Account = _AccountCorresponding
            AccountCorrespondingBookEntry.Ammount = CRound(CreditSum - DebetSum)
            result.Add(AccountCorrespondingBookEntry)

            Return result

        End Function

        Friend Function GetTotalBookEntryListForTransfer(ByVal TransferSum As Double) As BookEntryInternalList

            Dim result As BookEntryInternalList = _
               BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            Dim DebetSum As Double = 0
            Dim CreditSum As Double = 0

            If _CurrentAssetAmmount = _AmmountChange Then

                If CRound(_CurrentAmortizationAccountValue) > 0 Then
                    Dim AmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    AmortizationAccountBookEntry.Account = _AssetContraryAccount
                    AmortizationAccountBookEntry.Ammount = CRound(Me._CurrentAmortizationAccountValue)
                    result.Add(AmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentAmortizationAccountValue)
                End If

                If CRound(Me._CurrentValueDecreaseAccountValue) > 0 Then
                    Dim ValueDecreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueDecreaseAccountBookEntry.Account = Me._AssetValueDecreaseAccount
                    ValueDecreaseAccountBookEntry.Ammount = CRound(_CurrentValueDecreaseAccountValue)
                    result.Add(ValueDecreaseAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentValueDecreaseAccountValue)
                End If

                If CRound(Me._CurrentValueIncreaseAmortizationAccountValue) > 0 Then
                    Dim ValueIncreaseAmmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueIncreaseAmmortizationAccountBookEntry.Account = _
                        _AssetValueIncreaseAmortizationAccount
                    ValueIncreaseAmmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValue)
                    result.Add(ValueIncreaseAmmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + _CurrentValueIncreaseAmortizationAccountValue)
                End If

                Dim AcquisitionAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AcquisitionAccountBookEntry.Account = _AssetAcquiredAccount
                AcquisitionAccountBookEntry.Ammount = CRound(_CurrentAcquisitionAccountValue)
                result.Add(AcquisitionAccountBookEntry)
                CreditSum = CRound(CreditSum + _CurrentAcquisitionAccountValue)

                If CRound(Me._CurrentValueIncreaseAccountValue) > 0 Then
                    Dim ValueIncreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    ValueIncreaseAccountBookEntry.Account = Me._AssetValueIncreaseAccount
                    ValueIncreaseAccountBookEntry.Ammount = CRound(_CurrentValueIncreaseAccountValue)
                    result.Add(ValueIncreaseAccountBookEntry)
                    CreditSum = CRound(CreditSum + _CurrentValueIncreaseAccountValue)
                End If

            Else

                If CRound(_CurrentAmortizationAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim AmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    AmortizationAccountBookEntry.Account = _AssetContraryAccount
                    AmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentAmortizationAccountValuePerUnit * _AmmountChange)
                    result.Add(AmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + AmortizationAccountBookEntry.Ammount)
                End If

                If CRound(_CurrentValueDecreaseAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueDecreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueDecreaseAccountBookEntry.Account = _AssetValueDecreaseAccount
                    ValueDecreaseAccountBookEntry.Ammount = _
                        CRound(_CurrentValueDecreaseAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueDecreaseAccountBookEntry)
                    DebetSum = CRound(DebetSum + ValueDecreaseAccountBookEntry.Ammount)
                End If

                If CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueIncreaseAmmortizationAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                    ValueIncreaseAmmortizationAccountBookEntry.Account = _
                        _AssetValueIncreaseAmortizationAccount
                    ValueIncreaseAmmortizationAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAmortizationAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueIncreaseAmmortizationAccountBookEntry)
                    DebetSum = CRound(DebetSum + ValueIncreaseAmmortizationAccountBookEntry.Ammount)
                End If

                Dim AcquisitionAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AcquisitionAccountBookEntry.Account = _AssetAcquiredAccount
                AcquisitionAccountBookEntry.Ammount = _
                    CRound(_CurrentAcquisitionAccountValuePerUnit * _AmmountChange)
                result.Add(AcquisitionAccountBookEntry)
                CreditSum = CRound(CreditSum + AcquisitionAccountBookEntry.Ammount)

                If CRound(_CurrentValueIncreaseAccountValuePerUnit, ROUNDUNITASSET) > 0 Then
                    Dim ValueIncreaseAccountBookEntry As BookEntryInternal = _
                        BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                    ValueIncreaseAccountBookEntry.Account = _AssetValueIncreaseAccount
                    ValueIncreaseAccountBookEntry.Ammount = _
                        CRound(_CurrentValueIncreaseAccountValuePerUnit * _AmmountChange)
                    result.Add(ValueIncreaseAccountBookEntry)
                    CreditSum = CRound(CreditSum + ValueIncreaseAccountBookEntry.Ammount)
                End If

            End If

            If CRound(CreditSum - DebetSum) > CRound(TransferSum) Then

                Dim AccountCorrespondingBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                AccountCorrespondingBookEntry.Account = _AccountCorresponding
                AccountCorrespondingBookEntry.Ammount = CRound(CreditSum - DebetSum - TransferSum)
                result.Add(AccountCorrespondingBookEntry)

            ElseIf CRound(CreditSum - DebetSum) < CRound(TransferSum) Then

                Dim AccountCorrespondingBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AccountCorrespondingBookEntry.Account = _AccountCorresponding
                AccountCorrespondingBookEntry.Ammount = CRound(TransferSum - CreditSum + DebetSum)
                result.Add(AccountCorrespondingBookEntry)

            End If

            Return result

        End Function

        Friend Function GetTotalBookEntryListForAcquisitionValueChange() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
               BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            If CRound(_TotalValueChange) = 0 Then Return result

            Dim AcquisitionAccountBookEntry As BookEntryInternal 
            If CRound(_TotalValueChange) > 0 Then
                AcquisitionAccountBookEntry = BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                AcquisitionAccountBookEntry.Ammount = CRound(_TotalValueChange)
            Else
                AcquisitionAccountBookEntry = BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AcquisitionAccountBookEntry.Ammount = CRound(-_TotalValueChange)
            End If
            AcquisitionAccountBookEntry.Account = _AssetAcquiredAccount
            
            result.Add(AcquisitionAccountBookEntry)

            Return result

        End Function


        Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)

            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Jūsų teisių nepakanka šiems duomenims pašalinti.")

            Dim nOperationType As LtaOperationType
            Dim nJournalEntryID As Integer

            CheckIfCanDelete(criteria.Id, True, True, True, nOperationType, nJournalEntryID)

            Dim TransactionExisted As Boolean = DatabaseAccess.TransactionExists
            If Not TransactionExisted Then DatabaseAccess.TransactionBegin()

            If nOperationType = LtaOperationType.AccountChange OrElse _
                nOperationType = LtaOperationType.Amortization OrElse _
                nOperationType = LtaOperationType.Discard Then _
                General.JournalEntry.DoDelete(nJournalEntryID)

            DoDelete(criteria.Id)

            If Not TransactionExisted Then DatabaseAccess.TransactionCommit()

        End Sub

        Private Shared Sub DoDelete(ByVal nID As Integer)
            Dim myComm As New SQLCommand("DeleteLongTermAssetOperation")
            myComm.AddParam("?CD", nID)
            myComm.Execute()
        End Sub


        Friend Shared Sub CheckIfCanDelete(ByVal nID As Integer, ByVal ThrowOnInvoiceTransfer As Boolean, _
            ByVal ThrowOnComplexDocument As Boolean, ByVal ThrowOnDependantDocument As Boolean, _
            Optional ByRef nOperationType As LtaOperationType = LtaOperationType.AccountChange, _
            Optional ByRef nJournalEntryID As Integer = -1)

            Dim myComm As New SQLCommand("CheckIfCanDeleteLongTermAssetOperation1")
            myComm.AddParam("?CD", nID)

            Dim nDate As Date
            Dim nAssetName As String
            Dim nAssetID As Integer
            Dim nJournalEntryDocType As DocumentType
            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception( _
                    "Klaida. Nerasti IT operacijos duomenys, operacijos ID=" & nID.ToString & ".")

                nDate = CDate(myData.Rows(0).Item(0))
                nOperationType = ConvertEnumDatabaseStringCode(Of LtaOperationType) _
                    (CStrSafe(myData.Rows(0).Item(1)))
                nAssetName = CStrSafe(myData.Rows(0).Item(2))
                nAssetID = CIntSafe(myData.Rows(0).Item(3), 0)
                nJournalEntryID = CIntSafe(myData.Rows(0).Item(4), 0)
                nJournalEntryDocType = ConvertEnumDatabaseStringCode(Of DocumentType) _
                    (CStrSafe(myData.Rows(0).Item(5)))

                If ThrowOnComplexDocument AndAlso ConvertDbBoolean(CIntSafe(myData.Rows(0).Item(6), 0)) Then _
                    Throw New Exception("Klaida. Ši IT operacija buvo registruota formuojant " _
                    & "kompleksinį dokumentą (aktą). Ji gali būti pašalinta tik redaguojant " _
                    & "šį kompleksinį dokumentą (aktą).")

                If ThrowOnInvoiceTransfer AndAlso nOperationType = LtaOperationType.Transfer _
                    AndAlso (nJournalEntryDocType = DocumentType.InvoiceMade OrElse _
                    nJournalEntryDocType = DocumentType.InvoiceReceived) Then Throw New Exception( _
                    "Klaida. Ši IT operacija buvo registruota formuojant " _
                    & "sąskaitą faktūrą. Ji gali būti pašalinta tik redaguojant " _
                    & "šią sąskaitą faktūrą.")

            End Using

            If ThrowOnDependantDocument AndAlso nOperationType = LtaOperationType.AccountChange Then
                IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted( _
                    nJournalEntryID, DocumentType.LongTermAssetAccountChange)
            ElseIf ThrowOnDependantDocument AndAlso nOperationType = LtaOperationType.Amortization Then
                IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted( _
                    nJournalEntryID, DocumentType.Amortization)
            ElseIf ThrowOnDependantDocument AndAlso nOperationType = LtaOperationType.Discard Then
                IndirectRelationInfoList.CheckIfJournalEntryCanBeDeleted( _
                    nJournalEntryID, DocumentType.LongTermAssetDiscard)
            End If

            myComm = New SQLCommand("CheckIfCanDeleteLongTermAssetOperation2")
            myComm.AddParam("?SD", nDate.Date)
            myComm.AddParam("?CD", nAssetID)

            Dim RowType As LtaOperationType
            Using myData As DataTable = myComm.Fetch

                If nOperationType = LtaOperationType.AccountChange AndAlso myData.Rows.Count > 0 Then _
                    Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")


                For i As Integer = 1 To myData.Rows.Count

                    RowType = ConvertEnumDatabaseStringCode(Of LtaOperationType) _
                        (CStrSafe(myData.Rows(i - 1).Item(0)))

                    If nOperationType = LtaOperationType.AccountChange AndAlso _
                        RowType <> LtaOperationType.AmortizationPeriod AndAlso _
                        RowType <> LtaOperationType.UsingEnd AndAlso _
                        RowType <> LtaOperationType.UsingStart Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If nOperationType = LtaOperationType.AcquisitionValueIncrease AndAlso _
                        RowType <> LtaOperationType.AmortizationPeriod AndAlso _
                        RowType <> LtaOperationType.UsingEnd AndAlso _
                        RowType <> LtaOperationType.UsingStart AndAlso _
                        RowType <> LtaOperationType.ValueChange Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If nOperationType = LtaOperationType.Amortization AndAlso _
                        RowType <> LtaOperationType.AcquisitionValueIncrease AndAlso _
                        RowType <> LtaOperationType.AmortizationPeriod AndAlso _
                        RowType <> LtaOperationType.UsingEnd AndAlso _
                        RowType <> LtaOperationType.UsingStart Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If nOperationType = LtaOperationType.AmortizationPeriod AndAlso _
                        RowType = LtaOperationType.Amortization Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If (nOperationType = LtaOperationType.Discard OrElse _
                        nOperationType = LtaOperationType.Transfer) AndAlso _
                        RowType <> LtaOperationType.AmortizationPeriod AndAlso _
                        RowType <> LtaOperationType.Discard AndAlso _
                        RowType <> LtaOperationType.Transfer AndAlso _
                        RowType <> LtaOperationType.UsingStart AndAlso _
                        RowType <> LtaOperationType.UsingEnd Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If (nOperationType = LtaOperationType.UsingEnd OrElse _
                        nOperationType = LtaOperationType.UsingStart) AndAlso _
                        (RowType = LtaOperationType.Amortization OrElse _
                        RowType = LtaOperationType.UsingEnd OrElse _
                        RowType = LtaOperationType.UsingStart) Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                    If nOperationType = LtaOperationType.ValueChange AndAlso _
                        RowType <> LtaOperationType.AcquisitionValueIncrease AndAlso _
                        RowType <> LtaOperationType.AmortizationPeriod AndAlso _
                        RowType <> LtaOperationType.UsingEnd AndAlso _
                        RowType <> LtaOperationType.UsingStart Then _
                        Throw New Exception("Klaida. Su turtu '" & nAssetName & _
                        "' yra registruota vėlesnių operacijų.")

                Next

            End Using

        End Sub

        Friend Sub CheckIfCanDeleteChild(ByVal parentValidator As IChronologicValidator)

            If IsNew Then Exit Sub

            _ChronologyValidator = OperationChronologicValidator.GetOperationChronologicValidator( _
                _AssetID, _AssetName, _Type, _AccountChangeType, _AssetDateAcquired, _
                _ID, _ChronologyValidator.CurrentOperationDate, parentValidator)

            If Not _ChronologyValidator.FinancialDataCanChange Then Throw New Exception( _
                "Klaida. Operacijos su IT """ & _AssetName & """ pašalinti neleidžiama: " _
                & _ChronologyValidator.FinancialDataCanChangeExplanation)

        End Sub

        Friend Sub CheckAllRulesChild(ByVal parentValidator As IChronologicValidator)

            If IsNew Then
                GetOperationBackgroundInfo(_AssetID, _ID, Today.AddYears(100))
                _ChronologyValidator = OperationChronologicValidator.NewOperationChronologicValidator( _
                    _AssetID, _AssetName, _Type, _AccountChangeType, _AssetDateAcquired, parentValidator)
            Else
                GetOperationBackgroundInfo(_AssetID, _ID, _OldDate)
                _ChronologyValidator = OperationChronologicValidator.GetOperationChronologicValidator( _
                    _AssetID, _AssetName, _Type, _AccountChangeType, _AssetDateAcquired, _ID, _
                    _ChronologyValidator.CurrentOperationDate, parentValidator)
            End If

            ValidationRules.CheckRules()
            If Not IsValid Then Throw New Exception("Turto '" & _AssetName _
                & "' operacijos duomenyse yra klaidų: " & Me.BrokenRulesCollection.ToString)

            If Not IsNew AndAlso Not IsChild Then CheckIfUpdateDateChanged()

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfLongTermAssetOperationUpdateDateChanged")
            myComm.AddParam("?CD", _ID)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count < 1 OrElse CDateTimeSafe(myData.Rows(0).Item(0), _
                    Date.MinValue) = Date.MinValue Then Throw New Exception( _
                    "Klaida. Objektas, kurio ID=" & _ID.ToString & ", nerastas.")
                If DateTime.SpecifyKind(CDateTimeSafe(myData.Rows(0).Item(0), DateTime.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime <> _UpdateDate Then Throw New Exception( _
                    "Turto '" & _AssetName & "' operacijos duomenų atnaujinimo data pasikeitė. " _
                    & "Teigtina, kad kitas vartotojas redagavo šį objektą.")
            End Using

        End Sub

#End Region

    End Class

End Namespace