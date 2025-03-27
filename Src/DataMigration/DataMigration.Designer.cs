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
            label1 = new UILabel();
            label2 = new UILabel();
            txtSourceConn = new UITextBox();
            txtToConn = new UITextBox();
            label3 = new UILabel();
            label5 = new UILabel();
            cmbSourceDbType = new UIComboBox();
            cmbToDbType = new UIComboBox();
            btnDataMigration = new UIButton();
            txtTableNames = new UITextBox();
            label4 = new UILabel();
            groupBox1 = new UIGroupBox();
            rdoAll = new UIRadioButton();
            rdoData = new UIRadioButton();
            rdoStructure = new UIRadioButton();
            dgvDataMigration = new UIDataGridView();
            dataMigrationDtoBindingSource = new BindingSource(components);
            uiPage = new UIPagination();
            tableName = new DataGridViewTextBoxColumn();
            tableDescription = new DataGridViewTextBoxColumn();
            isStructure = new DataGridViewTextBoxColumn();
            isData = new DataGridViewTextBoxColumn();
            dataCount = new DataGridViewTextBoxColumn();
            structureStatus = new DataGridViewTextBoxColumn();
            dataStatus = new DataGridViewTextBoxColumn();
            errMessage = new DataGridViewTextBoxColumn();
            createTime = new DataGridViewTextBoxColumn();
            btnRetry = new DataGridViewButtonColumn();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDataMigration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataMigrationDtoBindingSource).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(48, 48, 48);
            label1.Location = new Point(14, 37);
            label1.Name = "label1";
            label1.Size = new Size(89, 12);
            label1.TabIndex = 0;
            label1.Text = "源数据库链接：";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(48, 48, 48);
            label2.Location = new Point(2, 117);
            label2.Name = "label2";
            label2.Size = new Size(101, 12);
            label2.TabIndex = 1;
            label2.Text = "目标数据库链接：";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSourceConn
            // 
            txtSourceConn.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtSourceConn.Location = new Point(104, 12);
            txtSourceConn.Margin = new Padding(4, 5, 4, 5);
            txtSourceConn.MinimumSize = new Size(1, 16);
            txtSourceConn.Multiline = true;
            txtSourceConn.Name = "txtSourceConn";
            txtSourceConn.Padding = new Padding(5);
            txtSourceConn.ShowScrollBar = true;
            txtSourceConn.ShowText = false;
            txtSourceConn.Size = new Size(324, 70);
            txtSourceConn.TabIndex = 2;
            txtSourceConn.TextAlignment = ContentAlignment.MiddleLeft;
            txtSourceConn.Watermark = "";
            // 
            // txtToConn
            // 
            txtToConn.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtToConn.Location = new Point(104, 93);
            txtToConn.Margin = new Padding(4, 5, 4, 5);
            txtToConn.MinimumSize = new Size(1, 16);
            txtToConn.Multiline = true;
            txtToConn.Name = "txtToConn";
            txtToConn.Padding = new Padding(5);
            txtToConn.ShowScrollBar = true;
            txtToConn.ShowText = false;
            txtToConn.Size = new Size(324, 70);
            txtToConn.TabIndex = 3;
            txtToConn.TextAlignment = ContentAlignment.MiddleLeft;
            txtToConn.Watermark = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(48, 48, 48);
            label3.Location = new Point(501, 40);
            label3.Name = "label3";
            label3.Size = new Size(89, 12);
            label3.TabIndex = 4;
            label3.Text = "源数据库类型：";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(48, 48, 48);
            label5.Location = new Point(489, 117);
            label5.Name = "label5";
            label5.Size = new Size(101, 12);
            label5.TabIndex = 6;
            label5.Text = "目标数据库类型：";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbSourceDbType
            // 
            cmbSourceDbType.DataSource = null;
            cmbSourceDbType.DropDownStyle = UIDropDownStyle.DropDownList;
            cmbSourceDbType.FillColor = Color.White;
            cmbSourceDbType.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            cmbSourceDbType.FormattingEnabled = true;
            cmbSourceDbType.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cmbSourceDbType.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cmbSourceDbType.Location = new Point(589, 37);
            cmbSourceDbType.Margin = new Padding(4, 5, 4, 5);
            cmbSourceDbType.MinimumSize = new Size(63, 0);
            cmbSourceDbType.Name = "cmbSourceDbType";
            cmbSourceDbType.Padding = new Padding(0, 0, 30, 2);
            cmbSourceDbType.Size = new Size(121, 25);
            cmbSourceDbType.TabIndex = 7;
            cmbSourceDbType.TextAlignment = ContentAlignment.MiddleLeft;
            cmbSourceDbType.Watermark = "";
            // 
            // cmbToDbType
            // 
            cmbToDbType.DataSource = null;
            cmbToDbType.DropDownStyle = UIDropDownStyle.DropDownList;
            cmbToDbType.FillColor = Color.White;
            cmbToDbType.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            cmbToDbType.FormattingEnabled = true;
            cmbToDbType.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cmbToDbType.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cmbToDbType.Location = new Point(589, 114);
            cmbToDbType.Margin = new Padding(4, 5, 4, 5);
            cmbToDbType.MinimumSize = new Size(63, 0);
            cmbToDbType.Name = "cmbToDbType";
            cmbToDbType.Padding = new Padding(0, 0, 30, 2);
            cmbToDbType.Size = new Size(121, 25);
            cmbToDbType.TabIndex = 8;
            cmbToDbType.TextAlignment = ContentAlignment.MiddleLeft;
            cmbToDbType.Watermark = "";
            // 
            // btnDataMigration
            // 
            btnDataMigration.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            btnDataMigration.Location = new Point(257, 525);
            btnDataMigration.MinimumSize = new Size(1, 1);
            btnDataMigration.Name = "btnDataMigration";
            btnDataMigration.Size = new Size(197, 40);
            btnDataMigration.TabIndex = 9;
            btnDataMigration.Text = "开始迁移";
            btnDataMigration.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnDataMigration.Click += btnDataMigration_Click;
            // 
            // txtTableNames
            // 
            txtTableNames.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtTableNames.Location = new Point(104, 175);
            txtTableNames.Margin = new Padding(4, 5, 4, 5);
            txtTableNames.MinimumSize = new Size(1, 16);
            txtTableNames.Multiline = true;
            txtTableNames.Name = "txtTableNames";
            txtTableNames.Padding = new Padding(5);
            txtTableNames.ShowScrollBar = true;
            txtTableNames.ShowText = false;
            txtTableNames.Size = new Size(324, 70);
            txtTableNames.TabIndex = 11;
            txtTableNames.TextAlignment = ContentAlignment.MiddleLeft;
            txtTableNames.Watermark = "";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(48, 48, 48);
            label4.Location = new Point(62, 200);
            label4.Name = "label4";
            label4.Size = new Size(41, 12);
            label4.TabIndex = 10;
            label4.Text = "表名：";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rdoAll);
            groupBox1.Controls.Add(rdoData);
            groupBox1.Controls.Add(rdoStructure);
            groupBox1.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox1.Location = new Point(489, 175);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.MinimumSize = new Size(1, 1);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(0, 32, 0, 0);
            groupBox1.Size = new Size(221, 70);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "迁移类型";
            groupBox1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // rdoAll
            // 
            rdoAll.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rdoAll.Location = new Point(123, 33);
            rdoAll.MinimumSize = new Size(1, 1);
            rdoAll.Name = "rdoAll";
            rdoAll.Size = new Size(87, 18);
            rdoAll.TabIndex = 2;
            rdoAll.Text = "结构和数据";
            // 
            // rdoData
            // 
            rdoData.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rdoData.Location = new Point(65, 33);
            rdoData.MinimumSize = new Size(1, 1);
            rdoData.Name = "rdoData";
            rdoData.Size = new Size(52, 18);
            rdoData.TabIndex = 1;
            rdoData.Text = "数据";
            // 
            // rdoStructure
            // 
            rdoStructure.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            rdoStructure.Location = new Point(7, 33);
            rdoStructure.MinimumSize = new Size(1, 1);
            rdoStructure.Name = "rdoStructure";
            rdoStructure.Size = new Size(52, 18);
            rdoStructure.TabIndex = 0;
            rdoStructure.Text = "结构";
            // 
            // dgvDataMigration
            // 
            dgvDataMigration.AllowUserToAddRows = false;
            dgvDataMigration.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(235, 243, 255);
            dgvDataMigration.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvDataMigration.AutoGenerateColumns = false;
            dgvDataMigration.BackgroundColor = Color.White;
            dgvDataMigration.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle2.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvDataMigration.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvDataMigration.ColumnHeadersHeight = 32;
            dgvDataMigration.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDataMigration.Columns.AddRange(new DataGridViewColumn[] { tableName, tableDescription, isStructure, isData, dataCount, structureStatus, dataStatus, errMessage, createTime, btnRetry });
            dgvDataMigration.DataSource = dataMigrationDtoBindingSource;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.NullValue = "False";
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvDataMigration.DefaultCellStyle = dataGridViewCellStyle4;
            dgvDataMigration.EnableHeadersVisualStyles = false;
            dgvDataMigration.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dgvDataMigration.GridColor = Color.FromArgb(80, 160, 255);
            dgvDataMigration.Location = new Point(6, 251);
            dgvDataMigration.Name = "dgvDataMigration";
            dgvDataMigration.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle5.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle5.SelectionForeColor = Color.White;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            dgvDataMigration.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDataMigration.RowsDefaultCellStyle = dataGridViewCellStyle6;
            dgvDataMigration.SelectedIndex = -1;
            dgvDataMigration.Size = new Size(712, 225);
            dgvDataMigration.StripeEvenColor = Color.Empty;
            dgvDataMigration.StripeOddColor = Color.FromArgb(235, 243, 255);
            dgvDataMigration.TabIndex = 14;
            dgvDataMigration.CellContentClick += dgvDataMigration_CellContentClick;
            dgvDataMigration.CellDoubleClick += dgvDataMigration_CellDoubleClick;
            dgvDataMigration.RowPostPaint += dgvDataMigration_RowPostPaint;
            // 
            // dataMigrationDtoBindingSource
            // 
            dataMigrationDtoBindingSource.DataSource = typeof(DataMigrationDto);
            // 
            // uiPage
            // 
            uiPage.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiPage.Location = new Point(160, 482);
            uiPage.Margin = new Padding(4, 5, 4, 5);
            uiPage.MinimumSize = new Size(1, 1);
            uiPage.Name = "uiPage";
            uiPage.PagerCount = 5;
            uiPage.PageSize = 10;
            uiPage.RectSides = ToolStripStatusLabelBorderSides.None;
            uiPage.ShowJumpButton = false;
            uiPage.ShowText = false;
            uiPage.Size = new Size(388, 35);
            uiPage.TabIndex = 15;
            uiPage.Text = "uiPagination1";
            uiPage.TextAlignment = ContentAlignment.MiddleCenter;
            uiPage.PageChanged += uiPage_PageChanged;
            // 
            // tableName
            // 
            tableName.DataPropertyName = "TableName";
            tableName.HeaderText = "表名";
            tableName.Name = "tableName";
            tableName.ReadOnly = true;
            // 
            // tableDescription
            // 
            tableDescription.DataPropertyName = "TableDescription";
            tableDescription.HeaderText = "表描述";
            tableDescription.Name = "tableDescription";
            tableDescription.ReadOnly = true;
            // 
            // isStructure
            // 
            isStructure.DataPropertyName = "IsStructure";
            isStructure.HeaderText = "是否迁移结构";
            isStructure.Name = "isStructure";
            isStructure.ReadOnly = true;
            // 
            // isData
            // 
            isData.DataPropertyName = "IsData";
            isData.HeaderText = "是否迁移数据";
            isData.Name = "isData";
            isData.ReadOnly = true;
            // 
            // dataCount
            // 
            dataCount.DataPropertyName = "DataCount";
            dataCount.HeaderText = "数据条数";
            dataCount.Name = "dataCount";
            dataCount.ReadOnly = true;
            // 
            // structureStatus
            // 
            structureStatus.DataPropertyName = "StructureStatus";
            structureStatus.HeaderText = "结构迁移状态";
            structureStatus.Name = "structureStatus";
            structureStatus.ReadOnly = true;
            // 
            // dataStatus
            // 
            dataStatus.DataPropertyName = "DataStatus";
            dataStatus.HeaderText = "数据迁移状态";
            dataStatus.Name = "dataStatus";
            dataStatus.ReadOnly = true;
            // 
            // errMessage
            // 
            errMessage.DataPropertyName = "ErrMessage";
            errMessage.HeaderText = "异常信息";
            errMessage.Name = "errMessage";
            errMessage.ReadOnly = true;
            // 
            // createTime
            // 
            createTime.DataPropertyName = "CreateTime";
            createTime.HeaderText = "迁移时间";
            createTime.Name = "createTime";
            createTime.ReadOnly = true;
            // 
            // btnRetry
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle3.NullValue = "重试";
            btnRetry.DefaultCellStyle = dataGridViewCellStyle3;
            btnRetry.HeaderText = "";
            btnRetry.Name = "btnRetry";
            btnRetry.ReadOnly = true;
            btnRetry.Text = "重试";
            // 
            // DataMigration
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 570);
            Controls.Add(uiPage);
            Controls.Add(dgvDataMigration);
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
            ((System.ComponentModel.ISupportInitialize)dgvDataMigration).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataMigrationDtoBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Sunny.UI.UILabel label1;
        private Sunny.UI.UILabel label2;
        private Sunny.UI.UITextBox txtSourceConn;
        private Sunny.UI.UITextBox txtToConn;
        private Sunny.UI.UILabel label3;
        private Sunny.UI.UILabel label5;
        private Sunny.UI.UIComboBox cmbSourceDbType;
        private Sunny.UI.UIComboBox cmbToDbType;
        private Sunny.UI.UIButton btnDataMigration;
        private Sunny.UI.UITextBox txtTableNames;
        private Sunny.UI.UILabel label4;
        private Sunny.UI.UIGroupBox groupBox1;
        private Sunny.UI.UIRadioButton rdoAll;
        private Sunny.UI.UIRadioButton rdoData;
        private Sunny.UI.UIRadioButton rdoStructure;
        private Sunny.UI.UIDataGridView dgvDataMigration;
        private BindingSource dataMigrationDtoBindingSource;
        private Sunny.UI.UIPagination uiPage;
        private DataGridViewTextBoxColumn tableName;
        private DataGridViewTextBoxColumn tableDescription;
        private DataGridViewTextBoxColumn isStructure;
        private DataGridViewTextBoxColumn isData;
        private DataGridViewTextBoxColumn dataCount;
        private DataGridViewTextBoxColumn structureStatus;
        private DataGridViewTextBoxColumn dataStatus;
        private DataGridViewTextBoxColumn errMessage;
        private DataGridViewTextBoxColumn createTime;
        private DataGridViewButtonColumn btnRetry;
    }
}
