using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRMS_WEB.Repositories
{
    public class OCRFieldExtractor : IFieldExtractor
    {
        private readonly HRMSDbContext _db;
        private readonly IScraper _scraper;

        public OCRFieldExtractor(HRMSDbContext db, IScraper scraper)
        {
            _db = db;
            _scraper = scraper;
        }
        public async Task ExtractFields(Upload upload, RegexComponent regexComponent)
        {
            // get the supplier owner for the template that owns regexCompenent
            var assignment = await _db.SupplierTemplateAssignment
                .FirstOrDefaultAsync(a => a.TemplateID == regexComponent.TemplateID);
            upload.SupplierID = assignment.SupplierID;
            _db.Upload.Update(upload);
            await _db.SaveChangesAsync();

            // fetch fieldData from Regex
            var fieldList = await _scraper.Scrape(upload.FilePath, await _db.RegexComponent
                .Where(c => c.TemplateID == assignment.TemplateID)
                .ToListAsync(), upload);
            var fieldJson = JsonConvert.SerializeObject(fieldList);

            // save detected templateId to uploadData
            var uploadData = new UploadData()
            {
                UploadID = upload.ID,
                DetectedTemplateID = assignment.TemplateID,
                FieldJson = fieldJson
            };
            await _db.UploadData.AddAsync(uploadData);
            await _db.SaveChangesAsync();
        }
    }
}