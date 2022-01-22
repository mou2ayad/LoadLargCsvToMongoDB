using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using test.Services;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly UploadFileService _uploadFileService;
        private ILogger<UploadFileController> _logger;

        public UploadFileController(UploadFileService uploadFileService, ILogger<UploadFileController> logger)
        {
            _uploadFileService = uploadFileService;
            _logger = logger;
        }

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
