
using System;
using System.Data;
using System.Data.Common;

namespace Sqlite.Converters
{
    public class SqlStringEnumConverter : ISqlDataConverter
    {
        public bool CanConvert(SqlDbType sqlDbType, Type objectType)
        {
            if (objectType.IsEnum)
            {
                return true;
            }
            return false;
        }

        public object ReadValue(DbDataReader reader, string columnName, SqlDbType sqlDbType, Type objectType)
        {
            object value = SqlDataConverter.Convert(reader, columnName, sqlDbType);

            if (value.GetType() == typeof(string))
            {
                return Enum.Parse(objectType, Convert.ToString(value), true);
            }

                // Assuming it is an 'int' type (so no parsing required)
            return value;
        }

        public object ReadValue(DbDataReader reader, int ordinal, SqlDbType sqlDbType, Type objectType)
        {
            object value = SqlDataConverter.Convert(reader, ordinal, sqlDbType);

            if (value.GetType() == typeof(string))
            {
                return Enum.Parse(objectType, Convert.ToString(value), true);
            }

            // Assuming it is an 'int' type (so no parsing required)
            return value;
        }
    }
}
