using Assignment3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Assignment3.Filters;

namespace Assignment3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        [ActionFilter]
        [HttpGet("/about")]
        public IActionResult About()
        {
            return View("About");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
