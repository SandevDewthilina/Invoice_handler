using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class GatewayApiController : Controller
    {
        private readonly HRMSDbContext _db;

        public GatewayApiController(HRMSDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUploadList()
        {
            // get uploads
            var uploads = await _db.Upload.Include(u => u.Supplier)
                .Select(u => new {
                    id = u.ID,
                    file_name = u.FileName,
                    supplier_name = u.Supplier.Name,
                    upload_date = u.UploadedDate.ToString("MM/dd/yyyy"),
                    document_name = u.DocumentName,
                    template_name = _db.SupplierTemplateAssignment.FirstOrDefault(a => a.SupplierID == u.SupplierID).Template.Name
                }).ToListAsync();
            
            return Json(new
            {
                success = true,
                data = uploads
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadDataForUploadId(int id)
        {
            try
            {
                var results = await _db.UploadData.FirstOrDefaultAsync(u => u.UploadID == id);
                return Json(results);
            }
            catch (Exception e)
            {
                return Json(new {success = false, error = e.Message});
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLastUploadDataForSync()
        {
            try
            {
                var results = await _db.UploadData
                    .Where(i => !string.IsNullOrEmpty(i.FieldJson) && !string.IsNullOrEmpty(i.TableJson))
                    .Include(i => i.DetectedTemplate)
                    .OrderByDescending(u => u.ID)
                    .Take(1000)
                    .Select(i => new UploadDataWithTemplate()
                    {
                        ID = i.ID,
                        DocType = i.DetectedTemplate.TemplateType,
                        FieldJson = i.FieldJson,
                        TableJson = i.TableJson,
                        UploadID = i.UploadID,
                        DetectedTemplateID = i.DetectedTemplateID
                    })
                    .ToListAsync();
                return Json(results);
            }
            catch (Exception e)
            {
                return Json(new {success = false, error = e.Message});
            }
        }
        
        public class UploadDataWithTemplate : UploadData
        {
            public string DocType { get; set; }
        }
    }
}