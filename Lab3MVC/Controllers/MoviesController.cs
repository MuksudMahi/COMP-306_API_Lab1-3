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
        private static  User savedUser;


        public MoviesController(IAmazonDynamoDB dynamoDBClient, IAmazonS3 s3Client, lab3Context context)
        {
            _amazonService = new AmazonService(dynamoDBClient, s3Client, context);
            _amazonService.CreateDynamoDBTable();
            
        }

        [HttpGet]
        public IActionResult Home()
        {
            return RedirectToAction("List", new { savedUser.Email });
        }

        [HttpPost]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult List(string email)
        {
            savedUser = _amazonService.GetUsers(email).Result;
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
                movieBucket.SelectedBucket = "comp-306-bucket-3";
                var MovieVideo = new MemoryStream();
                movieBucket.MovieVideo.CopyTo(MovieVideo);
                var MovieImage = new MemoryStream();
                movieBucket.MovieImage.CopyTo(MovieImage);

                var result = _amazonService.UploadFile(movieBucket.SelectedBucket,
                    movieBucket.Movie.MovieTitle, movieBucket.Movie.Rate, movieBucket.Movie.Actors, movieBucket.Movie.Description,
                    MovieVideo,
                    MovieImage
                  ).Result;
            }
            return RedirectToAction("List", new { movieBucket.Email, movieBucket.Movie.MovieId });

        }
        [HttpGet]
        public IActionResult Upload(string email)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            return View(new MovieBucket() { Buckets = _amazonService.GetBuckets(), Email = email });
        }

        [HttpGet]
        public IActionResult Details(string email, string movieId)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            return View(new UsersMovie()
            {
                Users = email,
                Movie = _amazonService.GetMovie(movieId).Result
            });
        }

        [HttpGet]
        public IActionResult Edit(string email, string movieId)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            return View(new UsersMovie()
            {
                Users = email,
                Movie = _amazonService.GetMovie(movieId).Result
            });
        }

        [HttpPost]
        public IActionResult Edit(UsersMovie usersMovie)
        {

            Movie movie = _amazonService.GetMovie(usersMovie.Movie.MovieId).Result;
            movie.MovieTitle = usersMovie.Movie.MovieTitle;
            movie.Actors = usersMovie.Movie.Actors;
            movie.Description = usersMovie.Movie.Description;
            movie.Rate = usersMovie.Movie.Rate;
            _amazonService.SaveMovie(movie);
            Console.WriteLine("After save");

            Console.WriteLine(usersMovie.Users);

            MovieList movieList = new MovieList
            {
                Users = usersMovie.Users,
                Movies = _amazonService.GetMovies().Result
            };
            savedUser = _amazonService.GetUsers(usersMovie.Users).Result;
            return View("List", movieList);

        }

        [HttpGet]
        public IActionResult Delete(string email, string movieId)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            Movie movie = _amazonService.GetMovie(movieId).Result;
            _amazonService.DeleteMovie(movie);
            return RedirectToAction("List", new { email});

        }

        [HttpGet]
        public IActionResult Search(string email, int rate)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            MovieList movieList = new MovieList()
            {
                Users = email,
                Movies = _amazonService.Search(rate).Items.ToList()
            };
            return View("List", movieList);
        }

        [HttpGet]
        public IActionResult AddComment(string email, string movieId)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            return View(new Rating()
            {
                MovieId = movieId,
                Users = email
            });
        }


        [HttpPost]
        public IActionResult AddComment(string email, string movieId, string comment, int RateNum)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            _amazonService.SaveComment(email, movieId, comment, RateNum);
            return RedirectToAction("Details", new { email, movieId });
        }

        [HttpGet]
        public IActionResult DeleteComment(string email, string movieId, string comment, int rateNum, DateTime rateDate)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            Movie movie = _amazonService.GetMovie(movieId).Result;

            Rating rating = new Rating
            {
                MovieId = movieId,
                Users = email,
                Comment = comment,
                RateNum = rateNum,
                RateDate = rateDate
            };
            foreach (Rating rate in movie.Ratings)
            {

                if (rate.Comment == rating.Comment && rate.RateDate.ToString()==rating.RateDate.ToString())

                {
                    movie.Ratings.Remove(rate);
                    break;
                }
            }

            _amazonService.SaveMovie(movie);
            return RedirectToAction("Details", new { email, movieId });
        }

        [HttpGet]
        public IActionResult EditComment(string email, string movieId, string comment, int rateNum, DateTime rateDate)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            return View( new Rating
            {
                MovieId = movieId,
                Users = email,
                Comment = comment,
                RateNum = rateNum,
                RateDate = rateDate
            });
        }
        [HttpPost]
        public IActionResult EditComment(string email, string movieId, string comment, int RateNum, DateTime rateDate, string oldComment)
        {
            savedUser = _amazonService.GetUsers(email).Result;

            Movie movie = _amazonService.GetMovie(movieId).Result;
            foreach (Rating rate in movie.Ratings)
            {

                if (rate.Comment == oldComment && rate.RateDate.ToString() == rateDate.ToString())

                {
                    rate.Comment = comment;
                    rate.RateNum = RateNum;
                    break;
                }
            }

            _amazonService.SaveMovie(movie);
            return RedirectToAction("Details", new { email, movieId });
        }

    }
}
