namespace HRMS_WEB.Repositories
{
    public interface IScraper
    {
        string Scrape(string input, string regex);
    }
}