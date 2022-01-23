using CSVUploaderAPI.Model;

namespace CSVUploaderAPI.Bus
{
    public class FileUploadedEvent
    {
        public UploadedFileInfo UploadedFile { set; get; }

        public FileUploadedEvent(UploadedFileInfo file)
        {
            UploadedFile = file;
        }

    }
}
