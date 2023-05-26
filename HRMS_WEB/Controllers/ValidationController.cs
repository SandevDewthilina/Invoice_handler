using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HRMS_WEB.Controllers
{
    public class ValidationController : Controller
    {
        private readonly HRMSDbContext _db;
        private readonly IApiGateway _apiGateway;
        private readonly IComparisonRepository _comparisonRepository;

        public ValidationController(HRMSDbContext db, IApiGateway apiGateway, IComparisonRepository comparisonRepository)
        {
            _db = db;
            _apiGateway = apiGateway;
            _comparisonRepository = comparisonRepository;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> Index(int firstSourceBatchId)
        {

            var firstSourceExternalData = await _db.ExternalData.FirstOrDefaultAsync(e => e.ID == firstSourceBatchId);

            var chassisNumber = firstSourceExternalData.ChassisNumber;
            ViewBag.chassisNumber = chassisNumber;
            ViewBag.ComparisonPair = new int[] {};
            ViewBag.comparisonSources = await _db.ExternalData
                .Where(e => e.ChassisNumber.Equals(chassisNumber) && e.ID != firstSourceBatchId)
                .Select(e => new object[] {e.Type, e.ID})
                .ToListAsync();
            return View(new ComparisonData() {FirstSourceBatchId = firstSourceBatchId});
        }

        [HttpPost]
        public async Task<IActionResult> Index(ComparisonData model)
        {
            var firstSourceExternalData = await _db.ExternalData.FirstOrDefaultAsync(e => e.ID == model.FirstSourceBatchId);
            var chassisNumber = firstSourceExternalData.ChassisNumber;
            ViewBag.chassisNumber = chassisNumber;
            ViewBag.comparisonSources = await _db.ExternalData
                .Where(e => e.ChassisNumber.Equals(chassisNumber) && e.ID != model.FirstSourceBatchId)
                .Select(e => new object[] {e.Type, e.ID})
                .ToListAsync();
            var newComparisonLog = await _comparisonRepository.RetryCompare(model);
            ViewBag.ComparisonPair = JsonConvert.DeserializeObject<List<List<KeyValuePair>>>(newComparisonLog.Couplings);
            return View(model);
        }
    }

    public class ComparisonData
    {
        public int FirstSourceBatchId { get; set; }
        public int SecondSourceBatchId { get; set; }
    }

    public class KeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}