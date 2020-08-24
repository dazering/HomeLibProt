using HomeLibServices.Managers;
using HomeLibServices.Models;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerMessenger
    {
        private IHubContext<ScannerHub> hubContext;

        public ScannerMessenger(IHubContext<ScannerHub> cntx, ScannerManager manager)
        {
            hubContext = cntx;
            manager.ScnanningStateChanged += SendMessage;
        }

        public void SendMessage(object o, ScannerEventArgs e)
        {
            hubContext.Clients.All.SendAsync("getStatus", e.ScannerState);
        }
    }
}
