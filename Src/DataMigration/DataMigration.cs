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
    private const string textTableNames = "需要同步的表名，多个用英文逗号隔开，为空默认同步所有表";
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

        //保存源数据库链接和目标数据库链接到config
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
                MessageBox.Show("操作失败，数据库类型异常", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlSugarClient sourceDb = new SqlSugarClient(sourceConfig, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (commonColumns.IsNotEmptyOrNull() && sql.ToLower().Contains($"select {commonColumns}"))
                    {
                        // 打开日志文件，将日志写入文件末尾
                        using (StreamWriter writer = File.AppendText(sourceDblogFilePath))
                        {
                            writer.WriteLine("sql：" + sql);
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
                MessageBox.Show("操作失败，数据库类型异常", "迁移异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SqlSugarClient toDb = new SqlSugarClient(toConfig, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (!sql.ToLower().Contains("select"))
                    {
                        gridModel.Sql += sql + ";\r\n";
                        // 打开日志文件，将日志写入文件末尾
                        using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                        {
                            writer.WriteLine("sql：" + sql);
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
                if (isData || isAll)
                    gridModel.IsData = "是";

                try
                {
                    var sourceColumns = sourceDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//查询源数据库当前表所有字段                    

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
                MessageBox.Show("迁移失败：" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show("迁移失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))//判断目标数据库当前表是否存在
        {
            try
            {
                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//查询目标数据库当前表所有字段
                commonColumns = string.Join(",", sourceColumns
                    .Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToList());//取出两边表中都存在的字段并以逗号拼接
                var dataCount = sourceDb.Queryable<DataTable>().AS(table.Name).Count();
                gridModel.DataCount = dataCount;
                var pageSize = 100000;
                var pageCount = Math.Ceiling(dataCount.ObjToDecimal() / pageSize);
                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                {
                    var data = sourceDb.CopyNew().Queryable<DataTable>().AS(table.Name).Select(commonColumns).ToDataTablePage(pageIndex, pageSize);
                    toDb.CopyNew().Fastest<DataTable>().AS(table.Name).BulkCopy(data);//将数据分页批量插入到目标表中
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
            //创建类
            var type = typeBilder.BuilderType();

            var toDbNew = toDb.CopyNew();
            if (toDbNew.DbMaintenance.IsAnyTable(table.Name, false))
                toDbNew.DbMaintenance.DropTable(table.Name);

            //创建表
            toDbNew.CodeFirst.InitTables(type);

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

    /// <summary>
    /// 弹窗
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public static void ShowCustomMessageBox(string title, string message)
    {
        ScrollableMessageBox mb = new ScrollableMessageBox(title, message);
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