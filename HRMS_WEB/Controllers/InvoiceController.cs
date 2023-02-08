using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult CreateSupplier()
        {
            return View();
        }

        public IActionResult CreateTemplate()
        {
            return View();
        }
    }
}