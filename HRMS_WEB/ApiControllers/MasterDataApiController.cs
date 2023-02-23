using System;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        [HttpGet]
        public async Task<IActionResult> GetTemplateForId(int Id)
        {
            var template = await _db.Template.FirstOrDefaultAsync(t => t.ID == Id);
            var obj = new
            {
                form = new
                {
                    template_name = template.Name,
                    templateRegexList = await _db.RegexComponent.Where(r => r.TemplateID == template.ID)
                        .Select(r => new
                        {
                            id = r.ID,
                            Key = r.Key,
                            value = r.Value,
                            area = r.Area,
                            isArea = r.IsArea
                        }).ToListAsync()
                }
            };
            return Json(new {success = true, data = obj});
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
                    Area = regexItem.area,
                    IsArea = regexItem.isArea,
                    TemplateID = template.ID
                };
                await _db.RegexComponent.AddAsync(regexComponent);
            }

            await _db.SaveChangesAsync();
            return Json(new {success = true});
        }

        [HttpPost]
        public async Task<IActionResult> EditTemplate(int Id, CreateTemplateModel model)
        {
            var template = await _db.Template.FirstOrDefaultAsync(t => t.ID == Id);
            template.Name = model.template_name;
            _db.Template.Update(template);
            await _db.SaveChangesAsync();

            //delete previous assignements
            _db.RegexComponent.RemoveRange(await _db.RegexComponent.Where(r => r.TemplateID == Id).ToListAsync());
            await _db.SaveChangesAsync();

            foreach (var regexItem in model.templateRegexList)
            {
                var regexComponent = new RegexComponent()
                {
                    Key = regexItem.key,
                    Value = regexItem.value,
                    Area = regexItem.area,
                    IsArea = regexItem.isArea,
                    TemplateID = template.ID
                };
                await _db.RegexComponent.AddAsync(regexComponent);
            }

            await _db.SaveChangesAsync();
            return Json(new {success = true});
        }
    }
}