
using Sqlite.SqliteAttributes;
using System;

namespace Sqlite.Converters
{
    public class SqlDataConverterFactory
    {
        private static ISqlDataConverter sqlDataConverter = new SqlDataConverter();
        private static ISqlDataConverter sqlStringEnumConverter = new SqlStringEnumConverter();

        public static ISqlDataConverter Create(SQLiteColumnAttribute sqlColumnAttribute, Type propertyType)
        {
            if (sqlColumnAttribute.ConverterType != null)
            {
                return (ISqlDataConverter)Activator.CreateInstance(sqlColumnAttribute.ConverterType);
            }
            else if (propertyType.IsEnum)
            {
                return sqlStringEnumConverter;
            }
            else
            {
                return sqlDataConverter;
            }
        }

        public static ISqlDataConverter Create(Type converterType)
        {
            return (ISqlDataConverter)Activator.CreateInstance(converterType);
        }

    }
}
