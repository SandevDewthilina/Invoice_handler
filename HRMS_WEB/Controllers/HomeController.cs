using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HRMSDbContext _db;

        public HomeController(ILogger<HomeController> logger, HRMSDbContext db)
        {
            _logger = logger;
            this._db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /*
         * supplier 
         */
        public async Task<IActionResult> CreateSupplier()
        {
            return View(new SupplierViewModel()
            {
                Templates = await _db.Template.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierViewModel model)
        {
            var supplier = new Supplier()
            {
                Name = model.Name,
                Code = model.Code
            };
            await _db.Supplier.AddAsync(supplier);
            await _db.SaveChangesAsync();
            
            // create new assignments
            foreach (int tempId in model.NewlySelectedIdList)
            {
                var assignement = new SupplierTemplateAssignment()
                {
                    SupplierID = supplier.ID,
                    TemplateID = tempId
                };
                await _db.SupplierTemplateAssignment.AddAsync(assignement);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("ViewSuppliers");
        }

        public async Task<IActionResult> EditSupplier(int Id)
        {
            var supplier = await _db.Supplier.FirstOrDefaultAsync(s => s.ID == Id);
            var supplierTemplates = await _db.SupplierTemplateAssignment.Where(a => a.SupplierID == supplier.ID).Select(a => a.Template.ID).ToArrayAsync();
            return View(new SupplierViewModel()
            {
                Id = supplier.ID,
                Name = supplier.Name,
                Code = supplier.Code,
                AlreadySelectedIdList = supplierTemplates,
                Templates = await _db.Template.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditSupplier(SupplierViewModel model)
        {
            var supplier = await _db.Supplier.FirstOrDefaultAsync(s => s.ID == model.Id);
            supplier.Name = model.Name;
            supplier.Code = model.Code;
            _db.Supplier.Update(supplier);

            // drop already assigned templates
            var alreadyAssignedTemplates = _db.SupplierTemplateAssignment.Where(a => a.SupplierID == model.Id);
            _db.SupplierTemplateAssignment.RemoveRange(alreadyAssignedTemplates);

            if (model.NewlySelectedIdList != null )
            {
                // create new assignments
                foreach (int tempId in model.NewlySelectedIdList)
                {
                    var assignement = new SupplierTemplateAssignment()
                    {
                        SupplierID = model.Id,
                        TemplateID = tempId
                    };
                    await _db.SupplierTemplateAssignment.AddAsync(assignement);
                }   
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("ViewSuppliers");
        }

        public IActionResult ViewSuppliers()
        {
            return View();
        }

        public async Task<IActionResult> DeleteSupplier(int Id)
        {
            var supplier = await _db.Supplier.FirstOrDefaultAsync(s => s.ID == Id);
            _db.Supplier.Remove(supplier);
            await _db.SaveChangesAsync();
            return RedirectToAction("ViewSuppliers");
        }

        /*
         * templates
         */
        public IActionResult ViewTemplates()
        {
            return View();
        }

        public IActionResult CreateTemplate()
        {
            return View(new TemplateViewModel());
        }

        [HttpPost]
        public IActionResult CreateTemplate(TemplateViewModel model)
        {
            return RedirectToAction("ViewTemplates");
        }

        public IActionResult EditTemplate(int Id)
        {
            return View();
        }

        public async Task<IActionResult> DeleteTemplate(int Id)
        {
            var template = await _db.Template.FirstOrDefaultAsync(s => s.ID == Id);
            _db.Template.Remove(template);
            await _db.SaveChangesAsync();
            return RedirectToAction("ViewTemplates");
        }
    }
}