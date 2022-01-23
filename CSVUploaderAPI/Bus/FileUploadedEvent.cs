using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Model;

namespace CSVUploaderAPI.Bus
{
    public class FileUploadedEvent :IDomainEvent<FileUploadedEvent>
    {
        public UploadedFileInfo UploadedFile { set; get; }

        public FileUploadedEvent(UploadedFileInfo file)
        {
            UploadedFile = file;
        }
        public FileUploadedEvent GetEvent() => this;
    }
}
