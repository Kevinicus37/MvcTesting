using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;

namespace MvcTesting.Controllers
{
    public class AudioFormatController : Controller
    {

        private MovieCollectorContext _context;


        public AudioFormatController(MovieCollectorContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
            var vm = new AudioFormatIndexViewModel() { Items = audioFormats };

            return View(vm);
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
                _context.AudioFormats.Add(newAudio);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(addAudioFormatViewModel);
        }

        public IActionResult Remove()
        {
            List<AudioFormat> audioFormats = _context.AudioFormats.ToList();
            RemoveAudioFormatViewModel vm = new RemoveAudioFormatViewModel() { Items = audioFormats };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Remove(int[] IDs)
        {
            foreach (int id in IDs)
            {
                AudioFormat removeAudioFormat = _context.AudioFormats.SingleOrDefault(a => a.ID == id);
                _context.AudioFormats.Remove(removeAudioFormat);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            AudioFormat editAudioFormat = _context.AudioFormats.SingleOrDefault(a => a.ID == id);
            if (editAudioFormat != null)
            {
                EditAudioFormatViewModel vm = new EditAudioFormatViewModel() { ID = id, Name = editAudioFormat.Name };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(EditAudioFormatViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AudioFormat editAudioFormat = _context.AudioFormats.SingleOrDefault(a => a.ID == vm.ID);
                editAudioFormat.Name = vm.Name;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}