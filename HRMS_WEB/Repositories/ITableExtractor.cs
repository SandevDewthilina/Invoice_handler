using HRMS_WEB.Entities;

namespace HRMS_WEB.Repositories
{
    public interface ITableExtractor
    {
        void ExtractTables(Upload upload);
    }
}