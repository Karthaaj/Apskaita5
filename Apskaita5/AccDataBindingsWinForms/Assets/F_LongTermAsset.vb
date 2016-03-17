Imports ApskaitaObjects.Assets
Imports ApskaitaObjects.HelperLists
Imports AccControlsWinForms
Imports AccDataBindingsWinForms.CachedInfoLists

Friend Class F_LongTermAsset
    Implements IObjectEditForm

    Private ReadOnly _RequiredCachedLists As Type() = New Type() _
        {GetType(HelperLists.AccountInfoList), GetType(LongTermAssetCustomGroupInfoList), GetType(NameInfoList)}

    Private WithEvents _FormManager As CslaActionExtenderEditForm(Of LongTermAsset)
    Private _QueryManager As CslaActionExtenderQueryObject

    Private _DocumentToEdit As LongTermAsset = Nothing


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If _FormManager Is Nothing OrElse _FormManager.DataSource Is Nothing Then
                If _DocumentToEdit Is Nothing OrElse _DocumentToEdit.IsNew Then
                    Return Integer.MinValue
                Else
                    Return _DocumentToEdit.ID
                End If
            End If
            Return _FormManager.DataSource.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(LongTermAsset)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal documentToEdit As LongTermAsset)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _DocumentToEdit = documentToEdit

    End Sub


    Private Sub F_LongTermAsset_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If _DocumentToEdit Is Nothing Then
            _DocumentToEdit = LongTermAsset.GetNewLongTermAsset()
        End If

        If Not SetDataSources() Then Exit Sub

        Try
            _FormManager = New CslaActionExtenderEditForm(Of LongTermAsset) _
                (Me, LongTermAssetBindingSource, _DocumentToEdit, _
                _RequiredCachedLists, nOkButton, ApplyButton, nCancelButton, _
                LimitationsButton, ProgressFiller1)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        ConfigureButtons()

    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, _RequiredCachedLists) Then Return False

        Try

            _QueryManager = New CslaActionExtenderQueryObject(Me, ProgressFiller2)

            SetupDefaultControls(Of LongTermAsset)(Me, LongTermAssetBindingSource)

            LoadNameInfoListToCombo(LegalGroupComboBox, _
                ApskaitaObjects.Settings.NameType.LongTermAssetLegalGroup, True)

            LoadLongTermAssetCustomGroupInfoToListCombo(CustomGroupInfoAccGridComboBox, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function


    Private Sub RefreshJournalEntryInfoButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles RefreshJournalEntryInfoButton.Click

        If _FormManager.DataSource Is Nothing OrElse _FormManager.IsChild Then Exit Sub

        'ActiveReports.JournalEntryInfoList.GetList(_FormManager.DataSource.AcquisitionDate, _
        '    _FormManager.DataSource.AcquisitionDate, "", -1, -1, DocumentType.None, False, "", "")
        _QueryManager.InvokeQuery(Of ActiveReports.JournalEntryInfoList)(Nothing, "GetList", True, _
            AddressOf OnJournalEntryInfoListFetched, _FormManager.DataSource.AcquisitionDate, _
            _FormManager.DataSource.AcquisitionDate, "", -1, -1, DocumentType.None, False, "", "")

    End Sub

    Private Sub OnJournalEntryInfoListFetched(ByVal result As Object, ByVal exceptionHandled As Boolean)

        If result Is Nothing Then Exit Sub

        JournalEntryInfoComboBox.DataSource = DirectCast(result, ActiveReports.JournalEntryInfoList)

    End Sub

    Private Sub AttachJournalEntryInfoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AttachJournalEntryInfoButton.Click

        If _FormManager.DataSource Is Nothing OrElse _FormManager.IsChild Then Exit Sub

        If JournalEntryInfoComboBox.SelectedItem Is Nothing Then Exit Sub

        Try
            _FormManager.DataSource.LoadAssociatedJournalEntry(CType(JournalEntryInfoComboBox.SelectedItem,  _
                ActiveReports.JournalEntryInfo))
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click
        If _FormManager.DataSource Is Nothing OrElse Not _FormManager.DataSource.AcquisitionJournalEntryID > 0 Then Exit Sub
        OpenJournalEntryEditForm(_QueryManager, _FormManager.DataSource.AcquisitionJournalEntryID)
    End Sub


    Private Sub _FormManager_DataSourceStateHasChanged(ByVal sender As Object, _
         ByVal e As System.EventArgs) Handles _FormManager.DataSourceStateHasChanged
        ConfigureButtons()
    End Sub

    Private Sub ConfigureButtons()

        If _FormManager.DataSource Is Nothing Then
            DisableAllControls(Me)
            LimitationsButton.Visible = False
            Exit Sub
        End If

        If _FormManager.DataSource.AllDataIsReadOnly Then
            DisableAllControls(Me)
            LimitationsButton.Visible = False
            ViewJournalEntryButton.Enabled = True
            Exit Sub
        End If

        AccountAcquisitionAccGridComboBox.Enabled = Not _FormManager.DataSource.FinancialDataIsReadOnly
        AccountValueDecreaseAccGridComboBox.Enabled = Not _FormManager.DataSource.FinancialDataIsReadOnly
        AccountAccumulatedAmortizationAccGridComboBox.Enabled = Not _FormManager.DataSource.FinancialDataIsReadOnly
        AccountValueIncreaseAccGridComboBox.Enabled = Not _FormManager.DataSource.FinancialDataIsReadOnly
        AccountRevaluedPortionAmmortizationAccGridComboBox.Enabled = Not _FormManager.DataSource.FinancialDataIsReadOnly
        AcquisitionAccountValuePerUnitAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        AmortizationAccountValuePerUnitAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueDecreaseAccountValuePerUnitAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueIncreaseAccountValuePerUnitAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueIncreaseAmmortizationAccountValuePerUnitAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        AmmountAccTextBox.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        AcquisitionAccountValueCorrectionNumericUpDown.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        AmortizationAccountValueCorrectionNumericUpDown.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueDecreaseAccountValueCorrectionNumericUpDown.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueIncreaseAccountValueCorrectionNumericUpDown.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly
        ValueIncreaseAmmortizationAccountValueCorrectionNumericUpDown.ReadOnly = _FormManager.DataSource.FinancialDataIsReadOnly

        Dim increment As Integer = 1
        If _FormManager.DataSource.FinancialDataIsReadOnly Then
            increment = 0
        End If
        AcquisitionAccountValueCorrectionNumericUpDown.Increment = increment
        AmortizationAccountValueCorrectionNumericUpDown.Increment = increment
        ValueDecreaseAccountValueCorrectionNumericUpDown.Increment = increment
        ValueIncreaseAccountValueCorrectionNumericUpDown.Increment = increment
        ValueIncreaseAmmortizationAccountValueCorrectionNumericUpDown.Increment = increment

        AcquisitionDateDateTimePicker.Enabled = Not _FormManager.DataSource.AcquisitionDateIsReadOnly

        Me.AmortizationCalculatedForMonthsAccTextBox.ReadOnly = _FormManager.DataSource.AmortizationDataIsReadOnly
        Me.DefaultAmortizationPeriodAccTextBox.ReadOnly = _FormManager.DataSource.AmortizationDataIsReadOnly
        Me.LiquidationUnitValueAccTextBox.ReadOnly = _FormManager.DataSource.AmortizationDataIsReadOnly
        Me.ContinuedUsageCheckBox.Enabled = Not _FormManager.DataSource.AmortizationDataIsReadOnly

        ApplyButton.Enabled = Not _FormManager.DataSource Is Nothing AndAlso Not _FormManager.IsChild
        nCancelButton.Enabled = Not _FormManager.DataSource Is Nothing AndAlso (_FormManager.IsChild _
            OrElse Not _FormManager.DataSource.IsNew)

        EditedBaner.Visible = Not _FormManager.DataSource.IsNew

        JournalEntryInfoComboBox.Enabled = Not _FormManager.DataSource Is Nothing AndAlso Not _FormManager.IsChild
        RefreshJournalEntryInfoButton.Enabled = Not _FormManager.DataSource Is Nothing AndAlso Not _FormManager.IsChild
        AttachJournalEntryInfoButton.Enabled = Not _FormManager.DataSource Is Nothing AndAlso Not _FormManager.IsChild

        ViewJournalEntryButton.Enabled = Not _FormManager.DataSource.IsNew

    End Sub

End Class