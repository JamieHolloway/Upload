using System.Data;
using System.Linq;

namespace Helper
{
    public static class CreateTable
    {
        private static readonly object TheLock = new object();
        public static string GetDDL(string tableName, DataTable dataTable)
        {
            lock (TheLock)
            {
                var sqlsc = "create table " + tableName + "(";
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    sqlsc += "\n [" + dataTable.Columns[i].ColumnName + "] ";
                    var columnType = dataTable.Columns[i].DataType.ToString();
                    var thisColumnLenth = (from DataRow row in dataTable.Rows select row[i].ToString().Length).Concat(new[] {100}).Max();
                    var columnLength = thisColumnLenth > 3000 ? "max" : thisColumnLenth.ToString();
                    switch (columnType)
                    {
                        case "System.Int32":
                            sqlsc += " int ";
                            break;
                        case "System.Int64":
                            sqlsc += " bigint ";
                            break;
                        case "System.Int16":
                            sqlsc += " smallint";
                            break;
                        case "System.Byte":
                            sqlsc += " tinyint";
                            break;
                        case "System.Decimal":
                            sqlsc += " decimal ";
                            break;
                        case "System.DateTime":
                            sqlsc += " datetime ";
                            break;
                        case "System.Guid":
                            sqlsc += " uniqueidentifier ";
                            break;
                        case "System.DateTimeOffset":
                            sqlsc += " datetimeoffset ";
                            break;
                        case "System.String":
                        default:
                            sqlsc += $" nvarchar({columnLength})";
                            break;
                    }
                    if (dataTable.Columns[i].AutoIncrement)
                        sqlsc += " IDENTITY(" + dataTable.Columns[i].AutoIncrementSeed + "," + dataTable.Columns[i].AutoIncrementStep + ") ";
                    if (!dataTable.Columns[i].AllowDBNull) sqlsc += " not null ";
                    sqlsc += ",";
                }
                return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
            }
        }
    }
}