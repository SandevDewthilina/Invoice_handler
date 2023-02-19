using System.Collections.Generic;
using HRMS_WEB.Models;

namespace HRMS_WEB.Repositories
{
    public interface IScraper
    {
        object Scrape(string filepath, List<RegexComponent> regexComponents);
    }
}