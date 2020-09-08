using HomeLib.Infrostructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeLib.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            if (TempData["error"] != null)
            {
                return View(new ErrorModel() { ErrorMessage = TempData["error"]as string});
            }
            return View(new ErrorModel(){ErrorMessage = "Что-то пошло не так..."})
            ;
        }
    }
}
