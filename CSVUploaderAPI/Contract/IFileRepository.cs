using System;
using System.Threading.Tasks;

namespace CSVUploaderAPI.Contract
{
    public interface IFileRepository :IDisposable
    {
        Task OpenFile(string filePath);
        Task Write(object ob);
        Task CloseFile();
    }
}
