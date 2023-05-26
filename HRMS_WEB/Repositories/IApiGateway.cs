using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using KeyValuePair = HRMS_WEB.Controllers.KeyValuePair;

namespace HRMS_WEB.Repositories
{
    public interface IApiGateway
    {
        public Task<UploadData> GetUploadDataForUploadId(int id);
        public Task SyncDataWithPdfReader();
    }

    public class ApiGateway : IApiGateway
    {
        private readonly HRMSDbContext _db;
        private string BASE_URL = "http://localhost:9100";

        public ApiGateway(HRMSDbContext db)
        {
            _db = db;
        }
        private async Task<ApiResponse<T>> HttpGet<T>(string url)
        {
            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    T data =  JsonConvert.DeserializeObject<T>(jsonString);
                    return new ApiResponse<T>(data, true, "");
                }

                return new ApiResponse<T>(default(T), false, $"HTTP request failed with status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(default(T), false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<UploadData> GetUploadDataForUploadId(int id)
        {
            var apiResponse = await HttpGet<UploadData>($"{BASE_URL}/api/GatewayApi/GetUploadDataForUploadId?id={id}");
            return apiResponse.data;
        }

        public async Task SyncDataWithPdfReader()
        {
            // var lastId = await _db.ExternalData.MaxAsync(d => d.ID);
            var apiResponse = await HttpGet<List<UploadData>>($"{BASE_URL}/api/GatewayApi/GetLastUploadDataForSync");
            foreach (UploadData uploadData in apiResponse.data)
            {
                var fieldKeyValuePairs = JsonConvert.DeserializeObject<List<KeyValuePair>>(uploadData.fieldJson);
                
                // pre process values
                fieldKeyValuePairs = fieldKeyValuePairs?
                    .Select(p => new KeyValuePair()
                    {
                        Key = p.Key, 
                        Value = p.Value.ToUpper().Replace(" ", "")
                    }).ToList();
                
                var chassisNumber = fieldKeyValuePairs!
                    .FirstOrDefault(p => p.Key.Equals("ChassisNumber"))
                    ?.Value;
                
                var externalData = await _db.ExternalData.FirstOrDefaultAsync(e => e.ChassisNumber.Equals(chassisNumber));
        
                if (externalData != null)
                {
                    _db.Remove(externalData);
                    await _db.SaveChangesAsync();
                }
                
                externalData = new ExternalData() { ChassisNumber = chassisNumber, Type = "PDF_READER"};
                await _db.ExternalData.AddAsync(externalData);
                await _db.SaveChangesAsync();

                var externalDataPairs = fieldKeyValuePairs
                    .Select(p => new ExternalDataPair()
                    {
                        Key = p.Key, 
                        Value = p.Value,
                        ExternalDataID = externalData.ID
                    });

                await _db.ExternalDataPair.AddRangeAsync(externalDataPairs);
                await _db.SaveChangesAsync();
            }
        }
    }

    public class ApiResponse<T>
    {
        public T data { get; }
        public bool success { get; }
        public string error { get; }

        public ApiResponse(T data, bool success, string error)
        {
            this.data = data;
            this.success = success;
            this.error = error;
        }
    }
}