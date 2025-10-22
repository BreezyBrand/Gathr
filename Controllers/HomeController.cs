using System.Diagnostics;
using CrummyApp.Data;
using CrummyApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrummyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;

        public HomeController(ApplicationContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<CardImages> imgs = _context.Images.OrderBy(n => Guid.NewGuid()).Take(3).ToList();
            
            
            return View(imgs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
