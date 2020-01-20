using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeLibServices.Models;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerMessanger
    {
        private IHubContext<ScannerHub> hubContext;

        public ScannerMessanger(IHubContext<ScannerHub> cntx)
        {
            hubContext = cntx;
        }

        public void SendMessage(object o, ScannerEventArgs e)
        {
            hubContext.Clients.All.SendAsync("getStatus", e.ScannerState);
        }
    }
}
