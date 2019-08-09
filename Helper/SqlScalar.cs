using System;
using System.Data.SqlClient;
using Utilities;

namespace Helper
{
    public static class SqlScalar
    {
        private static readonly object TheLock = new object();
        public static string ExecuteScalar(SimpleLogger log, string sqlConnectionString, string commandString)
        {
            lock (TheLock)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConnectionString))
                    {
                        using (var command = new SqlCommand(commandString, connection))
                        {
                            connection.Open();
                            return command.ExecuteScalar().ToString();
                        }
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