using System.Collections.Generic;

namespace CSVUploaderAPI.Config
{
    public class UploadFileConfig
    {
        public string StoredFilesPath { get; set; }
        public List<string> PermittedExtensions { get; set; }
    }
}