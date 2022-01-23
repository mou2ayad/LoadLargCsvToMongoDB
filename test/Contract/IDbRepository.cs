using System.Threading.Tasks;
using CSVUploaderAPI.Model;

namespace CSVUploaderAPI.Contract
{
    public interface IDbRepository
    {
        Task Insert(string fromFile, params Clothe[] clothes);
    }
}
