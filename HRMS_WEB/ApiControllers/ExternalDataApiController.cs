using System;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Entities;
using HRMS_WEB.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExternalDataApiController : Controller
    {
        private readonly IApiGateway _apiGateway;
        private readonly HRMSDbContext _db;

        public ExternalDataApiController(IApiGateway apiGateway, HRMSDbContext db)
        {
            _apiGateway = apiGateway;
            _db = db;
        }
        
        [HttpPost]
        public async Task<IActionResult> EditExternalData(EditExternalDataModel model)
        {
            try
            {
                var externalDataPair = await _db.ExternalDataPair.FirstOrDefaultAsync(p => p.ExternalDataID == model.sourceId && p.Key.Equals(model.key));

                if (externalDataPair == null)
                {
                    externalDataPair = new ExternalDataPair()
                    {
                        ExternalDataID = model.sourceId,
                        Key = model.key,
                        Value = model.newValue
                    };
                    await _db.ExternalDataPair.AddAsync(externalDataPair);
                    await _db.SaveChangesAsync();
                }

                externalDataPair.Value = model.newValue;
                _db.ExternalDataPair.Update(externalDataPair);
                await _db.SaveChangesAsync();
                return Json(new {success = true});
            }
            catch (Exception e)
            {
                return Json(new {success = false});
            }
        }

        public class EditExternalDataModel
        {
            public string key { get; set; }
            public int sourceId { get; set; }
            public string newValue { get; set; }
        }
    }
}