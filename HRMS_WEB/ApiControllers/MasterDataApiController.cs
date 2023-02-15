using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MasterDataApiController : Controller
    {
        private readonly HRMSDbContext _db;

        public MasterDataApiController(HRMSDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        public IActionResult GetSuppliers()
        {
            return Json(new
            {
                success = true, data = _db.Supplier.Select(s => new
                {
                    id = s.ID,
                    name = s.Name
                })
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            return Json(new
            {
                success = true, data = await _db.Template.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate(CreateTemplateModel model)
        {
            var template = new Template()
            {
                Name = model.template_name
            };
            await _db.Template.AddAsync(template);
            await _db.SaveChangesAsync();

            foreach (var regexItem in model.templateRegexList)
            {
                var regexComponent = new RegexComponent()
                {
                    Key = regexItem.key,
                    Value = regexItem.value,
                    TemplateID = template.ID
                };
                await _db.RegexComponent.AddAsync(regexComponent);
            }

            await _db.SaveChangesAsync();
            return Json(new {success = true});
        }
    }
}