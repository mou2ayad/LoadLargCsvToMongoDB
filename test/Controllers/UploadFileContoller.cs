using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using test.Contract;
using test.Model;
using test.Services;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly UploadFileService _uploadFileService;
        private readonly ILogger<UploadFileController> _logger;
        private readonly IImportCsvFile _importCsvFile;
        public UploadFileController(UploadFileService uploadFileService, ILogger<UploadFileController> logger, IImportCsvFile importCsvFile)
        {
            _uploadFileService = uploadFileService;
            _logger = logger;
            _importCsvFile = importCsvFile;
        }

        [HttpPost]
        [Route(nameof(UploadLargeFile))]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadLargeFile()
        {
            try
            {
                var uploadedFile = await _uploadFileService.Upload(HttpContext.Request);
                BackgroundJob.Enqueue(() => _importCsvFile.Import(uploadedFile));
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
