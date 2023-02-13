using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using HRMS_WEB.DbContext;
using System.Text;
using HRMS_WEB.Viewmodels;
using HRMS_WEB.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Xml.Linq;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HRMSDbContext db;

        public HomeController(ILogger<HomeController> logger, HRMSDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {

            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
        /*
         * supplier 
         */
        public IActionResult CreateSupplier()
        {
            return View(new SupplierViewModel());
        }

        [HttpPost]
        public IActionResult CreateSupplier(SupplierViewModel model)
        {
            return RedirectToAction("ViewSuppliers");
        }

        public IActionResult ViewSuppliers()
        {
            return View();
        }
        /*
         * templates
         */
        public IActionResult ViewTemplates()
        {
            return View();
        }
        public IActionResult CreateTemplate()
        {
            return View();
        }
    }
}
