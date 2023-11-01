using System.IO;
using System.Threading.Tasks;

namespace Bookstore.Domain
{
    public interface IImageValidationService
    {
        Task<bool> IsSafeAsync(Stream image);
    }
}