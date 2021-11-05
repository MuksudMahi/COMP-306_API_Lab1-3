using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Lab3MVC.Models;
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
        private DynamoDBContext context;
        string domain = AppDomain.CurrentDomain.BaseDirectory;
        public AmazonService(IAmazonDynamoDB dynamoDBClient, IAmazonS3 client)
        {
            _dynamoDBClient = dynamoDBClient;
            context = new DynamoDBContext(_dynamoDBClient);
            _s3Client = client;
        }

        public void CreateDynamoDBTable()
        {
            try
            {
                CreateMovieTable();
                CreateUserTable();
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

        private void CreateUserTable()
        {
            string tableName = "User";
            var request = new CreateTableRequest()
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Email",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Email",
                        KeyType = "HASH"
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

        public async Task<Movie> UploadFile(string BucketName, string MovieTitle, Stream MovieVideoPath, Stream MovieImagePath)
        {
            DynamoDBContext Context = new DynamoDBContext(_dynamoDBClient);
            Movie movie = new Movie();
            movie.MovieId = Guid.NewGuid().ToString();
            movie.MovieTitle = MovieTitle;
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


        public async Task<Users> GetUsers(string email)
        {

            return await context.LoadAsync<Users>(email);
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


        public async Task SaveComment(Movie movie)
        {
            await context.SaveAsync<Movie>(movie);
        }
    }
}
