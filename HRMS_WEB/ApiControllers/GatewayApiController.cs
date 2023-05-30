using System;
using System.Threading.Tasks;
using HRMS_WEB.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_WEB.ApiControllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [AllowAnonymous]
    public class GatewayApiController : Controller
    {
        private readonly IApiGateway _apiGateway;

        public GatewayApiController(IApiGateway apiGateway)
        {
            _apiGateway = apiGateway;
        }
        [HttpPost]
        public async Task<IActionResult> SyncDataWithPdfReader()
        {
            try
            {
                await _apiGateway.SyncDataWithPdfReader();
                return RedirectToAction("ExternalDataBatches", "ExternalData");
            }
            catch (Exception e)
            {
                return Json(new {success = false, error = e.Message});
            }
        }
    }
}