using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcMovie.Data;

namespace web.Controllers {
    public class HelloWorldController : Controller {
        private readonly ILogger<HelloWorldController> _log;
        private readonly MvcMovieContext context;

        public HelloWorldController (ILogger<HelloWorldController> log, MvcMovieContext context) {
            this._log = log;
            this.context = context;
        }

        // public IActionResult Index (string name,int numTimes=1) {

        //     ViewData["Message"]="hello"+name;
        //     ViewData["NumTimes"]= numTimes;
        //     return View ();
        // }
        public async Task<IActionResult> Index () {
            return View (await context.Movie.ToListAsync ());
        }
        public async Task<IActionResult> Details (int? id) {
            if (id == null) {
                return NotFound ();
            }
            var movie = await context.Movie.FirstOrDefaultAsync (a => a.Id == id);
            if (movie == null) {
                return NotFound ();
            }
            return View (movie);
        }

    }
}