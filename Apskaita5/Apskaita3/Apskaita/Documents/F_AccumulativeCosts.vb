Imports ApskaitaObjects.Documents
Public Class F_AccumulativeCosts
    Implements IObjectEditForm

    Private Obj As AccumulativeCosts = Nothing
    Private Loading As Boolean = True
    Private OperationID As Integer = 0

    Public ReadOnly Property ObjectID() As Integer _
        Implements IObjectEditForm.ObjectID
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type _
        Implements IObjectEditForm.ObjectType
        Get
            Return GetType(AccumulativeCosts)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nOperationID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OperationID = nOperationID

    End Sub


    Private Sub F_AccumulativeCosts_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList)) Then Exit Sub

    End Sub

    Private Sub F_AccumulativeCosts_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti duomenis?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then
                e.Cancel = True
                Exit Sub
            End If
            If answ = "Taip" Then
                If Not SaveObj() Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If
        End If

        If Not Obj Is Nothing AndAlso Obj.IsDirty Then CancelObj()

        GetFormLayout(Me)
        GetDataGridViewLayOut(ItemsDataGridView)

    End Sub

    Private Sub F_AccumulativeCosts_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not AccumulativeCosts.CanGetObject AndAlso OperationID > 0 Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        ElseIf Not AccumulativeCosts.CanAddObject AndAlso Not OperationID > 0 Then
            MsgBox("Klaida. Jūsų teisių nepakanka prekių duomenims tvarkyti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        If Not SetDataSources() Then Exit Sub

        If OperationID > 0 Then

            Try
                Obj = LoadObject(Of AccumulativeCosts)(Nothing, "GetAccumulativeCosts", True, OperationID)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try

        Else

            Try
                Obj = LoadObject(Of AccumulativeCosts)(Nothing, "NewAccumulativeCosts", True)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try

        End If

        AccumulativeCostsBindingSource.DataSource = Obj

        ConfigureLimitationButton()

        AddDGVColumnSelector(ItemsDataGridView)

        SetDataGridViewLayOut(ItemsDataGridView)
        SetFormLayout(Me)

        ConfigureButtons()

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OkButton.Click
        If Obj Is Nothing Then Exit Sub
        If SaveObj() Then Me.Close()
    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButton.Click
        If Obj Is Nothing Then Exit Sub
        If SaveObj() Then ConfigureButtons()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CancelButton.Click
        If Obj Is Nothing OrElse Obj.IsNew Then Exit Sub
        CancelObj()
    End Sub


    Private Sub DistributeButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles DistributeButton.Click

        If Obj Is Nothing Then Exit Sub

        Try
            Obj.Distribute()
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Private Sub NewDistributionButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewDistributionButton.Click

        If Obj Is Nothing Then Exit Sub

        If Obj.Items.Count > 0 Then
            If Not YesOrNo("DĖMESIO. Operacijai jau yra sukurtų eilučių. Sukuriant naują " _
                & "paskirstymą jos bus prarastos. Ar tikrai norite tęsti?") Then Exit Sub
        End If

        Try
            Obj.Distribute(FirstPeriodDateTimePicker.Value, Convert.ToInt32(PeriodLengthNumericUpDown.Value), _
                Convert.ToInt32(PeriodCountNumericUpDown.Value))
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Private Sub LimitationsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LimitationsButton.Click

        If Obj Is Nothing Then Exit Sub
        MsgBox(Obj.ChronologyValidator.LimitsExplanation & vbCrLf & vbCrLf & _
            Obj.ChronologyValidator.BackgroundExplanation, MsgBoxStyle.Information, "")

    End Sub


    Private Function SaveObj() As Boolean

        If Obj Is Nothing OrElse Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllBrokenRules, MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf & Obj.GetAllWarnings & vbCrLf
        Else
            Question = ""
        End If
        If Obj.IsNew Then
            Question = Question & "Ar tikrai norite įtraukti naujus duomenis?"
            Answer = "Nauji duomenys sėkmingai įtraukti."
        Else
            Question = Question & "Ar tikrai norite pakeisti duomenis?"
            Answer = "Duomenys sėkmingai pakeisti."
        End If

        If Not YesOrNo(Question) Then Return False

        Using bm As New BindingsManager(AccumulativeCostsBindingSource, ItemsSortedBindingSource, _
            Nothing, True, False)

            Try
                Obj = LoadObject(Of AccumulativeCosts)(Obj, "Save", False)
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        ConfigureButtons()
        ConfigureLimitationButton()

        MsgBox(Answer, MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        If Obj Is Nothing OrElse Obj.IsNew Then Exit Sub
        Using bm As New BindingsManager(AccumulativeCostsBindingSource, ItemsSortedBindingSource, _
            Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList)) Then Return False

        Try

            LoadAccountInfoListToGridCombo(AccountCostsAccGridComboBox, False, 6)
            LoadAccountInfoListToGridCombo(AccountAccumulatedCostsAccGridComboBox, False, 2, 4)
            LoadAccountInfoListToGridCombo(AccountDistributedCostsAccGridComboBox, False, 6)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Private Sub ConfigureButtons()

        If Obj Is Nothing Then Exit Sub

        AccountCostsAccGridComboBox.Enabled = Not Obj.AccountCostsIsReadOnly
        AccountAccumulatedCostsAccGridComboBox.Enabled = Not Obj.AccountAccumulatedCostsIsReadOnly
        AccountDistributedCostsAccGridComboBox.Enabled = Not Obj.AccountDistributedCostsIsReadOnly
        SumAccTextBox.ReadOnly = Obj.SumIsReadOnly

        CancelButton.Enabled = Not Obj.IsNew

        EditedBaner.Visible = Not Obj.IsNew

    End Sub

    Private Sub ConfigureLimitationButton()

        If Obj Is Nothing Then Exit Sub

        If Not Obj.ChronologyValidator.LimitsExplanation Is Nothing AndAlso _
            Not String.IsNullOrEmpty(Obj.ChronologyValidator.LimitsExplanation.Trim) Then
            LimitationsButton.Visible = True
        Else
            LimitationsButton.Visible = False
        End If

    End Sub
    
End Class