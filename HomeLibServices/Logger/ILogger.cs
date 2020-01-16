using System;

namespace HomeLibServices.Logger
{
    public interface ILogger
    {
        void WriteError(Exception exception, string archiveName, string fileName);
    }
}
