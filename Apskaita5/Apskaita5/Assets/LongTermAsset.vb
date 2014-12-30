Namespace Assets

    <Serializable()> _
Public Class LongTermAsset
        Inherits BusinessBase(Of LongTermAsset)

#Region " Business Methods "

        Private AllowedJournalEntryTypes As DocumentType() = New DocumentType() _
            {DocumentType.AdvanceReport, DocumentType.BankOperation, _
            DocumentType.GoodsWriteOff, DocumentType.LongTermAssetDiscard, _
            DocumentType.None, DocumentType.TillSpendingOrder, DocumentType.TransferOfBalance}

        Private _ID As Integer = -1
        Private _InsertDate As DateTime = Now
        Private _UpdateDate As DateTime = Now
        Private _ChronologyValidator As AcquisitionChronologicValidator = Nothing
        Private _Name As String = ""
        Private _MeasureUnit As String = "Vnt."
        Private _LegalGroup As String = ""
        Private _CustomGroupInfo As LongTermAssetCustomGroupInfo = _
            LongTermAssetCustomGroupInfo.NewLongTermAssetCustomGroupInfo
        Private _AcquisitionDate As Date = Today
        Private _OldAcquisitionDate As Date = Today
        Private _AcquisitionJournalEntryID As Integer = -1
        Private _AcquisitionJournalEntryDocNumber As String = ""
        Private _AcquisitionJournalEntryDocContent As String = ""
        Private _AcquisitionJournalEntryDocType As DocumentType
        Private _InventoryNumber As String = ""
        Private _AccountAcquisition As Long = 0
        Private _AccountAccumulatedAmortization As Long = 0
        Private _AccountValueIncrease As Long = 0
        Private _AccountValueDecrease As Long = 0
        Private _AccountRevaluedPortionAmmortization As Long = 0

        Private _AcquisitionAccountValue As Double = 0
        Private _AcquisitionAccountValuePerUnit As Double = 0
        Private _AmortizationAccountValue As Double = 0
        Private _AmortizationAccountValuePerUnit As Double = 0
        Private _ValueDecreaseAccountValue As Double = 0
        Private _ValueDecreaseAccountValuePerUnit As Double = 0
        Private _ValueIncreaseAccountValue As Double = 0
        Private _ValueIncreaseAccountValuePerUnit As Double = 0
        Private _ValueIncreaseAmmortizationAccountValue As Double = 0
        Private _ValueIncreaseAmmortizationAccountValuePerUnit As Double = 0
        Private _Ammount As Integer = 0

        Private _LiquidationUnitValue As Double = 0
        Private _ContinuedUsage As Boolean = False
        Private _DefaultAmortizationPeriod As Integer = 0
        Private _AmortizationCalculatedForMonths As Integer = 0

        Private _IsInvoiceBound As Boolean = False

        Private _Value As Double = 0
        Private _ValuePerUnit As Double = 0
        Private _ValueRevaluedPortion As Double = 0
        Private _ValueRevaluedPortionPerUnit As Double = 0


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

        Public ReadOnly Property ChronologyValidator() As AcquisitionChronologicValidator
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ChronologyValidator
            End Get
        End Property

        Public Property Name() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Name
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If _Name.Trim <> value.Trim AndAlso (Not _IsInvoiceBound OrElse IsChild) Then
                    _Name = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property MeasureUnit() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _MeasureUnit
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If _MeasureUnit.Trim <> value.Trim AndAlso (Not _IsInvoiceBound OrElse IsChild) Then
                    _MeasureUnit = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property LegalGroup() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _LegalGroup
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If _LegalGroup.Trim <> value.Trim AndAlso (Not _IsInvoiceBound OrElse IsChild) Then
                    _LegalGroup = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property CustomGroupInfo() As LongTermAssetCustomGroupInfo
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CustomGroupInfo
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As LongTermAssetCustomGroupInfo)
                CanWriteProperty(True)
                If _IsInvoiceBound AndAlso Not IsChild Then Exit Property
                If _CustomGroupInfo Is Nothing AndAlso value Is Nothing Then Exit Property
                If _CustomGroupInfo Is Nothing OrElse value Is Nothing _
                    OrElse _CustomGroupInfo.ID <> value.ID Then

                    _CustomGroupInfo = value
                    PropertyHasChanged()

                End If
            End Set
        End Property

        Public Property AcquisitionDate() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AcquisitionDate
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Date)
                CanWriteProperty(True)
                If _AcquisitionDate.Date <> value.Date AndAlso Not IsChild AndAlso Not _IsInvoiceBound Then
                    _AcquisitionDate = value.Date
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property AcquisitionJournalEntryID() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AcquisitionJournalEntryID
            End Get
        End Property

        Public ReadOnly Property AcquisitionJournalEntryDocNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AcquisitionJournalEntryDocNumber
            End Get
        End Property

        Public ReadOnly Property AcquisitionJournalEntryDocContent() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AcquisitionJournalEntryDocContent
            End Get
        End Property

        Public ReadOnly Property AcquisitionJournalEntryDocType() As DocumentType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AcquisitionJournalEntryDocType
            End Get
        End Property

        Public ReadOnly Property AcquisitionJournalEntryDocTypeHumanReadable() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return ConvertEnumHumanReadable(_AcquisitionJournalEntryDocType)
                ConvertEnumHumanReadable(_AcquisitionJournalEntryDocType)
            End Get
        End Property

        Public Property InventoryNumber() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InventoryNumber
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As String)
                CanWriteProperty(True)
                If _InventoryNumber.Trim <> value.Trim AndAlso (Not _IsInvoiceBound OrElse IsChild) Then
                    _InventoryNumber = value.Trim
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountAcquisition() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountAcquisition
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _AccountAcquisition <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AccountAcquisition = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountAccumulatedAmortization() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountAccumulatedAmortization
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _AccountAccumulatedAmortization <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AccountAccumulatedAmortization = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountValueIncrease() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountValueIncrease
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _AccountValueIncrease <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AccountValueIncrease = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountValueDecrease() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountValueDecrease
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _AccountValueDecrease <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AccountValueDecrease = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AccountRevaluedPortionAmmortization() As Long
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AccountRevaluedPortionAmmortization
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Long)
                CanWriteProperty(True)
                If _AccountRevaluedPortionAmmortization <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AccountRevaluedPortionAmmortization = value
                    PropertyHasChanged()
                End If
            End Set
        End Property



        Public Property AcquisitionAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AcquisitionAccountValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_AcquisitionAccountValue) <> CRound(value) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AcquisitionAccountValue = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property AcquisitionAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AcquisitionAccountValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_AcquisitionAccountValuePerUnit, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AcquisitionAccountValuePerUnit = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                    RecalculatePerUnit(True)
                End If
            End Set
        End Property

        Public Property AmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AmortizationAccountValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_AmortizationAccountValue) <> CRound(value) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AmortizationAccountValue = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property AmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_AmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_AmortizationAccountValuePerUnit, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _AmortizationAccountValuePerUnit = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                    RecalculatePerUnit(True)
                End If
            End Set
        End Property

        Public Property ValueDecreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueDecreaseAccountValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueDecreaseAccountValue) <> CRound(value) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueDecreaseAccountValue = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property ValueDecreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueDecreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueDecreaseAccountValuePerUnit, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueDecreaseAccountValuePerUnit = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                    RecalculatePerUnit(True)
                End If
            End Set
        End Property

        Public Property ValueIncreaseAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueIncreaseAccountValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueIncreaseAccountValue) <> CRound(value) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueIncreaseAccountValue = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property ValueIncreaseAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueIncreaseAccountValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueIncreaseAccountValuePerUnit, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueIncreaseAccountValuePerUnit = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                    RecalculatePerUnit(True)
                End If
            End Set
        End Property

        Public Property ValueIncreaseAmmortizationAccountValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueIncreaseAmmortizationAccountValue)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueIncreaseAmmortizationAccountValue) <> CRound(value) _
                    AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueIncreaseAmmortizationAccountValue = CRound(value)
                    PropertyHasChanged()
                    Recalculate(True)
                End If
            End Set
        End Property

        Public Property ValueIncreaseAmmortizationAccountValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET) _
                    <> CRound(value, ROUNDUNITASSET) AndAlso (Not _IsInvoiceBound OrElse IsChild) _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _ValueIncreaseAmmortizationAccountValuePerUnit = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                    RecalculatePerUnit(True)
                End If
            End Set
        End Property

        Public ReadOnly Property Value() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Value)
            End Get
        End Property

        Public ReadOnly Property ValuePerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValuePerUnit, ROUNDUNITASSET)
            End Get
        End Property

        Public Property Ammount() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _Ammount
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If _Ammount <> value AndAlso Not IsChild AndAlso Not _IsInvoiceBound _
                    AndAlso _ChronologyValidator.FinancialDataCanChange Then
                    _Ammount = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property ValueRevaluedPortion() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueRevaluedPortion)
            End Get
        End Property

        Public ReadOnly Property ValueRevaluedPortionPerUnit() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_ValueRevaluedPortionPerUnit, ROUNDUNITASSET)
            End Get
        End Property



        Public Property LiquidationUnitValue() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_LiquidationUnitValue, ROUNDUNITASSET)
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Double)
                CanWriteProperty(True)
                If CRound(_LiquidationUnitValue, ROUNDUNITASSET) <> CRound(value, ROUNDUNITASSET) _
                    AndAlso (Not _IsInvoiceBound OrElse IsChild) AndAlso _
                    Not _ChronologyValidator.AmortizationIsCalculated Then
                    _LiquidationUnitValue = CRound(value, ROUNDUNITASSET)
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property ContinuedUsage() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ContinuedUsage
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Boolean)
                CanWriteProperty(True)
                If _ContinuedUsage <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) AndAlso _
                    Not _ChronologyValidator.AmortizationIsCalculated Then
                    _ContinuedUsage = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property DefaultAmortizationPeriod() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DefaultAmortizationPeriod
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If _DefaultAmortizationPeriod <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) AndAlso _
                    Not _ChronologyValidator.AmortizationIsCalculated Then
                    _DefaultAmortizationPeriod = value
                    PropertyHasChanged()
                End If
            End Set
        End Property

        Public Property AmortizationCalculatedForMonths() As Integer
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _AmortizationCalculatedForMonths
            End Get
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Set(ByVal value As Integer)
                CanWriteProperty(True)
                If _AmortizationCalculatedForMonths <> value AndAlso (Not _IsInvoiceBound OrElse IsChild) AndAlso _
                    Not _ChronologyValidator.AmortizationIsCalculated Then
                    _AmortizationCalculatedForMonths = value
                    PropertyHasChanged()
                End If
            End Set
        End Property


        Public ReadOnly Property IsInvoiceBound() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsInvoiceBound
            End Get
        End Property




        Friend Sub LoadAssociatedJournalEntry(ByVal nAcquisitionJournalEntryID As Integer, _
            ByVal nAcquisitionJournalEntryDocType As DocumentType, _
            ByVal nAcquisitionJournalEntryDocContent As String, _
            ByVal nAcquisitionJournalEntryDocNumber As String)

            If Not nAcquisitionJournalEntryID > 0 Then
                _AcquisitionJournalEntryID = -1
                _AcquisitionJournalEntryDocType = DocumentType.None
                _AcquisitionJournalEntryDocContent = "nėra"
                _AcquisitionJournalEntryDocNumber = ""

            ElseIf Array.IndexOf(AllowedJournalEntryTypes, nAcquisitionJournalEntryDocType) < 0 Then

                Throw New Exception("Klaida. Susieta bendrojo žurnalo operacija " & _
                    "negali turėti tipo: " & ConvertEnumHumanReadable( _
                    nAcquisitionJournalEntryDocType) & ".")

            Else

                _AcquisitionJournalEntryID = nAcquisitionJournalEntryID
                _AcquisitionJournalEntryDocType = nAcquisitionJournalEntryDocType
                _AcquisitionJournalEntryDocContent = nAcquisitionJournalEntryDocContent
                _AcquisitionJournalEntryDocNumber = nAcquisitionJournalEntryDocNumber

            End If

        End Sub

        Public Sub LoadAssociatedJournalEntry(ByVal Entry As ActiveReports.JournalEntryInfo)

            If _IsInvoiceBound OrElse IsChild Then Throw New Exception( _
                "Klaida. Bendrojo žurnalo operacija negali būti priskiriama " _
                & "per sąskaitą faktūrą įsigytam turtui.")

            If Entry Is Nothing Then

                LoadAssociatedJournalEntry(-1, DocumentType.None, "", "")

            Else

                If Entry.DocType = DocumentType.InvoiceMade OrElse _
                    Entry.DocType = DocumentType.InvoiceReceived Then _
                    Throw New Exception("Klaida. Turto įsigijimas per sąskaitą - faktūrą " _
                    & "gali būti registruojamas tik išrašant arba registruojant sąskaitas - faktūras.")

                LoadAssociatedJournalEntry(Entry.Id, Entry.DocType, Entry.Content, Entry.DocNumber)

            End If

            PropertyHasChanged("AcquisitionJournalEntryDocNumber")
            PropertyHasChanged("AcquisitionJournalEntryDocContent")
            PropertyHasChanged("AcquisitionJournalEntryDocType")
            PropertyHasChanged("AcquisitionJournalEntryID")

        End Sub


        Private Sub RecalculateAll(ByVal RaisePropertyChanged As Boolean)

            Recalculate(RaisePropertyChanged)
            RecalculatePerUnit(RaisePropertyChanged)

        End Sub

        Private Sub Recalculate(ByVal RaisePropertyChanged As Boolean)

            _Value = CRound(_AcquisitionAccountValue - AmortizationAccountValue _
                - _ValueDecreaseAccountValue + _ValueIncreaseAccountValue _
                - _ValueIncreaseAmmortizationAccountValue)
            _ValueRevaluedPortion = CRound(_ValueIncreaseAccountValue - _ValueDecreaseAccountValue _
                - _ValueIncreaseAmmortizationAccountValue)

            If RaisePropertyChanged Then
                OnPropertyChanged("Value")
                OnPropertyChanged("ValueRevaluedPortion")
            End If

        End Sub

        Private Sub RecalculatePerUnit(ByVal RaisePropertyChanged As Boolean)

            _ValuePerUnit = CRound(_AcquisitionAccountValuePerUnit - AmortizationAccountValuePerUnit _
                - _ValueDecreaseAccountValuePerUnit + _ValueIncreaseAccountValuePerUnit _
                - _ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET)
            _ValueRevaluedPortionPerUnit = CRound(_ValueIncreaseAccountValuePerUnit _
                - _ValueDecreaseAccountValuePerUnit _
                - _ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET)

            If RaisePropertyChanged Then
                OnPropertyChanged("ValuePerUnit")
                OnPropertyChanged("ValueRevaluedPortionPerUnit")
            End If

        End Sub


        Friend Sub SetValues(ByVal nAmount As Integer, ByVal nValue As Double, ByVal nValuePerUnit As Double)

            If Not _ChronologyValidator.FinancialDataCanChange Then Exit Sub

            AcquisitionAccountValue = CRound(nValue + AmortizationAccountValue _
                + _ValueDecreaseAccountValue - _ValueIncreaseAccountValue _
                + _ValueIncreaseAmmortizationAccountValue)

            AcquisitionAccountValuePerUnit = CRound(nValuePerUnit + _AmortizationAccountValuePerUnit _
                + _ValueDecreaseAccountValuePerUnit - _ValueIncreaseAccountValuePerUnit _
                + _ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET)

            If _Ammount <> nAmount Then
                _Ammount = nAmount
                PropertyHasChanged("Ammount")
            End If

        End Sub

        Friend Sub SetDate(ByVal value As Date)
            If _AcquisitionDate.Date <> value.Date Then
                _AcquisitionDate = value.Date
                PropertyHasChanged("AcquisitionDate")
            End If
        End Sub


        Public Overrides Function Save() As LongTermAsset

            Me.ValidationRules.CheckRules()
            If Not Me.IsValid Then Throw New Exception("Ilgalaikio turto duomenyse yra klaidų: " & _
                Me.BrokenRulesCollection.ToString)

            If IsChild OrElse _AcquisitionJournalEntryDocType = DocumentType.InvoiceReceived _
                OrElse _AcquisitionJournalEntryDocType = DocumentType.InvoiceMade Then _
                Throw New Exception("Klaida. Įsigijimą per sąskaitą - faktūrą " & _
                "galima registruoti tik per sąskaitų - faktūrų modulį.")

            Return MyBase.Save()

        End Function

        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

