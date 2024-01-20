namespace DataMigration;

public class DbFirstProvider
{
    public virtual string GetPropertyTypeName(DbColumnInfo item, ISqlSugarClient db)
    {
        string text = ((item.PropertyType != null) ? item.PropertyType.Name : db.Ado.DbBind.GetPropertyTypeName(item.DataType));
        if (text != "string" && text != "byte[]" && text != "object" && item.IsNullable)
        {
            text += "?";
        }
        if (text == "Int32")
        {
            text = (item.IsNullable ? "int?" : "int");
        }
        if (text == "String")
        {
            text = "string";
        }
        if (text == "string" && item.IsNullable)
        {
            text += "?";
        }
        if (item.OracleDataType.EqualCase("raw") && item.Length == 16)
        {
            return "Guid";
        }
        if (item.OracleDataType.EqualCase("number") && item.Length == 1 && item.Scale == 0)
        {
            return "bool";
        }
        if (text.EqualCase("char") || text.EqualCase("char?"))
        {
            return "string";
        }
        if (item.DataType == "tinyint unsigned")
        {
            return "short";
        }
        if (item.DataType == "smallint unsigned")
        {
            return "ushort";
        }
        if (item.DataType == "bigint unsigned")
        {
            return "ulong";
        }
        if (item.DataType == "int unsigned")
        {
            return "uint";
        }
        if (item.DataType == "MediumInt")
        {
            return "int";
        }
        if (item.DataType == "MediumInt unsigned")
        {
            return "uint";
        }
        return text;
    }

    public static Type GetPropertyType(string propertyTypeName)
    {
        Type propertyType;
        switch (propertyTypeName)
        {            
            case "string":
                propertyType = typeof(string);
                break;
            case "DateTime":
                propertyType = typeof(DateTime);
                break;
            case "Guid":
                propertyType = typeof(Guid);
                break;
            case "bool":
                propertyType = typeof(bool);
                break;
            case "byte":
                propertyType = typeof(byte);
                break;
            case "short":
                propertyType = typeof(short);
                break;
            case "ushort":
                propertyType = typeof(ushort);
                break;
            case "ulong":
                propertyType = typeof(ulong);
                break;
            case "int":
                propertyType = typeof(int);
                break;
            case "uint":
                propertyType = typeof(uint);
                break;
            default:
                propertyType = typeof(string);
                break;
        }
        return propertyType;
    }
}

public class DbFirstHelper : DbFirstProvider
{
    public override string GetPropertyTypeName(DbColumnInfo item, ISqlSugarClient db)
    {
        if (item.DataType == "tinyint" && item.Length == 1 && !db.CurrentConnectionConfig.ConnectionString.ToLower().Contains("treattinyasboolea"))
        {
            item.DataType = "bit";
            item.DefaultValue = "true";
            return "bool";
        }
        if (item.DataType == "mediumint")
        {
            item.DataType = "int";
            return "int";
        }
        if (item.DataType == "mediumint unsigned")
        {
            item.DataType = "mediumint unsigned";
            return "uint";
        }
        if (item.DataType == "double unsigned")
        {
            return "double";
        }
        return base.GetPropertyTypeName(item, db);
    }
}