using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
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
        public IActionResult GetFileUploads()
        {
            return Json(new
            {
                success = true,
                data = _db.Upload.Select(u => new
                {
                    id = u.ID,
                    file_name = u.FileName,
                    supplier_name = u.Supplier.Name,
                    upload_date = u.UploadedDate.ToString("MM/dd/yyyy"),

                })
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
        
    }
}