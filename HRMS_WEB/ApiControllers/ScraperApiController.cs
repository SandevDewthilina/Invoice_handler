using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Repositories;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScraperApiController : Controller
    {
        private readonly HRMSDbContext _db;
        private readonly IScraper _scraper;

        public ScraperApiController(HRMSDbContext db, IScraper scraper)
        {
            _db = db;
            _scraper = scraper;
        }

        public async Task<IActionResult> ScrapeUploadForTemplate(int uploadId, int templateId)
        {
            // get the template
            var template = await _db.Template.FirstOrDefaultAsync(t => t.ID == templateId);
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == uploadId);
            var regexComponents = await _db.RegexComponent.Where(c => c.TemplateID == templateId).ToListAsync();
            var results = await _scraper.Scrape(upload.FilePath, regexComponents, upload);
            return Json(new
            {
                success = true,
                data = results
            });
        }

        [HttpPost]
        public async Task<IActionResult> ScrapeTableOfPdf(ScrapeBody body)
        {
            var json = JsonConvert.SerializeObject(body);
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:5000/ExtractData", content);
                response.EnsureSuccessStatusCode();
                return Json(await response.Content.ReadAsStringAsync());
            }
        }
    }

    public class ScrapeBody
    {
        public string url { get; set; }
        public string filename { get; set; }
        public int upload_name { get; set; }
    }

}