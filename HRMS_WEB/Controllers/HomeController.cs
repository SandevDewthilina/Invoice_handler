using HRMS_WEB.DbContext;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HRMSDbContext db;

        public HomeController(ILogger<HomeController> logger, HRMSDbContext db)
        {
            _logger = logger;
            this.db = db;
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
        public IActionResult CreateSupplier()
        {
            return View(new SupplierViewModel());
        }

        [HttpPost]
        public IActionResult CreateSupplier(SupplierViewModel model)
        {
            return RedirectToAction("ViewSuppliers");
        }
        public IActionResult EditSupplier(int Id)
        {
            return View(new SupplierViewModel()
            {
                Id = Id,
                Name = "beedle bard",
                SelectedIdList = new int[]{1}
            });
        }

        [HttpPost]
        public IActionResult EditSupplier(SupplierViewModel model)
        {
            return RedirectToAction("ViewSuppliers");
        }

        public IActionResult ViewSuppliers()
        {
            return View();
        }

        public IActionResult DeleteSupplier(int Id)
        {
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
            model.Content = model.Content.Replace("\r", "");
            return RedirectToAction("ViewTemplates");
        }
        public IActionResult EditTemplate(int Id)
        {
            return View(new TemplateViewModel()
            {
                Id = Id,
                Name = "sandev dewthilina",
                Content = "$kfdkfdskjk&&&fisdfi"
            });
        }

        [HttpPost]
        public IActionResult EditTemplate(TemplateViewModel model)
        {
            return RedirectToAction("ViewSuppliers");
        }
        public IActionResult DeleteTemplate(int Id)
        {
            return RedirectToAction("ViewTemplates");
        }

    }
}