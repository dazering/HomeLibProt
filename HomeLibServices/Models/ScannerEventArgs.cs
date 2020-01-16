using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibServices.Models
{
    public class ScannerEventArgs : EventArgs
    {
        public ScannerState ScannerState { get; set; }
        public ScannerEventArgs(ScannerState scanner)
        {
            ScannerState = scanner;
        }
    }
}
