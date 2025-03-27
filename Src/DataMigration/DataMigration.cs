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
    private const string textTableNames = "需要同步的表名，多个用英文逗号隔开，为空默认同步所有表";
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
        var sourceConnStr = this.txtSourceConn.Text.Trim();//获取源数据库连接字符串
        var toConnStr = this.txtToConn.Text.Trim();//获取目标数据库连接字符串
        var tableNames = this.txtTableNames.Text.Trim().Replace(textTableNames, "");//获取需要同步的表名
        var tables = tableNames.IsNotEmptyOrNull() ? tableNames.ToLower().Split(',') : null;
        if (sourceConnStr.IsNullOrEmpty() || toConnStr.IsNullOrEmpty())
        {
            MessageBox.Show("请输入源数据库链接和目标数据库链接后进行迁移", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!(this.cmbSourceDbType.SelectedIndex >= 0) || !(this.cmbToDbType.SelectedIndex >= 0))
        {
            MessageBox.Show("请选择源数据库类型和目标数据库类型后进行迁移", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        //将窗体所有控件设为不可用
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

        //保存源数据库链接和目标数据库链接到config
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
                MessageBox.Show("操作失败，数据库类型异常", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            // 打开日志文件，将日志写入文件末尾
                            using (StreamWriter writer = File.AppendText(sourceDblogFilePath))
                            {
                                writer.WriteLine("sql：" + sql);
                                writer.WriteLine("pars：" + pars);
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
                MessageBox.Show("操作失败，数据库类型异常", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            // 打开日志文件，将日志写入文件末尾
                            using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                            {
                                writer.WriteLine("sql：" + sql);
                                writer.WriteLine("pars：" + pars);
                            }
                        }
                    }
                };
            });

            gridList = new List<DataMigrationDto>();

            var tableList = sourceDb.DbMaintenance.GetTableInfoList(false);//查询源数据库所有表
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
                    IsStructure = "否",
                    IsData = "否",
                    StructureStatus = "",
                    DataStatus = "",
                    //Sql = "",
                    ErrMessage = "",
                    CreateTime = DateTime.Now
                };

                if (isStructure || isAll)
                    gridModel.IsStructure = "是";
                if (isDataChecked || isAll)
                    gridModel.IsData = "是";
                try
                {
                    var sourceDbCopy = sourceDb.CopyNew();
                    var toDbCopy = toDb.CopyNew();

                    var sourceColumns = sourceDbCopy.DbMaintenance.GetColumnInfosByTableName(tableName, false);//查询源数据库当前表所有字段

                    if (isStructure || isAll)
                    {
                        StructuralMigration(sourceDbCopy, toDbCopy, table, gridModel, sourceColumns);
                    }

                    if (isDataChecked || isAll)
                    {
                        if (!toDbCopy.DbMaintenance.IsAnyTable(tableName, false))
                        {
                            lock (_lockObject)
                            {
                                using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                                {
                                    writer.WriteLine($"{tableName}：不存在");
                                }
                            }
                            return;
                        }
                        await MigrationData(sourceDbCopy, toDbCopy, table, gridModel, sourceColumns);
                    }
                }
                catch (Exception ex)
                {
                    gridModel.ErrMessage = ex.Message;
                    lock (_lockObject)
                    {
                        // 打开日志文件，将日志写入文件末尾
                        using (StreamWriter writer = File.AppendText(errorLogFilePath))
                        {
                            writer.WriteLine("时间：" + DateTime.Now);
                            writer.WriteLine("tableName：" + tableName);
                            writer.WriteLine("错误信息：" + ex.Message);
                        }
                    }
                    lock (_lockObject)
                    {
                        msg += $"tableName：{tableName}，错误信息：{ex.Message}\r\n";
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
                MessageBox.Show("迁移失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("迁移成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show("迁移异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        var toTableName = sourceTableName;
        if (toDb.DbMaintenance.IsAnyTable(toTableName, false))//判断目标数据库当前表是否存在
        {
            try
            {
                var order = "";
                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(toTableName, false);//查询目标数据库当前表所有字段
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
                gridModel.DataStatus = "成功";
            }
            catch (Exception ex)
            {
                gridModel.DataStatus = "失败";
                throw ex;
            }
        }
    }

    private static void StructuralMigration(SqlSugarClient sourceDb, SqlSugarClient toDb, DbTableInfo? table, DataMigrationDto gridModel, List<DbColumnInfo> sourceColumns)
    {
        try
        {
            var sourceTableName = table.Name.ToUpper();
            var toTableName = sourceTableName;

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
            //创建类
            tableType = typeBilder.BuilderType();

            if (toDb.DbMaintenance.IsAnyTable(toTableName, false))
                toDb.DbMaintenance.DropTable(toTableName);

            //创建表
            toDb.CodeFirst.InitTables(tableType);

            //创建索引
            var indexService = new IndexService(sourceDb);
            var indexInfoList = indexService.GetIndexes(sourceTableName.ToLower());
            if (indexInfoList != null && indexInfoList.Count > 0)
            {
                foreach (var item in indexInfoList)
                {
                    toDb.DbMaintenance.CreateIndex(toTableName, item.Columns, item.IndexName, item.IsUnique);
                }
            }

            gridModel.StructureStatus = "成功";
        }
        catch (Exception ex)
        {
            gridModel.StructureStatus = "失败";
            throw ex;
        }
    }

    /// <summary>
    /// 数据展示
    /// </summary>
    /// <param name="currentPage">当前页</param>
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
    /// 展示异常信息和sql详细信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dgvDataMigration_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        // 确保双击的不是表头  
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
            // 获取双击的列名  
            DataGridViewColumn clickedColumn = dgvDataMigration.Columns[e.ColumnIndex];
            string columnName = clickedColumn.Name;
            // 检查是否双击了我们感兴趣的列  
            if (columnName == "errMessage" || columnName == "sql")
            {
                // 获取双击的单元格的值  
                string cellValue = dgvDataMigration.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ObjToString();
                if (cellValue.IsNotEmptyOrNull())
                {
                    string headerText = dgvDataMigration.Columns[columnName].HeaderText;
                    // 弹出消息框显示详细内容
                    ShowCustomMessageBox($"{headerText}详细内容", cellValue);
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
    /// 翻页
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
    /// 重试
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