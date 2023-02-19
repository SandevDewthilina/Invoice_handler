using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ScraperApiController:Controller
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
            var results = _scraper.Scrape(upload.FilePath, regexComponents);
            return Json(new
            {
                success = true,
                data = results
            });
        }
    }
}