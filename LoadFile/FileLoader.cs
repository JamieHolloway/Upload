using Helper;
using System;
using System.Data;
using System.Linq;
using Utilities;

namespace FileLoader
{
    public class FileLoader
    {
        private readonly DataTable dt;
        private readonly string ldt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        private string SqlConnectionString { get; set; }
        private string FileName { get; set; }
        private readonly string tableName;

        public FileLoader(SimpleLogger log, string sqlConnectionString, string fileName)
        {
            SqlConnectionString = sqlConnectionString;
            FileName = fileName;

            dt = new DataTable();

            using (var reader = new CsvReader(fileName))
            {
                var newRow = dt.NewRow();
                var firstRow = true;

                var defaultColCount = 0;

                tableName = fileName.Split('\\').Last().Replace(".csv", "");

                foreach (string[] values in reader.RowEnumerator)
                {
                    if (firstRow)
                    {
                        AddColumns(ref dt, values);
                        firstRow = false;
                        continue;
                    }

                    for (var i = 0; i < values.Length; i++)
                    {
                        newRow[defaultColCount + i] = values[i];
                    }
                    dt.Rows.Add(newRow);
                    newRow = dt.NewRow();
                }
            }

            const string namePrefix = "temp_from_JamieHo_FileLoader_";
            const string nameSuffix = "_JamieHo_Auto_Upload";
            Sql.ExecuteSqlCommand(log, sqlConnectionString, $@"begin try drop table {namePrefix + tableName} end try begin catch end catch;");
            Sql.ExecuteSqlCommand(log, sqlConnectionString, CreateTable.GetDDL(namePrefix + tableName, dt));
            var bulkCopy = new BulkCopy(sqlConnectionString, namePrefix + tableName, dt);
            Sql.ExecuteSqlCommand(log, sqlConnectionString, $@"begin try drop table {tableName + nameSuffix} end try begin catch end catch;");
            Sql.ExecuteSqlCommand(log,sqlConnectionString, $@"exec sp_rename '{namePrefix + tableName}','{tableName + nameSuffix}'");
            Sql.ExecuteSqlCommand(log,sqlConnectionString, $@"begin try drop table {namePrefix + tableName} end try begin catch end catch;");
        }

        private void AddColumns(ref DataTable dtr, string[] columns)
        {
            foreach (var col in columns)
            {
                dtr.Columns.Add(col, typeof(string));
            }
        }
    }
}
