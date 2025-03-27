global using DataMigration.Dtos;
global using SqlSugar;
global using Sunny.UI;
global using System.Configuration;
global using System.Data;
using System.Collections.Concurrent;
namespace DataMigration;

public partial class DataMigration : Form
{
    private static readonly object _lockObject = new object();
    private static List<DataMigrationDto> gridList;
    private static Type tableType;
    private static SqlSugar.DbType toDbType;
    private const string textTableNames = "��Ҫͬ���ı����������Ӣ�Ķ��Ÿ�����Ϊ��Ĭ��ͬ�����б�";
    private static bool isDataChecked;
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
        this.cmbSourceDbType.DataSource = Enum.GetValues<SqlSugar.DbType>();//typeof(SqlSugar.DbType)
        this.cmbToDbType.DataSource = Enum.GetValues<SqlSugar.DbType>();//typeof(SqlSugar.DbType)
        this.cmbSourceDbType.SelectedIndex = -1;
        this.cmbToDbType.SelectedIndex = -1;
        var sourceDbType = ConfigurationManager.AppSettings["sourceDbType"];
        var toDbType = ConfigurationManager.AppSettings["toDbType"];
        if (sourceDbType.IsNotEmptyOrNull())
        {
            int sourceDbTypeIndex = this.cmbSourceDbType.FindStringExact(sourceDbType);
            if (sourceDbTypeIndex != -1)
                this.cmbSourceDbType.SelectedIndex = sourceDbTypeIndex;
        }
        if (toDbType.IsNotEmptyOrNull())
        {
            int toDbTypeIndex = this.cmbToDbType.FindStringExact(toDbType);
            if (toDbTypeIndex != -1)
                this.cmbToDbType.SelectedIndex = toDbTypeIndex;
        }
        this.rdoAll.Checked = true;
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
        isDataChecked = this.rdoData.Checked;
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
        config.AppSettings.Settings["sourceDbType"].Value = this.cmbSourceDbType.SelectedItem.ObjToString();
        config.AppSettings.Settings["toDbType"].Value = this.cmbToDbType.SelectedItem.ObjToString();
        config.AppSettings.SectionInformation.ForceSave = true;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
        ConfigurationManager.RefreshSection("appSettings");
        try
        {
            this.uiPage.DataSource = null;
            this.dgvDataMigration.DataSource = null;
            ConnectionConfig sourceConfig = new()
            {
                ConfigId = 1,
                ConnectionString = sourceConnStr,
                IsAutoCloseConnection = true
            };
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
            SqlSugarClient sourceDb = new(sourceConfig, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {

                    if (sql.ToLower().Contains("rowindex"))
                    {
                        lock (_lockObject)
                        {
                            // ����־�ļ�������־д���ļ�ĩβ
                            using (StreamWriter writer = File.AppendText(sourceDblogFilePath))
                            {
                                writer.WriteLine("sql��" + sql);
                                writer.WriteLine("pars��" + pars);
                            }
                        }
                    }
                };
            });

