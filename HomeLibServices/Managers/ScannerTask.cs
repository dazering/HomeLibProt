using HomeLibServices.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeLibServices.Managers
{
    internal class ScannerTasker
    {
        private readonly Scanner scanner;
        private CancellationTokenSource source;

        public event EventHandler<ScannerEventArgs> ScnanningStateChanged;

        public ScannerTasker(string path, IServiceProvider provider)
        {
            scanner = new Scanner(path, provider);
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
            if (scanner.GetScannerState().IsScanningRun)
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
            if(scanner.GetScannerState().IsScanningRun)
            {
                source?.Cancel();
                Thread.Sleep(1000);
                source?.Dispose();
                source = new CancellationTokenSource();
            }
        }
    }
}
