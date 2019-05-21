
using System;
using System.Data;
using System.Data.Common;

namespace Sqlite.Converters
{
    public interface ISqlDataConverter
    {
        bool CanConvert(SqlDbType sqlDbType, Type objectType);

        object ReadValue(DbDataReader reader, string columnName, SqlDbType sqlDbType, Type objectType);

        object ReadValue(DbDataReader reader, int ordinal, SqlDbType sqlDbType, Type objectType);
    }
}
