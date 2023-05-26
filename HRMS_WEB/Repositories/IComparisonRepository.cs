using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.Controllers;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using KeyValuePair = HRMS_WEB.Controllers.KeyValuePair;

namespace HRMS_WEB.Repositories
{
    public interface IComparisonRepository
    {
        public Task<ComparisonLog> RetryCompare(ComparisonData comparisonData);
    }

    public class ComparisonRepository : IComparisonRepository
    {
        private readonly HRMSDbContext _db;
        private readonly IApiGateway _apiGateway;

        public ComparisonRepository(HRMSDbContext db, IApiGateway apiGateway)
        {
            _db = db;
            _apiGateway = apiGateway;
        }

        public async Task<ComparisonLog> RetryCompare(ComparisonData comparisonData)
        {

            var comparisonLog = await _db.ComparisonLog
                .Include(l => l.FirstExternalData)
                .ThenInclude(e => e.ExternalDataPairs)
                .Include(l => l.SecondExternalData)
                .ThenInclude(e => e.ExternalDataPairs)
                .FirstOrDefaultAsync(l => 
                    l.FirstExternalDataID == comparisonData.FirstSourceBatchId &&
                    l.SecondExternalDataID == comparisonData.SecondSourceBatchId);
            
            
            if (comparisonLog == null)
            {
                comparisonLog = new ComparisonLog()
                {
                    FirstExternalDataID = comparisonData.FirstSourceBatchId,
                    SecondExternalDataID = comparisonData.SecondSourceBatchId,
                    Status = false,
                    ErrorMessage = "Comparison Not started yet"
                };
                await _db.ComparisonLog.AddAsync(comparisonLog);
                await _db.SaveChangesAsync();
                
                comparisonLog = await _db.ComparisonLog
                    .Include(l => l.FirstExternalData)
                    .ThenInclude(e => e.ExternalDataPairs)
                    .Include(l => l.SecondExternalData)
                    .ThenInclude(e => e.ExternalDataPairs)
                    .FirstOrDefaultAsync(l => 
                        l.FirstExternalDataID == comparisonData.FirstSourceBatchId &&
                        l.SecondExternalDataID == comparisonData.SecondSourceBatchId);
            }
            
            comparisonLog.Status = true;
            comparisonLog.ErrorMessage = "";
            
            var coupling = new List<List<KeyValuePair>>();
            
            var firstKeyValuePairs = comparisonLog
                .FirstExternalData.ExternalDataPairs
                .Select(p => new KeyValuePair() {Key = p.Key, Value = p.Value});
            foreach (KeyValuePair firstKeyValuePair in firstKeyValuePairs)
            {
                // matching second key value pair
                var secondKeyValuePair = comparisonLog.SecondExternalData.ExternalDataPairs
                    .Select(p => new KeyValuePair() {Key = p.Key, Value = p.Value})
                    .FirstOrDefault(p => p.Key.Equals(firstKeyValuePair.Key));
            
                if (secondKeyValuePair == null)
                {
                    coupling.Add(new List<KeyValuePair>() {firstKeyValuePair, new KeyValuePair() {Key = firstKeyValuePair.Key, Value = null}});
                    comparisonLog.Status = false;
                    comparisonLog.ErrorMessage = $"No matching keys found for key: {firstKeyValuePair.Key}";
                }
                else
                {
                    if (!firstKeyValuePair.Value.Equals(secondKeyValuePair.Value))
                    {
                        comparisonLog.Status = false;
                        comparisonLog.ErrorMessage = $"Values don't match for key: {firstKeyValuePair.Key}";
                    }
            
                    coupling.Add(new List<KeyValuePair>() {firstKeyValuePair, secondKeyValuePair});
                }
            }

            comparisonLog.Couplings = JsonConvert.SerializeObject(coupling);

            _db.Update(comparisonLog);
            await _db.SaveChangesAsync();

            return comparisonLog;
        }
    }
}