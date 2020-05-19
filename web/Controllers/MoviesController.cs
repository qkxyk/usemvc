using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcMovie.Data;
using web.Models;

namespace web.Controllers {
    public class MoviesController : Controller {
        private readonly ILogger<MoviesController> _log;
        private readonly MvcMovieContext context;

        public MoviesController (ILogger<MoviesController> log, MvcMovieContext context) {
            this._log = log;
            this.context = context;
        }

        // public IActionResult Index (string name,int numTimes=1) {

        //     ViewData["Message"]="hello"+name;
        //     ViewData["NumTimes"]= numTimes;
        //     return View ();
        // }
        public async Task<IActionResult> Index (string movieGenre,string searchString) {
            var movies = from m in context.Movie
                 select m;
            var genreQuery = context.Movie.OrderBy(a=>a.Genre).Select(a=>a.Genre);
            var movie = context.Movie.AsNoTracking();
            if(!string.IsNullOrWhiteSpace(searchString)){
                movie = movie.Where(a=>a.Title.Contains(searchString));
            }
            if(!string.IsNullOrWhiteSpace(movieGenre)){
                movie=movie.Where(a=>a.Genre==movieGenre);
            }
            var movieGenreVM= new MovieGenreViewModel{
                Genres=new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies=await movie.ToListAsync()
            };
            return View (movieGenreVM);
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

        public IActionResult Create(){
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie){
            if(ModelState.IsValid){
                context.Movie.Add(movie);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id){
            if(id==null){
                return NotFound();
            }
            var movie =await context.Movie.FirstOrDefaultAsync(m=>m.Id==id);
            if(movie==null){
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int Id){
            var movie =await context.Movie.FindAsync(Id);
            context.Movie.Remove(movie);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}