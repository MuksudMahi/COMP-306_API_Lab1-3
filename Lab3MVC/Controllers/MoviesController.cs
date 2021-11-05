using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Lab3MVC.AWSServices;
using Lab3MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Controllers
{
    public class MoviesController : Controller
    {

        private  readonly IAmazonService _amazonService;


        public MoviesController(IAmazonDynamoDB dynamoDBClient, IAmazonS3 s3Client, lab3Context context)
        {
            _amazonService = new AmazonService(dynamoDBClient, s3Client, context);
            _amazonService.CreateDynamoDBTable();
            
        }

        [HttpPost]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List(string email)
        {
            MovieList movieList = new MovieList()
            {
                Users = email,
                Movies = _amazonService.GetMovies().Result
            };
            return View("List", movieList);
        }

        [HttpPost]
        public IActionResult Upload(MovieBucket movieBucket)
        {
            string domain = AppDomain.CurrentDomain.BaseDirectory;

            if (ModelState.IsValid)
            {

                var MovieVideo = new MemoryStream();
                movieBucket.MovieVideo.CopyTo(MovieVideo);
                var MovieImage = new MemoryStream();
                movieBucket.MovieImage.CopyTo(MovieImage);

                var result = _amazonService.UploadFile(movieBucket.SelectedBucket,
                    movieBucket.Movie.MovieTitle,
                    MovieVideo,
                    MovieImage
                  ).Result;
            }
            return List();

        }
        [HttpGet]
        public IActionResult Upload(string email)
        {
            return View(new MovieBucket() { Buckets = _amazonService.GetBuckets(), Email = email });
        }

        [HttpGet]
        public IActionResult Details(string email, string movieId)
        {

            return View(new UsersMovie()
            {
                Users = email,
                Movie = _amazonService.GetMovie(movieId).Result
            });
        }


        [HttpGet]
        public IActionResult AddComment(string email, string movieId)
        {

            return View(new Rating()
            {
                MovieId = movieId,
                Users = email
            });
        }


        [HttpPost]
        public IActionResult AddComment(string email, string movieId, string comment, int RateNum)
        {

            _amazonService.SaveComment(email, movieId, comment, RateNum);
            return RedirectToAction("Details", new { email, movieId });
        }

    }
}
