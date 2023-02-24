using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS_WEB.Entities;

namespace HRMS_WEB.Repositories
{
    public interface ITableExtractor
    {
        Task<List<object>> ExtractTables(Upload upload, int templateId);
    }
}