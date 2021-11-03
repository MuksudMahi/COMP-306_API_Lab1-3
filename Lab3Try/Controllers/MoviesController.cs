using Lab3Try.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab3Try.Controllers
{
    public class MoviesController : Controller
    {
        // GET: Movies
        public ActionResult List()
        {
            Movie movie = new Movie
            {
                Name = "Movie1"
            };
         return View(movie);
        }
    }
}