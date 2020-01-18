using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerHub : Hub
    {
        public void Send(Scanner scanner)
        {
            
        }
    }
}
