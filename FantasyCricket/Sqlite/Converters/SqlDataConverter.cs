
using System;
using System.Data;
using System.Data.Common;

namespace Sqlite.Converters
{
    public class SqlDataConverter : ISqlDataConverter
    {
        public bool CanConvert(SqlDbType sqlDbType, Type objectType)
        {
            return true;
        }

        public object ReadValue(DbDataReader reader, string columnName, SqlDbType sqlDbType, Type objectType)
        {
            return Convert(reader, columnName, sqlDbType);
        }

        public object ReadValue(DbDataReader reader, int ordinal, SqlDbType sqlDbType, Type objectType)
        {
            return Convert(reader, ordinal, sqlDbType);
        }

        public static object Convert(DbDataReader reader, string columnName, SqlDbType sqlDbType)
        {
            return Convert(reader, reader.GetOrdinal(columnName), sqlDbType);
        }

        public static object Convert(DbDataReader reader, int ordinal, SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.NVarChar:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.Char:
                    return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

                case SqlDbType.Bit:
                    return reader.IsDBNull(ordinal) ? (bool?)null : reader.GetBoolean(ordinal);

                case SqlDbType.Int:
                    return reader.IsDBNull(ordinal) ? (Int32?)null : reader.GetInt32(ordinal);

                case SqlDbType.SmallInt:
                    return reader.IsDBNull(ordinal) ? (Int16?)null : reader.GetInt16(ordinal);

                case SqlDbType.BigInt:
                    return reader.IsDBNull(ordinal) ? (Int64?)null : reader.GetInt64(ordinal);

                case SqlDbType.Float:
                    return reader.IsDBNull(ordinal) ? (float?)null : reader.GetFloat(ordinal);

                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);

                case SqlDbType.UniqueIdentifier:
                    return reader.IsDBNull(ordinal) ? (Guid?)null : reader.GetGuid(ordinal);

                case SqlDbType.Binary:
                    byte[] byteArray = null;

                    if (!reader.IsDBNull(ordinal))
                    {
                        int byteArraySize = System.Convert.ToInt32(reader.GetBytes(ordinal, 0, null, 0, 0));
                        byteArray = new byte[byteArraySize];
                        reader.GetBytes(ordinal, 0, byteArray, 0, 1);
                    }
                    return byteArray;

                default:
                    throw new NotSupportedException(
                        "Unsupported SqlDbType: " + sqlDbType.ToString());
            }
        }

        public static SqlDbType ConvertDbType(string dataTypeName)
        {
            // SqlLite conversions 
            switch (dataTypeName)
            {
                case "NVARCHAR(255)":
                    return SqlDbType.NVarChar;
                case "VARCHAR(255)":
                    return SqlDbType.VarChar;
                case "INTEGER":
                    return SqlDbType.Int;
                case "BOOLEAN":
                    return SqlDbType.Bit;
                case "DATETIME":
                    return SqlDbType.DateTime;
                case "GUID":
                    return SqlDbType.UniqueIdentifier;
            }

            return (SqlDbType)Enum.Parse(typeof(SqlDbType), dataTypeName, true);
        }

        public static SqlDbType ConvertFieldType(Type type)
        {
            // SqlLite conversions 
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;
                default:
                    throw new Exception("Unable to figure field type");


            }

        }



    }
}
