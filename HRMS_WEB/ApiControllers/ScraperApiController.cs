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
using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> DetectAreaOfPdfUrl(DetectBody body)
        {
            var json = JsonConvert.SerializeObject(body);
            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:8200/checkDetectionArea", content);
            response.EnsureSuccessStatusCode();
            return File(await response.Content.ReadAsStreamAsync(), "image/png");
        }
    }

    public class DetectBody
    {
        public string file_url {get; set;}
        public string file_name {get; set;}
        public int upload_name {get; set;}
        public string table_areas {get; set;}
        public int edge_tol {get; set;}
        public int row_tol {get; set;}
        public string flavor {get; set;}
        public bool flag_size {get; set;}
        public int id {get; set;}
        public string page_no {get; set;}
        public bool split_text {get; set;}
        public string columns {get; set;}
    }
    
    public class ScrapeBody
    {
        public string url { get; set; }
        public string filename { get; set; }
        public int upload_name { get; set; }
    }

}