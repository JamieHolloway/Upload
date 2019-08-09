using System;
using System.Data.SqlClient;
using System.Data;
using Utilities;

namespace Helper
{
    public class SqlToDatatable
    {
        public DataTable MyTable;

        private static readonly object TheLock = new object();

        public SqlToDatatable(SimpleLogger log, string sqlConnectionString, string query)
        {
            lock (TheLock)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConnectionString))
                    {
                        MyTable = new DataTable();
                        var cmd = new SqlCommand(query, connection) {CommandTimeout = 20000000};
                        var da = new SqlDataAdapter(cmd);
                        da.Fill(MyTable);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"Exception raised in {GetType().Name} ----- {e}");
                }
            }
        }
    }
}