using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    public class MediaController : Controller
    {
        private MovieCollectorContext context;


        public MediaController(MovieCollectorContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            AddMediaFormatViewModel addMediaFormatViewModel = new AddMediaFormatViewModel();
            return View(addMediaFormatViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMediaFormatViewModel addMediaFormatViewModel)
        {
            if (ModelState.IsValid)
            {
                MediaFormat newMediaFormat = new MediaFormat { Name = addMediaFormatViewModel.Name };
                context.MediaFormats.Add(newMediaFormat);
                context.SaveChanges();
                return View();
            }
            return View(addMediaFormatViewModel);
        }
    }
}

