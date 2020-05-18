using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcMovie.Data;
using web.Models;

namespace web.Controllers {
    public class MovieController : Controller {
        private readonly ILogger<MovieController> _log;
        private readonly MvcMovieContext context;

        public MovieController (ILogger<MovieController> log, MvcMovieContext context) {
            this._log = log;
            this.context = context;
        }

        // public IActionResult Index (string name,int numTimes=1) {

        //     ViewData["Message"]="hello"+name;
        //     ViewData["NumTimes"]= numTimes;
        //     return View ();
        // }
        public async Task<IActionResult> Index (string searchString) {
            var movie = context.Movie.AsNoTracking();
            if(!string.IsNullOrWhiteSpace(searchString)){
                movie = movie.Where(a=>a.Title.Contains(searchString));
            }

            return View (await movie.ToListAsync ());
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

        [HttpGet]
        public async Task<IActionResult> Edit (int? id) {
            if (id == null) {
                return NotFound ();
            }
            var movie = await context.Movie.FindAsync (id);
            if (movie == null) {
                return NotFound ();
            }
            return View (movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit (int id, Movie movie) {
            if(id!=movie.Id){
                return NotFound();
            }
            if(ModelState.IsValid){
                try{
                        context.Update(movie);
                        await context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException){
                    if(!MovieExists(movie.Id)){
                        return NotFound();
                    }
                    else{
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        private bool MovieExists(int id)
        {
            var data =context.Movie.Find(id);
            if(data==null){
                return false;
            }
            return true;
        }
    }
}