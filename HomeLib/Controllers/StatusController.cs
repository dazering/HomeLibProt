using HomeLibServices.Managers;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeLib.Controllers
{
    public class StatusController : Controller
    {
        private BookManager BookManager;
        public StatusController(BookManager manager)
        {
            BookManager = manager;
        }

        [Route("Status")]
        public IActionResult GetStatus()
        {
            return View("Status", BookManager.GetScannerState());
        }
    }
}
