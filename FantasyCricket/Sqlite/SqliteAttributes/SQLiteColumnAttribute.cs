
using System;

namespace Sqlite.SqliteAttributes

{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SQLiteColumnAttribute : Attribute
    {      
        public string ColumnName { get; }

        public Type ConverterType { get; set; } = null;

        public SQLiteColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
