Imports AccDataAccessLayer
Imports ApskaitaObjects.ActiveReports

Friend Module CachedListsLoaders

#Region "*** CACHE BINDINGS MANAGER METHODS ***"

    Private BindingSourceList As List(Of BindingSourceItem)

    Public Function GetBindingSourceForCachedList(ByVal CachedItemBaseType As Type, _
        ByVal ParamArray FilterCriteria As Object()) As BindingSource

        Dim result As New BindingSourceItem(CachedItemBaseType, FilterCriteria)

        If BindingSourceList Is Nothing Then BindingSourceList = New List(Of BindingSourceItem)
        BindingSourceList.Add(result)

        AddHandler result.BindingSourceInstance.Disposed, AddressOf BindingSource_Disposed

        Return result.BindingSourceInstance

    End Function

    Public Sub CachedListChanged(ByVal e As CacheChangedEventArgs)
        If BindingSourceList Is Nothing Then Exit Sub
        For Each bs As BindingSourceItem In BindingSourceList
            If bs.BaseType Is e.Type Then bs.UpdateDataSource()
        Next
    End Sub

    Private Sub BindingSource_Disposed(ByVal sender As Object, ByVal e As System.EventArgs)

        If BindingSourceList Is Nothing Then Exit Sub

        For i As Integer = BindingSourceList.Count To 1 Step -1
            If BindingSourceList(i - 1).BindingSourceInstance Is DirectCast(sender, BindingSource) Then
                BindingSourceList.RemoveAt(i - 1)
                RemoveHandler DirectCast(sender, BindingSource).Disposed, AddressOf BindingSource_Disposed
                Exit For
            End If
        Next

    End Sub

