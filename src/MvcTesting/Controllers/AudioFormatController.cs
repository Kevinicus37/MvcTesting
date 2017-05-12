using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;

namespace MvcTesting.Controllers
{
    public class AudioFormatController : Controller
    {

        private MovieCollectorContext context;


        public AudioFormatController(MovieCollectorContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            AddAudioFormatViewModel addAudioFormatViewModel = new AddAudioFormatViewModel();
            return View(addAudioFormatViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddAudioFormatViewModel addAudioFormatViewModel)
        {
            if (ModelState.IsValid)
            {
                AudioFormat newAudio = new AudioFormat { Name = addAudioFormatViewModel.Name };
                context.AudioFormats.Add(newAudio);
                context.SaveChanges();
                return View();
            }
            return View(addAudioFormatViewModel);
        }
    }
}