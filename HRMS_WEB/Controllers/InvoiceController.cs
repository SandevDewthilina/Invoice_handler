using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using HRMS_WEB.Repositories;
using HRMS_WEB.Viewmodels;
using HRMS_WEB.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace HRMS_WEB.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly HRMSDbContext _db;

        public InvoiceController(IStorageRepository storageRepository, HRMSDbContext db)
        {
            _storageRepository = storageRepository;
            _db = db;
        }

        public IActionResult ExtractDataFromPdf()
        {
            return View();
        }

        public async Task<IActionResult> Upload()
        {
            return View(new ImageUploadViewModel()
            {
                SupplierList = await _db.Supplier.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadViewModel viewModel)
        {
            if (viewModel.SupplierID == 0)
            {
                throw new Exception("No matching supplier");
            }

            var files = Request.Form.Files;
            foreach (var file in files)
            {
                await _storageRepository.SaveFile(file);
                var upload = new Upload()
                {
                    FileName = file.FileName,
                    FilePath = $"/Invoice/{file.FileName}",
                    UploadedDate = DateTime.Now,
                    SupplierID = viewModel.SupplierID
                };
                await _db.Upload.AddAsync(upload);
            }

            await _db.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> EditUpload(int Id)
        {
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            return View(new UploadViewModel()
            {
                ID = Id,
                FileName = upload.FileName,
                FilePath = upload.FilePath,
                SupplierList = await _db.Supplier.ToListAsync(),
                SupplierID = upload.SupplierID,
                UploadedDate = upload.UploadedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUpload(UploadViewModel model)
        {
            var upload = new Upload()
            {
                ID = model.ID,
                FileName = model.FileName,
                FilePath = model.FilePath,
                UploadedDate = model.UploadedDate,
                SupplierID = model.SupplierID
            };
            _db.Upload.Update(upload);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}