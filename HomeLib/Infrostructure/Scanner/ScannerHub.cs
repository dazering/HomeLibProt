using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeLibServices.Managers;
using HomeLibServices.Models;
using Microsoft.AspNetCore.SignalR;

namespace HomeLib.Infrostructure.Scanner
{
    public class ScannerHub : Hub
    {
        private BookManager BookManager;

        public ScannerHub(BookManager manager)
        {
            BookManager = manager;
        }

        public void StopScanning()
        {
            BookManager.StopUpdatingDbRepository();
        }

        public void StartScanning()
        {
            BookManager.StartUpdateDbRepository();
        }
    }
}
