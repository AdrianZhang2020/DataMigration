global using DataMigration.Dtos;
global using SqlSugar;
global using System.Configuration;
global using System.Data;
namespace DataMigration;

public partial class DataMigration : Form
{
    private static List<DataMigrationDto> gridList;
    private static DataMigrationDto gridModel;
    private static string commonColumns = "";
    private const string textTableNames = "��Ҫͬ���ı����������Ӣ�Ķ��Ÿ�����Ϊ��Ĭ��ͬ�����б�";
    public DataMigration()
    {
        InitializeComponent();
        this.txtTableNames.Text = textTableNames;
        this.txtTableNames.ForeColor = Color.Gray;
        this.txtTableNames.GotFocus += txtTableNames_GotFocus;
        this.txtTableNames.LostFocus += txtTableNames_LostFocus;

        this.uiPage.DataSource = null;
        this.StartPosition = FormStartPosition.CenterScreen;
    }
    private void txtTableNames_GotFocus(object sender, EventArgs e)
    {
        if (txtTableNames.Text == textTableNames)
        {
            txtTableNames.Text = "";
            txtTableNames.ForeColor = SystemColors.WindowText;
        }
    }

    private void txtTableNames_LostFocus(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTableNames.Text))
        {
            txtTableNames.Text = textTableNames;
            txtTableNames.ForeColor = Color.Gray;
        }
    }

    private void DataMigration_Load(object sender, EventArgs e)
    {
        this.txtSourceConn.Text = ConfigurationManager.ConnectionStrings["sourceConnStr"].ConnectionString;
        this.txtToConn.Text = ConfigurationManager.ConnectionStrings["toConnStr"].ConnectionString;
        this.cmbSourceDbType.DataSource = Enum.GetValues(typeof(SqlSugar.DbType));
        this.cmbToDbType.DataSource = Enum.GetValues(typeof(SqlSugar.DbType));
        this.cmbSourceDbType.SelectedIndex = -1;
        this.cmbToDbType.SelectedIndex = -1;
        this.rdoData.Checked = true;
    }

    private async void btnDataMigration_Click(object sender, EventArgs e)
    {
        await Migration();
    }

    private async Task Migration(string tableName = "")
    {
        var msg = "";
        var sourceConnStr = this.txtSourceConn.Text.Trim();//��ȡԴ���ݿ������ַ���
        var toConnStr = this.txtToConn.Text.Trim();//��ȡĿ�����ݿ������ַ���
        var tableNames = this.txtTableNames.Text.Trim().Replace(textTableNames, "");//��ȡ��Ҫͬ���ı���
        var tables = tableNames.IsNotEmptyOrNull() ? tableNames.ToLower().Split(',') : null;
        if (sourceConnStr.IsNullOrEmpty() || toConnStr.IsNullOrEmpty())
        {
            MessageBox.Show("������Դ���ݿ����Ӻ�Ŀ�����ݿ����Ӻ����Ǩ��", "Ǩ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!(this.cmbSourceDbType.SelectedIndex >= 0) || !(this.cmbToDbType.SelectedIndex >= 0))
        {
            MessageBox.Show("��ѡ��Դ���ݿ����ͺ�Ŀ�����ݿ����ͺ����Ǩ��", "Ǩ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        //���������пؼ���Ϊ������
        var isStructure = this.rdoStructure.Checked;
        var isData = this.rdoData.Checked;
        var isAll = this.rdoAll.Checked;
        this.btnDataMigration.Enabled = false;
        this.cmbSourceDbType.Enabled = false;
        this.cmbToDbType.Enabled = false;
        this.txtSourceConn.Enabled = false;
        this.txtToConn.Enabled = false;
        this.txtTableNames.Enabled = false;
        this.rdoStructure.Enabled = false;
        this.rdoData.Enabled = false;
        this.rdoAll.Enabled = false;

        string sourceDblogFilePath = "logs/sourceDb.log";
        string toDbLogFilePath = "logs/toDb.log";
        string errorLogFilePath = "logs/error.log";
        Directory.CreateDirectory(Path.GetDirectoryName(sourceDblogFilePath));
        if (File.Exists(sourceDblogFilePath))
            File.Delete(sourceDblogFilePath);
        if (File.Exists(toDbLogFilePath))
            File.Delete(toDbLogFilePath);
        if (File.Exists(errorLogFilePath))
            File.Delete(errorLogFilePath);

        //����Դ���ݿ����Ӻ�Ŀ�����ݿ����ӵ�config
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var connectionStrings = config.ConnectionStrings.ConnectionStrings;
        connectionStrings["sourceConnStr"].ConnectionString = sourceConnStr;
        connectionStrings["toConnStr"].ConnectionString = toConnStr;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
        try
        {
            this.uiPage.DataSource = null;
            this.dgvDataMigration.DataSource = null;
            ConnectionConfig sourceConfig = new ConnectionConfig();
            sourceConfig.ConfigId = 1;
            sourceConfig.ConnectionString = sourceConnStr;
            sourceConfig.IsAutoCloseConnection = true;
            sourceConfig.MoreSettings = new ConnMoreSettings()
            {
                IsWithNoLockQuery = sourceConfig.DbType == SqlSugar.DbType.SqlServer ? true : false,
                MaxParameterNameLength = 30
            };
            if (Enum.TryParse(this.cmbSourceDbType.SelectedItem.ToString(), out SqlSugar.DbType source))
            {
                sourceConfig.DbType = source;
            }
            else
            {
                MessageBox.Show("����ʧ�ܣ����ݿ������쳣", "Ǩ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlSugarClient sourceDb = new SqlSugarClient(sourceConfig, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (commonColumns.IsNotEmptyOrNull() && sql.ToLower().Contains($"select {commonColumns}"))
                    {
                        // ����־�ļ�������־д���ļ�ĩβ
                        using (StreamWriter writer = File.AppendText(sourceDblogFilePath))
                        {
                            writer.WriteLine("sql��" + sql);
                        }
                    }
                };
            });

            ConnectionConfig toConfig = new ConnectionConfig();
            toConfig.ConfigId = 1;
            toConfig.ConnectionString = toConnStr;
            toConfig.IsAutoCloseConnection = true;
            toConfig.MoreSettings = new ConnMoreSettings()
            {
                MaxParameterNameLength = 30
            };

            if (Enum.TryParse(this.cmbToDbType.SelectedItem.ToString(), out SqlSugar.DbType to))
            {
                toConfig.DbType = to;
            }
            else
                MessageBox.Show("����ʧ�ܣ����ݿ������쳣", "Ǩ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SqlSugarClient toDb = new SqlSugarClient(toConfig, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (!sql.ToLower().Contains("select"))
                    {
                        gridModel.Sql += sql + ";\r\n";
                        // ����־�ļ�������־д���ļ�ĩβ
                        using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                        {
                            writer.WriteLine("sql��" + sql);
                        }
                    }
                };
            });

            gridList = new List<DataMigrationDto>();

            var tableList = sourceDb.DbMaintenance.GetTableInfoList(false);//��ѯԴ���ݿ����б�
            if (tables != null && tables.Length > 0)
                tableList = tableList.Where(w => tables.Contains(w.Name.ToLower())).ToList();
            if (tableName.IsNotEmptyOrNull())
                tableList = tableList.Where(w => w.Name.ToLower() == tableName.ToLower()).ToList();
            tableList = tableList.OrderBy(o => o.Name).ToList();

            foreach (var table in tableList)
            {
                gridModel = new DataMigrationDto()
                {
                    TableName = table.Name,
                    TableDescription = table.Description.ObjToString(),
                    IsStructure = "��",
                    IsData = "��",
                    ErrMessage = "",
                    CreateTime = DateTime.Now
                };

                if (isStructure || isAll)
                    gridModel.IsStructure = "��";
                if (isData || isAll)
                    gridModel.IsData = "��";

                try
                {
                    var sourceColumns = sourceDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//��ѯԴ���ݿ⵱ǰ�������ֶ�                    

                    if (isStructure || isAll)
                    {
                        await Task.Run(() => StructuralMigration(sourceDb, toDb, table, gridModel, sourceColumns));
                    }

                    if (isData || isAll)
                    {
                        await Task.Run(() => MigrationData(sourceDb, toDb, table, gridModel, sourceColumns));
                    }
                }
                catch (Exception ex)
                {
                    gridModel.ErrMessage = ex.Message;
                    // ����־�ļ�������־д���ļ�ĩβ
                    using (StreamWriter writer = File.AppendText(errorLogFilePath))
                    {
                        writer.WriteLine("tableName��" + table.Name);
                        writer.WriteLine("������Ϣ��" + ex.Message);
                    }
                    msg += "tableName��" + table.Name + "��" + "������Ϣ��" + ex.Message + "\r\n";
                }
                gridModel.StructureStatus = gridModel.StructureStatus.IsNullOrEmpty() ? "-" : gridModel.StructureStatus;
                gridModel.DataStatus = gridModel.DataStatus.IsNullOrEmpty() ? "-" : gridModel.DataStatus;
                gridList.Add(gridModel);
                ShowDatas(this.uiPage.ActivePage == 0 ? 1 : this.uiPage.ActivePage);
            }
            if (msg.IsNotEmptyOrNull())
                MessageBox.Show("Ǩ��ʧ�ܣ�" + msg, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Ǩ�Ƴɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.btnDataMigration.Enabled = true;
            this.cmbSourceDbType.Enabled = true;
            this.cmbToDbType.Enabled = true;
            this.txtSourceConn.Enabled = true;
            this.txtToConn.Enabled = true;
            this.txtTableNames.Enabled = true;
            this.rdoStructure.Enabled = true;
            this.rdoData.Enabled = true;
            this.rdoAll.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ǩ��ʧ�ܣ�" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.btnDataMigration.Enabled = true;
            this.cmbSourceDbType.Enabled = true;
            this.cmbToDbType.Enabled = true;
            this.txtSourceConn.Enabled = true;
            this.txtToConn.Enabled = true;
            this.txtTableNames.Enabled = true;
            this.rdoStructure.Enabled = true;
            this.rdoData.Enabled = true;
            this.rdoAll.Enabled = true;
        }
    }

    private static void MigrationData(SqlSugarClient sourceDb, SqlSugarClient toDb, DbTableInfo? table, DataMigrationDto gridModel, List<DbColumnInfo> sourceColumns)
    {
        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))//�ж�Ŀ�����ݿ⵱ǰ���Ƿ����
        {
            try
            {
                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//��ѯĿ�����ݿ⵱ǰ�������ֶ�
                commonColumns = string.Join(",", sourceColumns
                    .Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToList());//ȡ�����߱��ж����ڵ��ֶβ��Զ���ƴ��
                var dataCount = sourceDb.Queryable<DataTable>().AS(table.Name).Count();
                gridModel.DataCount = dataCount;
                var pageSize = 100000;
                var pageCount = Math.Ceiling(dataCount.ObjToDecimal() / pageSize);
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    var data = sourceDb.CopyNew().Queryable<DataTable>().AS(table.Name).Select(commonColumns).ToDataTablePage(pageIndex, pageSize);
                    toDb.CopyNew().Fastest<DataTable>().AS(table.Name).BulkCopy(data);//�����ݷ�ҳ�������뵽Ŀ�����
                }
                gridModel.DataStatus = "�ɹ�";
            }
            catch (Exception ex)
            {
                gridModel.DataStatus = "ʧ��";
                throw ex;
            }
        }
    }

    private static void StructuralMigration(SqlSugarClient sourceDb, SqlSugarClient toDb, DbTableInfo? table, DataMigrationDto gridModel, List<DbColumnInfo> sourceColumns)
    {
        try
        {
            var sourceDtColumns = (sourceDb.Queryable<DataTable>().AS(table.Name).Select("*").Where(w => false).ToDataTable()).Columns;

            var typeBilder = toDb.DynamicBuilder().CreateClass(table.Name, new SugarTable() { TableDescription = table.Description });

            foreach (var item in sourceColumns)
            {
                Type propertyType = sourceDtColumns[item.DbColumnName].DataType;
                var column = new SugarColumn()
                {
                    IsPrimaryKey = item.IsPrimarykey,
                    IsIdentity = item.IsIdentity,
                    IsNullable = item.IsNullable,
                    ColumnDescription = item.ColumnDescription
                };
                if (propertyType != typeof(DateTime) && !item.DefaultValue.ObjToString().ToLower().Contains("newid"))
                {
                    column.DecimalDigits = item.DecimalDigits;
                    column.DefaultValue = item.DefaultValue.ObjToString().Replace("(", "").Replace(")", "").Replace("'", "");
                }
                if ((propertyType == typeof(string) && item.Length < 4000) || propertyType == typeof(decimal))
                {
                    column.Length = item.Length;
                }
                if (item.Length >= 4000 || item.Length == -1)
                {
                    column.ColumnDataType = StaticConfig.CodeFirst_BigString;
                }

                typeBilder.CreateProperty(item.DbColumnName, propertyType, column);
            }
            //������
            var type = typeBilder.BuilderType();

            var toDbNew = toDb.CopyNew();
            if (toDbNew.DbMaintenance.IsAnyTable(table.Name, false))
                toDbNew.DbMaintenance.DropTable(table.Name);

            //������
            toDbNew.CodeFirst.InitTables(type);

            gridModel.StructureStatus = "�ɹ�";
        }
        catch (Exception ex)
        {
            gridModel.StructureStatus = "ʧ��";
            throw ex;
        }
    }

    /// <summary>
    /// ����չʾ
    /// </summary>
    /// <param name="currentPage">��ǰҳ</param>
    private void ShowDatas(int currentPage)
    {
        if (null != gridList && gridList.Count > 0)
        {
            this.uiPage.DataSource = gridList.OrderBy(o => o.CreateTime).ToList();
            this.uiPage.ActivePage = currentPage;
            if (this.uiPage.PageDataSource != null)
                this.dgvDataMigration.DataSource = this.uiPage.PageDataSource;
        }
        else
        {
            this.uiPage.DataSource = null;
        }
    }

    private void dgvDataMigration_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
        Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        e.RowBounds.Location.Y,
        dgvDataMigration.RowHeadersWidth - 4,
        e.RowBounds.Height);
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + (this.uiPage.ActivePage - 1) * this.uiPage.PageSize + 1).ToString(),
        dgvDataMigration.RowHeadersDefaultCellStyle.Font,
        rectangle,
        dgvDataMigration.RowHeadersDefaultCellStyle.ForeColor,
        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
    }

    /// <summary>
    /// չʾ�쳣��Ϣ��sql��ϸ��Ϣ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dgvDataMigration_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        // ȷ��˫���Ĳ��Ǳ�ͷ  
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
            // ��ȡ˫��������  
            DataGridViewColumn clickedColumn = dgvDataMigration.Columns[e.ColumnIndex];
            string columnName = clickedColumn.Name;
            // ����Ƿ�˫�������Ǹ���Ȥ����  
            if (columnName == "errMessage" || columnName == "sql")
            {
                // ��ȡ˫���ĵ�Ԫ���ֵ  
                string cellValue = dgvDataMigration.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ObjToString();
                if (cellValue.IsNotEmptyOrNull())
                {
                    string headerText = dgvDataMigration.Columns[columnName].HeaderText;
                    // ������Ϣ����ʾ��ϸ����
                    ShowCustomMessageBox($"{headerText}��ϸ����", cellValue);
                }
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public static void ShowCustomMessageBox(string title, string message)
    {
        ScrollableMessageBox mb = new ScrollableMessageBox(title, message);
        mb.Show();
    }

    /// <summary>
    /// ��ҳ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pagingSource"></param>
    /// <param name="pageIndex"></param>
    /// <param name="count"></param>
    private void uiPage_PageChanged(object sender, object pagingSource, int pageIndex, int count)
    {
        this.dgvDataMigration.DataSource = pagingSource;
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void dgvDataMigration_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
            DataGridViewColumn clickedColumn = dgvDataMigration.Columns[e.ColumnIndex];
            string columnName = clickedColumn.Name;
            if (columnName == "btnRetry")
            {
                string tableName = dgvDataMigration.Rows[e.RowIndex].Cells["tableName"].Value.ObjToString();
                if (tableName.IsNotEmptyOrNull())
                {
                    await Migration(tableName);
                }
            }
        }
    }
}