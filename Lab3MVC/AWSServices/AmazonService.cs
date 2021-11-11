using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Lab3MVC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lab3MVC.AWSServices
{
    public class AmazonService: IAmazonService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly DynamoDBContext context;
        private readonly lab3Context _rdsContext;
        string domain = AppDomain.CurrentDomain.BaseDirectory;
        public AmazonService(IAmazonDynamoDB dynamoDBClient, IAmazonS3 client, lab3Context rdsContext)
        {
            _rdsContext = rdsContext;
            _dynamoDBClient = dynamoDBClient;
            context = new DynamoDBContext(_dynamoDBClient);
            _s3Client = client;
        }

        public void CreateDynamoDBTable()
        {
            try
            {
                List<string> currentTables = _dynamoDBClient.ListTablesAsync().Result.TableNames;
                if (!currentTables.Contains("Movie"))
                {
                    CreateMovieTable();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void CreateMovieTable()
        {
            string tableName = "Movie";
            var request = new CreateTableRequest()
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "MovieId",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "MovieTitle",
                        AttributeType = "S"
                    },
                     new AttributeDefinition
                    {
                        AttributeName = "Rate",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "MovieId",
                        KeyType = "HASH"
                    }
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>()
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName="SearchByRating",
                        KeySchema = new List<KeySchemaElement>()
                        {
                            new KeySchemaElement
                            {
                                AttributeName="Rate",
                                KeyType="HASH"
                            },
                            new KeySchemaElement
                            {
                                AttributeName="MovieTitle",
                                KeyType = "RANGE"
                            }
                        },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits=5,
                            WriteCapacityUnits=5
                        },
                        Projection = new Projection
                        {
                            ProjectionType="ALL"
                        }
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tableName
            };
            var response = _dynamoDBClient.CreateTableAsync(request);

            WaitUntilTableIsReady(tableName);
        }

        private void WaitUntilTableIsReady(string tableName)
        {
            string status = null;
            do
            {
                Thread.Sleep(5000);
                try
                {
                    var res = _dynamoDBClient.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });
                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException e)
                {

                    Console.WriteLine(e.Message);
                }
            } while (status != "ACTIVE");
            Console.WriteLine("Table Created");
        }


        public List<String> GetBuckets()
        {
            List<String> buckets = new List<string>();
            ListBucketsResponse response = _s3Client.ListBucketsAsync().Result;
            foreach (S3Bucket bucket in response.Buckets)
            {
                buckets.Add(bucket.BucketName);
            }
            return buckets;
        }

        public async Task<Movie> UploadFile(string BucketName, string MovieTitle,int Rate, string Actors, string Description, Stream MovieVideoPath, Stream MovieImagePath)
        {
            DynamoDBContext Context = new DynamoDBContext(_dynamoDBClient);
            Movie movie = new Movie();
            movie.MovieId = Guid.NewGuid().ToString();
            movie.MovieTitle = MovieTitle;
            movie.Rate = Rate;
            movie.Actors = Actors;
            movie.Description = Description;

            movie.MovieImage = S3Link.Create(Context, BucketName, MovieTitle+".jpg", RegionEndpoint.CACentral1);
            movie.MovieVideo = S3Link.Create(Context, BucketName, MovieTitle + ".avi", RegionEndpoint.CACentral1);

            var fileTransferUtility = new TransferUtility(_s3Client);
            var MovieImage = new TransferUtilityUploadRequest()
            {
                CannedACL = S3CannedACL.PublicRead,
                BucketName = BucketName,
                Key = MovieTitle +".jpg",
                InputStream = MovieImagePath
            };

            var MovieVideo = new TransferUtilityUploadRequest()
            {
                CannedACL = S3CannedACL.PublicRead,
                BucketName = BucketName,
                Key = MovieTitle + ".avi",
                InputStream = MovieVideoPath
            };
            await fileTransferUtility.UploadAsync(MovieImage);
            await fileTransferUtility.UploadAsync(MovieVideo);

            await Context.SaveAsync<Movie>(movie);
            return await GetMovie(movie.MovieId);
        }
        public Task<Movie> GetMovie(string MovieId)
        {
            DynamoDBContext Context = new DynamoDBContext(_dynamoDBClient);

            return Context.LoadAsync<Movie>(MovieId);

        }


        public async Task<User> GetUsers(string email)
        {

            return await _rdsContext.Users
                            .FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<List<Movie>> GetMovies()
        {
            var conditions = new List<ScanCondition>();
            List<Movie> movies = await context.ScanAsync<Movie>(conditions).GetRemainingAsync();
            foreach (Movie movie in movies)
            {
                Task task1 = _s3Client.DownloadToFilePathAsync(
                    movie.MovieImage.BucketName,
                    movie.MovieImage.Key, domain,
                    new Dictionary<string, object>(),
                    default);
            }
            return movies;
        }


        public async Task SaveComment(string email, string movieId,string comment, int RateNum)
        {
            Movie movie = await GetMovie(movieId);
            movie.Ratings.Add(new Rating()
            {
                RateDate = DateTime.Now,
                Comment = comment,
                RateNum = RateNum,
                Users = email
            });
            await context.SaveAsync<Movie>(movie);
        }

        public async Task SaveMovie(Movie movie)
        {
            await context.SaveAsync(movie);
        }

        public async Task DeleteMovie(Movie movie)
        {
            await context.DeleteAsync(movie);
        }

        public Movies Search(int rate)
        {
            var request = new QueryRequest
            {
                TableName = "Movie",
                IndexName = "SearchByRating",
                KeyConditionExpression = "Rate = :v_rate",
                ProjectionExpression = "MovieId, MovieTitle, Rate",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":v_rate", new AttributeValue
                        {
                            N=rate.ToString()
                        }
                    }
                }
            };
            var result = _dynamoDBClient.QueryAsync(request).Result;
            return new Movies
            {
                Items = result.Items.Select(Map).ToList()
            };

        }

        private Movie Map(Dictionary<string, AttributeValue> result)
        {

            return new Movie
            {
                MovieId = result["MovieId"].S,
                MovieTitle = result["MovieTitle"].S,
                Rate = Convert.ToInt32(result["Rate"].N),
                MovieImage=GetMovie(result["MovieId"].S).Result.MovieImage,
                MovieVideo = GetMovie(result["MovieId"].S).Result.MovieVideo,
                Ratings = GetMovie(result["MovieId"].S).Result.Ratings
            };
        }
    }
}