#End Region


    Public Function PrepareCache(ByRef CallingForm As Form, ByVal ParamArray nBaseType As Type()) As Boolean

        Try
            Using busy As New StatusBusy
                CacheObjectList.GetList(nBaseType)
            End Using
        Catch ex As Exception
            ShowError(ex)
            If Not CallingForm Is Nothing Then DisableAllControls(CallingForm)
            Return False
        End Try

        Return True

    End Function



    Public Sub LoadAssetCustomGroupInfoListToCombo(ByRef ComboObject As ComboBox, _
        ByVal AddEmptyItem As Boolean)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.ValueMember = "GetMe"
        ComboObject.DisplayMember = "Name"
        ComboObject.DataSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.LongTermAssetCustomGroupInfoList), AddEmptyItem)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Sub LoadAssetCustomGroupInfoListToCombo(ByRef ComboObject As DataGridViewComboBoxColumn, _
        ByVal AddEmptyItem As Boolean)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.ValueMember = "GetMe"
        ComboObject.DisplayMember = "Name"
        ComboObject.DataSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.LongTermAssetCustomGroupInfoList), AddEmptyItem)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Sub LoadPersonGroupInfoListToCombo(ByRef ComboObject As ComboBox)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.ValueMember = "GetMe"
        ComboObject.DisplayMember = "Name"
        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(HelperLists.PersonGroupInfoList), True)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Sub LoadPersonGroupInfoListToCombo(ByRef ComboObject As DataGridViewComboBoxColumn)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.ValueMember = "GetMe"
        ComboObject.DisplayMember = "Name"
        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(HelperLists.PersonGroupInfoList), True)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Sub LoadAssignableCRItemListToCombo(ByRef ComboObject As ComboBox)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(HelperLists.AssignableCRItemList), True)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Sub LoadAssignableCRItemListToCombo(ByRef ComboObject As DataGridViewComboBoxColumn)

        If Not ComboObject.DataSource Is Nothing Then Exit Sub

        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(HelperLists.AssignableCRItemList), True)

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

    End Sub

    Public Function LoadTaxRateListToCombo(ByRef ComboObject As ComboBox, _
        ByVal TaxType As TaxTarifType) As Boolean

        If Not ComboObject.DataSource Is Nothing Then Exit Function

        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(Settings.CommonSettings), TaxType, True)

        ComboObject.ValueMember = "TaxRate"
        ComboObject.DisplayMember = "TaxRate"

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

        Return True

    End Function

    Public Function LoadTaxRateListToCombo(ByRef ComboObject As DataGridViewComboBoxColumn, _
        ByVal TaxType As TaxTarifType) As Boolean

        If Not ComboObject.DataSource Is Nothing Then Exit Function

        ComboObject.DataSource = GetBindingSourceForCachedList(GetType(Settings.CommonSettings), TaxType, True)

        ComboObject.ValueMember = "TaxRate"
        ComboObject.DisplayMember = "TaxRate"

        AddHandler ComboObject.Disposed, AddressOf ComboControl_Disposed

        Return True

    End Function

    Public Sub LoadCurrencyCodeListToComboBox(ByRef ComboControl As ComboBox, _
       ByVal AddEmptyItem As Boolean)
        ComboControl.DataSource = CurrencyCodes
    End Sub

    Public Sub LoadCurrencyCodeListToComboBox(ByRef ComboControl As DataGridViewComboBoxColumn, _
       ByVal AddEmptyItem As Boolean)
        ComboControl.DataSource = CurrencyCodes
    End Sub

    Public Sub LoadEnumHumanReadableListToComboBox(ByRef ComboControl As ComboBox, _
        ByVal EnumType As Type, ByVal AddEmptyItem As Boolean)
        ComboControl.DataSource = GetEnumValuesHumanReadableList(EnumType, AddEmptyItem)
    End Sub

    Public Sub LoadEnumLocalizedListToComboBox(ByRef comboControl As ComboBox, _
        ByVal enumType As Type, ByVal addEmptyItem As Boolean)
        Dim datasource As List(Of String) = EnumValueAttribute.GetLocalizedNameList(enumType)
        If AddEmptyItem Then datasource.Insert(0, "")
        comboControl.DataSource = datasource
    End Sub

    Public Sub LoadEnumHumanReadableListToComboBox(ByRef ComboControl As DataGridViewComboBoxColumn, _
        ByVal EnumType As Type, ByVal AddEmptyItem As Boolean)
        ComboControl.DataSource = GetEnumValuesHumanReadableList(EnumType, AddEmptyItem)
    End Sub

    Public Sub LoadEnumLocalizedListToComboBox(ByRef comboControl As DataGridViewComboBoxColumn, _
        ByVal enumType As Type, ByVal addEmptyItem As Boolean)
        Dim datasource As List(Of String) = EnumValueAttribute.GetLocalizedNameList(enumType)
        If addEmptyItem Then datasource.Insert(0, "")
        comboControl.DataSource = datasource
    End Sub

    Public Sub LoadDocumentSerialInfoListToCombo(ByRef ComboObject As ComboBox, _
        ByVal DocType As ApskaitaObjects.Settings.DocumentSerialType, _
        ByVal AddEmptyItem As Boolean, ByVal Reload As Boolean)

        If Not Reload Then
            ComboObject.ValueMember = "Serial"
            ComboObject.DisplayMember = "Serial"
            ComboObject.DataSource = GetBindingSourceForCachedList( _
                GetType(HelperLists.DocumentSerialInfoList), AddEmptyItem, DocType)
        End If

    End Sub

    Public Sub LoadDocumentSerialInfoListToCombo(ByRef ComboObject As DataGridViewComboBoxColumn, _
        ByVal DocType As ApskaitaObjects.Settings.DocumentSerialType, _
        ByVal AddEmptyItem As Boolean, ByVal Reload As Boolean)

        If Not Reload Then
            ComboObject.ValueMember = "Serial"
            ComboObject.DisplayMember = "Serial"
            ComboObject.ValueType = GetType(HelperLists.DocumentSerialInfo)
            ComboObject.DataSource = GetBindingSourceForCachedList( _
                GetType(HelperLists.DocumentSerialInfoList), AddEmptyItem, DocType)
        End If

    End Sub

    Public Sub LoadLanguageListToComboBox(ByRef ComboControl As ComboBox, ByVal AddEmptyItem As Boolean)

        If Not ComboControl.DataSource Is Nothing Then Exit Sub

        ComboControl.DataSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.CompanyRegionalInfoList), AddEmptyItem)

        AddHandler ComboControl.Disposed, AddressOf LanguageListComboBox_Disposed

    End Sub

    Public Sub LoadLanguageListToComboBox(ByRef ComboControl As DataGridViewComboBoxColumn, ByVal AddEmptyItem As Boolean)

        If Not ComboControl.DataSource Is Nothing Then Exit Sub

        ComboControl.DataSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.CompanyRegionalInfoList), AddEmptyItem)

        AddHandler ComboControl.Disposed, AddressOf LanguageListComboBox_Disposed

    End Sub



    Public Sub LoadCashAccountInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, ByVal AddEmptyItem As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn4 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.CashAccountInfoList), True, AddEmptyItem)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3, DataGridViewTextBoxColumn4})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Account"
        DataGridViewTextBoxColumn2.HeaderText = "Sąskaita"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True

        DataGridViewTextBoxColumn3.DataPropertyName = "CurrencyCode"
        DataGridViewTextBoxColumn3.HeaderText = "Valiuta"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn4.DataPropertyName = "Name"
        DataGridViewTextBoxColumn4.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn4.Name = ""
        DataGridViewTextBoxColumn4.ReadOnly = True
        DataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Account")

    End Sub

    Public Sub LoadPersonInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal ShowClients As Boolean, _
        ByVal ShowSuppliers As Boolean, ByVal ShowWorkers As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.PersonInfoList), AddEmptyItem, ShowClients, ShowSuppliers, ShowWorkers)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Name"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True

        DataGridViewTextBoxColumn3.DataPropertyName = "Code"
        DataGridViewTextBoxColumn3.HeaderText = "Code"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Public Sub LoadAccountInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal ParamArray ClassFilter() As Integer)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.AccountInfoList), AddEmptyItem, ClassFilter)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.DataPropertyName = "ID"
        DataGridViewTextBoxColumn2.HeaderText = "Nr."
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet

        DataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewTextBoxColumn3.DataPropertyName = "Name"
        DataGridViewTextBoxColumn3.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetValueMember("ID")
        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("ID")
        ComboObject.SetEmptyValueString("0")

    End Sub

    Public Sub LoadServiceInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal ShowSales As Boolean, _
        ByVal ShowPurchases As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.ServiceInfoList), AddEmptyItem, ShowSales, ShowPurchases, True)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.DataPropertyName = "NameShort"
        DataGridViewTextBoxColumn2.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("NameShort")

    End Sub

    Public Sub LoadLongTermAssetInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn1 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = GetType(LongTermAssetOperationInfoList)
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn1, DataGridViewTextBoxColumn2})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn1.DataPropertyName = "InventoryNumber"
        DataGridViewTextBoxColumn1.HeaderText = "Inv. Nr."
        DataGridViewTextBoxColumn1.Name = ""
        DataGridViewTextBoxColumn1.ReadOnly = True
        DataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Public Sub LoadWorkTimeClassInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal ShowWithoutHours As Boolean, _
        ByVal ShowWithHours As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.WorkTimeClassInfoList), AddEmptyItem, ShowWithoutHours, ShowWithHours)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.DataPropertyName = "Code"
        DataGridViewTextBoxColumn2.HeaderText = "Kodas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn3.DataPropertyName = "Name"
        DataGridViewTextBoxColumn3.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Code")

    End Sub

    Public Sub LoadGoodsInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal TradedType As Documents.TradedItemType)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn4 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.GoodsInfoList), True, AddEmptyItem, TradedType)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3, DataGridViewTextBoxColumn4})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        DataGridViewTextBoxColumn3.DataPropertyName = "GoodsCode"
        DataGridViewTextBoxColumn3.HeaderText = "Kodas"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn4.DataPropertyName = "GoodsBarcode"
        DataGridViewTextBoxColumn4.HeaderText = "BAR Kodas"
        DataGridViewTextBoxColumn4.Name = ""
        DataGridViewTextBoxColumn4.ReadOnly = True
        DataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Public Sub LoadGoodsGroupInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.GoodsGroupInfoList), AddEmptyItem)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {DataGridViewTextBoxColumn2})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Public Sub LoadWarehouseInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn4 As New System.Windows.Forms.DataGridViewCheckBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.WarehouseInfoList), AddEmptyItem, True)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3, DataGridViewTextBoxColumn4})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        DataGridViewTextBoxColumn3.DataPropertyName = "WarehouseAccount"
        DataGridViewTextBoxColumn3.HeaderText = "Sąskaita"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn4.DataPropertyName = "IsProduction"
        DataGridViewTextBoxColumn4.HeaderText = "Gamyba"
        DataGridViewTextBoxColumn4.Name = ""
        DataGridViewTextBoxColumn4.ReadOnly = True
        DataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Public Sub LoadProductionCalculationInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal AddEmptyItem As Boolean, ByVal ShowObsolete As Boolean)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn4 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = GetBindingSourceForCachedList( _
            GetType(HelperLists.ProductionCalculationInfoList), AddEmptyItem, ShowObsolete)

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3, DataGridViewTextBoxColumn4})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Date"
        DataGridViewTextBoxColumn2.HeaderText = "Data"
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn3.DataPropertyName = "GoodsName"
        DataGridViewTextBoxColumn3.HeaderText = "Gaminamos Prekės"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn4.DataPropertyName = "Description"
        DataGridViewTextBoxColumn4.HeaderText = "Aprašymas"
        DataGridViewTextBoxColumn4.Name = ""
        DataGridViewTextBoxColumn4.ReadOnly = True
        DataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("GoodsName")

    End Sub

    Public Sub LoadMunicipalityCodeInfoListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ML As HelperLists.NameValueItemList
        Try
            ML = HelperLists.NameValueItemList.GetNameValueItemList( _
                HelperLists.SettingListType.MunicipalityCodeList)
        Catch ex As Exception
            Throw New Exception("Nepavyko įkrauti savivaldybių kodų: " & ex.Message, ex)
        End Try

        Dim ListBindingSource As New BindingSource
        ListBindingSource.DataSource = ML

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.DataPropertyName = "Value"
        DataGridViewTextBoxColumn2.HeaderText = "Nr."
        DataGridViewTextBoxColumn2.Name = ""
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewTextBoxColumn3.DataPropertyName = "Name"
        DataGridViewTextBoxColumn3.HeaderText = "Pavadinimas"
        DataGridViewTextBoxColumn3.Name = ""
        DataGridViewTextBoxColumn3.ReadOnly = True

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf ComboControl_Disposed

        ComboObject.SetValueMember("Value")
        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub


    Public Sub LoadLocalUserListToGridCombo(Of T As IGridComboBox)(ByRef ComboObject As T, _
        ByVal list As AccDataAccessLayer.Security.LocalUserList)

        If ComboObject.HasAtachedGrid Then Exit Sub

        Dim result As New DataGridView
        Dim DataGridViewTextBoxColumn2 As New System.Windows.Forms.DataGridViewTextBoxColumn
        Dim DataGridViewTextBoxColumn3 As New System.Windows.Forms.DataGridViewTextBoxColumn

        Dim ListBindingSource As BindingSource = New BindingSource
        ListBindingSource.DataSource = list

        CType(result, System.ComponentModel.ISupportInitialize).BeginInit()

        result.AllowUserToAddRows = False
        result.AllowUserToDeleteRows = False
        result.AutoGenerateColumns = False
        result.AllowUserToResizeRows = False
        result.ColumnHeadersVisible = False
        result.RowHeadersVisible = False
        result.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        result.DataSource = ListBindingSource
        result.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() _
            {DataGridViewTextBoxColumn2, DataGridViewTextBoxColumn3})
        result.ReadOnly = True
        result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        result.Size = New System.Drawing.Size(300, 220)
        result.AutoSize = False

        DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewTextBoxColumn2.HeaderText = "Name"
        DataGridViewTextBoxColumn2.Name = "NameColumn"
        DataGridViewTextBoxColumn2.ReadOnly = True
        DataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

        DataGridViewTextBoxColumn3.DataPropertyName = "ServerAddress"
        DataGridViewTextBoxColumn3.HeaderText = "Server Address"
        DataGridViewTextBoxColumn3.Name = "ServerAddressColumn"
        DataGridViewTextBoxColumn3.ReadOnly = True
        DataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

        result.BindingContext = ComboObject.GetBindingContext

        CType(result, System.ComponentModel.ISupportInitialize).EndInit()

        AddHandler result.Disposed, AddressOf LocalUserListToGridCombo_Disposed

        ComboObject.SetNestedDataGridView(result)
        ComboObject.SetCloseOnSingleClick(True)
        ComboObject.SetFilterPropertyName("Name")

    End Sub

    Private Sub LocalUserListToGridCombo_Disposed(ByVal sender As Object, ByVal e As System.EventArgs)

        If TypeOf sender Is DataGridView Then
            RemoveHandler DirectCast(sender, DataGridView).Disposed, _
                AddressOf LocalUserListToGridCombo_Disposed
            DirectCast(DirectCast(sender, DataGridView).DataSource, BindingSource).Dispose()
        End If

    End Sub



    Private Sub ComboControl_Disposed(ByVal sender As Object, ByVal e As System.EventArgs)
        If TypeOf sender Is ComboBox Then
            RemoveHandler DirectCast(sender, ComboBox).Disposed, AddressOf ComboControl_Disposed
        ElseIf TypeOf sender Is DataGridViewComboBoxColumn Then
            RemoveHandler DirectCast(sender, DataGridViewComboBoxColumn).Disposed, _
                AddressOf ComboControl_Disposed
        ElseIf TypeOf sender Is DataGridView Then
            RemoveHandler DirectCast(sender, DataGridView).Disposed, AddressOf ComboControl_Disposed
        End If
        If TypeOf sender Is ComboBox Then
            Try
                DirectCast(DirectCast(sender, ComboBox).DataSource, BindingSource).Dispose()
            Catch ex As Exception
            End Try
        ElseIf TypeOf sender Is DataGridViewComboBoxColumn Then
            Try
                DirectCast(DirectCast(sender, DataGridViewComboBoxColumn).DataSource, BindingSource).Dispose()
            Catch ex As Exception
            End Try
        ElseIf TypeOf sender Is DataGridView Then
            Try
                DirectCast(DirectCast(sender, DataGridView).DataSource, BindingSource).Dispose()
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub LanguageListComboBox_Disposed(ByVal sender As Object, _
        ByVal e As System.EventArgs)
        If TypeOf sender Is ComboBox Then
            Try
                DirectCast(DirectCast(sender, ComboBox).DataSource, BindingSource).Dispose()
            Catch ex As Exception
            End Try
            RemoveHandler DirectCast(sender, ComboBox).Disposed, _
                AddressOf LanguageListComboBox_Disposed
        Else
            Try
                DirectCast(DirectCast(sender, DataGridViewComboBoxColumn).DataSource, BindingSource).Dispose()
            Catch ex As Exception
            End Try
            RemoveHandler DirectCast(sender, DataGridViewComboBoxColumn).Disposed, _
                AddressOf LanguageListComboBox_Disposed
        End If
    End Sub

End Module