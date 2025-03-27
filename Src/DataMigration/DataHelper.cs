namespace DataMigration
{
    public class DataHelper
    {
        public static List<string> NoToUpper = new List<string>()
        {
            "tb_cid",
            "pay_cid",
            "pay_lkcid",
            "rsdcp_ceoid"
        };

        public static DataTable DataTableToUpper(DataTable dtTemp)
        {
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                DataTable dt = dtTemp.Clone();

                // 遍历原始DataTable的列，找到Guid类型的列，并在克隆的DataTable中添加对应的String类型列  
                foreach (DataColumn column in dtTemp.Columns)
                {
                    if (column.DataType == typeof(Guid))
                    {
                        dt.Columns.Remove(column.ColumnName);
                        dt.Columns.Add(column.ColumnName, typeof(string));
                    }
                }

                foreach (DataRow row in dtTemp.Rows)
                {
                    DataRow rowNew = dt.NewRow();
                    foreach (DataColumn item in row.Table.Columns)
                    {
                        if ((item.ColumnName.ToLower().EndsWith("id") && !NoToUpper.Contains(item.ColumnName.ToLower()) && row[item].ToString().Length == 36) || item.ColumnName.ToLower().Contains("tbname") || item.ColumnName.ToLower() == "tables_name")
                        {
                            var value = row[item].ToString().ToUpper();
                            rowNew[item.ColumnName] = value;
                        }
                        else
                        {
                            rowNew[item.ColumnName] = row[item];
                        }
                    }
                    dt.Rows.Add(rowNew);
                }
                return dt;
            }
            return dtTemp;
        }
    }
}
