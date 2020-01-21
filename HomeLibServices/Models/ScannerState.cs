using System;

namespace HomeLibServices.Models
{
    public class ScannerState
    {
        public int CurrentErrorsCount { get; set; }
        public int BooksInLocalRepository { get; set; }
        public int BooksAdded { get; set; }
        public int BooksInDataBase { get; set; }
        public int BooksNotAddedInDb { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string ElapsedTime { get; set; }
        public bool IsScanningRun { get; set; } = false;

        private DateTime startTime;
        private DateTime finishTime;
        private TimeSpan elapsedTime;

        public void SetStartTime()
        {
            startTime = DateTime.Now;
            StartTime = $"{startTime:dd MMM hh:mm}";
        }

        public void SetFinishTime()
        {
            finishTime = DateTime.Now;
            elapsedTime = finishTime - startTime;
            FinishTime = $"{finishTime:dd MMM hh:mm}";
            ElapsedTime = $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}.{elapsedTime.Milliseconds:D3}";
        }
    }
}
