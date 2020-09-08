using System;
using System.Data.SqlClient;
using System.IO;
using HomeLib.Infrostructure.Scanner;
using HomeLibServices.Managers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class StatusController : Controller
    {
        private ScannerManager ScannerManager;
        private ScannerMessenger scannerMessenger;
        public StatusController(ScannerManager manager, ScannerMessenger messenger)
        {
            ScannerManager = manager;
            scannerMessenger = messenger;
        }

        [Route("Status")]
        public IActionResult GetStatus()
        {
            try
            {
                return View("Status", ScannerManager.GetScannerState());
            }
            catch (DirectoryNotFoundException)
            {
                TempData["error"] = "Директории с книгами не существует.";
                return RedirectToAction("Index", "Error");
            }
            catch (SqlException)
            {
                TempData["error"] = "Упс... Сервер базы данных не доступен.";
                return RedirectToAction("Index", "Error");
            }
        }
    }
}
