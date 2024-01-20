global using Newtonsoft.Json.Linq;
using System.Reflection;

namespace DataMigration;

public static class UtilConvert
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static ulong ObjToULong(this object thisValue)
    {
        ulong reval = 0;
        if (thisValue == null) return 0;
        try
        {
            if (thisValue != DBNull.Value && ulong.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            else
            {
                reval = Convert.ToUInt64(thisValue);
            }
        }
        catch (Exception)
        {
            reval = 0;
        }
        return reval;
    }

    public static long ObjToLong(this object thisValue)
    {
        long reval = 0;
        if (thisValue == null) return 0;
        if (thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }

        return reval;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static int ObjToInt(this object thisValue)
    {
        int reval = 0;
        if (thisValue == null) return 0;
        if (thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }
        return reval;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static int ObjToInt(this object thisValue, int errorValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
        {
            return reval;
        }
        return errorValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static double ObjToMoney(this object thisValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
        {
            return reval;
        }
        return 0;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static double ObjToMoney(this object thisValue, double errorValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
        {
            return reval;
        }
        return errorValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string ObjToString(this object thisValue)
    {
        if (thisValue != null) return thisValue.ToString().Trim();
        return "";
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static bool IsNotEmptyOrNull(this object thisValue)
    {
        return ObjToString(thisValue) != "" && ObjToString(thisValue) != "undefined" && ObjToString(thisValue) != "null";
    }

    public static bool IsNullOrEmpty(this object thisValue) => thisValue == null || thisValue == DBNull.Value || string.IsNullOrWhiteSpace(ObjToString(thisValue)) || ObjToString(thisValue) == "undefined" && ObjToString(thisValue) == "null";

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static string ObjToString(this object thisValue, string errorValue)
    {
        if (thisValue != null) return thisValue.ToString().Trim();
        return errorValue;
    }
    public static Double ObjToDouble(this object thisValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
        {
            return reval;
        }
        return 0;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static Decimal ObjToDecimal(this object thisValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
        {
            return reval;//.ObjToString().Replace(".0000", "").Replace(".00", "").ObjToInt();
        }
        return 0;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static Decimal ObjToDecimal(this object thisValue, decimal errorValue)
    {
        if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
        {
            return reval;
        }
        return errorValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static DateTime ObjToDate(this object thisValue)
    {
        DateTime reval = DateTime.MinValue;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            reval = Convert.ToDateTime(thisValue);
        }
        return reval;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static DateTime ObjToDate(this object thisValue, DateTime errorValue)
    {
        DateTime reval = DateTime.MinValue;
        if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }
        return errorValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static bool ObjToBool(this object thisValue)
    {
        bool reval = false;
        if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
        {
            return reval;
        }
        else
        {
            try
            {
                reval = Convert.ToBoolean(thisValue);
            }
            catch
            {
            }
        }
        return reval;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string JobjectToStr(this object thisValue)
    {
        if (thisValue != null && thisValue is JObject)
            return (JObject.FromObject(thisValue)).Properties().FirstOrDefault()?.Value.ToString();
        else
            return thisValue.ToString();
    }

    public static bool EqualCase(this string thisValue, string equalValue)
    {
        if (thisValue != null && equalValue != null)
        {
            return thisValue.ToLower() == equalValue.ToLower();
        }
        return thisValue == equalValue;
    }

    public static bool IsContainsIn(this string thisValue, params string[] inValues)
    {
        return inValues.Any((string it) => thisValue.Contains(it));
    }

    public static List<T> ModelToList<T>(this T model)
    {
        var list = new List<T>
            {
                model
            };
        return list;
    }

    /// <summary>
    /// 获取当前时间的时间戳
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static string DateToTimeStamp(this DateTime thisValue)
    {
        TimeSpan ts = thisValue - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static object ChangeType(this object value, Type type)
    {
        if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
        if (value == null) return null;
        if (type == value.GetType()) return value;
        if (type.IsEnum)
        {
            if (value is string)
                return Enum.Parse(type, value as string);
            else
                return Enum.ToObject(type, value);
        }

        if (!type.IsInterface && type.IsGenericType)
        {
            Type innerType = type.GetGenericArguments()[0];
            object innerValue = ChangeType(value, innerType);
            return Activator.CreateInstance(type, new object[] { innerValue });
        }

        if (value is string && type == typeof(Guid)) return new Guid(value as string);
        if (value is string && type == typeof(Version)) return new Version(value as string);
        if (!(value is IConvertible)) return value;
        return Convert.ChangeType(value, type);
    }

    public static object ChangeTypeList(this object value, Type type)
    {
        if (value == null) return default;

        var gt = typeof(List<>).MakeGenericType(type);
        dynamic lis = Activator.CreateInstance(gt);

        var addMethod = gt.GetMethod("Add");
        string values = value.ToString();
        if (values != null && values.StartsWith("(") && values.EndsWith(")"))
        {
            string[] splits;
            if (values.Contains("\",\""))
            {
                splits = values.Remove(values.Length - 2, 2)
                    .Remove(0, 2)
                    .Split("\",\"");
            }
            else
            {
                splits = values.Remove(0, 1)
                    .Remove(values.Length - 2, 1)
                    .Split(",");
            }

            foreach (var split in splits)
            {
                var str = split;
                if (split.StartsWith("\"") && split.EndsWith("\""))
                {
                    str = split.Remove(0, 1)
                        .Remove(split.Length - 2, 1);
                }

                addMethod.Invoke(lis, new object[] { ChangeType(str, type) });
            }
        }
        return lis;
    }

    public static bool IsGuidByParse(this string strSrc)
    {
        return Guid.TryParse(strSrc, out Guid g);
    }

    /// <summary>
    /// 获取对象字段的值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldID"></param>
    /// <returns></returns>
    public static string ObjGetFieldValue(this object obj, string fieldID)
    {
        Type type = obj.GetType();
        PropertyInfo[] propertyInfos = type.GetProperties();
        foreach (PropertyInfo item in propertyInfos)
        {
            if (item.Name.ToUpper() == fieldID.ToUpper())
                return (item.GetValue(obj, null) == null ? "" : item.GetValue(obj, null)).ToString();
        }
        return "";
    }

    /// <summary>
    /// 设置对象字段的值
    /// </summary>
    /// <param name="obj">Class对象</param>
    /// <param name="fieldID">字段</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static bool ObjSetFieldValue(this object obj, string fieldID, string value)
    {
        Type type = obj.GetType();
        PropertyInfo[] propertyInfos = type.GetProperties();
        foreach (PropertyInfo item in propertyInfos)
        {
            if (item.Name.ToUpper() == fieldID.ToUpper())
            {
                item.SetValue(obj, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, item.PropertyType), null);
                return true;
            }
        }
        return false;
    }
}