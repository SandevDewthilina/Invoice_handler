using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UploadsApiController : Controller
    {
        // database instance
        private readonly HRMSDbContext _db;

        public UploadsApiController(HRMSDbContext db)
        {
            _db = db;
        }
        
        // return file upload list
        [HttpGet]
        public async Task<IActionResult> GetFileUploads()
        {
            
            // get uploads
            var uploads = await _db.Upload.Include(u =>u.Supplier).ToListAsync();
            foreach (Upload upload in uploads)
            {
                if (upload.SupplierID != null)
                {
                    continue;
                }
                // check for any detections
                var detectedTemplateIds = await _db.UploadData.Where(ud => ud.UploadID == upload.ID).Select(ud => (int)ud.DetectedTemplateID).ToListAsync();
                if (detectedTemplateIds.Count > 0)
                {
                    // get the first choice
                    var suggestedTemplateId = detectedTemplateIds[0];
                    var assignedSupplierIdForTemplate = await _db.SupplierTemplateAssignment.Include(a => a.Supplier).FirstOrDefaultAsync(a => a.TemplateID == suggestedTemplateId);
                    if (assignedSupplierIdForTemplate != null)
                    {
                        upload.SupplierID = assignedSupplierIdForTemplate.SupplierID;
                        upload.Supplier = assignedSupplierIdForTemplate.Supplier;
                    }
                }
            }

            var list = uploads.Select(u => new
            {
                id = u.ID,
                file_name = u.FileName,
                supplier_name = u.Supplier?.Name,
                upload_date = u.UploadedDate.ToString("MM/dd/yyyy")
            });
            return Json(new
            {
                success = true,
                data = list
            });
        }

        [HttpGet]
        public IActionResult GetUrlForUpload(int Id)
        {
            return Json(new
            {
                success = true,
                data = _db.Upload.FirstOrDefault(u => u.ID == Id).FilePath
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadForId(int Id)
        {
            return Json(new
            {
                success = true,
                data = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id)
            });
        }

        public async Task<IActionResult> GetTemplatesForUpload(int Id)
        {
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            var templates = await _db.SupplierTemplateAssignment.Include(a => a.Template)
                .Where(a => a.SupplierID == upload.SupplierID).Select(u => u.Template).ToListAsync();
            return Json(new
            {
                success = true,
                data = templates
            });
        }

        public async Task<IActionResult> GetUploadDataForUploadId(int Id)
        {
            var uploadData = await _db.UploadData.FirstOrDefaultAsync(ud => ud.UploadID == Id);
            return Json(new {success = true, data = uploadData});
        }
    }
}