using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;

namespace HRMS_WEB.Repositories
{
    public interface IScraper
    {
        Task<List<List<object>>> Scrape(string filepath, List<RegexComponent> regexComponents, Upload upload);
        object GetCapturedGroup(string content, string pattern, string groupName);
    }
}