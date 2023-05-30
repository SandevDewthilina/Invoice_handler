using System.Linq;
using System.Threading.Tasks;
using HRMS_WEB.DbContext;
using HRMS_WEB.Models;
using HRMS_WEB.Repositories;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HRMSDbContext _db;

        public HomeController(ILogger<HomeController> logger, HRMSDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
    }
}