#End Region

#Region " Validation Rules "

        Protected Overrides Sub AddBusinessRules()

            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("Name", "turto pavadinimas"))
            ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
                New CommonValidation.SimpleRuleArgs("MeasureUnit", "turto mato vienetas"))
            'ValidationRules.AddRule(AddressOf CommonValidation.StringRequired, _
            '    New CommonValidation.SimpleRuleArgs("LegalGroup", "turto grupė"))

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("Ammount", "įsigytas kiekis"))
            ValidationRules.AddRule(AddressOf CommonValidation.ChronologyValidation, _
                New CommonValidation.ChronologyRuleArgs("AcquisitionDate", "ChronologyValidator"))

            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountAccumulatedAmortization", _
                "sukauptos amortizacijos sąskaita (kontrarinė sąskaita)"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountAcquisition", _
                "savikainos apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountRevaluedPortionAmmortization", _
                "pervertintos (vertės padidėjimo) dalies amortizacijos apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountValueDecrease", _
                "vertės sumažėjimo apskaitos sąskaita"))
            ValidationRules.AddRule(AddressOf CommonValidation.PositiveNumberRequired, _
                New CommonValidation.SimpleRuleArgs("AccountValueIncrease", _
                "pervertintos dalies (vertės padidėjimo) apskaitos sąskaita"))
            

            ValidationRules.AddRule(AddressOf LiquidationValueValidation, "LiquidationUnitValue")
            ValidationRules.AddDependantProperty("AcquisitionAccountValuePerUnit", "LiquidationUnitValue", False)

            ValidationRules.AddRule(AddressOf AcquisitionAccountValueValidation, "AcquisitionAccountValue")
            ValidationRules.AddRule(AddressOf AcquisitionAccountValuePerUnitValidation, _
                "AcquisitionAccountValuePerUnit")
            ValidationRules.AddDependantProperty("AmortizationAccountValue", _
                "AcquisitionAccountValue", False)
            ValidationRules.AddDependantProperty("ValueDecreaseAccountValue", _
                "AcquisitionAccountValue", False)
            ValidationRules.AddDependantProperty("AmortizationAccountValuePerUnit", _
                "AcquisitionAccountValuePerUnit", False)
            ValidationRules.AddDependantProperty("ValueDecreaseAccountValuePerUnit", _
                "AcquisitionAccountValuePerUnit", False)

            ValidationRules.AddRule(AddressOf ValueDecreaseAccountValueValidation, _
                "ValueDecreaseAccountValue")
            ValidationRules.AddRule(AddressOf ValueDecreaseAccountValuePerUnitValidation, _
                "ValueDecreaseAccountValuePerUnit")
            ValidationRules.AddDependantProperty("ValueIncreaseAccountValue", _
                "ValueDecreaseAccountValue", False)
            ValidationRules.AddDependantProperty("ValueIncreaseAmmortizationAccountValue", _
                "ValueDecreaseAccountValue", False)
            ValidationRules.AddDependantProperty("ValueIncreaseAccountValuePerUnit", _
                "ValueDecreaseAccountValuePerUnit", False)
            ValidationRules.AddDependantProperty("ValueIncreaseAmmortizationAccountValuePerUnit", _
                "ValueDecreaseAccountValuePerUnit", False)

            ValidationRules.AddRule(AddressOf ValueIncreaseAccountValueValidation, _
                "ValueIncreaseAccountValue")
            ValidationRules.AddRule(AddressOf ValueIncreaseAccountValuePerUnitValidation, _
                "ValueIncreaseAccountValuePerUnit")
            ValidationRules.AddDependantProperty("ValueIncreaseAmmortizationAccountValue", _
                "ValueIncreaseAccountValue", False)
            ValidationRules.AddDependantProperty("ValueIncreaseAmmortizationAccountValuePerUnit", _
                "ValueIncreaseAccountValuePerUnit", False)

            ValidationRules.AddRule(AddressOf DefaultAmortizationPeriodValidation, _
                "DefaultAmortizationPeriod")
            ValidationRules.AddDependantProperty("AmortizationCalculatedForMonths", _
                "DefaultAmortizationPeriod", False)

            ValidationRules.AddRule(AddressOf JournalEntryValidation, "AcquisitionJournalEntryID")

        End Sub

        ''' <summary>
        ''' Rule ensuring liquidation value is set and valid, i.e. less then 10% of the unit value.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function LiquidationValueValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If Not CRound(ValObj._LiquidationUnitValue, ROUNDUNITASSET) > 0 Then

                e.Description = "Nenurodyta turto vieneto likvidacinė vertė."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            If CRound(ValObj._AcquisitionAccountValuePerUnit * 0.1, ROUNDUNITASSET) < _
                CRound(ValObj._LiquidationUnitValue, ROUNDUNITASSET) Then

                e.Description = "Turto vieneto likvidacinė vertė negali būti didesnė " _
                    & "kaip 10 % turto vieneto įsigijimo vertės."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring Acquisition account value is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AcquisitionAccountValueValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If Not CRound(ValObj._AcquisitionAccountValue) > 0 Then

                e.Description = "Nenurodyta turto įsigijimo (savikainos) vertė."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            If CRound(ValObj._AcquisitionAccountValue - ValObj._AmortizationAccountValue _
                - ValObj._ValueDecreaseAccountValue) < 0 Then

                e.Description = "Turto įsigijimo (savikainos) vertė negali būti mažesnė " _
                    & "už sukauptos amortizacijos ir vertės sumažėjimo sumą."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring Acquisition account value per unit is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function AcquisitionAccountValuePerUnitValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If Not CRound(ValObj._AcquisitionAccountValuePerUnit, ROUNDUNITASSET) > 0 Then

                e.Description = "Nenurodyta turto įsigijimo (savikainos) vertė vienetui."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            If CRound(ValObj._AcquisitionAccountValuePerUnit - ValObj._AmortizationAccountValuePerUnit _
                - ValObj._ValueDecreaseAccountValuePerUnit, ROUNDUNITASSET) < 0 Then

                e.Description = "Turto įsigijimo (savikainos) vertė turto vienetui negali būti mažesnė " _
                    & "už sukauptos amortizacijos ir vertės sumažėjimo sumą turto vienetui."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring value decrease account value is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ValueDecreaseAccountValueValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If CRound(ValObj._ValueDecreaseAccountValue) > 0 AndAlso _
                CRound(ValObj._ValueIncreaseAccountValue - _
                ValObj._ValueIncreaseAmmortizationAccountValue) > 0 Then

                e.Description = "Turto vertės sumažėjimo sąskaitos likutis negali būti teigiamas, " _
                    & "jei pervertinimo (vertės padidėjimo) sąskaitos likutis po nudėvėjimo yra teigiamas."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring value decrease account value per unit is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ValueDecreaseAccountValuePerUnitValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If CRound(ValObj._ValueDecreaseAccountValuePerUnit, ROUNDUNITASSET) > 0 AndAlso _
                CRound(ValObj._ValueIncreaseAccountValuePerUnit - _
                ValObj._ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET) > 0 Then

                e.Description = "Turto vertės sumažėjimo sąskaitos likutis turto vienetui " _
                    & "negali būti teigiamas, jei pervertinimo (vertės padidėjimo) sąskaitos " _
                    & "likutis po nudėvėjimo turto vienetui yra teigiamas."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring value increase account value is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ValueIncreaseAccountValueValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If CRound(ValObj._ValueIncreaseAccountValue - _
                ValObj._ValueIncreaseAmmortizationAccountValue) < 0 Then

                e.Description = "Turto perkainojimo (vertės padidėjimo) sąskaitos likutis negali būti " _
                    & "mažesnis už turto perkainojimo (vertės padidėjimo) amortizacijos sąskaitos likutį."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring value increase account value per unit is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function ValueIncreaseAccountValuePerUnitValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If CRound(ValObj._ValueIncreaseAccountValuePerUnit - _
                ValObj._ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET) < 0 Then

                e.Description = "Turto perkainojimo (vertės padidėjimo) sąskaitos likutis " _
                    & "turto vienetui negali būti mažesnis už turto perkainojimo " _
                    & "(vertės padidėjimo) amortizacijos sąskaitos likutį turto vienetui."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True
        End Function

        ''' <summary>
        ''' Rule ensuring journal entry is assigned correctly.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function JournalEntryValidation(ByVal target As Object, _
          ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If Not ValObj.IsChild AndAlso Not ValObj._AcquisitionJournalEntryID > 0 Then

                e.Description = "Nėra susieto bendrojo žurnalo įrašo (dokumento), " _
                    & "kuris būtų įsigijimo pagrindu."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Rule ensuring default amortization period is acceptable.
        ''' </summary>
        ''' <param name="target">Object containing the data to validate</param>
        ''' <param name="e">Arguments parameter specifying the name of the string
        ''' property to validate</param>
        ''' <returns><see langword="false" /> if the rule is broken</returns>
        <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
        Private Shared Function DefaultAmortizationPeriodValidation(ByVal target As Object, _
            ByVal e As Validation.RuleArgs) As Boolean

            Dim ValObj As LongTermAsset = DirectCast(target, LongTermAsset)

            If Not ValObj._DefaultAmortizationPeriod > 0 Then

                e.Description = "Nenurodytas pradinis amortizacijos laikotarpis."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            If Math.Ceiling(ValObj._AmortizationCalculatedForMonths / 12) > _
                ValObj._DefaultAmortizationPeriod Then

                e.Description = "Pradinis amortizacijos laikotarpis negali būti trumpesnis " _
                    & "už naudojimo laiką, kuriam jau paskaičiuota amortizacija."
                e.Severity = Validation.RuleSeverity.Error
                Return False

            End If

            Return True

        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()
            AuthorizationRules.AllowWrite("Assets.LongTermAsset2")
        End Sub

        Public Shared Function CanAddObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAsset2")
        End Function

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAsset1")
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAsset3")
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return ApplicationContext.User.IsInRole("Assets.LongTermAsset3")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function GetNewLongTermAsset() As LongTermAsset
            Return New LongTermAsset(False, Nothing)
        End Function

        Friend Shared Function GetNewLongTermAssetChild(ByVal parentChronologyValidator _
            As SimpleChronologicValidator) As LongTermAsset
            Return New LongTermAsset(True, parentChronologyValidator)
        End Function

        Public Shared Function GetLongTermAsset(ByVal id As Integer) As LongTermAsset
            Return DataPortal.Fetch(Of LongTermAsset)(New Criteria(id))
        End Function

        Friend Shared Function GetLongTermAssetChild(ByVal id As Integer, _
            ByVal parentChronologyValidator As SimpleChronologicValidator) As LongTermAsset
            Return New LongTermAsset(id, parentChronologyValidator)
        End Function

        Public Shared Sub DeleteLongTermAsset(ByVal id As Integer)
            DataPortal.Delete(New Criteria(id))
        End Sub


        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal CreateChild As Boolean, ByVal parentChronologyValidator As SimpleChronologicValidator)
            If CreateChild Then MarkAsChild()
            Create(parentChronologyValidator)
        End Sub

        Private Sub New(ByVal id As Integer, ByVal parentChronologyValidator As SimpleChronologicValidator)
            MarkAsChild()
            Fetch(id, parentChronologyValidator)
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


        Private Sub Create(ByVal parentChronologyValidator As SimpleChronologicValidator)
            _ChronologyValidator = AcquisitionChronologicValidator. _
                NewAcquisitionChronologicValidator(parentChronologyValidator)
            ValidationRules.CheckRules()
        End Sub

        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai informacijai gauti.")
            Fetch(criteria.Id, Nothing)
        End Sub

        Private Sub Fetch(ByVal nAssetID As Integer, ByVal parentChronologyValidator As SimpleChronologicValidator)

            Dim myComm As New SQLCommand("FetchLongTermAssetOperationInfoListParent")
            myComm.AddParam("?AD", nAssetID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception("Klaida. Nerasti turto duomenys.")
                Dim dr As DataRow = myData.Rows(0)

                _Name = CStrSafe(dr.Item(0)).Trim
                _MeasureUnit = CStrSafe(dr.Item(1)).Trim
                _LegalGroup = CStrSafe(dr.Item(2)).Trim
                If IsDBNull(dr.Item(4)) Then dr.Item(4) = ""
                _CustomGroupInfo = LongTermAssetCustomGroupInfo.GetLongTermAssetCustomGroupInfo(dr, 3)
                _InventoryNumber = CStrSafe(dr.Item(5)).Trim
                _AcquisitionJournalEntryID = CIntSafe(dr.Item(6), 0)
                _AcquisitionDate = CDateSafe(dr.Item(7), Today)
                _OldAcquisitionDate = _AcquisitionDate
                _AcquisitionJournalEntryDocNumber = CStrSafe(dr.Item(8)).Trim
                _AcquisitionJournalEntryDocContent = CStrSafe(dr.Item(9)).Trim
                _AcquisitionJournalEntryDocType = ConvertEnumDatabaseStringCode(Of DocumentType) _
                    (CStrSafe(dr.Item(10)))
                _AccountAcquisition = CLongSafe(dr.Item(11), 0)
                _AccountAccumulatedAmortization = CLongSafe(dr.Item(12), 0)
                _AccountValueDecrease = CLongSafe(dr.Item(13), 0)
                _AccountValueIncrease = CLongSafe(dr.Item(14), 0)
                _AccountRevaluedPortionAmmortization = CLongSafe(dr.Item(15), 0)
                _LiquidationUnitValue = CDblSafe(dr.Item(16), ROUNDUNITASSET, 0)
                _DefaultAmortizationPeriod = CIntSafe(dr.Item(17), 0)
                _AcquisitionAccountValuePerUnit = CDblSafe(dr.Item(18), ROUNDUNITASSET, 0)
                _Ammount = CIntSafe(dr.Item(19), 0)
                _AcquisitionAccountValue = CDblSafe(dr.Item(20), 2, 0)
                _AmortizationAccountValue = CDblSafe(dr.Item(23), 2, 0)
                _AmortizationAccountValuePerUnit = CDblSafe(dr.Item(24), ROUNDUNITASSET, 0)
                _ValueIncreaseAmmortizationAccountValue = CDblSafe(dr.Item(25), 2, 0)
                _ValueIncreaseAmmortizationAccountValuePerUnit = CDblSafe(dr.Item(26), ROUNDUNITASSET, 0)
                _ContinuedUsage = ConvertDbBoolean(CIntSafe(dr.Item(27), 0))
                _AmortizationCalculatedForMonths = CIntSafe(dr.Item(28), 0)
                _InsertDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(29), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime
                _UpdateDate = DateTime.SpecifyKind(CDateTimeSafe(dr.Item(30), Date.UtcNow), _
                    DateTimeKind.Utc).ToLocalTime

                If CDblSafe(dr.Item(21), 0) < 0 Then
                    _ValueDecreaseAccountValuePerUnit = -CDblSafe(dr.Item(21), ROUNDUNITASSET, 0)
                    _ValueIncreaseAccountValuePerUnit = 0
                ElseIf CDblSafe(dr.Item(21), 0) > 0 Then
                    _ValueIncreaseAccountValuePerUnit = CDblSafe(dr.Item(21), ROUNDUNITASSET, 0)
                    _ValueDecreaseAccountValuePerUnit = 0
                Else
                    _ValueIncreaseAccountValuePerUnit = 0
                    _ValueDecreaseAccountValuePerUnit = 0
                End If
                If CDblSafe(dr.Item(22), 0) < 0 Then
                    _ValueDecreaseAccountValue = -CDblSafe(dr.Item(22), 2, 0)
                    _ValueIncreaseAccountValue = 0
                ElseIf CDblSafe(dr.Item(22), 0) > 0 Then
                    _ValueIncreaseAccountValue = CDblSafe(dr.Item(22), 2, 0)
                    _ValueDecreaseAccountValue = 0
                Else
                    _ValueIncreaseAccountValue = 0
                    _ValueDecreaseAccountValue = 0
                End If

                _ValueIncreaseAccountValue = CRound(_ValueIncreaseAccountValue + _
                    _ValueIncreaseAmmortizationAccountValue)
                _ValueIncreaseAccountValuePerUnit = CRound(_ValueIncreaseAccountValuePerUnit + _
                    _ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET)

                _IsInvoiceBound = (_AcquisitionJournalEntryDocType = DocumentType.InvoiceReceived _
                    OrElse _AcquisitionJournalEntryDocType = DocumentType.InvoiceMade)

            End Using

            _ID = nAssetID

            RecalculateAll(False)

            _ChronologyValidator = AcquisitionChronologicValidator.GetAcquisitionChronologicValidator( _
                _ID, _AcquisitionDate, _Name, _ContinuedUsage, parentChronologyValidator)

            MarkOld()
            ValidationRules.CheckRules()

        End Sub


        Protected Overrides Sub DataPortal_Insert()
            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai operacijai atlikti.")
            CheckIfCanUpdate()
            DoSave(False)
        End Sub

        Protected Overrides Sub DataPortal_Update()
            If Not CanEditObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai operacijai atlikti.")
            CheckIfCanUpdate()
            DoSave(False)
        End Sub

        ''' <summary>
        ''' Does a save operation from server side. Doesn't check for critical rules 
        ''' (fetch or programatical error within transaction crashes program).
        ''' Critical rules CheckIfInventoryNumberUnique, CheckIfLimitingOperationsExists, 
        ''' CheckIfDateIsValid needs to be invoked before starting a transaction.
        ''' </summary>
        Friend Sub SaveServerSide(ByVal parentID As Integer, ByVal FinancialDataReadOnly As Boolean)
            _AcquisitionJournalEntryID = parentID
            DoSave(FinancialDataReadOnly)
        End Sub

        Private Sub DoSave(ByVal FinancialDataReadOnly As Boolean)

            Dim myComm As SQLCommand
            If IsNew Then

                myComm = New SQLCommand("InsertLongTermAsset")
                AddAcountsAndValuesParams(myComm)
                AddAmortizationRelatedParams(myComm)

            Else

                If _ChronologyValidator.FinancialDataCanChange AndAlso Not FinancialDataReadOnly Then

                    myComm = New SQLCommand("UpdateLongTermAsset1")
                    AddAcountsAndValuesParams(myComm)
                    AddAmortizationRelatedParams(myComm)

                ElseIf (Not _ChronologyValidator.FinancialDataCanChange OrElse FinancialDataReadOnly) _
                    AndAlso Not _ChronologyValidator.AmortizationIsCalculated Then

                    myComm = New SQLCommand("UpdateLongTermAsset2")
                    AddAmortizationRelatedParams(myComm)

                ElseIf (Not _ChronologyValidator.FinancialDataCanChange OrElse FinancialDataReadOnly) _
                    AndAlso _ChronologyValidator.AmortizationIsCalculated Then

                    myComm = New SQLCommand("UpdateLongTermAsset3")

                Else
                    Throw New Exception("Klaida. Neįmanoma ribojančių veiksnių konfiguracija: " _
                        & "IsInvoiceBound=" & _IsInvoiceBound.ToString & "; FinancialDataCanChange=" _
                        & _ChronologyValidator.FinancialDataCanChange.ToString & "; AmortizationIsCalculated=" _
                        & _ChronologyValidator.AmortizationIsCalculated.ToString & ".")
                End If

                myComm.AddParam("?TD", _ID)

            End If

            myComm.AddParam("?JD", _AcquisitionJournalEntryID)
            myComm.AddParam("?NM", _Name.Trim)
            myComm.AddParam("?GP", _LegalGroup.Trim)
            myComm.AddParam("?MU", _MeasureUnit.Trim)
            myComm.AddParam("?VN", _InventoryNumber.Trim)
            If CustomGroupInfo Is Nothing OrElse Not CustomGroupInfo.ID > 0 Then
                myComm.AddParam("?CG", 0)
            Else
                myComm.AddParam("?CG", CustomGroupInfo.ID)
            End If

            _UpdateDate = DateTime.Now
            _UpdateDate = New DateTime(Convert.ToInt64(Math.Floor(_UpdateDate.Ticks / TimeSpan.TicksPerSecond) _
                * TimeSpan.TicksPerSecond))
            If Me.IsNew Then _InsertDate = _UpdateDate
            myComm.AddParam("?UD", _UpdateDate.ToUniversalTime)

            myComm.Execute()

            If IsNew Then _ID = Convert.ToInt32(myComm.LastInsertID)

            MarkOld()

        End Sub

        Private Sub AddAcountsAndValuesParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?CN", _Ammount)
            myComm.AddParam("?UV", CRound(_AcquisitionAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?AA", _AccountAcquisition)
            myComm.AddParam("?CA", CRound(_AmortizationAccountValue))
            myComm.AddParam("?TV", CRound(_AcquisitionAccountValue))
            myComm.AddParam("?AC", _AccountAccumulatedAmortization)
            myComm.AddParam("?FA", _AccountValueIncrease)
            myComm.AddParam("?FB", _AccountValueDecrease)
            myComm.AddParam("?FC", _AccountRevaluedPortionAmmortization)
            myComm.AddParam("?FD", ValueRevaluedPortionPerUnit)
            myComm.AddParam("?FE", ValueRevaluedPortion)
            myComm.AddParam("?FG", CRound(_AmortizationAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?FH", CRound(_ValueIncreaseAmmortizationAccountValuePerUnit, ROUNDUNITASSET))
            myComm.AddParam("?FI", CRound(_ValueIncreaseAmmortizationAccountValue))
        End Sub

        Private Sub AddAmortizationRelatedParams(ByRef myComm As SQLCommand)
            myComm.AddParam("?LQ", CRound(_LiquidationUnitValue, ROUNDUNITASSET))
            myComm.AddParam("?DP", _DefaultAmortizationPeriod)
            myComm.AddParam("?FJ", ConvertDbBoolean(_ContinuedUsage))
            myComm.AddParam("?FK", _AmortizationCalculatedForMonths)
        End Sub


        Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
            If Not CanDeleteObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai operacijai atlikti.")
            CheckIfAnyOperationExists(criteria.Id, True)
            DoDelete(criteria.Id)
        End Sub

        ''' <summary>
        ''' Does a delete operation from server side. Doesn't check for critical rules
        ''' (fetch or programatical error within transaction crashes program).
        ''' Critical rule CheckIfAnyOperationExists needs to be invoked before starting transaction.
        ''' </summary>
        ''' <param name="nID"></param>
        Friend Shared Sub DeleteServerSide(ByVal nID As Integer)
            DoDelete(nID)
        End Sub

        Private Shared Sub DoDelete(ByVal nID As Integer)
            Dim myComm As New SQLCommand("DeleteLongTermAsset")
            myComm.AddParam("?LD", nID)
            myComm.Execute()
        End Sub

        ''' <summary>
        ''' Throws an error if there are any operations with the LTA with ID=nID. Used before deletion.
        ''' </summary>
        Friend Shared Sub CheckIfAnyOperationExists(ByVal nID As Integer, _
            ByVal ThrowOnInvoiceBasedAcquisition As Boolean)

            Dim myComm As New SQLCommand("CheckIfLongTermAssetCanBeDeleted")
            myComm.AddParam("?AD", nID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception( _
                    "Klaida. Nerasti ilgalaikio turto duomenys, ID=" & nID.ToString & ".")

                If myData.Rows.Count > 1 Then Throw New Exception( _
                    "Klaida. Su IT, kurio ID=" & nID.ToString & ", yra registruota operacijų.")

                If ThrowOnInvoiceBasedAcquisition AndAlso (ConvertEnumDatabaseStringCode(Of DocumentType) _
                    (CStrSafe(myData.Rows(0).Item(0))) = DocumentType.InvoiceMade OrElse _
                    ConvertEnumDatabaseStringCode(Of DocumentType)(CStrSafe(myData.Rows(0).Item(0))) = _
                    DocumentType.InvoiceReceived) Then Throw New Exception( _
                    "Klaida. Turto įsigijimo pagrindas buvo sąskaita - faktūra. " _
                    & "Turto duomenis leidžiama pašalinti tik redaguojant/pašalinant šią sąskaitą - faktūrą.")

            End Using

        End Sub

        '''' <summary>
        '''' Throws an error if the related journal entry doesn't have debet coresp 
        '''' with account specified for LTA and value >= TotalValueAcquired - AccumulatedAmortization
        '''' </summary>
        'Private Sub CheckIfJournalEntryValueIsValid()

        '    If Not _ID > 0 Then Throw New Exception("Klaida. Nepasirinktas susietas bendrojo žurnalo įrašas.")

        '    Dim myComm As New SQLCommand("CheckIfJournalEntryValidForLTAPurchase")
        '    myComm.AddParam("?LD", _ID)
        '    myComm.AddParam("?AC", _AccountAcquisition)

        '    Using myData As DataTable = myComm.Fetch
        '        If myData.Rows.Count < 1 OrElse IsDBNull(myData.Rows(0).Item(0)) Then _
        '            Throw New Exception("Klaida. Pasirinktame bendrojo žurnalo įraše " & _
        '            "nėra debeto korespondencijos su turtui priskirta apskaitos sąskaita.")

        '        Dim CorrSum As Double = CRound(CDbl(myData.Rows(0).Item(0)))
        '        If myData.Rows.Count > 1 Then
        '            For i As Integer = 2 To myData.Rows.Count
        '                If CRound(CDbl(myData.Rows(i - 1).Item(0))) > CorrSum Then _
        '                    CorrSum = CRound(CDbl(myData.Rows(i - 1).Item(0)))
        '            Next
        '        End If

        '        If CRound(CDbl(myData.Rows(0).Item(0))) < CRound(_AcquisitionAccountValue - _
        '            _AmortizationAccountValue) Then Throw New Exception( _
        '            "Klaida. Pasirinktame bendrojo žurnalo įraše " & _
        '            "debeto korespondencijos su turtui priskirta apskaitos sąskaita suma " & _
        '            "yra mažesnė nei nurodyta bendra turto vertė.")

        '    End Using

        'End Sub

        ''' <summary>
        ''' Throws an error if the inventory number specified is not unique
        ''' </summary>
        Friend Sub CheckIfInventoryNumberUnique()

            If String.IsNullOrEmpty(_InventoryNumber.Trim) Then Exit Sub

            Dim myComm As New SQLCommand("CheckIfInventoryNumUniqueForLTA")
            myComm.AddParam("?LD", _ID)
            myComm.AddParam("?NM", _InventoryNumber.Trim)

            Using myData As DataTable = myComm.Fetch
                If myData.Rows.Count > 0 Then Throw New Exception( _
                    "Klaida. Ilgalaikio turto """ & _Name & """ inventorinis numeris nėra unikalus.")
            End Using

        End Sub

        Friend Sub CheckIfCanDelete()
            CheckIfAnyOperationExists(_ID, False)
        End Sub

        Friend Sub CheckIfCanUpdate()

            CheckIfInventoryNumberUnique()

            If Not IsNew Then

                _ChronologyValidator = AcquisitionChronologicValidator.GetAcquisitionChronologicValidator( _
                    _ID, _OldAcquisitionDate, _Name, _ContinuedUsage, Nothing)

                ValidationRules.CheckRules()

                If Not IsValid Then Throw New Exception("Turto '" & _Name _
                    & "' duomenyse yra klaidų: " & BrokenRulesCollection.ToString( _
                    Validation.RuleSeverity.Error))

                CheckIfUpdateDateChanged()

            End If

        End Sub

        Private Sub CheckIfUpdateDateChanged()

            Dim myComm As New SQLCommand("CheckIfLongTermAssetUpdateDateChanged")
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

        Friend Function GetTotalBookEntryList() As BookEntryInternalList

            Dim result As BookEntryInternalList = _
               BookEntryInternalList.NewBookEntryInternalList(BookEntryType.Debetas)

            Dim DebetSum As Double = 0
            Dim CreditSum As Double = 0

            Dim AcquisitionAccountBookEntry As BookEntryInternal = _
                BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
            AcquisitionAccountBookEntry.Account = _AccountAcquisition
            AcquisitionAccountBookEntry.Ammount = CRound(_AcquisitionAccountValue)
            result.Add(AcquisitionAccountBookEntry)

            If CRound(_AmortizationAccountValue) > 0 Then
                Dim AmortizationAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                AmortizationAccountBookEntry.Account = _AccountAccumulatedAmortization
                AmortizationAccountBookEntry.Ammount = CRound(_AmortizationAccountValue)
                result.Add(AmortizationAccountBookEntry)
            End If

            If CRound(_ValueDecreaseAccountValue) > 0 Then
                Dim ValueDecreaseAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                ValueDecreaseAccountBookEntry.Account = _AccountValueDecrease
                ValueDecreaseAccountBookEntry.Ammount = CRound(_ValueDecreaseAccountValue)
                result.Add(ValueDecreaseAccountBookEntry)
            End If

            If CRound(_ValueIncreaseAccountValue) > 0 Then
                Dim ValueIncreaseAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Debetas)
                ValueIncreaseAccountBookEntry.Account = _AccountValueIncrease
                ValueIncreaseAccountBookEntry.Ammount = CRound(_ValueIncreaseAccountValue)
                result.Add(ValueIncreaseAccountBookEntry)
            End If

            If CRound(_ValueIncreaseAmmortizationAccountValue) > 0 Then
                Dim ValueIncreaseAmmortizationAccountBookEntry As BookEntryInternal = _
                    BookEntryInternal.NewBookEntryInternal(BookEntryType.Kreditas)
                ValueIncreaseAmmortizationAccountBookEntry.Account = _AccountRevaluedPortionAmmortization
                ValueIncreaseAmmortizationAccountBookEntry.Ammount = CRound(_ValueIncreaseAmmortizationAccountValue)
                result.Add(ValueIncreaseAmmortizationAccountBookEntry)
            End If

            Return result

        End Function


#End Region

    End Class

End Namespace