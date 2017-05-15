using Microsoft.AspNetCore.Mvc;
using MvcTesting.Models;
using MvcTesting.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcTesting.Controllers
{
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
            return View();
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
    }
}
