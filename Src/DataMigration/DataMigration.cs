global using SqlSugar;
global using System.Configuration;
global using System.Data;
namespace DataMigration;

public partial class DataMigration : Form
{
    private static string commonColumns = "";
    private const string textTableNames = "��Ҫͬ���ı����������Ӣ�Ķ��Ÿ�����Ϊ��Ĭ��ͬ�����б�";
    public DataMigration()
    {
        InitializeComponent();
        this.txtTableNames.Text = textTableNames;
        this.txtTableNames.ForeColor = Color.Gray;
        this.txtTableNames.GotFocus += txtTableNames_GotFocus;
        this.txtTableNames.LostFocus += txtTableNames_LostFocus;
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
        var msg = "";
        var sourceConnStr = this.txtSourceConn.Text.Trim();//��ȡԴ���ݿ������ַ���
        var toConnStr = this.txtToConn.Text.Trim();//��ȡĿ�����ݿ������ַ���
        var tableName = this.txtTableNames.Text.Trim().Replace(textTableNames, "");//��ȡ��Ҫͬ���ı���
        var tables = tableName.IsNotEmptyOrNull() ? tableName.ToLower().Split(',') : null;
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

        string sourceDblogFilePath = Path.GetDirectoryName("logs/sourceDb.log");
        string toDbLogFilePath = Path.GetDirectoryName("logs/toDb.log");
        string errorLogFilePath = Path.GetDirectoryName("logs/error.log");
        if (File.Exists(sourceDblogFilePath))
            File.Delete(sourceDblogFilePath);
        if (File.Exists(toDbLogFilePath))
            File.Delete(toDbLogFilePath);
        if (File.Exists(errorLogFilePath))
            File.Delete(errorLogFilePath);
        Directory.CreateDirectory(sourceDblogFilePath);
        Directory.CreateDirectory(toDbLogFilePath);
        Directory.CreateDirectory(errorLogFilePath);

        //����Դ���ݿ����Ӻ�Ŀ�����ݿ����ӵ�config
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var connectionStrings = config.ConnectionStrings.ConnectionStrings;
        connectionStrings["sourceConnStr"].ConnectionString = sourceConnStr;
        connectionStrings["toConnStr"].ConnectionString = toConnStr;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
        try
        {
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
                            writer.WriteLine("pars��" + pars);
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
                        // ����־�ļ�������־д���ļ�ĩβ
                        using (StreamWriter writer = File.AppendText(toDbLogFilePath))
                        {
                            writer.WriteLine("sql��" + sql);
                            writer.WriteLine("pars��" + pars);
                        }
                    }
                };
            });

            var tableList = sourceDb.DbMaintenance.GetTableInfoList(false);//��ѯԴ���ݿ����б�
            if (tables != null && tables.Length > 0)
                tableList = tableList.Where(w => tables.Contains(w.Name.ToLower())).ToList();
            foreach (var table in tableList)
            {
                try
                {
                    var sourceColumns = sourceDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//��ѯԴ���ݿ⵱ǰ�������ֶ�

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
                                ColumnDescription = item.ColumnDescription
                            };
                            if (propertyType != typeof(DateTime) && !item.DefaultValue.ObjToString().ToLower().Contains("newid"))
                            {
                                column.DecimalDigits = item.DecimalDigits;
                                column.DefaultValue = item.DefaultValue.ObjToString().Replace("(", "").Replace(")", "");
                            }
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
                        //������
                        var type = typeBilder.BuilderType();

                        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))
                            toDb.DbMaintenance.DropTable(table.Name);

                        //������
                        toDb.CodeFirst.InitTables(type);
                    }

                    if (isData || isAll)
                    {
                        if (toDb.DbMaintenance.IsAnyTable(table.Name, false))//�ж�Ŀ�����ݿ⵱ǰ���Ƿ����
                        {
                            await Task.Run(() =>
                            {
                                var toColumns = toDb.DbMaintenance.GetColumnInfosByTableName(table.Name, false);//��ѯĿ�����ݿ⵱ǰ�������ֶ�
                                commonColumns = string.Join(",", sourceColumns
                                    .Where(c1 => toColumns.Any(c2 => c2.DbColumnName.ToLower() == c1.DbColumnName.ToLower())).Select(s => s.DbColumnName).ToList());//ȡ�����߱��ж����ڵ��ֶβ��Զ���ƴ��                                
                                var dataCount = sourceDb.Queryable<DataTable>().AS(table.Name).Count();
                                var pageSize = 100000;
                                var pageCount = Math.Ceiling(dataCount.ObjToDecimal() / pageSize);
                                for (int pageIndex = 1; pageIndex <= pageCount; pageIndex++)
                                {
                                    var data = sourceDb.CopyNew().Queryable<DataTable>().AS(table.Name).Select(commonColumns).ToDataTablePage(pageIndex, pageSize);//��ҳ��ѯԴ���ݿ⵱ǰ������
                                    toDb.CopyNew().Fastest<DataTable>().AS(table.Name).BulkCopy(data);//�����ݷ�ҳ�������뵽Ŀ�����
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // ����־�ļ�������־д���ļ�ĩβ
                    using (StreamWriter writer = File.AppendText(errorLogFilePath))
                    {
                        writer.WriteLine("tableName��" + table.Name);
                        writer.WriteLine("������Ϣ��" + ex.Message);
                    }
                    msg += "tableName��" + table.Name + "��" + "������Ϣ��" + ex.Message + "\r\n";
                }
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
}
