using System;

namespace ScannerService.Models
{
    public enum StateScanner
    {
        Unscanned,
        Canceled,
        InProgress,
        Scanned,
        Error
    }
    public class Status
    {
        public StateScanner StateScanner { get; set; }
        private int countBookInLocalRepository = 0;
        private int countBookInDataBase = 0;
        public int CountBookInLocalRepository { get => countBookInLocalRepository; set => countBookInLocalRepository = value; }
        public int CountBookInDataBase { get => countBookInDataBase; private set => countBookInDataBase = value; }
        public void UpdateCountBookInDb(int sum)
        {
            CountBookInDataBase = sum;
        }
        private string timeElapsed;
        public string TimeElapsed
        {
            get => timeElapsed;
        }
        public void GetTime(TimeSpan time)
        {
            timeElapsed = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 10);
        }
    }
}
