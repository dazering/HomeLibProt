using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeLib.Infrostructure.Scanner;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class StatusController : Controller
    {
        Scanner Scanner;
        public StatusController(Scanner scanner)
        {
            Scanner = scanner;
        }
        [Route("Status/StartScan")]
        public IActionResult Scan()
        {
            Scanner.StartScanAsync();
            return RedirectToAction(nameof(GetStatus));
        }

        [Route("Status")]
        public IActionResult GetStatus()
        {
            Status status = Scanner.GetStatus();
            return View("Status", status);
        }

        [Route("Status/CancelScanning")]
        public IActionResult CancelScanning()
        {
            Scanner.CancelScanning();
            return RedirectToAction(nameof(GetStatus));
        }
    }
}
