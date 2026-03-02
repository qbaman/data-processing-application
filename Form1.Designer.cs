namespace FBZ_System
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.grpFilters = new System.Windows.Forms.GroupBox();
            this.btnShowListOnly = new System.Windows.Forms.Button();
            this.btnShowAllRecords = new System.Windows.Forms.Button();
            this.btnClearSearchList = new System.Windows.Forms.Button();
            this.btnRemoveFromSearchList = new System.Windows.Forms.Button();
            this.btnAddToSearchList = new System.Windows.Forms.Button();
            this.lstSearchList = new System.Windows.Forms.ListBox();
            this.lblSearchList = new System.Windows.Forms.Label();
            this.btnShowStats = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbGroupBy = new System.Windows.Forms.ComboBox();
            this.lblGroupBy = new System.Windows.Forms.Label();
            this.cmbSortTitle = new System.Windows.Forms.ComboBox();
            this.lblSortTitle = new System.Windows.Forms.Label();
            this.txtYearTo = new System.Windows.Forms.TextBox();
            this.lblYearTo = new System.Windows.Forms.Label();
            this.txtYearFrom = new System.Windows.Forms.TextBox();
            this.lblYearFrom = new System.Windows.Forms.Label();
            this.cmbResourceType = new System.Windows.Forms.ComboBox();
            this.lblResourceType = new System.Windows.Forms.Label();
            this.cmbGenre = new System.Windows.Forms.ComboBox();
            this.lblGenre = new System.Windows.Forms.Label();
            this.txtNameFilter = new System.Windows.Forms.TextBox();
            this.lblNameFilter = new System.Windows.Forms.Label();
            this.txtTitleFilter = new System.Windows.Forms.TextBox();
            this.lblTitleFilter = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.txtLanguage = new System.Windows.Forms.TextBox();
            this.lblEdition = new System.Windows.Forms.Label();
            this.txtEdition = new System.Windows.Forms.TextBox();
            this.lblNameType = new System.Windows.Forms.Label();
            this.txtNameType = new System.Windows.Forms.TextBox();
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.btnShowHistory = new System.Windows.Forms.Button();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.txtStats = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.grpFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecords.BackgroundColor = System.Drawing.Color.White;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvRecords.Location = new System.Drawing.Point(0, 0);
            this.dgvRecords.MultiSelect = false;
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.ReadOnly = true;
            this.dgvRecords.RowHeadersVisible = false;
            this.dgvRecords.RowTemplate.Height = 25;
            this.dgvRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecords.Size = new System.Drawing.Size(984, 260);
            this.dgvRecords.TabIndex = 0;
            // 
            // grpFilters
            // 
            this.grpFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFilters.BackColor = System.Drawing.Color.FromArgb(30, 30, 55);
            this.grpFilters.Controls.Add(this.btnShowListOnly);
            this.grpFilters.Controls.Add(this.btnShowAllRecords);
            this.grpFilters.Controls.Add(this.btnClearSearchList);
            this.grpFilters.Controls.Add(this.btnRemoveFromSearchList);
            this.grpFilters.Controls.Add(this.btnAddToSearchList);
            this.grpFilters.Controls.Add(this.lstSearchList);
            this.grpFilters.Controls.Add(this.lblSearchList);
            this.grpFilters.Controls.Add(this.btnShowStats);
            this.grpFilters.Controls.Add(this.btnClear);
            this.grpFilters.Controls.Add(this.btnSearch);
            this.grpFilters.Controls.Add(this.cmbGroupBy);
            this.grpFilters.Controls.Add(this.lblGroupBy);
            this.grpFilters.Controls.Add(this.cmbSortTitle);
            this.grpFilters.Controls.Add(this.lblSortTitle);
            this.grpFilters.Controls.Add(this.txtYearTo);
            this.grpFilters.Controls.Add(this.lblYearTo);
            this.grpFilters.Controls.Add(this.txtYearFrom);
            this.grpFilters.Controls.Add(this.lblYearFrom);
            this.grpFilters.Controls.Add(this.cmbResourceType);
            this.grpFilters.Controls.Add(this.lblResourceType);
            this.grpFilters.Controls.Add(this.cmbGenre);
            this.grpFilters.Controls.Add(this.lblGenre);
            this.grpFilters.Controls.Add(this.txtNameFilter);
            this.grpFilters.Controls.Add(this.lblNameFilter);
            this.grpFilters.Controls.Add(this.txtTitleFilter);
            this.grpFilters.Controls.Add(this.lblTitleFilter);
            this.grpFilters.Controls.Add(this.lblLanguage);
            this.grpFilters.Controls.Add(this.txtLanguage);
            this.grpFilters.Controls.Add(this.lblEdition);
            this.grpFilters.Controls.Add(this.txtEdition);
            this.grpFilters.Controls.Add(this.lblNameType);
            this.grpFilters.Controls.Add(this.txtNameType);
            this.grpFilters.Controls.Add(this.lstHistory);
            this.grpFilters.Controls.Add(this.btnShowHistory);
            this.grpFilters.Controls.Add(this.btnClearHistory);
            this.grpFilters.ForeColor = System.Drawing.Color.White;
            this.grpFilters.Location = new System.Drawing.Point(12, 266);
            this.grpFilters.Name = "grpFilters";
            this.grpFilters.Size = new System.Drawing.Size(960, 270);
            this.grpFilters.TabIndex = 1;
            this.grpFilters.TabStop = false;
            this.grpFilters.Text = "Search filters";
            // 
            // LEFT COLUMN
            // 
            // lblTitleFilter
            this.lblTitleFilter.AutoSize = true;
            this.lblTitleFilter.Location = new System.Drawing.Point(16, 28);
            this.lblTitleFilter.Name = "lblTitleFilter";
            this.lblTitleFilter.Size = new System.Drawing.Size(80, 15);
            this.lblTitleFilter.TabIndex = 0;
            this.lblTitleFilter.Text = "Title contains:";
            // txtTitleFilter
            this.txtTitleFilter.BackColor = System.Drawing.Color.White;
            this.txtTitleFilter.ForeColor = System.Drawing.Color.Black;
            this.txtTitleFilter.Location = new System.Drawing.Point(120, 24);
            this.txtTitleFilter.Name = "txtTitleFilter";
            this.txtTitleFilter.Size = new System.Drawing.Size(260, 23);
            this.txtTitleFilter.TabIndex = 1;
            // lblNameFilter
            this.lblNameFilter.AutoSize = true;
            this.lblNameFilter.Location = new System.Drawing.Point(16, 57);
            this.lblNameFilter.Name = "lblNameFilter";
            this.lblNameFilter.Size = new System.Drawing.Size(90, 15);
            this.lblNameFilter.TabIndex = 2;
            this.lblNameFilter.Text = "Creator name:";
            // txtNameFilter
            this.txtNameFilter.BackColor = System.Drawing.Color.White;
            this.txtNameFilter.ForeColor = System.Drawing.Color.Black;
            this.txtNameFilter.Location = new System.Drawing.Point(120, 53);
            this.txtNameFilter.Name = "txtNameFilter";
            this.txtNameFilter.Size = new System.Drawing.Size(260, 23);
            this.txtNameFilter.TabIndex = 3;
            // lblGenre
            this.lblGenre.AutoSize = true;
            this.lblGenre.Location = new System.Drawing.Point(16, 86);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Size = new System.Drawing.Size(42, 15);
            this.lblGenre.TabIndex = 4;
            this.lblGenre.Text = "Genre:";
            // cmbGenre
            this.cmbGenre.BackColor = System.Drawing.Color.White;
            this.cmbGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGenre.ForeColor = System.Drawing.Color.Black;
            this.cmbGenre.FormattingEnabled = true;
            this.cmbGenre.Location = new System.Drawing.Point(120, 82);
            this.cmbGenre.Name = "cmbGenre";
            this.cmbGenre.Size = new System.Drawing.Size(260, 23);
            this.cmbGenre.TabIndex = 5;
            // lblResourceType
            this.lblResourceType.AutoSize = true;
            this.lblResourceType.Location = new System.Drawing.Point(16, 115);
            this.lblResourceType.Name = "lblResourceType";
            this.lblResourceType.Size = new System.Drawing.Size(85, 15);
            this.lblResourceType.TabIndex = 6;
            this.lblResourceType.Text = "Resource type:";
            // cmbResourceType
            this.cmbResourceType.BackColor = System.Drawing.Color.White;
            this.cmbResourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResourceType.ForeColor = System.Drawing.Color.Black;
            this.cmbResourceType.FormattingEnabled = true;
            this.cmbResourceType.Location = new System.Drawing.Point(120, 111);
            this.cmbResourceType.Name = "cmbResourceType";
            this.cmbResourceType.Size = new System.Drawing.Size(260, 23);
            this.cmbResourceType.TabIndex = 7;
            // lblYearFrom
            this.lblYearFrom.AutoSize = true;
            this.lblYearFrom.Location = new System.Drawing.Point(16, 144);
            this.lblYearFrom.Name = "lblYearFrom";
            this.lblYearFrom.Size = new System.Drawing.Size(62, 15);
            this.lblYearFrom.TabIndex = 8;
            this.lblYearFrom.Text = "Year from:";
            // txtYearFrom
            this.txtYearFrom.BackColor = System.Drawing.Color.White;
            this.txtYearFrom.ForeColor = System.Drawing.Color.Black;
            this.txtYearFrom.Location = new System.Drawing.Point(120, 140);
            this.txtYearFrom.Name = "txtYearFrom";
            this.txtYearFrom.Size = new System.Drawing.Size(80, 23);
            this.txtYearFrom.TabIndex = 9;
            // lblYearTo
            this.lblYearTo.AutoSize = true;
            this.lblYearTo.Location = new System.Drawing.Point(210, 144);
            this.lblYearTo.Name = "lblYearTo";
            this.lblYearTo.Size = new System.Drawing.Size(49, 15);
            this.lblYearTo.TabIndex = 10;
            this.lblYearTo.Text = "Year to:";
            // txtYearTo
            this.txtYearTo.BackColor = System.Drawing.Color.White;
            this.txtYearTo.ForeColor = System.Drawing.Color.Black;
            this.txtYearTo.Location = new System.Drawing.Point(270, 140);
            this.txtYearTo.Name = "txtYearTo";
            this.txtYearTo.Size = new System.Drawing.Size(110, 23);
            this.txtYearTo.TabIndex = 11;
            // 
            // RIGHT COLUMN (advanced + sort / group)
            // 
            // lblLanguage
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(410, 28);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(63, 15);
            this.lblLanguage.TabIndex = 23;
            this.lblLanguage.Text = "Language:";
            // txtLanguage
            this.txtLanguage.BackColor = System.Drawing.Color.White;
            this.txtLanguage.ForeColor = System.Drawing.Color.Black;
            this.txtLanguage.Location = new System.Drawing.Point(500, 24);
            this.txtLanguage.Name = "txtLanguage";
            this.txtLanguage.Size = new System.Drawing.Size(150, 23);
            this.txtLanguage.TabIndex = 24;
            // lblEdition
            this.lblEdition.AutoSize = true;
            this.lblEdition.Location = new System.Drawing.Point(410, 57);
            this.lblEdition.Name = "lblEdition";
            this.lblEdition.Size = new System.Drawing.Size(48, 15);
            this.lblEdition.TabIndex = 25;
            this.lblEdition.Text = "Edition:";
            // txtEdition
            this.txtEdition.BackColor = System.Drawing.Color.White;
            this.txtEdition.ForeColor = System.Drawing.Color.Black;
            this.txtEdition.Location = new System.Drawing.Point(500, 53);
            this.txtEdition.Name = "txtEdition";
            this.txtEdition.Size = new System.Drawing.Size(150, 23);
            this.txtEdition.TabIndex = 26;
            // lblNameType (hidden)
            this.lblNameType.AutoSize = true;
            this.lblNameType.Location = new System.Drawing.Point(410, 86);
            this.lblNameType.Name = "lblNameType";
            this.lblNameType.Size = new System.Drawing.Size(0, 15);
            this.lblNameType.TabIndex = 27;
            // txtNameType
            this.txtNameType.BackColor = System.Drawing.Color.White;
            this.txtNameType.ForeColor = System.Drawing.Color.Black;
            this.txtNameType.Location = new System.Drawing.Point(500, 82);
            this.txtNameType.Name = "txtNameType";
            this.txtNameType.Size = new System.Drawing.Size(150, 23);
            this.txtNameType.TabIndex = 28;
            this.txtNameType.Visible = false;
            // lblSortTitle
            this.lblSortTitle.AutoSize = true;
            this.lblSortTitle.Location = new System.Drawing.Point(410, 115);
            this.lblSortTitle.Name = "lblSortTitle";
            this.lblSortTitle.Size = new System.Drawing.Size(53, 15);
            this.lblSortTitle.TabIndex = 12;
            this.lblSortTitle.Text = "Sort title:";
            // cmbSortTitle
            this.cmbSortTitle.BackColor = System.Drawing.Color.White;
            this.cmbSortTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortTitle.ForeColor = System.Drawing.Color.Black;
            this.cmbSortTitle.FormattingEnabled = true;
            this.cmbSortTitle.Location = new System.Drawing.Point(500, 111);
            this.cmbSortTitle.Name = "cmbSortTitle";
            this.cmbSortTitle.Size = new System.Drawing.Size(150, 23);
            this.cmbSortTitle.TabIndex = 13;
            // lblGroupBy
            this.lblGroupBy.AutoSize = true;
            this.lblGroupBy.Location = new System.Drawing.Point(410, 144);
            this.lblGroupBy.Name = "lblGroupBy";
            this.lblGroupBy.Size = new System.Drawing.Size(57, 15);
            this.lblGroupBy.TabIndex = 14;
            this.lblGroupBy.Text = "Group by:";
            // cmbGroupBy
            this.cmbGroupBy.BackColor = System.Drawing.Color.White;
            this.cmbGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGroupBy.ForeColor = System.Drawing.Color.Black;
            this.cmbGroupBy.FormattingEnabled = true;
            this.cmbGroupBy.Location = new System.Drawing.Point(500, 140);
            this.cmbGroupBy.Name = "cmbGroupBy";
            this.cmbGroupBy.Size = new System.Drawing.Size(150, 23);
            this.cmbGroupBy.TabIndex = 15;
            // 
            // Search / stats / clear buttons
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(675, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 27);
            this.btnSearch.TabIndex = 16;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // btnShowStats
            this.btnShowStats.BackColor = System.Drawing.Color.Gainsboro;
            this.btnShowStats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowStats.ForeColor = System.Drawing.Color.Black;
            this.btnShowStats.Location = new System.Drawing.Point(675, 57);
            this.btnShowStats.Name = "btnShowStats";
            this.btnShowStats.Size = new System.Drawing.Size(90, 27);
            this.btnShowStats.TabIndex = 18;
            this.btnShowStats.Text = "Show stats";
            this.btnShowStats.UseVisualStyleBackColor = false;
            this.btnShowStats.Click += new System.EventHandler(this.btnShowStats_Click);
            // btnClear
            this.btnClear.BackColor = System.Drawing.Color.Gainsboro;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.ForeColor = System.Drawing.Color.Black;
            this.btnClear.Location = new System.Drawing.Point(675, 90);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 27);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear filters";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Search list (session)
            // 
            this.lblSearchList.AutoSize = true;
            this.lblSearchList.Location = new System.Drawing.Point(16, 184);
            this.lblSearchList.Name = "lblSearchList";
            this.lblSearchList.Size = new System.Drawing.Size(122, 15);
            this.lblSearchList.TabIndex = 19;
            this.lblSearchList.Text = "Search list (session):";
            // lstSearchList
            this.lstSearchList.BackColor = System.Drawing.Color.White;
            this.lstSearchList.ForeColor = System.Drawing.Color.Black;
            this.lstSearchList.FormattingEnabled = true;
            this.lstSearchList.HorizontalScrollbar = true;
            this.lstSearchList.ItemHeight = 15;
            this.lstSearchList.Location = new System.Drawing.Point(144, 180);
            this.lstSearchList.Name = "lstSearchList";
            this.lstSearchList.Size = new System.Drawing.Size(320, 49);
            this.lstSearchList.TabIndex = 20;
            // btnAddToSearchList
            this.btnAddToSearchList.BackColor = System.Drawing.Color.Gainsboro;
            this.btnAddToSearchList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddToSearchList.ForeColor = System.Drawing.Color.Black;
            this.btnAddToSearchList.Location = new System.Drawing.Point(480, 180);
            this.btnAddToSearchList.Name = "btnAddToSearchList";
            this.btnAddToSearchList.Size = new System.Drawing.Size(94, 23);
            this.btnAddToSearchList.TabIndex = 21;
            this.btnAddToSearchList.Text = "Add to list";
            this.btnAddToSearchList.UseVisualStyleBackColor = false;
            this.btnAddToSearchList.Click += new System.EventHandler(this.btnAddToSearchList_Click);
            // btnRemoveFromSearchList
            this.btnRemoveFromSearchList.BackColor = System.Drawing.Color.Gainsboro;
            this.btnRemoveFromSearchList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveFromSearchList.ForeColor = System.Drawing.Color.Black;
            this.btnRemoveFromSearchList.Location = new System.Drawing.Point(480, 206);
            this.btnRemoveFromSearchList.Name = "btnRemoveFromSearchList";
            this.btnRemoveFromSearchList.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveFromSearchList.TabIndex = 22;
            this.btnRemoveFromSearchList.Text = "Remove";
            this.btnRemoveFromSearchList.UseVisualStyleBackColor = false;
            this.btnRemoveFromSearchList.Click += new System.EventHandler(this.btnRemoveFromSearchList_Click);
            // btnShowListOnly
            this.btnShowListOnly.BackColor = System.Drawing.Color.Gainsboro;
            this.btnShowListOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowListOnly.ForeColor = System.Drawing.Color.Black;
            this.btnShowListOnly.Location = new System.Drawing.Point(144, 235);
            this.btnShowListOnly.Name = "btnShowListOnly";
            this.btnShowListOnly.Size = new System.Drawing.Size(100, 23);
            this.btnShowListOnly.TabIndex = 31;
            this.btnShowListOnly.Text = "Show list only";
            this.btnShowListOnly.UseVisualStyleBackColor = false;
            this.btnShowListOnly.Click += new System.EventHandler(this.btnShowListOnly_Click);
            // btnShowAllRecords
            this.btnShowAllRecords.BackColor = System.Drawing.Color.Gainsboro;
            this.btnShowAllRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAllRecords.ForeColor = System.Drawing.Color.Black;
            this.btnShowAllRecords.Location = new System.Drawing.Point(250, 235);
            this.btnShowAllRecords.Name = "btnShowAllRecords";
            this.btnShowAllRecords.Size = new System.Drawing.Size(120, 23);
            this.btnShowAllRecords.TabIndex = 32;
            this.btnShowAllRecords.Text = "Show all records";
            this.btnShowAllRecords.UseVisualStyleBackColor = false;
            this.btnShowAllRecords.Click += new System.EventHandler(this.btnShowAllRecords_Click);
            // btnClearSearchList
            this.btnClearSearchList.BackColor = System.Drawing.Color.Gainsboro;
            this.btnClearSearchList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSearchList.ForeColor = System.Drawing.Color.Black;
            this.btnClearSearchList.Location = new System.Drawing.Point(376, 235);
            this.btnClearSearchList.Name = "btnClearSearchList";
            this.btnClearSearchList.Size = new System.Drawing.Size(88, 23);
            this.btnClearSearchList.TabIndex = 33;
            this.btnClearSearchList.Text = "Clear list";
            this.btnClearSearchList.UseVisualStyleBackColor = false;
            this.btnClearSearchList.Click += new System.EventHandler(this.btnClearSearchList_Click);
            // 
            // History area
            // 
            this.lstHistory.BackColor = System.Drawing.Color.White;
            this.lstHistory.ForeColor = System.Drawing.Color.Black;
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.HorizontalScrollbar = true;
            this.lstHistory.ItemHeight = 15;
            this.lstHistory.Location = new System.Drawing.Point(580, 180);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new System.Drawing.Size(260, 49);
            this.lstHistory.TabIndex = 34;
            this.lstHistory.DoubleClick += new System.EventHandler(this.lstHistory_DoubleClick);
            // btnShowHistory
            this.btnShowHistory.BackColor = System.Drawing.Color.Gainsboro;
            this.btnShowHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowHistory.ForeColor = System.Drawing.Color.Black;
            this.btnShowHistory.Location = new System.Drawing.Point(846, 180);
            this.btnShowHistory.Name = "btnShowHistory";
            this.btnShowHistory.Size = new System.Drawing.Size(90, 23);
            this.btnShowHistory.TabIndex = 35;
            this.btnShowHistory.Text = "Show history";
            this.btnShowHistory.UseVisualStyleBackColor = false;
            this.btnShowHistory.Click += new System.EventHandler(this.btnShowHistory_Click);
            // btnClearHistory
            this.btnClearHistory.BackColor = System.Drawing.Color.Gainsboro;
            this.btnClearHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearHistory.ForeColor = System.Drawing.Color.Black;
            this.btnClearHistory.Location = new System.Drawing.Point(846, 206);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(90, 23);
            this.btnClearHistory.TabIndex = 36;
            this.btnClearHistory.Text = "Clear history";
            this.btnClearHistory.UseVisualStyleBackColor = false;
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            // 
            // txtStats
            // 
            this.txtStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStats.BackColor = System.Drawing.Color.FromArgb(20, 20, 40);
            this.txtStats.ForeColor = System.Drawing.Color.White;
            this.txtStats.Location = new System.Drawing.Point(12, 542);
            this.txtStats.Multiline = true;
            this.txtStats.Name = "txtStats";
            this.txtStats.ReadOnly = true;
            this.txtStats.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStats.Size = new System.Drawing.Size(960, 50);
            this.txtStats.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(15, 15, 30);
            this.ClientSize = new System.Drawing.Size(984, 604);
            this.Controls.Add(this.txtStats);
            this.Controls.Add(this.grpFilters);
            this.Controls.Add(this.dgvRecords);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FBZ System - BL Records Explorer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.grpFilters.ResumeLayout(false);
            this.grpFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.GroupBox grpFilters;
        private System.Windows.Forms.Button btnShowListOnly;
        private System.Windows.Forms.Button btnShowAllRecords;
        private System.Windows.Forms.Button btnClearSearchList;
        private System.Windows.Forms.Button btnRemoveFromSearchList;
        private System.Windows.Forms.Button btnAddToSearchList;
        private System.Windows.Forms.ListBox lstSearchList;
        private System.Windows.Forms.Label lblSearchList;
        private System.Windows.Forms.Button btnShowStats;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbGroupBy;
        private System.Windows.Forms.Label lblGroupBy;
        private System.Windows.Forms.ComboBox cmbSortTitle;
        private System.Windows.Forms.Label lblSortTitle;
        private System.Windows.Forms.TextBox txtYearTo;
        private System.Windows.Forms.Label lblYearTo;
        private System.Windows.Forms.TextBox txtYearFrom;
        private System.Windows.Forms.Label lblYearFrom;
        private System.Windows.Forms.ComboBox cmbResourceType;
        private System.Windows.Forms.Label lblResourceType;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.TextBox txtNameFilter;
        private System.Windows.Forms.Label lblNameFilter;
        private System.Windows.Forms.TextBox txtTitleFilter;
        private System.Windows.Forms.Label lblTitleFilter;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.TextBox txtLanguage;
        private System.Windows.Forms.Label lblEdition;
        private System.Windows.Forms.TextBox txtEdition;
        private System.Windows.Forms.Label lblNameType;
        private System.Windows.Forms.TextBox txtNameType;
        private System.Windows.Forms.ListBox lstHistory;
        private System.Windows.Forms.Button btnShowHistory;
        private System.Windows.Forms.Button btnClearHistory;
        private System.Windows.Forms.TextBox txtStats;
    }
}
