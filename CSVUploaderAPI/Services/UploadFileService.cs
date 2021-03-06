using System;
using System.IO;
using System.Threading.Tasks;
using CSVUploaderAPI.Bus;
using CSVUploaderAPI.Config;
using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CSVUploaderAPI.Services
{
    public class UploadFileService
    {
        private readonly UploadFileConfig _config;
        private readonly IEventDispatcher<FileUploadedEvent> _dispatcher;
        public UploadFileService(IOptions<UploadFileConfig> options, IEventDispatcher<FileUploadedEvent> dispatcher)
        {
            _dispatcher = dispatcher;
            _config = options.Value;
        }

        public async Task Upload(HttpRequest request)
        {
            var mediaTypeHeader = ExtractMediaTypeHeader(request);
            var file = await StartUpload(mediaTypeHeader.Boundary.Value, request.Body);
             await _dispatcher.Dispatch(new FileUploadedEvent(file));
        }
        private async Task<UploadedFileInfo> StartUpload(string boundary, Stream body)
        {
            var reader = new MultipartReader(boundary, body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    AssertFileExtension(contentDisposition.FileName.Value);
                    FileInfo fileInfo = new FileInfo(contentDisposition.FileName.Value);
                    string tempFileName = Guid.NewGuid()+fileInfo.Extension;
                    var saveToPath = Path.Combine(_config.StoredFilesPath, tempFileName);
                    await using var targetStream = File.Create(saveToPath);
                    await section.Body.CopyToAsync(targetStream);
                    return new UploadedFileInfo
                        { FullTempName = saveToPath, OriginalName = contentDisposition.FileName.Value };
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
