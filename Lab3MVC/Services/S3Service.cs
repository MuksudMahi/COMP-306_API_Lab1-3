using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Lab3MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3MVC.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }
        public async Task<S3Response> CreateBucketAsync(string bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };
                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return new S3Response
                    {
                        Status = response.HttpStatusCode,
                        Message = response.ResponseMetadata.RequestId,
                    };
                    Console.WriteLine("Created");
                }
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            catch(Exception e)
            {
                return new S3Response
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }
            return new S3Response
            {
                Status = System.Net.HttpStatusCode.InternalServerError,
                Message = "Something is wrong"
            };
        }

        private const string FilePath = "C:/abc.txt";
        private const string UploadWithKeyName = "UploadWithKeyName";
        private const string FileStreamUpload = "FileStreamUpload";
        private const string AdvancedUpload = "AdvancedUpload";


        public async Task UploadFileAsync(string bucketName)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_client);
                //
                await fileTransferUtility.UploadAsync(FilePath, bucketName);
                //
                await fileTransferUtility.UploadAsync(FilePath, bucketName, UploadWithKeyName);
                //
                using(var fileToUpload = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    await fileTransferUtility.UploadAsync(fileToUpload, bucketName, FileStreamUpload);
                }
                //
                var fileTrasferUtilityRequest = new TransferUtilityUploadRequest()
                {
                    BucketName = bucketName,
                    FilePath = FilePath,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = 6291456,
                    Key = AdvancedUpload,
                    CannedACL = S3CannedACL.NoACL
                };
                fileTrasferUtilityRequest.Metadata.Add("param1", "value1");
                fileTrasferUtilityRequest.Metadata.Add("param2", "value2");

                await fileTransferUtility.UploadAsync(fileTrasferUtilityRequest);

            }
            catch(AmazonS3Exception e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task GetObjectFromS3Async(string bucketName)
        {
            const string keyName = "abc.txt";
            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                string responseBody;
                using(var response = await _client.GetObjectAsync(request))
                using(var responseStrem = response.ResponseStream)
                using(var reader = new StreamReader(responseStrem))
                {
                    var title = response.Metadata["x-amz-meta-title"];
                    var contentType = response.Headers["Content-Type"];
                    Console.WriteLine($"Object meta, Title: {title}");
                    Console.WriteLine($"Content type: {contentType}");
                    responseBody = reader.ReadToEnd();
                }
                var pathAndFileName = $"C:\\comp 229\\{keyName}";
                var createText = responseBody;
                File.WriteAllText(pathAndFileName, createText);
            }
            catch(Exception e)
            {

            }
        }
    }
}
