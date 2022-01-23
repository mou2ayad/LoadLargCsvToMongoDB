using System;

namespace CSVUploaderAPI.Contract
{
    public interface IFileRepository :IDisposable
    {
        void OpenFile(string filePath);
        void Write(object ob);
        void CloseFile();
    }
}
