using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Repositories;
using HRMS_WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class ExternalApiController : Controller
    {
        // database instance
        private readonly HRMSDbContext _db;
        private readonly IStorageRepository _storageRepository;
        private readonly IScraper _scraper;
        private readonly ITableExtractor _tableExtractor;

        public ExternalApiController(
            HRMSDbContext db, 
            IStorageRepository storageRepository,
            IScraper scraper,
            ITableExtractor tableExtractor)
        {
            _db = db;
            _storageRepository = storageRepository;
            _scraper = scraper;
            _tableExtractor = tableExtractor;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ExternalUploadModel model)
        {
            try
            {
                var files = new List<IFormFile>();
                foreach (var url in model.Urls)
                {
                    files.Add(await DownloadPdfAsync(url));
                }

                var supplier = await _db.Supplier.FirstOrDefaultAsync(s => s.Code.Equals(model.SupplierCode));
                var SupplierId = supplier.ID;

                var uploadDataList = new List<UploadData>();

                foreach (var file in files)
                {
                    await _storageRepository.SaveFile(file);
                    var upload = new Upload()
                    {
                        FileName = file.FileName,
                        FilePath = Path.Combine("Invoices", file.FileName),
                        UploadedDate = DateTime.Now,
                        SupplierID = SupplierId
                    };
                    await _db.Upload.AddAsync(upload);
                    await _db.SaveChangesAsync();
                    uploadDataList.Add(await ProcessUploadForIdSync(upload.ID, SupplierId));
                }

                return Json(new
                {
                    success = true,
                    data = uploadDataList
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    error = e.Message
                });
            }
        }
        
        public async Task<UploadData> ProcessUploadForIdSync(int uploadId, int supplierId)
        {
            // pdf content
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == uploadId);
            
            // get the regex list for the invoice template of supplier
            // suppose there is only one template for supplier
            var templateForSupplier = await _db.SupplierTemplateAssignment.Where(a => a.SupplierID == supplierId).Select(a => a.Template).FirstOrDefaultAsync();
            var fullRegexListForMatchingTemplate = await _db.RegexComponent.Where(rc => rc.TemplateID == templateForSupplier.ID).ToListAsync();
           
            // fetch fields and save to db
            var fieldDataList = await _scraper.Scrape(upload.FilePath, fullRegexListForMatchingTemplate, upload);
            // fetch tables and save to db using a job
            var tableDataList = await _tableExtractor.ExtractTables(upload, templateForSupplier.ID);
            
            var uploadData = new UploadData();
            
            if (await _db.UploadData.AnyAsync(ud => ud.DetectedTemplateID == templateForSupplier.ID && ud.UploadID == upload.ID))
            {
                uploadData = await _db.UploadData.FirstOrDefaultAsync(ud => ud.DetectedTemplateID == templateForSupplier.ID && ud.UploadID == upload.ID);
                uploadData.FieldJson = JsonConvert.SerializeObject(fieldDataList);
                uploadData.TableJson = JsonConvert.SerializeObject(tableDataList);
                _db.UploadData.Update(uploadData);
            }
            else
            {
                uploadData = new UploadData()
                {
                    DetectedTemplateID = templateForSupplier.ID,
                    UploadID = upload.ID,
                    FieldJson = JsonConvert.SerializeObject(fieldDataList),
                    TableJson = JsonConvert.SerializeObject(tableDataList)
                };
                await _db.UploadData.AddAsync(uploadData);
            }

            await _db.SaveChangesAsync();
            return uploadData;
        }

        public async Task<IFormFile> DownloadPdfAsync(string url)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url);
            var fileName = GetFileName(response);
            await using var stream = await response.Content.ReadAsStreamAsync();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return new FormFile(memoryStream, 0, memoryStream.Length, fileName, fileName);
        }
        
        
        private string GetFileName(HttpResponseMessage response)
        {
            return Guid.NewGuid() + ".pdf";
        }
    }

    public class ExternalUploadModel
    {
        public string[] Urls { get; set; }
        public string SupplierCode { get; set; }
    }
}