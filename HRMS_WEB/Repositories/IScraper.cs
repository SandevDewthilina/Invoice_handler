using System.Collections.Generic;
using HRMS_WEB.Models;

namespace HRMS_WEB.Repositories
{
    public interface IScraper
    {
        List<object> Scrape(string filepath, List<RegexComponent> regexComponents);
        object GetCapturedGroup(string content, string pattern, string groupName);
    }
}