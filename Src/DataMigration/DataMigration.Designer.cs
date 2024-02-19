namespace DataMigration
{
    partial class DataMigration
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMigration));
            label1 = new Label();
            label2 = new Label();
            txtSourceConn = new TextBox();
            txtToConn = new TextBox();
            label3 = new Label();
            label5 = new Label();
            cmbSourceDbType = new ComboBox();
            cmbToDbType = new ComboBox();
            btnDataMigration = new Button();
            txtTableNames = new TextBox();
            label4 = new Label();
            groupBox1 = new GroupBox();
            rdoAll = new RadioButton();
            rdoData = new RadioButton();
            rdoStructure = new RadioButton();
            userPage = new UserPage();
            dgvDataMigration = new DataGridView();
            tableName = new DataGridViewTextBoxColumn();
            tableDescription = new DataGridViewTextBoxColumn();
            isStructure = new DataGridViewTextBoxColumn();
            isDataData = new DataGridViewTextBoxColumn();
            dataCount = new DataGridViewTextBoxColumn();
            structureStatus = new DataGridViewTextBoxColumn();
            dataStatus = new DataGridViewTextBoxColumn();
            errMessage = new DataGridViewTextBoxColumn();
            Sql = new DataGridViewTextBoxColumn();
            createTime = new DataGridViewTextBoxColumn();
            dataMigrationDtoBindingSource = new BindingSource(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDataMigration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataMigrationDtoBindingSource).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 37);
            label1.Name = "label1";
            label1.Size = new Size(92, 17);
            label1.TabIndex = 0;
            label1.Text = "源数据库链接：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(2, 117);
            label2.Name = "label2";
            label2.Size = new Size(104, 17);
            label2.TabIndex = 1;
            label2.Text = "目标数据库链接：";
            // 
            // txtSourceConn
            // 
            txtSourceConn.Location = new Point(104, 12);
            txtSourceConn.Multiline = true;
            txtSourceConn.Name = "txtSourceConn";
            txtSourceConn.ScrollBars = ScrollBars.Vertical;
            txtSourceConn.Size = new Size(324, 70);
            txtSourceConn.TabIndex = 2;
            // 
            // txtToConn
            // 
            txtToConn.Location = new Point(104, 93);
            txtToConn.Multiline = true;
            txtToConn.Name = "txtToConn";
            txtToConn.ScrollBars = ScrollBars.Vertical;
            txtToConn.Size = new Size(324, 70);
            txtToConn.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(501, 40);
            label3.Name = "label3";
            label3.Size = new Size(92, 17);
            label3.TabIndex = 4;
            label3.Text = "源数据库类型：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(489, 117);
            label5.Name = "label5";
            label5.Size = new Size(104, 17);
            label5.TabIndex = 6;
            label5.Text = "目标数据库类型：";
            // 
            // cmbSourceDbType
            // 
            cmbSourceDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSourceDbType.FormattingEnabled = true;
            cmbSourceDbType.Location = new Point(589, 37);
            cmbSourceDbType.Name = "cmbSourceDbType";
            cmbSourceDbType.Size = new Size(121, 25);
            cmbSourceDbType.TabIndex = 7;
            // 
            // cmbToDbType
            // 
            cmbToDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbToDbType.FormattingEnabled = true;
            cmbToDbType.Location = new Point(589, 114);
            cmbToDbType.Name = "cmbToDbType";
            cmbToDbType.Size = new Size(121, 25);
            cmbToDbType.TabIndex = 8;
            // 
            // btnDataMigration
            // 
            btnDataMigration.Font = new Font("Microsoft YaHei UI", 16F);
            btnDataMigration.Location = new Point(257, 534);
            btnDataMigration.Name = "btnDataMigration";
            btnDataMigration.Size = new Size(197, 40);
            btnDataMigration.TabIndex = 9;
            btnDataMigration.Text = "开始数据迁移";
            btnDataMigration.UseVisualStyleBackColor = true;
            btnDataMigration.Click += btnDataMigration_Click;
            // 
            // txtTableNames
            // 
            txtTableNames.Location = new Point(104, 175);
            txtTableNames.Multiline = true;
            txtTableNames.Name = "txtTableNames";
            txtTableNames.ScrollBars = ScrollBars.Vertical;
            txtTableNames.Size = new Size(324, 70);
            txtTableNames.TabIndex = 11;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(62, 200);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 10;
            label4.Text = "表名：";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rdoAll);
            groupBox1.Controls.Add(rdoData);
            groupBox1.Controls.Add(rdoStructure);
            groupBox1.Location = new Point(491, 175);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(219, 70);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "迁移类型";
            // 
            // rdoAll
            // 
            rdoAll.AutoSize = true;
            rdoAll.Location = new Point(132, 25);
            rdoAll.Name = "rdoAll";
            rdoAll.Size = new Size(86, 21);
            rdoAll.TabIndex = 2;
            rdoAll.TabStop = true;
            rdoAll.Text = "结构和数据";
            rdoAll.UseVisualStyleBackColor = true;
            // 
            // rdoData
            // 
            rdoData.AutoSize = true;
            rdoData.Location = new Point(75, 25);
            rdoData.Name = "rdoData";
            rdoData.Size = new Size(50, 21);
            rdoData.TabIndex = 1;
            rdoData.TabStop = true;
            rdoData.Text = "数据";
            rdoData.UseVisualStyleBackColor = true;
            // 
            // rdoStructure
            // 
            rdoStructure.AutoSize = true;
            rdoStructure.Location = new Point(16, 25);
            rdoStructure.Name = "rdoStructure";
            rdoStructure.Size = new Size(50, 21);
            rdoStructure.TabIndex = 0;
            rdoStructure.TabStop = true;
            rdoStructure.Text = "结构";
            rdoStructure.UseVisualStyleBackColor = true;
            // 
            // userPage
            // 
            userPage.CurrentPage = 0;
            userPage.Location = new Point(53, 483);
            userPage.Margin = new Padding(4, 4, 4, 4);
            userPage.Name = "userPage";
            userPage.PageSize = 0;
            userPage.Size = new Size(607, 44);
            userPage.TabIndex = 13;
            userPage.TotalPages = 0;
            // 
            // dgvDataMigration
            // 
            dgvDataMigration.AllowUserToAddRows = false;
            dgvDataMigration.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDataMigration.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvDataMigration.AutoGenerateColumns = false;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvDataMigration.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvDataMigration.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDataMigration.Columns.AddRange(new DataGridViewColumn[] { tableName, tableDescription, isStructure, isDataData, dataCount, structureStatus, dataStatus, errMessage, Sql, createTime });
            dgvDataMigration.DataSource = dataMigrationDtoBindingSource;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvDataMigration.DefaultCellStyle = dataGridViewCellStyle4;
            dgvDataMigration.Location = new Point(6, 251);
            dgvDataMigration.Name = "dgvDataMigration";
            dgvDataMigration.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = SystemColors.Control;
            dataGridViewCellStyle5.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle5.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvDataMigration.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDataMigration.RowsDefaultCellStyle = dataGridViewCellStyle6;
            dgvDataMigration.Size = new Size(712, 225);
            dgvDataMigration.TabIndex = 14;
            dgvDataMigration.CellDoubleClick += dgvDataMigration_CellDoubleClick;
            dgvDataMigration.RowPostPaint += dgvDataMigration_RowPostPaint;
            // 
            // tableName
            // 
            tableName.DataPropertyName = "TableName";
            tableName.HeaderText = "表名";
            tableName.Name = "tableName";
            tableName.ReadOnly = true;
            tableName.Width = 67;
            // 
            // tableDescription
            // 
            tableDescription.DataPropertyName = "TableDescription";
            tableDescription.HeaderText = "表注释";
            tableDescription.Name = "tableDescription";
            tableDescription.ReadOnly = true;
            tableDescription.Width = 67;
            // 
            // isStructure
            // 
            isStructure.DataPropertyName = "IsStructure";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "否";
            dataGridViewCellStyle3.NullValue = "False";
            isStructure.DefaultCellStyle = dataGridViewCellStyle3;
            isStructure.HeaderText = "是否同步结构";
            isStructure.Name = "isStructure";
            isStructure.ReadOnly = true;
            isStructure.Width = 67;
            // 
            // isDataData
            // 
            isDataData.DataPropertyName = "IsData";
            isDataData.HeaderText = "是否同步数据";
            isDataData.Name = "isDataData";
            isDataData.ReadOnly = true;
            isDataData.Width = 67;
            // 
            // dataCount
            // 
            dataCount.DataPropertyName = "DataCount";
            dataCount.HeaderText = "数据行数";
            dataCount.Name = "dataCount";
            dataCount.ReadOnly = true;
            dataCount.Width = 67;
            // 
            // structureStatus
            // 
            structureStatus.DataPropertyName = "StructureStatus";
            structureStatus.HeaderText = "结构同步状态";
            structureStatus.Name = "structureStatus";
            structureStatus.ReadOnly = true;
            structureStatus.Width = 66;
            // 
            // dataStatus
            // 
            dataStatus.DataPropertyName = "DataStatus";
            dataStatus.HeaderText = "数据同步状态";
            dataStatus.Name = "dataStatus";
            dataStatus.ReadOnly = true;
            dataStatus.Width = 67;
            // 
            // errMessage
            // 
            errMessage.DataPropertyName = "ErrMessage";
            errMessage.HeaderText = "异常信息";
            errMessage.Name = "errMessage";
            errMessage.ReadOnly = true;
            errMessage.Width = 67;
            // 
            // Sql
            // 
            Sql.DataPropertyName = "Sql";
            Sql.HeaderText = "Sql语句";
            Sql.Name = "Sql";
            Sql.ReadOnly = true;
            Sql.Width = 67;
            // 
            // createTime
            // 
            createTime.DataPropertyName = "CreateTime";
            createTime.HeaderText = "同步时间";
            createTime.Name = "createTime";
            createTime.ReadOnly = true;
            createTime.Width = 67;
            // 
            // dataMigrationDtoBindingSource
            // 
            dataMigrationDtoBindingSource.DataSource = typeof(DataMigrationDto);
            // 
            // DataMigration
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 579);
            Controls.Add(dgvDataMigration);
            Controls.Add(userPage);
            Controls.Add(groupBox1);
            Controls.Add(txtTableNames);
            Controls.Add(label4);
            Controls.Add(btnDataMigration);
            Controls.Add(cmbToDbType);
            Controls.Add(cmbSourceDbType);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(txtToConn);
            Controls.Add(txtSourceConn);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "DataMigration";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DataMigration";
            Load += DataMigration_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDataMigration).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataMigrationDtoBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtSourceConn;
        private TextBox txtToConn;
        private Label label3;
        private Label label5;
        private ComboBox cmbSourceDbType;
        private ComboBox cmbToDbType;
        private Button btnDataMigration;
        private TextBox txtTableNames;
        private Label label4;
        private GroupBox groupBox1;
        private RadioButton rdoAll;
        private RadioButton rdoData;
        private RadioButton rdoStructure;
        private UserPage userPage;
        private DataGridView dgvDataMigration;
        private BindingSource dataMigrationDtoBindingSource;
        private DataGridViewTextBoxColumn tableName;
        private DataGridViewTextBoxColumn tableDescription;
        private DataGridViewTextBoxColumn isStructure;
        private DataGridViewTextBoxColumn isDataData;
        private DataGridViewTextBoxColumn dataCount;
        private DataGridViewTextBoxColumn structureStatus;
        private DataGridViewTextBoxColumn dataStatus;
        private DataGridViewTextBoxColumn errMessage;
        private DataGridViewTextBoxColumn Sql;
        private DataGridViewTextBoxColumn createTime;
    }
}