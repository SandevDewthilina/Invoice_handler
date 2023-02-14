using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbContext
{
    public class HRMSDbContext : IdentityDbContext<ApplicationUser>
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        // Entities
        public DbSet<Supplier> Supplier{ get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<Upload> Upload { get; set; }
    }
}
