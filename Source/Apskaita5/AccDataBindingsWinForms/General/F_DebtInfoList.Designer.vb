<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Friend Class F_DebtInfoList
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_DebtInfoList))
        Me.IsBuyerRadioButton = New System.Windows.Forms.RadioButton
        Me.IsSupplierRadioButton = New System.Windows.Forms.RadioButton
        Me.IgnorePersonTypeCheckBox = New System.Windows.Forms.CheckBox
        Me.AccountAccGridComboBox = New AccControlsWinForms.AccListComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.PersonGroupComboBox = New AccControlsWinForms.AccListComboBox
        Me.RefreshButton = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel
        Me.MarginOfErrorAccTextBox = New AccControlsWinForms.AccTextBox
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel
        Me.ShowZeroDebtsCheckBox = New System.Windows.Forms.CheckBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.DateFromAccDatePicker = New AccControlsWinForms.AccDatePicker
        Me.DateToAccDatePicker = New AccControlsWinForms.AccDatePicker
        Me.DebtInfoListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.DebtInfoListDataListView = New BrightIdeasSoftware.DataListView
        Me.OlvColumn2 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn1 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn3 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn4 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn5 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn6 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn7 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn8 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn9 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn10 = New BrightIdeasSoftware.OLVColumn
        Me.ProgressFiller1 = New AccControlsWinForms.ProgressFiller
        Me.Panel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        CType(Me.DebtInfoListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DebtInfoListDataListView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'IsBuyerRadioButton
        '
        Me.IsBuyerRadioButton.AutoSize = True
        Me.IsBuyerRadioButton.Checked = True
        Me.IsBuyerRadioButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IsBuyerRadioButton.Location = New System.Drawing.Point(389, 3)
        Me.IsBuyerRadioButton.Name = "IsBuyerRadioButton"
        Me.IsBuyerRadioButton.Size = New System.Drawing.Size(64, 17)
        Me.IsBuyerRadioButton.TabIndex = 1
        Me.IsBuyerRadioButton.TabStop = True
        Me.IsBuyerRadioButton.Text = "Pirkėjų"
        Me.IsBuyerRadioButton.UseVisualStyleBackColor = True
        '
        'IsSupplierRadioButton
        '
        Me.IsSupplierRadioButton.AutoSize = True
        Me.IsSupplierRadioButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IsSupplierRadioButton.Location = New System.Drawing.Point(316, 3)
        Me.IsSupplierRadioButton.Name = "IsSupplierRadioButton"
        Me.IsSupplierRadioButton.Size = New System.Drawing.Size(67, 17)
        Me.IsSupplierRadioButton.TabIndex = 0
        Me.IsSupplierRadioButton.Text = "Tiekėjų"
        Me.IsSupplierRadioButton.UseVisualStyleBackColor = True
        '
        'IgnorePersonTypeCheckBox
        '
        Me.IgnorePersonTypeCheckBox.AutoSize = True
        Me.IgnorePersonTypeCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IgnorePersonTypeCheckBox.Location = New System.Drawing.Point(158, 3)
        Me.IgnorePersonTypeCheckBox.Margin = New System.Windows.Forms.Padding(15, 3, 3, 3)
        Me.IgnorePersonTypeCheckBox.Name = "IgnorePersonTypeCheckBox"
        Me.IgnorePersonTypeCheckBox.Size = New System.Drawing.Size(152, 17)
        Me.IgnorePersonTypeCheckBox.TabIndex = 2
        Me.IgnorePersonTypeCheckBox.Text = "Ignoruoti kontrah. tipą"
        Me.IgnorePersonTypeCheckBox.UseVisualStyleBackColor = True
        '
        'AccountAccGridComboBox
        '
        Me.AccountAccGridComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AccountAccGridComboBox.EmptyValueString = ""
        Me.AccountAccGridComboBox.InstantBinding = True
        Me.AccountAccGridComboBox.Location = New System.Drawing.Point(617, 3)
        Me.AccountAccGridComboBox.Name = "AccountAccGridComboBox"
        Me.AccountAccGridComboBox.Size = New System.Drawing.Size(201, 21)
        Me.AccountAccGridComboBox.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(551, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(60, 27)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Sąskaita:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(314, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.Label2.Size = New System.Drawing.Size(24, 27)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "iki:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(0, 7, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(98, 27)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Laikotarpis nuo:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(3, 53)
        Me.Label4.Name = "Label4"
        Me.Label4.Padding = New System.Windows.Forms.Padding(0, 7, 0, 0)
        Me.Label4.Size = New System.Drawing.Size(98, 28)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Asmenų grupė:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'PersonGroupComboBox
        '
        Me.TableLayoutPanel3.SetColumnSpan(Me.PersonGroupComboBox, 5)
        Me.PersonGroupComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PersonGroupComboBox.EmptyValueString = ""
        Me.PersonGroupComboBox.InstantBinding = True
        Me.PersonGroupComboBox.Location = New System.Drawing.Point(107, 56)
        Me.PersonGroupComboBox.Name = "PersonGroupComboBox"
        Me.PersonGroupComboBox.Size = New System.Drawing.Size(711, 21)
        Me.PersonGroupComboBox.TabIndex = 4
        '
        'RefreshButton
        '
        Me.RefreshButton.Image = Global.AccDataBindingsWinForms.My.Resources.Resources.Button_Reload_icon_24p
        Me.RefreshButton.Location = New System.Drawing.Point(824, 3)
        Me.RefreshButton.Name = "RefreshButton"
        Me.TableLayoutPanel3.SetRowSpan(Me.RefreshButton, 3)
        Me.RefreshButton.Size = New System.Drawing.Size(33, 34)
        Me.RefreshButton.TabIndex = 0
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.Controls.Add(Me.GroupBox1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(866, 100)
        Me.Panel1.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel3)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(866, 100)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Gauti duomenis iš duomenų bazės"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 7
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel3.Controls.Add(Me.MarginOfErrorAccTextBox, 1, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.PersonGroupComboBox, 1, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.Label4, 0, 2)
        Me.TableLayoutPanel3.Controls.Add(Me.RefreshButton, 6, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.AccountAccGridComboBox, 5, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.FlowLayoutPanel1, 2, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Label5, 0, 1)
        Me.TableLayoutPanel3.Controls.Add(Me.Label3, 4, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Label2, 2, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.DateFromAccDatePicker, 1, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.DateToAccDatePicker, 3, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 3
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(860, 81)
        Me.TableLayoutPanel3.TabIndex = 6
        '
        'MarginOfErrorAccTextBox
        '
        Me.MarginOfErrorAccTextBox.DecimalValue = 5
        Me.MarginOfErrorAccTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MarginOfErrorAccTextBox.Location = New System.Drawing.Point(107, 30)
        Me.MarginOfErrorAccTextBox.Name = "MarginOfErrorAccTextBox"
        Me.MarginOfErrorAccTextBox.NegativeValue = False
        Me.MarginOfErrorAccTextBox.Size = New System.Drawing.Size(201, 20)
        Me.MarginOfErrorAccTextBox.TabIndex = 0
        Me.MarginOfErrorAccTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'FlowLayoutPanel1
        '
        Me.TableLayoutPanel3.SetColumnSpan(Me.FlowLayoutPanel1, 4)
        Me.FlowLayoutPanel1.Controls.Add(Me.ShowZeroDebtsCheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.IgnorePersonTypeCheckBox)
        Me.FlowLayoutPanel1.Controls.Add(Me.IsSupplierRadioButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.IsBuyerRadioButton)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(311, 27)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(510, 26)
        Me.FlowLayoutPanel1.TabIndex = 6
        '
        'ShowZeroDebtsCheckBox
        '
        Me.ShowZeroDebtsCheckBox.AutoSize = True
        Me.ShowZeroDebtsCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ShowZeroDebtsCheckBox.Location = New System.Drawing.Point(3, 3)
        Me.ShowZeroDebtsCheckBox.Name = "ShowZeroDebtsCheckBox"
        Me.ShowZeroDebtsCheckBox.Size = New System.Drawing.Size(137, 17)
        Me.ShowZeroDebtsCheckBox.TabIndex = 1
        Me.ShowZeroDebtsCheckBox.Text = "Rodyti ne skolingus"
        Me.ShowZeroDebtsCheckBox.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(3, 27)
        Me.Label5.Name = "Label5"
        Me.Label5.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.Label5.Size = New System.Drawing.Size(98, 26)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Paklaida:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'DateFromAccDatePicker
        '
        Me.DateFromAccDatePicker.BoldedDates = Nothing
        Me.DateFromAccDatePicker.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DateFromAccDatePicker.Location = New System.Drawing.Point(107, 3)
        Me.DateFromAccDatePicker.MaxDate = New Date(9998, 12, 31, 0, 0, 0, 0)
        Me.DateFromAccDatePicker.MinDate = New Date(1753, 1, 1, 0, 0, 0, 0)
        Me.DateFromAccDatePicker.Name = "DateFromAccDatePicker"
        Me.DateFromAccDatePicker.ReadOnly = False
        Me.DateFromAccDatePicker.ShowWeekNumbers = True
        Me.DateFromAccDatePicker.Size = New System.Drawing.Size(201, 21)
        Me.DateFromAccDatePicker.TabIndex = 0
        '
        'DateToAccDatePicker
        '
        Me.DateToAccDatePicker.BoldedDates = Nothing
        Me.DateToAccDatePicker.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DateToAccDatePicker.Location = New System.Drawing.Point(344, 3)
        Me.DateToAccDatePicker.MaxDate = New Date(9998, 12, 31, 0, 0, 0, 0)
        Me.DateToAccDatePicker.MinDate = New Date(1753, 1, 1, 0, 0, 0, 0)
        Me.DateToAccDatePicker.Name = "DateToAccDatePicker"
        Me.DateToAccDatePicker.ReadOnly = False
        Me.DateToAccDatePicker.ShowWeekNumbers = True
        Me.DateToAccDatePicker.Size = New System.Drawing.Size(201, 21)
        Me.DateToAccDatePicker.TabIndex = 1
        '
        'DebtInfoListBindingSource
        '
        Me.DebtInfoListBindingSource.DataSource = GetType(ApskaitaObjects.ActiveReports.DebtInfo)
        '
        'DebtInfoListDataListView
        '
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn2)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn1)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn3)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn4)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn5)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn6)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn7)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn8)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn9)
        Me.DebtInfoListDataListView.AllColumns.Add(Me.OlvColumn10)
        Me.DebtInfoListDataListView.AllowColumnReorder = True
        Me.DebtInfoListDataListView.AutoGenerateColumns = False
        Me.DebtInfoListDataListView.CausesValidation = False
        Me.DebtInfoListDataListView.CellEditUseWholeCell = False
        Me.DebtInfoListDataListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.OlvColumn2, Me.OlvColumn3, Me.OlvColumn5, Me.OlvColumn7, Me.OlvColumn8, Me.OlvColumn9, Me.OlvColumn10})
        Me.DebtInfoListDataListView.Cursor = System.Windows.Forms.Cursors.Default
        Me.DebtInfoListDataListView.DataSource = Me.DebtInfoListBindingSource
        Me.DebtInfoListDataListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DebtInfoListDataListView.FullRowSelect = True
        Me.DebtInfoListDataListView.HasCollapsibleGroups = False
        Me.DebtInfoListDataListView.HeaderWordWrap = True
        Me.DebtInfoListDataListView.HideSelection = False
        Me.DebtInfoListDataListView.IncludeColumnHeadersInCopy = True
        Me.DebtInfoListDataListView.Location = New System.Drawing.Point(0, 100)
        Me.DebtInfoListDataListView.Name = "DebtInfoListDataListView"
        Me.DebtInfoListDataListView.RenderNonEditableCheckboxesAsDisabled = True
        Me.DebtInfoListDataListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu
        Me.DebtInfoListDataListView.SelectedBackColor = System.Drawing.Color.PaleGreen
        Me.DebtInfoListDataListView.SelectedForeColor = System.Drawing.Color.Black
        Me.DebtInfoListDataListView.ShowCommandMenuOnRightClick = True
        Me.DebtInfoListDataListView.ShowGroups = False
        Me.DebtInfoListDataListView.ShowImagesOnSubItems = True
        Me.DebtInfoListDataListView.ShowItemCountOnGroups = True
        Me.DebtInfoListDataListView.ShowItemToolTips = True
        Me.DebtInfoListDataListView.Size = New System.Drawing.Size(866, 438)
        Me.DebtInfoListDataListView.TabIndex = 4
        Me.DebtInfoListDataListView.UnfocusedSelectedBackColor = System.Drawing.Color.PaleGreen
        Me.DebtInfoListDataListView.UnfocusedSelectedForeColor = System.Drawing.Color.Black
        Me.DebtInfoListDataListView.UseCellFormatEvents = True
        Me.DebtInfoListDataListView.UseCompatibleStateImageBehavior = False
        Me.DebtInfoListDataListView.UseFilterIndicator = True
        Me.DebtInfoListDataListView.UseFiltering = True
        Me.DebtInfoListDataListView.UseHotItem = True
        Me.DebtInfoListDataListView.UseNotifyPropertyChanged = True
        Me.DebtInfoListDataListView.View = System.Windows.Forms.View.Details
        '
        'OlvColumn2
        '
        Me.OlvColumn2.AspectName = "PersonName"
        Me.OlvColumn2.CellEditUseWholeCell = True
        Me.OlvColumn2.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn2.IsEditable = False
        Me.OlvColumn2.Text = "Pavadinimas (Vardas, Pavardė)"
        Me.OlvColumn2.ToolTipText = ""
        Me.OlvColumn2.Width = 100
        '
        'OlvColumn1
        '
        Me.OlvColumn1.AspectName = "PersonID"
        Me.OlvColumn1.CellEditUseWholeCell = True
        Me.OlvColumn1.DisplayIndex = 1
        Me.OlvColumn1.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn1.IsEditable = False
        Me.OlvColumn1.IsVisible = False
        Me.OlvColumn1.Text = "ID"
        Me.OlvColumn1.ToolTipText = ""
        Me.OlvColumn1.Width = 100
        '
        'OlvColumn3
        '
        Me.OlvColumn3.AspectName = "PersonCode"
        Me.OlvColumn3.CellEditUseWholeCell = True
        Me.OlvColumn3.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn3.IsEditable = False
        Me.OlvColumn3.Text = "Kodas"
        Me.OlvColumn3.ToolTipText = ""
        Me.OlvColumn3.Width = 100
        '
        'OlvColumn4
        '
        Me.OlvColumn4.AspectName = "PersonVatCode"
        Me.OlvColumn4.CellEditUseWholeCell = True
        Me.OlvColumn4.DisplayIndex = 3
        Me.OlvColumn4.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn4.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn4.IsEditable = False
        Me.OlvColumn4.IsVisible = False
        Me.OlvColumn4.Text = "PVM kodas"
        Me.OlvColumn4.ToolTipText = ""
        Me.OlvColumn4.Width = 100
        '
        'OlvColumn5
        '
        Me.OlvColumn5.AspectName = "PersonAddress"
        Me.OlvColumn5.CellEditUseWholeCell = True
        Me.OlvColumn5.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn5.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn5.IsEditable = False
        Me.OlvColumn5.Text = "Adresas"
        Me.OlvColumn5.ToolTipText = ""
        Me.OlvColumn5.Width = 100
        '
        'OlvColumn6
        '
        Me.OlvColumn6.AspectName = "PersonGroup"
        Me.OlvColumn6.CellEditUseWholeCell = True
        Me.OlvColumn6.DisplayIndex = 5
        Me.OlvColumn6.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn6.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn6.IsEditable = False
        Me.OlvColumn6.IsVisible = False
        Me.OlvColumn6.Text = "Grupės"
        Me.OlvColumn6.ToolTipText = ""
        Me.OlvColumn6.Width = 100
        '
        'OlvColumn7
        '
        Me.OlvColumn7.AspectName = "DebtBegin"
        Me.OlvColumn7.AspectToStringFormat = "{0:##,0.00}"
        Me.OlvColumn7.CellEditUseWholeCell = True
        Me.OlvColumn7.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn7.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn7.IsEditable = False
        Me.OlvColumn7.Text = "Skola pradžioje"
        Me.OlvColumn7.ToolTipText = ""
        Me.OlvColumn7.Width = 100
        '
        'OlvColumn8
        '
        Me.OlvColumn8.AspectName = "TurnoverDebet"
        Me.OlvColumn8.AspectToStringFormat = "{0:##,0.00}"
        Me.OlvColumn8.CellEditUseWholeCell = True
        Me.OlvColumn8.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn8.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn8.IsEditable = False
        Me.OlvColumn8.Text = "Apyvarta debetas"
        Me.OlvColumn8.ToolTipText = ""
        Me.OlvColumn8.Width = 100
        '
        'OlvColumn9
        '
        Me.OlvColumn9.AspectName = "TurnoverCredit"
        Me.OlvColumn9.AspectToStringFormat = "{0:##,0.00}"
        Me.OlvColumn9.CellEditUseWholeCell = True
        Me.OlvColumn9.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn9.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn9.IsEditable = False
        Me.OlvColumn9.Text = "Apyvarta kreditas"
        Me.OlvColumn9.ToolTipText = ""
        Me.OlvColumn9.Width = 100
        '
        'OlvColumn10
        '
        Me.OlvColumn10.AspectName = "DebtEnd"
        Me.OlvColumn10.AspectToStringFormat = "{0:##,0.00}"
        Me.OlvColumn10.CellEditUseWholeCell = True
        Me.OlvColumn10.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn10.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn10.IsEditable = False
        Me.OlvColumn10.Text = "Skola pabaigoje"
        Me.OlvColumn10.ToolTipText = ""
        Me.OlvColumn10.Width = 100
        '
        'ProgressFiller1
        '
        Me.ProgressFiller1.Location = New System.Drawing.Point(143, 242)
        Me.ProgressFiller1.Name = "ProgressFiller1"
        Me.ProgressFiller1.Size = New System.Drawing.Size(193, 14)
        Me.ProgressFiller1.TabIndex = 5
        Me.ProgressFiller1.Visible = False
        '
        'F_DebtInfoList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(866, 538)
        Me.Controls.Add(Me.DebtInfoListDataListView)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ProgressFiller1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "F_DebtInfoList"
        Me.ShowInTaskbar = False
        Me.Text = "Skolų žiniaraštis"
        Me.Panel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel3.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        CType(Me.DebtInfoListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DebtInfoListDataListView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents AccountAccGridComboBox As AccControlsWinForms.AccListComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents IsSupplierRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents IsBuyerRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ShowZeroDebtsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents DebtInfoListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents IgnorePersonTypeCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents DebtInfoListDataListView As BrightIdeasSoftware.DataListView
    Friend WithEvents OlvColumn1 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn2 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn3 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn4 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn5 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn6 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn7 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn8 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn9 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn10 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents ProgressFiller1 As AccControlsWinForms.ProgressFiller
    Friend WithEvents PersonGroupComboBox As AccControlsWinForms.AccListComboBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents MarginOfErrorAccTextBox As AccControlsWinForms.AccTextBox
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents DateFromAccDatePicker As AccControlsWinForms.AccDatePicker
    Friend WithEvents DateToAccDatePicker As AccControlsWinForms.AccDatePicker
End Class
