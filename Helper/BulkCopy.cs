using System;
using System.Data;
using System.Data.SqlClient;
using Utilities;

namespace Helper
{
    public class BulkCopy
    {
        public BulkCopy(string sqlConnectionString, string tableName, DataTable dt, int bulkCopyTimeout = 1200, int notifyAfter = 0, int batchSize = 100000)
        {
            LoadTable(sqlConnectionString, tableName, dt, bulkCopyTimeout, notifyAfter, batchSize);
        }

        public void LoadTable(string sqlConnectionString, string tableName, DataTable dt, int bulkCopyTimeout = 1200, int notifyAfter = 0, int batchSize = 100000)
        {
            var log = SimpleLogger.GetInstance;
            log.WriteLine($"Start SqlBulkCopy for {tableName}, total rows to load: {dt.Rows.Count}.");

            if (notifyAfter == 0) notifyAfter = dt.Rows.Count / 10 > 500 ? dt.Rows.Count / 10 : 500;

            using (var bulkCopy = new SqlBulkCopy(sqlConnectionString))
            {
                try
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                    bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsTransfer);
                    bulkCopy.NotifyAfter = notifyAfter;
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"bulkCopy failed: {e.Message}");
                }
                log.WriteLine($"SqlBulkCopy complete for {tableName}, total row count {dt.Rows.Count}.");
            }

            void OnSqlRowsTransfer(object sender, SqlRowsCopiedEventArgs e)
            {
                log.WriteLine($"Copied {Math.Floor(Convert.ToSingle(e.RowsCopied) / Convert.ToSingle(dt.Rows.Count) * 100)}% of rows, row count so far {e.RowsCopied}, for {tableName}");
            }
        }
    }
}