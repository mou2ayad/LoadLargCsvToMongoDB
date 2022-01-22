using System.Collections.Generic;

namespace test.Config
{
    public class UploadFileConfig
    {
        public string StoredFilesPath { get; set; }
        public List<string> PermittedExtensions { get; set; }
    }
}