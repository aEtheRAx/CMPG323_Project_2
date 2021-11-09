using CMPG323_Project_2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CMPG323_Project_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<GalleryController> logger)
        {
            _logger = logger;                   //Logging initiated
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home page accessed");
            return View();
        }

        public IActionResult About()
        {
            _logger.LogInformation("About page accessed");
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            _logger.LogInformation("Contact page accessed");
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy page accessed");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
