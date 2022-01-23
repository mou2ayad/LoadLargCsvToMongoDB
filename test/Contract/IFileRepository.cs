using System;

namespace test.Contract
{
    public interface IFileRepository :IDisposable
    {
        void OpenFile(string filePath);
        void Write(object ob);
        void CloseFile();
    }
}
