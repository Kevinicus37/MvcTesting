using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MediaController : Controller
    {
        private MovieCollectorContext _context;


        public MediaController(MovieCollectorContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
            var vm = new MediaIndexViewModel() { Items = mediaFormats };

            return View(vm);
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
                _context.MediaFormats.Add(newMediaFormat);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(addMediaFormatViewModel);
        }

        public IActionResult Remove()
        {
            List<MediaFormat> mediaFormats = _context.MediaFormats.ToList();
            RemoveMediaFormatViewModel vm = new RemoveMediaFormatViewModel() { Items = mediaFormats };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Remove(int[] IDs)
        {
            foreach (int id in IDs)
            {
                MediaFormat removeMediaFormat = _context.MediaFormats.SingleOrDefault(m => m.ID == id);
                _context.MediaFormats.Remove(removeMediaFormat);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            MediaFormat editMediaFormat = _context.MediaFormats.SingleOrDefault(g => g.ID == id);
            if (editMediaFormat != null)
            {
                EditMediaFormatViewModel vm = new EditMediaFormatViewModel() { ID = id, Name = editMediaFormat.Name };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(EditMediaFormatViewModel vm)
        {
            if (ModelState.IsValid)
            {
                MediaFormat editMediaFormat = _context.MediaFormats.SingleOrDefault(m => m.ID == vm.ID);
                editMediaFormat.Name = vm.Name;
                _context.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
    }
}

