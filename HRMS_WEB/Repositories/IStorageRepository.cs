using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRMS_WEB.Repositories
{
    public interface IStorageRepository
    {
        Task SaveFile(IFormFile file);
        Task DeleteUpload(int Id);
    }
}