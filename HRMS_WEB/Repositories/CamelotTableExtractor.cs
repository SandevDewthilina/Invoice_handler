using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HRMS_WEB.Repositories
{
    public class CamelotTableExtractor : ITableExtractor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CamelotTableExtractor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        } 
        
        public void ExtractTables(Upload upload)
        {
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
        }
    }
}