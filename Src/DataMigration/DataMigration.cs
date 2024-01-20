global using SqlSugar;
global using System.Configuration;
global using System.Data;
namespace DataMigration;

public partial class DataMigration : Form
{
    public DataMigration()
    {
        InitializeComponent();
        this.txtTableNames.Text = "需要同步的表名，多个用英文逗号隔开，为空默认同步所有表";
        this.txtTableNames.ForeColor = Color.Gray;
        this.txtTableNames.GotFocus += txtTableNames_GotFocus;
        this.txtTableNames.LostFocus += txtTableNames_LostFocus;
    }
    private void txtTableNames_GotFocus(object sender, EventArgs e)
    {
        if (txtTableNames.Text == "需要同步的表名，多个用英文逗号隔开，为空默认同步所有表")
        {
            txtTableNames.Text = "";
            txtTableNames.ForeColor = SystemColors.WindowText;
        }
    }

    private void txtTableNames_LostFocus(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTableNames.Text))
        {
            txtTableNames.Text = "需要同步的表名，多个用英文逗号隔开，为空默认同步所有表";
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
        var msg = "";
        var sourceConnStr = this.txtSourceConn.Text.Trim();//获取源数据库连接字符串
        var toConnStr = this.txtToConn.Text.Trim();//获取目标数据库连接字符串
        var tableName = this.txtTableNames.Text.Trim().Replace("需要同步的表名，多个用英文逗号隔开，为空默认同步所有表", "");//获取需要同步的表名
        var tables = tableName.IsNotEmptyOrNull() ? tableName.ToLower().Split(',') : null;
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

        //保存源数据库链接和目标数据库链接到config
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var connectionStrings = config.ConnectionStrings.ConnectionStrings;
        connectionStrings["sourceConnStr"].ConnectionString = sourceConnStr;
        connectionStrings["toConnStr"].ConnectionString = toConnStr;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
        try
        {
            //源数据库SqlSugar配置
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
            SqlSugarClient sourceDb = new SqlSugarClient(sourceConfig);

            //目标数据库SqlSugar配置
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
                        string logFilePath = "logs/toDb.log";

                        // 确保日志目录存在
                        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                        // 打开日志文件，将日志写入文件末尾
                        using (StreamWriter writer = File.AppendText(logFilePath))
                        {
                            writer.WriteLine("sql：" + sql);
                            writer.WriteLine("pars：" + pars);
                        }
                    }
                };
            });

            var tableList = sourceDb.DbMaintenance.GetTableInfoList(false);//查询源数据库所有表
            if (tables != null && tables.Length > 0)
                tableList = tableList.Where(w => tables.Contains(w.Name.ToLower())).ToList();
            foreach (var table in tableList)
            {
                try
                {
                    var sourceColumns = sourceDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//查询源数据库当前表所有字段

                    if (isStructure || isAll)
                    {
                        var typeBilder = toDb.DynamicBuilder().CreateClass(table.Name, new SugarTable() { TableDescription = table.Description });

                        foreach (var item in sourceColumns)
                        {
                            DbFirstProvider dbFirstProvider = new DbFirstProvider();
                            DbFirstHelper dbFirstHelper = new DbFirstHelper();
                            string propertyTypeName = dbFirstHelper.GetPropertyTypeName(item, sourceDb).Replace("?", "");
                            Type propertyType = DbFirstProvider.GetPropertyType(propertyTypeName);
                            var column = new SugarColumn()
                            {
                                IsPrimaryKey = item.IsPrimarykey,
                                IsIdentity = item.IsIdentity,
                                IsNullable = item.IsNullable,
                                DecimalDigits = item.DecimalDigits,
                                ColumnDescription = item.ColumnDescription,
                                DefaultValue = item.DefaultValue
                            };
                            if (propertyType == typeof(string) && item.Length < 4000)
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

                        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))
                            toDb.DbMaintenance.DropTable(table.Name);

                        //创建表
                        toDb.CodeFirst.InitTables(type);
                    }

                    if (isData || isAll)
                    {
                        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))//判断目标数据库当前表是否存在
                        {
                            await Task.Run(() =>
                            {
                                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//查询目标数据库当前表所有字段
                                var commonColumns = string.Join(",", sourceColumns
                                    .Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToList());//取出两边表中都存在的字段并以逗号拼接                            
                                var data = sourceDb.Ado.GetDataTable($@"select {commonColumns} from {table.Name}");//查询源数据库当前表所有数据
                                if (data != null && data.Rows.Count > 0)
                                {
                                    toDb.Fastest<DataTable>().AS(table.Name).PageSize(50000).BulkCopy(data);//将数据分页批量插入到目标表中
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string logFilePath = "logs/error.log";

                    // 确保日志目录存在
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                    // 打开日志文件，将日志写入文件末尾
                    using (StreamWriter writer = File.AppendText(logFilePath))
                    {
                        writer.WriteLine("tableName：" + table.Name);
                        writer.WriteLine("错误信息：" + ex.Message);
                    }
                    msg += "tableName：" + table.Name + "，" + "错误信息：" + ex.Message + "\r\n";
                }
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
}
