using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using HRMS_WEB.Repositories;
using HRMS_WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly HRMSDbContext _db;
        private readonly IScraper _scraper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITableExtractor _tableExtractor;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupplierController(IStorageRepository storageRepository,
            HRMSDbContext db,
            IScraper scraper,
            IServiceScopeFactory serviceScopeFactory,
            ITableExtractor tableExtractor,
            UserManager<ApplicationUser> userManager)
        {
            _storageRepository = storageRepository;
            _db = db;
            _scraper = scraper;
            _serviceScopeFactory = serviceScopeFactory;
            _tableExtractor = tableExtractor;
            _userManager = userManager;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upload()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            return View(new ImageUploadViewModel()
            {
                Templates = await _db.SupplierTemplateAssignment
                    .Include(a => a.Template)
                    .Where(a => a.Supplier.Name.Equals(user.Name))
                    .Select(a => a.Template)
                    .ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadViewModel viewModel)
        {
            var files = Request.Form.Files;
            var file = files.FirstOrDefault();
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

            // synchronous processing of invoice
            var uploadData = await ProcessUploadForIdSyncWithoutAutoDetect(upload.ID, viewModel.TemplateId);
            return Json(new
            {
                uploadId = uploadData.UploadID
            });
        }

        public async Task<IActionResult> AcceptOrRejectData(int uploadId)
        {
            return View(new UploadData() {UploadID = uploadId});
        }

        public async Task<UploadData> ProcessUploadForIdSyncWithoutAutoDetect(int Id, int TemplateID)
        {
            // pdf content
            var upload = await _db.Upload.FirstOrDefaultAsync(u => u.ID == Id);
            var fullRegexListForMatchingTemplate = await _db.RegexComponent.Where(rc => rc.TemplateID == TemplateID).ToListAsync();
            // fetch fields and save to db
            var fieldDataList = await _scraper.Scrape(upload.FilePath, fullRegexListForMatchingTemplate, upload);
            // fetch tables and save to db using a job
            var tableDataList = await _tableExtractor.ExtractTables(upload, TemplateID);
            var uploadData = new UploadData();
            if (await _db.UploadData.AnyAsync(ud => ud.DetectedTemplateID == TemplateID && ud.UploadID == upload.ID))
            {
                uploadData = await _db.UploadData.FirstOrDefaultAsync(ud => ud.DetectedTemplateID == TemplateID && ud.UploadID == upload.ID);
                uploadData.FieldJson = JsonConvert.SerializeObject(fieldDataList);
                uploadData.TableJson = JsonConvert.SerializeObject(tableDataList);
                _db.UploadData.Update(uploadData);
            }
            else
            {
                uploadData = new UploadData()
                {
                    DetectedTemplateID = TemplateID,
                    UploadID = upload.ID,
                    FieldJson = JsonConvert.SerializeObject(fieldDataList),
                    TableJson = JsonConvert.SerializeObject(tableDataList)
                };
                await _db.UploadData.AddAsync(uploadData);
            }

            await _db.SaveChangesAsync();
            return uploadData;
        }

        public async Task<IActionResult> ConfirmData(int uploadId, bool accept)
        {
            var uploadData = await _db.UploadData.FirstOrDefaultAsync(ud => ud.UploadID == uploadId);
            if (uploadData != null)
            {
                uploadData.SupplierConfirmed = accept;
                _db.UploadData.Update(uploadData);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}