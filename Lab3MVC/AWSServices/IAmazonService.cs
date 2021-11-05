﻿using Lab3MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.AWSServices
{
    interface IAmazonService
    {
        void CreateDynamoDBTable();
        Task<User> GetUsers(string email);
        Task<Movie> UploadFile(string selectedBucket, string movieTitle, Stream movieVideo, Stream movieImage);
        List<string> GetBuckets();
        Task<List<Movie>> GetMovies();
        Task<Movie> GetMovie(string MovieId);
        Task SaveComment(string email, string movieId, string comment, int RateNum);
    }
}
