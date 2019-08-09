using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilities;

namespace Upload
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = SimpleLogger.GetInstance;
            try
            {
                log.InitOptions();
                var tasks = new List<Task> { Task.Run(() => new UploadFiles(log)) };
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                log.WriteErrorLine(e.ToString());
            }
            finally
            {
                log.WriteLine("program completed");
                log.Close(waitBeforeConsoleClose: true);
            }
        }
    }
  internal class UploadFiles
    {
        public UploadFiles(SimpleLogger log)
        {
            try
            {
                const string sqlConnectionString = @"Data Source=localhost;Initial Catalog=Network;Integrated Security=True";
                var fileEntries = Directory.GetFiles(@"c:\uploads\");
                foreach (var fileName in fileEntries)
                {
                    var fl = new FileLoader.FileLoader(log, sqlConnectionString: sqlConnectionString, fileName: fileName);
                }
            }
            catch (Exception e)
            {
                log.WriteErrorLine($"Exception raised in {GetType().Name} -- {e.ToString()}");
            }
        }
    }
}
