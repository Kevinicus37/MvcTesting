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

         private MovieCollectorContext context;


    public GenreController(MovieCollectorContext dbContext)
        {
            context = dbContext;
        }
    // GET: /<controller>/
    public IActionResult Index()
        {
            List<Genre> genres = context.Genres.ToList();
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
                context.Genres.Add(newGenre);
                context.SaveChanges();
                return View();
            }
            return View(addGenreViewModel);
        }

        public IActionResult RemoveGenre()
        {
            List<Genre> genres = context.Genres.ToList();
            RemoveGenreViewModel vm = new RemoveGenreViewModel() { Items = genres };
            return View(vm);
        }

        [HttpPost]
        public IActionResult RemoveGenre(int[] genres)
        {
            foreach (int genre in genres)
            {
                Genre removeGenre = context.Genres.SingleOrDefault(g => g.ID == genre);
                context.Genres.Remove(removeGenre);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Genre editGenre = context.Genres.SingleOrDefault(g => g.ID == id);
            if (editGenre != null)
            {
                EditGenreViewModel vm = new EditGenreViewModel() { ID = id, Name = editGenre.Name };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(EditMovieViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Genre editGenre = context.Genres.SingleOrDefault(g => g.ID == vm.ID);
                editGenre.Name = vm.Name;
                context.SaveChanges();

            }


            return RedirectToAction("Index");
        }
    }
}
