using test.Model;

namespace test.Bus
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
