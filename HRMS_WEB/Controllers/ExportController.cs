using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
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

            // Create a new MemoryStream to hold the CSV data
            var memoryStream = new MemoryStream();

            // Create a new StreamWriter using the memory stream and UTF-8 encoding
            var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);

            // Create a new CsvWriter using the StreamWriter
            var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            // Write the data to the CSV file

            foreach (var row in csvContent)
            {
                foreach (var cell in row)
                {
                    csvWriter.WriteField(cell);
                }

                await csvWriter.NextRecordAsync();
            }
            
            
            // Flush the CsvWriter and StreamWriter
            await csvWriter.FlushAsync();
            await streamWriter.FlushAsync();

            // Reset the position of the MemoryStream to the beginning
            memoryStream.Position = 0;

            // Return the CSV file as a FileStreamResult
            return File(memoryStream, "text/csv", "data.csv");
        }

        private async Task<List<string[]>> BuildCsvContent(int templateId)
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

            List<string[]> records = new List<string[]>();

            // Append header row
            records.Add(keyMap.Keys.ToArray());

            // Append data rows
            foreach (UploadData uploadData in uploadDatas)
            {
                var keyValuePairs = JsonConvert.DeserializeObject<List<KeyValue>>(uploadData.FieldJson);
                var tempMap = new Dictionary<string, string>(keyMap);
                foreach (var key in keyMap.Keys)
                {
                    tempMap[key] = keyValuePairs?.FirstOrDefault(p => p.Key.Equals(key))?.Value.Replace("\n", "").Replace("\r", "");
                }

                records.Add(tempMap.Values.ToArray()); // Replace with your actual property names
            }

            return records;
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}