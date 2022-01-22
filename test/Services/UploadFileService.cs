using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using test.Config;

namespace test.Services
{
    public class UploadFileService
    {
        private readonly UploadFileConfig _config;

        public UploadFileService(IOptions<UploadFileConfig> options)
        {
            _config = options.Value;
        }
        public async Task Upload(HttpRequest request)
        {
            var mediaTypeHeader = ExtractMediaTypeHeader(request);

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    AssertFileExtension(contentDisposition.FileName.Value);
                    var saveToPath = Path.Combine(_config.StoredFilesPath, contentDisposition.FileName.Value);

                    await using var targetStream = File.Create(saveToPath);
                    await section.Body.CopyToAsync(targetStream);
                    return;
                }

                section = await reader.ReadNextSectionAsync();
            }

            throw new BadHttpRequestException("No files data in the request.");

        }

        private static MediaTypeHeaderValue ExtractMediaTypeHeader(HttpRequest request)
        {
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
                throw new UnsupportedContentTypeException("this media type in not supported");
            return mediaTypeHeader;
        }

        private void AssertFileExtension(string uploadedFileName)
        {
            var ext = Path.GetExtension(uploadedFileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || ! _config.PermittedExtensions.Contains(ext))
                throw new BadHttpRequestException("The extension is invalid");
        }
    }

}
