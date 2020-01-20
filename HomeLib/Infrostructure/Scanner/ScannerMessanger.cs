using HomeLibServices.Managers;
using HomeLibServices.Models;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerMessanger
    {
        private IHubContext<ScannerHub> hubContext;

        public ScannerMessanger(IHubContext<ScannerHub> cntx, BookManager manager)
        {
            hubContext = cntx;
            manager.ScannerMessage += SendMessage;
        }

        public void SendMessage(object o, ScannerEventArgs e)
        {
            hubContext.Clients.All.SendAsync("getStatus", e.ScannerState);
        }
    }
}
