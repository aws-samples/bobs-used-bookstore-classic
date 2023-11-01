using System.IO;
using System.Threading.Tasks;

namespace Bookstore.Domain
{
    public interface IImageResizeService
    {
        Task<Stream> ResizeImageAsync(Stream image);
    }
}
