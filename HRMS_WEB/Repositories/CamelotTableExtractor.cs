using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
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

        public async Task<List<object>> ExtractTables(Upload upload, int templateId)
        {
                using var scope = _serviceScopeFactory.CreateScope();
                // get the table components for the template
                var db = scope.ServiceProvider.GetService<HRMSDbContext>();
                var tableComponents = await db.TableComponent.Where(tc => tc.TemplateID == templateId).ToListAsync();
                var jsonList = new List<object>();
                foreach (var c in tableComponents)
                {
                    var body = new
                    {
                        file_url = "http://localhost:8100/" + upload.FilePath,
                        file_name = upload.FileName,
                        upload_name = upload.ID,
                        table_areas = c.Area,
                        edge_tol = c.EdgeTol,
                        row_tol = c.RowTol,
                        flavor = c.Flavor,
                        flag_size = c.FlagSize,
                        id = c.ID,
                        page_no = c.PageNo,
                        split_text = c.SplitText,
                        columns = c.Columns
                    };

                    var json = JsonConvert.SerializeObject(body);

                    using var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://localhost:8200/detectTableOfArea", content);
                    
                    if (!response.IsSuccessStatusCode) continue;
                    
                    var tableJson = await response.Content.ReadAsStringAsync();
                    var _2dArray = JsonConvert.DeserializeObject<List<object>>(tableJson);
                    
                    // check and add heading list as first element if exist
                    if (!string.IsNullOrEmpty(c.Headings))
                    {
                        _2dArray?.Insert(0, c.Headings.Split("|"));
                    }
                        
                        
                    jsonList.Add(_2dArray);
                }

                return jsonList;
        }
    }
}