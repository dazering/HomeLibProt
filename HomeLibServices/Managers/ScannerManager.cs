using HomeLibServices.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeLibServices.Managers
{
    public class ScannerManager
    {
        private readonly Scanner scanner;
        private CancellationTokenSource source;

        public event EventHandler<ScannerEventArgs> ScnanningStateChanged;

        public ScannerManager(Scanner scan)
        {
            scanner = scan;
            scanner.ScanningOver += ChangedScanningState;
            scanner.ChangedScanningState += ChangedScanningState;
            source = new CancellationTokenSource();
        }

        private void ChangedScanningState(object sender, ScannerEventArgs e)
        {
            ScnanningStateChanged?.Invoke(this, e);
        }

        public bool TryStartScanning()
        {
            if (scanner.IsScanningRun())
            {
                return false;
            }

            Task.Factory.StartNew(() =>
            {
                scanner.ReadLocalRepository(source.Token);
            }, source.Token);
            return true;
        }

        public void CancelScanning()
        {
            if (scanner.IsScanningRun())
            {
                source?.Cancel();
                Thread.Sleep(10000);
                source?.Dispose();
                source = new CancellationTokenSource();
            }
        }

        public ScannerState GetScannerState()
        {
            return scanner.GetScannerState();
        }
    }
}
