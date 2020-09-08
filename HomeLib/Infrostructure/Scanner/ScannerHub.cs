using HomeLibServices.Managers;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerHub : Hub
    {
        private ScannerManager ScannerManager;

        public ScannerHub(ScannerManager manager)
        {
            ScannerManager = manager;
        }

        public void StopScanning()
        {
            ScannerManager.CancelScanning();
        }

        public void StartScanning()
        {
            ScannerManager.TryStartScanning();
        }
    }
}
