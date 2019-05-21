using System;
using System.Data;
using System.Data.Common;

namespace Sqlite.Converters
{
    public class IntToTimeSpanConverter : ISqlDataConverter
    {
        public bool CanConvert(SqlDbType sqlDbType, Type objectType)
        {
            return true;
        }

        public object ReadValue(DbDataReader reader, string columnName, SqlDbType sqlDbType, Type objectType)
        {

            return ReadValue(reader, reader.GetOrdinal(columnName), sqlDbType, objectType);
        }

        public object ReadValue(DbDataReader reader, int ordinal, SqlDbType sqlDbType, Type objectType)
        {
            return TimeSpan.FromSeconds(reader.GetInt64(ordinal));
        }
    }
}
