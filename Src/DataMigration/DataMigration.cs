global using DataMigration.Dtos;
global using SqlSugar;
global using Sunny.UI;
global using System.Configuration;
global using System.Data;
namespace DataMigration;

public partial class DataMigration : Form
{
    private static List<DataMigrationDto> gridList;
    private static DataMigrationDto gridModel;
    private static Type tableType;
    private static bool isIdentity = false;
    private static SqlSugar.DbType toDbType;
    private static List<SelectModel> selector;
    private static string selectTableName = "";
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
                    if (selectTableName.IsNotEmptyOrNull() && sql.ToLower().Contains($"from {selectTableName}"))
                    {
                        // 打开日志文件，将日志写入文件末尾
                        using StreamWriter writer = File.AppendText(sourceDblogFilePath);
                        writer.WriteLine("sql：" + sql);
                        writer.WriteLine("pars：" + pars);
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
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (sql.ToLower().Contains("insert") || sql.ToLower().Contains("update") || sql.ToLower().Contains("bulkcopy") || sql.ToLower().Contains("create") || sql.ToLower().Contains("alter"))
                    {
                        gridModel.Sql += (sql + ";\r\n").ObjToString();
                        // 打开日志文件，将日志写入文件末尾
                        using StreamWriter writer = File.AppendText(toDbLogFilePath);
                        writer.WriteLine("sql：" + sql);
                        writer.WriteLine("pars：" + pars);
                    }
                };
            });

            gridList = [];

            var tableList = sourceDb.DbMaintenance.GetTableInfoList(false);//查询源数据库所有表
            if (tables != null && tables.Length > 0)
                tableList = tableList.Where(w => tables.Contains(w.Name.ToLower())).ToList();
            if (tableName.IsNotEmptyOrNull())
                tableList = tableList.Where(w => w.Name.ToLower() == tableName.ToLower()).ToList();
            tableList = tableList.OrderBy(o => o.Name).ToList();
            var ignoreTables = new string[] { "app_pushday", "shouji_temp" };
            tableList = tableList?.Where(w => !ignoreTables.Contains(w.Name.ToLower()) && !w.Name.ToLower().Contains("requestaccesslog")).ToList();
            foreach (var table in tableList)
            {
                gridModel = new DataMigrationDto()
                {
                    TableName = table.Name,
                    TableDescription = table.Description.ObjToString(),
                    IsStructure = "否",
                    IsData = "否",
                    ErrMessage = "",
                    CreateTime = DateTime.Now
                };

                if (isStructure || isAll)
                    gridModel.IsStructure = "是";
                if (isDataChecked || isAll)
                    gridModel.IsData = "是";

                try
                {
                    var sourceColumns = sourceDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//查询源数据库当前表所有字段                    

                    if (isStructure || isAll)
                    {
                        await Task.Run(() => StructuralMigration(sourceDb, toDb, table, gridModel, sourceColumns));
                    }

                    if (isDataChecked || isAll)
                    {
                        await Task.Run(() => MigrationData(sourceDb, toDb, table, gridModel, sourceColumns));
                    }
                }
                catch (Exception ex)
                {
                    gridModel.ErrMessage = ex.Message;
                    // 打开日志文件，将日志写入文件末尾
                    using (StreamWriter writer = File.AppendText(errorLogFilePath))
                    {
                        writer.WriteLine("tableName：" + table.Name);
                        writer.WriteLine("错误信息：" + ex.Message);
                    }
                    msg += "tableName：" + table.Name + "，" + "错误信息：" + ex.Message + "\r\n";
                }
                gridModel.StructureStatus = gridModel.StructureStatus.IsNullOrEmpty() ? "-" : gridModel.StructureStatus;
                gridModel.DataStatus = gridModel.DataStatus.IsNullOrEmpty() ? "-" : gridModel.DataStatus;
                gridList.Add(gridModel);

                ShowDatas(this.uiPage.ActivePage == 0 ? 1 : this.uiPage.ActivePage);
            }
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

    private static void MigrationData(SqlSugarClient sourceDb, SqlSugarClient toDb, DbTableInfo? table, DataMigrationDto gridModel, List<DbColumnInfo> sourceColumns)
    {
        var toTableName = table.Name;
        if (toDb.DbMaintenance.IsAnyTable(toTableName, false))//判断目标数据库当前表是否存在
        {
            try
            {
                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(toTableName, false);//查询目标数据库当前表所有字段
                var commonColumnList = sourceColumns.Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToArray();
                selectTableName = table.Name.ToLower();
                var dataCount = sourceDb.Queryable<DataTable>().AS(table.Name).Count();
                gridModel.DataCount = dataCount;
                if (dataCount > 0)
                {
                    var pageSize = 100000;
                    var pageCount = Math.Ceiling(dataCount.ObjToDecimal() / pageSize);

                    if (isDataChecked)
                    {
                        selector = [];
                        isIdentity = false;
                        var comlist = toDb.DbMaintenance.GetIsIdentities(toTableName);
                        if (comlist != null && comlist.Count > 0)
                        {
                            isIdentity = true;
                        }
                    }

                    for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                    {
                        var data = new DataTable();
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
                        data = sourceDb.CopyNew().Queryable<DataTable>().AS(table.Name).Select(selector).ToDataTablePage(pageIndex, pageSize);
                        if (data != null && data.Rows.Count > 0)
                        {
                            try
                            {
                                if (isIdentity)
                                    toDb.CopyNew().Fastest<DataTable>().AS(toTableName).OffIdentity().BulkCopy(data);//将数据分页批量插入到目标表中
                                else
                                    toDb.CopyNew().Fastest<DataTable>().AS(toTableName).BulkCopy(data);//将数据分页批量插入到目标表中
                            }
                            catch (Exception)
                            {
                                List<Dictionary<string, object>> dc = toDb.CopyNew().Utilities.DataTableToDictionaryList(data);
                                toDb.CopyNew().Insertable(dc).AS(toTableName).InsertColumns(commonColumnList).ExecuteCommand();
                            }
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
            isIdentity = false;
            selector = [];

            var sourceDtColumns = (sourceDb.Queryable<DataTable>().AS(table.Name).Select("*").Where(w => false).ToDataTable()).Columns;

            var tableName = table.Name;

            var typeBilder = toDb.DynamicBuilder().CreateClass(tableName, new SugarTable() { TableDescription = table.Description });

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
                    if (toDbType == SqlSugar.DbType.Dm && item.Length != 36 && propertyType == typeof(string))
                        item.Length = item.Length * 2;

                    column.Length = item.Length;
                }

                if (item.Length >= 4000 || item.Length == -1)
                {
                    column.ColumnDataType = StaticConfig.CodeFirst_BigString;
                }
                if (item.IsIdentity)
                    isIdentity = item.IsIdentity;

                selector.Add(new SelectModel()
                {
                    FieldName = item.DbColumnName
                });

                typeBilder.CreateProperty(item.DbColumnName, propertyType, column);
            }
            //创建类
            tableType = typeBilder.BuilderType();

            var toDbNew = toDb.CopyNew();
            if (toDbNew.DbMaintenance.IsAnyTable(tableName, false))
                toDbNew.DbMaintenance.DropTable(tableName);

            //创建表
            toDbNew.CodeFirst.InitTables(tableType);

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