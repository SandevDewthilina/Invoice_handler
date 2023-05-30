using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using CsvHelper;
using HRMS_WEB.Entities;
using HRMS_WEB.Repositories;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HRMS_WEB.Controllers
{
    [Authorize]
    public class ExternalDataController : Controller
    {
        private readonly HRMSDbContext _db;
        private readonly IComparisonRepository _comparisonRepository;

        public ExternalDataController(HRMSDbContext db, IComparisonRepository comparisonRepository)
        {
            _db = db;
            _comparisonRepository = comparisonRepository;
        }

        public async Task<IActionResult> ExternalDataBatches()
        {
            // return View(await _db.ExternalData
            //     .Where(e => 
            //         _db.ExternalData.Any(ei => ei.ChassisNumber.Equals(e.ChassisNumber) && e.ID != ei.ID)).ToListAsync());
            return View(await _db.ExternalData.ToListAsync());
        }

        public async Task<IActionResult> ComparisonLogs()
        {
            return View(await _db.ComparisonLog
                .Include(l => l.FirstExternalData)
                .Include(l => l.SecondExternalData)
                .ToListAsync());
        }

        public async Task<IActionResult> DeleteExternalData(int Id)
        {
            var ed = await _db.ExternalData.FindAsync(Id);
            _db.ExternalData.Remove(ed);
            await _db.SaveChangesAsync();
            return RedirectToAction("ExternalDataBatches");
        }

        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (Path.GetExtension(file.FileName).Equals(".csv"))
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                try
                {
                    var excelUploadRecords = csv.GetRecords<ExcelUploadRecord>().ToList();
                    var uploadGroups = excelUploadRecords.GroupBy(r => r.ChassisNumber);
                    foreach (var chassisGroup in uploadGroups)
                    {
                        // chassis number
                        var chassisNumber = chassisGroup.Key;
                        var typeGroups = chassisGroup
                            .ToList()
                            .GroupBy(g => g.Source);

                        foreach (var typeGroup in typeGroups)
                        {
                            // data source type
                            var type = typeGroup.Key;
                            var filteredRecords = typeGroup.ToList();
                            
                            _db.ExternalData
                                .RemoveRange(_db.ExternalData
                                    .Where(e => e.ChassisNumber.Equals(chassisNumber) && e.Type.Equals(type))
                                );
                            var ed = new ExternalData()
                            {
                                Type = type,
                                ChassisNumber = chassisNumber
                            };
                            await _db.ExternalData.AddAsync(ed);
                            await _db.SaveChangesAsync();
                            
                            foreach (var record in filteredRecords)
                            {
                                await _db.ExternalDataPair.AddAsync(new ExternalDataPair()
                                {
                                    Key = record.Key,
                                    Value = record.Value,
                                    ExternalDataID = ed.ID
                                });
                            }
                            
                            await _db.SaveChangesAsync();
                            await _comparisonRepository.CompareExternalDataByPlan(ed);
                        }
                        
                    }

                    return RedirectToAction("ExternalDataBatches");

                }
                catch (HeaderValidationException e)
                {
                    return Json(e.Message);
                }
            }
            return Json("File Error");
        }
    }

    public class ExcelUploadRecord
    {
        public string Source { get; set; }
        public string ChassisNumber { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}