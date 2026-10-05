using Microsoft.AspNetCore.Mvc;

namespace ColatonDeMelon.Controllers
{
    public class ContactoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}