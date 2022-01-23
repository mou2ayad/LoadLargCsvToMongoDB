using System.Threading.Tasks;
using test.Model;

namespace test.Contract
{
    public interface IDbRepository
    {
        Task Insert(string fromFile, params Clothe[] clothes);
    }
}
