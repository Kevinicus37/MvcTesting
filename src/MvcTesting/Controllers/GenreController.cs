using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
    [Authorize(Roles="Admin")]
    public class GenreController : Controller
    { 

         private MovieCollectorContext _context;


    public GenreController(MovieCollectorContext dbContext)
        {
            _context = dbContext;
        }
    // GET: /<controller>/
    public IActionResult Index()
        {
            List<Genre> genres = _context.Genres.ToList();
            var vm = new GenreIndexViewModel() { Items = genres };
            return View(vm);
        }

        public IActionResult Add()
        {
            AddGenreViewModel addGenreViewModel = new AddGenreViewModel();
            return View(addGenreViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddGenreViewModel addGenreViewModel)
        {
            if (ModelState.IsValid)
            {
                Genre newGenre = new Genre { Name = addGenreViewModel.Name };
                _context.Genres.Add(newGenre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(addGenreViewModel);
        }

        public IActionResult RemoveGenre()
        {
            List<Genre> genres = _context.Genres.ToList();
            RemoveGenreViewModel vm = new RemoveGenreViewModel() { Items = genres };
            return View(vm);
        }

        [HttpPost]
        public IActionResult RemoveGenre(int[] IDs)
        {
            foreach (int id in IDs)
            {
                Genre removeGenre = _context.Genres.SingleOrDefault(g => g.ID == id);
                _context.Genres.Remove(removeGenre);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Genre editGenre = _context.Genres.SingleOrDefault(g => g.ID == id);
            if (editGenre != null)
            {
                EditGenreViewModel vm = new EditGenreViewModel() { ID = id, Name = editGenre.Name };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(EditGenreViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Genre editGenre = _context.Genres.SingleOrDefault(g => g.ID == vm.ID);
                editGenre.Name = vm.Name;
                _context.SaveChanges();

            }


            return RedirectToAction("Index");
        }
    }
}
