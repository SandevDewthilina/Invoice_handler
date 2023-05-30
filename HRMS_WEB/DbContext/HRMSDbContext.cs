using HRMS_WEB.Entities;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRMS_WEB.DbContext
{
    public class HRMSDbContext : IdentityDbContext<ApplicationUser>
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<ExternalData> ExternalData { get; set; }
        public DbSet<ExternalDataPair> ExternalDataPair { get; set; }
        public DbSet<ComparisonLog> ComparisonLog { get; set; }
        public DbSet<DocumentComparisonPlan> DocumentComparisonPlan { get; set; }
    }
}
