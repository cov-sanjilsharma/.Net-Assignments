using Microsoft.AspNetCore.Mvc;
using Ecommerce_DBFirst.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace Ecommerce_DBFirst.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        [Route("Home/Error")]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionFeature?.Error;
            var path = exceptionFeature?.Path;

            _logger.LogError(exception, "Unhandled exception occurred while processing {Path}", path);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}