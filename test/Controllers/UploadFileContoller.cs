using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using test.Services;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly UploadFileService _uploadFileService;

        public UploadFileController(UploadFileService uploadFileService) => _uploadFileService = uploadFileService;

        [HttpPost]
        [Route(nameof(UploadLargeFile))]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadLargeFile()
        {
            try
            {
                 await _uploadFileService.Upload(HttpContext.Request);
                 return Ok();
            }
            catch (UnsupportedContentTypeException)
            {
                return new UnsupportedMediaTypeResult();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
