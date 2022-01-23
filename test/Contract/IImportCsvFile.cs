using test.Model;

namespace test.Contract
{
    public interface IImportCsvFile
    {
        void Import(UploadedFileInfo csvFile);
    }
}
