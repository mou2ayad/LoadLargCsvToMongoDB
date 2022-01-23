using System.Threading.Tasks;
using test.Model;

namespace test.Contract
{
    public interface IClothesRepository
    {
        Task Insert(string fromFile, params Clothe[] clothes);
    }
}
