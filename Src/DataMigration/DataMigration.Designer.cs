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
            groupBox1.SuspendLayout();
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
            label3.Location = new Point(444, 40);
            label3.Name = "label3";
            label3.Size = new Size(92, 17);
            label3.TabIndex = 4;
            label3.Text = "源数据库类型：";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(432, 117);
            label5.Name = "label5";
            label5.Size = new Size(104, 17);
            label5.TabIndex = 6;
            label5.Text = "目标数据库类型：";
            // 
            // cmbSourceDbType
            // 
            cmbSourceDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSourceDbType.FormattingEnabled = true;
            cmbSourceDbType.Location = new Point(532, 37);
            cmbSourceDbType.Name = "cmbSourceDbType";
            cmbSourceDbType.Size = new Size(121, 25);
            cmbSourceDbType.TabIndex = 7;
            // 
            // cmbToDbType
            // 
            cmbToDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbToDbType.FormattingEnabled = true;
            cmbToDbType.Location = new Point(532, 114);
            cmbToDbType.Name = "cmbToDbType";
            cmbToDbType.Size = new Size(121, 25);
            cmbToDbType.TabIndex = 8;
            // 
            // btnDataMigration
            // 
            btnDataMigration.Font = new Font("Microsoft YaHei UI", 16F);
            btnDataMigration.Location = new Point(231, 266);
            btnDataMigration.Name = "btnDataMigration";
            btnDataMigration.Size = new Size(197, 87);
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
            groupBox1.Location = new Point(434, 175);
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
            // DataMigration
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(660, 376);
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
            Name = "DataMigration";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DataMigration";
            Load += DataMigration_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
    }
}
