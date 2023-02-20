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

        public InvoiceController(IStorageRepository storageRepository, HRMSDbContext db, IScraper scraper, IServiceScopeFactory serviceScopeFactory)
        {
            _storageRepository = storageRepository;
            _db = db;
            _scraper = scraper;
            _serviceScopeFactory = serviceScopeFactory;
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
                var results = await ProcessUploadForId(upload.ID);
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

        public async Task<IActionResult> ProcessUploadForId(int Id)
        {
            // pdf content
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            var supplierRegexList = await _db.RegexComponent.Where(rc => rc.Key.Equals("Supplier")).ToListAsync();

            foreach (var regexComponent in supplierRegexList)
            {
                var results = _scraper.Scrape(upload.FilePath, new List<RegexComponent>() {regexComponent});
                if (results.Count > 0)
                {
                    // get the supplier owner for the template that owns regexCompenent
                    var assignment = await _db.SupplierTemplateAssignment
                        .FirstOrDefaultAsync(a => a.TemplateID == regexComponent.TemplateID);
                    upload.SupplierID = assignment.SupplierID;
                    _db.Upload.Update(upload);
                    await _db.SaveChangesAsync();

                    // fetch fieldData from Regex
                    var fieldList = _scraper.Scrape(upload.FilePath, await _db.RegexComponent
                        .Where(c => c.TemplateID == assignment.TemplateID)
                        .ToListAsync());
                    var fieldJson = JsonConvert.SerializeObject(fieldList);

                    // fetch tableData from backend by a background thread
                    var task = Task.Run(async () =>
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var body = new
                            {
                                url = "http://localhost:8100/" + upload.FilePath,
                                filename = upload.FileName,
                                upload_name = upload.ID
                            };
                            var json = JsonConvert.SerializeObject(body);
                            using (var client = new HttpClient())
                            {
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await client.PostAsync("http://localhost:5000/ExtractData", content);
                                response.EnsureSuccessStatusCode();
                                var tableJson = await response.Content.ReadAsStringAsync();
                                var db = scope.ServiceProvider.GetService<HRMSDbContext>();
                                var ud = await db.UploadData.FirstOrDefaultAsync(ud => ud.UploadID == upload.ID);
                                ud.TableJson = tableJson;
                                db.UploadData.Update(ud);
                                await db.SaveChangesAsync();
                            }
                        }
                    });

                    // save detected templateId to uploadData
                    var uploadData = new UploadData()
                    {
                        UploadID = upload.ID,
                        DetectedTemplateID = assignment.TemplateID,
                        FieldJson = fieldJson
                    };
                    await _db.UploadData.AddAsync(uploadData);
                    await _db.SaveChangesAsync();

                    break;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ProcessUploadForIdWithoutSupplierDetection(int Id)
        {
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            // fetch tableData from backend by a background thread
            var task = Task.Run(async () =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var body = new
                    {
                        url = "http://localhost:8100/" + upload.FilePath,
                        filename = upload.FileName,
                        upload_name = upload.ID
                    };
                    var json = JsonConvert.SerializeObject(body);
                    using (var client = new HttpClient())
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("http://localhost:5000/ExtractData", content);
                        response.EnsureSuccessStatusCode();
                        var tableJson = await response.Content.ReadAsStringAsync();
                        var db = scope.ServiceProvider.GetService<HRMSDbContext>();

                        var ud = new UploadData()
                        {
                            UploadID = upload.ID,
                            TableJson = tableJson
                        };
                        await db.UploadData.AddAsync(ud);
                        await db.SaveChangesAsync();
                    }
                }
            });

            return RedirectToAction("Index", "Home");
        }
    }
}