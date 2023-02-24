using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Repositories;
using HRMS_WEB.Viewmodels;
using HRMS_WEB.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;

namespace HRMS_WEB.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly HRMSDbContext _db;
        private readonly IScraper _scraper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITableExtractor _tableExtractor;

        public InvoiceController(
            IStorageRepository storageRepository,
            HRMSDbContext db,
            IScraper scraper,
            IServiceScopeFactory serviceScopeFactory,
            ITableExtractor tableExtractor
        )
        {
            _storageRepository = storageRepository;
            _db = db;
            _scraper = scraper;
            _serviceScopeFactory = serviceScopeFactory;
            _tableExtractor = tableExtractor;
        }

        public IActionResult ExtractDataFromPdf(int Id)
        {
            ViewBag.Id = Id;
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
                await _storageRepository.SaveFile(file);
                var upload = new Upload()
                {
                    FileName = file.FileName,
                    FilePath = Path.Combine("Invoices", file.FileName),
                    UploadedDate = DateTime.Now,
                    SupplierID = null
                };
                await _db.Upload.AddAsync(upload);
                await _db.SaveChangesAsync();
                await ProcessUploadForId(upload.ID);
            }


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

        public async Task<IActionResult> DeleteUpload(int Id)
        {
            await _storageRepository.DeleteUpload(Id);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ProcessUploads()
        {
            throw new Exception("Not completed page");
            return View();
        }

        public async Task ProcessUploadForId(int Id)
        {
            // pdf content
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            var supplierRegexList = await _db.RegexComponent.Where(rc => rc.Key.Equals("Supplier")).ToListAsync();

            
            var task = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                // get the table components for the template
                var db = scope.ServiceProvider.GetService<HRMSDbContext>();
                foreach (var regexComponent in supplierRegexList)
                {
                    var results = await _scraper.Scrape(upload.FilePath, new List<RegexComponent>() {regexComponent}, upload);
                    if (results.Count > 0)
                    {
                        var fullRegexListForMatchingTemplate = await db.RegexComponent.Where(rc => rc.TemplateID == regexComponent.TemplateID).ToListAsync();
                        // fetch fields and save to db
                        var fieldDataList = await _scraper.Scrape(upload.FilePath, fullRegexListForMatchingTemplate, upload);
                        // fetch tables and save to db using a job
                        var tableDataList = await _tableExtractor.ExtractTables(upload, regexComponent.TemplateID);
                        var uploadData = new UploadData();
                        if (await db.UploadData.AnyAsync(ud => ud.DetectedTemplateID == regexComponent.TemplateID && ud.UploadID == upload.ID))
                        {
                            uploadData = await db.UploadData.FirstOrDefaultAsync(ud => ud.DetectedTemplateID == regexComponent.TemplateID && ud.UploadID == upload.ID);
                            uploadData.FieldJson = JsonConvert.SerializeObject(fieldDataList);
                            uploadData.TableJson = JsonConvert.SerializeObject(tableDataList);
                            db.UploadData.Update(uploadData);
                        }
                        else
                        {
                            uploadData = new UploadData()
                            {
                                DetectedTemplateID = regexComponent.TemplateID,
                                UploadID = upload.ID,
                                FieldJson = JsonConvert.SerializeObject(fieldDataList),
                                TableJson = JsonConvert.SerializeObject(tableDataList)
                            };
                            await db.UploadData.AddAsync(uploadData);
                        }

                        await db.SaveChangesAsync();
                        break;
                    }
                }
            });
        }

        public async Task<IActionResult> ProcessUploadForIdWithoutSupplierDetection(int Id)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}