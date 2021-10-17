using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AWSS3.Services;
using System;

namespace AWSS3.Controllers
{
    [Produces("application/json")]
    [Route("api/S3Bucket")]
    public class S3BucketController : Controller
    {
        private readonly IS3Service _service;

        public S3BucketController(IS3Service service)
        {
            _service = service;
        }
        [HttpPost] //can create a new bucket was going to use to Test API
        [Route("CreateBucket/{bucketName}")]
        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName )
        {
            var response = await _service.CreateBucketAsync(bucketName);
            return Ok(response);

        }
        [HttpPost]
        [Route("AddFile/{bucketName}")]
        public async Task<IActionResult> AddFile([FromRoute] string bucketName)
        {
            await _service.UploadFileAsync(bucketName);
            
            return Ok();
        }
        [HttpPost]
        [Route("GetFile/{bucketName}")]
        public async Task<IActionResult> GetFile([FromRoute] string bucketName)
        {
            await _service.GetObjectFromS3Async(bucketName);
            
            return Ok();
        }

    }
}
