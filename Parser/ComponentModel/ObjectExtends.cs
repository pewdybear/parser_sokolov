using System;
using System.Threading;

namespace Parser.ComponentModel
{
    static class ObjectExtends
    {
        public static bool IsNumericType(this object o)
        {
            return IsNumericType(o.GetType());
        }
        public static bool IsFloatingType(this object o)
        {
            return IsFloatingType(o.GetType());
        }
        public static bool IsIntegerType(this object o)
        {
            return IsIntegerType(o.GetType());
        }
        public static bool IsNumericType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsFloatingType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsIntegerType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
        public static T SafeParse<T>(this string str)
        {
            return (T)str.SafeParse(typeof(T));
        }
        public static object SafeParse(this string str, Type type)
        {
            if (Type.GetTypeCode(type) == TypeCode.String) return str;
            var res = Activator.CreateInstance(type);

            //If nullable convert to underlying type and set nullable value by default(T)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);
            if (type.IsValueType)
            {
                str = str.Trim();
                if (Type.GetTypeCode(type) == TypeCode.Boolean)
                {
                    //try parse 1, 0 in string as bool value
                    switch (str.ToLower())
                    {
                        case "1":
                        case "true": return true;
                        case "0":
                        case "":
                        case "false": return false;
                        default: throw new FormatException("String not contain valid boolean value");
                    }
                }
                else if (Type.GetTypeCode(type) == TypeCode.Char)
                {
                    return str[0];
                }
                else if (type.IsNumericType())
                {
                    //if contains floating point it will be replaced to culture point format
                    char a = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    string newStr = str.Replace(',', a);
                    newStr = newStr.Replace('.', a);
                    //if type is integer then delete and ignore fractional part
                    if (type.IsIntegerType())
                    {
                        int index = newStr.IndexOf(a);
                        if (index > 0) newStr = newStr.Substring(0, index);
                    }
                    try { res = Convert.ChangeType(newStr, type); }
                    catch (Exception)
                    {
                        throw new FormatException("String not contain valid numeric value");
                    }
                }
            }
            else
            {
                throw new FormatException("Convertion type must be simple type or nullable value of simple types.");
            }
            return res;
        }
    }
}