            ConnectionConfig toConfig = new()
            {
                ConfigId = 1,
                ConnectionString = toConnStr,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    MaxParameterNameLength = 30
                }
            };

            if (Enum.TryParse(this.cmbToDbType.SelectedItem.ToString(), out SqlSugar.DbType to))
            {
                toConfig.DbType = to;
                toDbType = to;
            }
            else
                MessageBox.Show("����ʧ�ܣ����ݿ������쳣", "Ǩ���쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SqlSugarClient toDb = new(toConfig, db =>
            {
                db.Ado.CommandTimeOut = 1800;
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (sql.ToLower().Contains("insert") || sql.ToLower().Contains("update") || sql.ToLower().Contains("bulkcopy") || sql.ToLower().Contains("create") || sql.ToLower().Contains("alter"))
                    {
                        //gridModel.Sql += (sql + ";\r\n").ObjToString();
                        lock (_lockObject)
                        {
                            // ����־�ļ�������־д���ļ�ĩβ
                            using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                            {
                                writer.WriteLine("sql��" + sql);
                                writer.WriteLine("pars��" + pars);
                            }
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
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 10
            };
            await Parallel.ForEachAsync(tableList, options, async (table, cancellationToken) =>
            {
                var tableName = table.Name.ToUpper();
                var gridModel = new DataMigrationDto()
                {
                    TableName = tableName,
                    TableDescription = table.Description.ObjToString(),
                    IsStructure = "��",
                    IsData = "��",
                    StructureStatus = "",
                    DataStatus = "",
                    //Sql = "",
                    ErrMessage = "",
                    CreateTime = DateTime.Now
                };

                if (isStructure || isAll)
                    gridModel.IsStructure = "��";
                if (isDataChecked || isAll)
                    gridModel.IsData = "��";
                try
                {
                    var sourceDbCopy = sourceDb.CopyNew();
                    var toDbCopy = toDb.CopyNew();

                    var sourceColumns = sourceDbCopy.DbMaintenance.GetColumnInfosByTableName(tableName, false);//��ѯԴ���ݿ⵱ǰ�������ֶ�

                    if (isStructure || isAll)
                    {
                        StructuralMigration(sourceDbCopy, toDbCopy, table, gridModel, sourceColumns);
                    }

                    if (isDataChecked || isAll)
                    {
                        if (tableName == "SYS_CONFIG")
                        {
                            if (!toDbCopy.DbMaintenance.IsAnyTable(tableName + "_WHH", false))
                                return;
                        }
                        else
                        {
                            if (!toDbCopy.DbMaintenance.IsAnyTable(tableName, false))
                            {
                                lock (_lockObject)
                                {
                                    using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                                    {
                                        writer.WriteLine($"{tableName}��������");
                                    }
                                }
                                return;
                            }
                        }
                        await MigrationData(sourceDbCopy, toDbCopy, table, gridModel, sourceColumns);
                    }
                }
                catch (Exception ex)
                {
                    gridModel.ErrMessage = ex.Message;
                    lock (_lockObject)
                    {
                        // ����־�ļ�������־д���ļ�ĩβ
                        using (StreamWriter writer = File.AppendText(errorLogFilePath))
                        {
                            writer.WriteLine("ʱ�䣺" + DateTime.Now);
                            writer.WriteLine("tableName��" + tableName);
                            writer.WriteLine("������Ϣ��" + ex.Message);
                        }
                    }
                    lock (_lockObject)
                    {
                        msg += $"tableName��{tableName}��������Ϣ��{ex.Message}\r\n";
                    }
                }

                gridModel.StructureStatus = gridModel.StructureStatus.IsNullOrEmpty() ? "-" : gridModel.StructureStatus;
                gridModel.DataStatus = gridModel.DataStatus.IsNullOrEmpty() ? "-" : gridModel.DataStatus;
                lock (_lockObject)
                {
                    gridList.Add(gridModel);
                }

                this.Invoke((MethodInvoker)delegate
                {
                    ShowDatas(this.uiPage.ActivePage == 0 ? 1 : this.uiPage.ActivePage);
                });
            });

            if (msg.IsNotEmptyOrNull())
                MessageBox.Show("Ǩ��ʧ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show("Ǩ���쳣��" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    private static async Task MigrationData(SqlSugarClient sourceDb, SqlSugarClient toDb, DbTableInfo? table, DataMigrationDto gridModel, List<DbColumnInfo> sourceColumns)
    {
        var sourceTableName = table.Name.ToUpper();
        var toTableName = sourceTableName == "SYS_CONFIG" ? "SYS_CONFIG_WHH" : sourceTableName;
        if (toDb.DbMaintenance.IsAnyTable(toTableName, false))//�ж�Ŀ�����ݿ⵱ǰ���Ƿ����
        {
            try
            {
                var order = "";
                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(toTableName, false);//��ѯĿ�����ݿ⵱ǰ�������ֶ�
                var commonColumnList = sourceColumns.Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToArray();
                var dataCount = await sourceDb.Queryable<DataTable>().AS(sourceTableName).CountAsync();
                gridModel.DataCount = dataCount;
                if (dataCount > 0)
                {
                    var pageSize = 100000;
                    var pageCount = (int)Math.Ceiling(dataCount.ObjToDecimal() / pageSize);

                    var selector = new List<SelectModel>();
                    var isIdentity = false;
                    if (isDataChecked)
                    {
                        var comlist = toDb.DbMaintenance.GetIsIdentities(toTableName);
                        if (comlist != null && comlist.Count > 0)
                        {
                            isIdentity = true;
                        }
                    }

                    if (selector == null || selector.Count == 0)
                    {
                        foreach (var item in commonColumnList)
                        {
                            selector.Add(new SelectModel()
                            {
                                FieldName = item
                            });
                        }
                    }

                    if (commonColumnList.Contains("create_time"))
                        order = "create_time asc";

                    for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                    {
                        var data = new DataTable();
                        data = await sourceDb.Queryable<DataTable>().AS(sourceTableName).OrderByIF(order.IsNotEmptyOrNull(), order).Select(selector).ToDataTablePageAsync(pageIndex, pageSize);
                        data = DataHelper.DataTableToUpper(data);
                        if (data != null && data.Rows.Count > 0)
                        {
                            var fastInserter = isIdentity ?
                                    toDb.Fastest<DataTable>().AS(toTableName).OffIdentity() :
                                    toDb.Fastest<DataTable>().AS(toTableName);
                            await fastInserter.BulkCopyAsync(data);
                        }
                    }
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
            var sourceTableName = table.Name.ToUpper();
            var toTableName = sourceTableName == "SYS_CONFIG" ? "SYS_CONFIG_WHH" : sourceTableName;

            var sourceDtColumns = (sourceDb.Queryable<DataTable>().AS(sourceTableName).Select("*").Where(w => false).ToDataTable()).Columns;

            var typeBilder = toDb.DynamicBuilder().CreateClass(toTableName, new SugarTable() { TableDescription = table.Description });

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
                    column.DecimalDigits = item.DecimalDigits == 127 ? 0 : item.DecimalDigits;
                    column.DefaultValue = item.DefaultValue.ObjToString().Replace("(", "").Replace(")", "").Replace("'", "");
                }

                if (propertyType == typeof(string) || propertyType == typeof(decimal))
                {
                    if (toDbType == SqlSugar.DbType.Dm && item.Length != 36 && item.Length != -1 && propertyType == typeof(string))
                        item.Length = item.Length * 2;

                    if (item.Length < 4000 && item.Length > 0)
                        column.Length = item.Length;
                    else if (item.Length >= 4000 || item.Length <= -1)
                        column.ColumnDataType = StaticConfig.CodeFirst_BigString;
                }
                typeBilder.CreateProperty(item.DbColumnName, propertyType, column);
            }
            //������
            tableType = typeBilder.BuilderType();

            if (toDb.DbMaintenance.IsAnyTable(toTableName, false))
                toDb.DbMaintenance.DropTable(toTableName);

            //������
            toDb.CodeFirst.InitTables(tableType);

            //��������
            var indexService = new IndexService(sourceDb);
            var indexInfoList = indexService.GetIndexes(sourceTableName.ToLower());
            if (indexInfoList != null && indexInfoList.Count > 0)
            {
                foreach (var item in indexInfoList)
                {
                    toDb.DbMaintenance.CreateIndex(toTableName, item.Columns, item.IndexName, item.IsUnique);
                }
            }

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
        Rectangle rectangle = new(e.RowBounds.Location.X,
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

    public static void ShowCustomMessageBox(string title, string message)
    {
        ScrollableMessageBox mb = new(title, message);
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