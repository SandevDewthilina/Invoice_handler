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
    public class ExportController : Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly HRMSDbContext _db;
        private readonly IScraper _scraper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITableExtractor _tableExtractor;

        public ExportController(
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


        public async Task<IActionResult> ExportData()
        {
            return View(await _db.Template.ToListAsync());
        }

        public async Task<IActionResult> ExportDataForTemplate(int id)
        {
            // Build the CSV content
            var csvContent = await BuildCsvContent(id);

            // Set the content type and headers for the response
            var contentType = "text/csv";
            var fileName = "export.csv";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");

            // Return the CSV content as a FileResult
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), contentType);
        }

        private async Task<string> BuildCsvContent(int templateId)
        {
            var template = await _db.Template.FindAsync(templateId);

            var regexComponents = await _db.RegexComponent
                .Where(rc => rc.TemplateID == templateId)
                .OrderBy(rc => rc.ID)
                .Select(rc => rc.Key)
                .ToListAsync();

            var keyMap = new Dictionary<string, string>();

            foreach (var key in regexComponents)
            {
                keyMap[key] = "";
            }

            var uploadDatas = await _db.UploadData
                .Where(ud => ud.DetectedTemplateID == templateId && !string.IsNullOrEmpty(ud.FieldJson))
                .ToListAsync();

            var sb = new StringBuilder();

            // Append header row
            sb.AppendLine(string.Join(",", keyMap.Keys));

            // Append data rows
            foreach (UploadData uploadData in uploadDatas)
            {
                var keyValuePairs = JsonConvert.DeserializeObject<List<KeyValue>>(uploadData.FieldJson);
                var tempMap = new Dictionary<string, string>(keyMap);
                foreach (var key in keyMap.Keys)
                {
                    tempMap[key] = keyValuePairs?.FirstOrDefault(p => p.Key.Equals(key))?.Value;
                }

                sb.AppendLine(string.Join(",", tempMap.Values).Replace("\n", "").Replace("\r", "")); // Replace with your actual property names
            }

            return sb.ToString();
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}