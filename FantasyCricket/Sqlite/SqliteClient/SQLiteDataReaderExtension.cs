
using Sqlite.Converters;
using Sqlite.SqliteAttributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Sqlite.SqlClient
{
    public static class SQLiteDataReaderExtension
    {
        public static T[] ReadAll<T>(this DbDataReader reader)
        {
            List<T> list = new List<T>();

            T genericObject;

            while ((genericObject = reader.Read<T>()) != null)
            {
                list.Add(genericObject);
            }

            return list.ToArray();
        }

        public static T Read<T>(this DbDataReader reader)
        {
            bool rowsAvailable = reader.Read();

            if (rowsAvailable)
            {
                T genericObject = Activator.CreateInstance<T>();

                foreach (PropertyInfo propertyInfo in genericObject.GetType().GetProperties())
                {
                    string propertyName = propertyInfo.Name;

                    object[] attribute = propertyInfo.GetCustomAttributes(typeof(SQLiteColumnAttribute), true);

                    // No 'SqlDataReaderProperty' annotation on this property when (length == 0)
                    if (attribute.Length > 0)
                    {
                        SQLiteColumnAttribute sqlColumnAttribute = (SQLiteColumnAttribute)attribute[0];

                        // Get Column ordinal
                        int ordinal = reader.GetOrdinal(sqlColumnAttribute.ColumnName);

                        if (reader.IsDBNull(ordinal))
                        {
                            propertyInfo.SetValue(
                           genericObject,
                           null);
                        }
                        else
                        {
                            // Create column's converter
                            ISqlDataConverter converter = SqlDataConverterFactory.Create(sqlColumnAttribute, propertyInfo.PropertyType);


                            // Get SqlDbType from column's meta-data
                            SqlDbType sqlDbType;
                            string dataTypeName = reader.GetDataTypeName(ordinal);
                            if (!string.IsNullOrEmpty(dataTypeName))
                            {
                                sqlDbType = SqlDataConverter.ConvertDbType(dataTypeName);
                            }
                            else
                            {
                                sqlDbType = SqlDataConverter.ConvertFieldType(reader.GetFieldType(ordinal));
                            }

                            // Apply the converted object to the property's object
                            propertyInfo.SetValue(
                                genericObject,
                                converter.ReadValue(reader, ordinal, sqlDbType, propertyInfo.PropertyType));

                        }
                    }
                }

                return genericObject;
            }

            // No more rows available
            return default(T);
        }
    }
}
