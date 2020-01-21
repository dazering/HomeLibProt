using HomeLib.Infrostructure.Scanner;
using HomeLibServices.Managers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class StatusController : Controller
    {
        private BookManager BookManager;
        private ScannerMessenger scannerMessenger;
        public StatusController(BookManager manager, ScannerMessenger messenger)
        {
            BookManager = manager;
            scannerMessenger = messenger;
        }

        [Route("Status")]
        public IActionResult GetStatus()
        {
            return View("Status", BookManager.GetScannerState());
        }
    }
}
