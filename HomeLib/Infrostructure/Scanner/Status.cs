using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeLib.Infrostructure.Scanner
{
    public class Status
    {
        public Status()
        {
            SetStatusUnscanned();
        }
        public string StateScanner { get; private set; } = "Ready to scan";
        private int countBookInLocalRepository = 0;
        private int countBookInDataBase = 0;
        private List<string> errors = new List<string>();
        public List<string> Errors { get => errors; }
        private int notAddedBooks = 0;
        public int NotAddedBooks { get => notAddedBooks; private set => notAddedBooks = value; }
        public int CountBookInLocalRepository { get => countBookInLocalRepository; set => countBookInLocalRepository = value; }
        public int CountBookInDataBase { get => countBookInDataBase; set => countBookInDataBase = value; }

        private string timeElapsed = "No information.";

        public void SetTime(TimeSpan time)
        {
            timeElapsed = time.ToString();
        }

        public string GetTime()
        {
            return timeElapsed;
        }

        internal void IncreaseNotAddedBooks()
        {
            notAddedBooks++;
        }

        internal void AddErrorMessage(string message)
        {
            errors.Add(message);
        }

        internal void SetStatusError()
        {
            StateScanner = "Error";
        }

        internal void SetStatusInProgress()
        {
            StateScanner = "In Progress";
        }

        internal void SetStatusCanceled()
        {
            StateScanner = "Canceled";
        }

        internal void SetStatusScanned()
        {
            StateScanner = "Scanned";
        }

        internal void SetStatusUnscanned()
        {
            StateScanner = "Unscanned";
        }
        internal bool IsScannerInProgress()
        {
            if (StateScanner == "In Progress")
            {
                return true;
            }
            return false;
        }
    }
}
