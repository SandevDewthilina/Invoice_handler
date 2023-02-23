using System.Threading.Tasks;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;

namespace HRMS_WEB.Repositories
{
    public interface IFieldExtractor
    {
        Task ExtractFields(Upload upload, RegexComponent regexComponent);
    }
}