using Microsoft.AspNetCore.Mvc;

namespace TerminRessourcenPlaner.Controllers
{
    public class StartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
