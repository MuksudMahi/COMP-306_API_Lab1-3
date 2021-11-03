using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab3MVC.Services;

namespace Lab3MVC.Controllers
{
    [Produces("application/json")]
    [Route("api/S3Bucket")]
    [ApiController]
    public class S3BucketController : ControllerBase
    {
        private readonly IS3Service _service;
        public S3BucketController(IS3Service service)
        {
            _service = service;
        }
        [HttpPost]
        [Route("CreateBucket/{bucketName}")]
        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateBucketAsync(bucketName);
            return Ok();
        }
        [HttpPost]
        [Route("AddFile/{bucketName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName)
        {
            await _service.UploadFileAsync(bucketName);
            return Ok();
        }

        [HttpGet]
        [Route("GetFile/{bucketname}")]
        public async Task<IActionResult>GetActionResultAsync([FromRoute] string bucketName)
        {
            await _service.GetObjectFromS3Async(bucketName);
            return Ok();
        }

    }
}
