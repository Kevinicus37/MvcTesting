using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcTesting.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult About()
        {
            ViewData["Message"] = "Welcome to MovieLot";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Feel free to contact us with any questions.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
