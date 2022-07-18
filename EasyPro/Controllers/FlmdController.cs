using Microsoft.AspNetCore.Mvc;

namespace EasyPro.Controllers
{
    public class FlmdController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
