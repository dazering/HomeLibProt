using System;

namespace HomeLibServices.Models
{
    public class ScannerState
    {
        public ScannerState(int inDb, int inLocalRepo)
        {
            BooksInDataBase = inDb;
            BooksInLocalRepository = inLocalRepo;
        }

        public int CurrentErrorsCount { get; set; }
        public int BooksInLocalRepository { get; set; }
        public int BooksInDataBase { get; set; }
        public int BooksNotAddedInDb { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsScanningRun { get; set; } = false;
    }
}
