using System;
using System.Data.SqlClient;
using Utilities;

namespace Helper
{
    public class Sql
    {
        private static readonly object TheLock = new object();
        public static void ExecuteSqlCommand(SimpleLogger log, string sqlConnectionString, string commandString)
        {
            lock (TheLock)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConnectionString))
                    {
                        var command = new SqlCommand(commandString, connection) {CommandTimeout = 20000000};
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"Error encountered when executing SQL command '{commandString}', error was --> {e.GetBaseException().Message}");
                }
            }
        }
    }
}