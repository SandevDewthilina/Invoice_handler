using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HRMS_WEB.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace HRMS_WEB.Controllers
{

    public class InvoiceController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InvoiceController(IWebHostEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }
        public IActionResult ScrapePdf()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadViewModel viewModel)
        {
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "invoices");
                var filepath = Path.Combine(folderPath, file.FileName);
                await using Stream fileStream = new FileStream(filepath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }

            return Ok();
        }
    }
}