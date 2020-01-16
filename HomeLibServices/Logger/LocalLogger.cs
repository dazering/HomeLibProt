using System;
using System.IO;

namespace HomeLibServices.Logger
{
    public class LocalLogger : ILogger
    {
        private readonly string pathToLogDir;

        public LocalLogger()
        {
            pathToLogDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(pathToLogDir))
            {
                Directory.CreateDirectory(pathToLogDir);
            }
        }

        public void WriteError(Exception exception, string archiveName, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(pathToLogDir, $"{DateTime.Now:yy-MM-dd}.txt"), true))
            {
                sw.WriteLine($"{DateTime.Now:T} {exception.GetType()} {archiveName} {fileName} {exception.Message}");
            }
        }
    }
}
