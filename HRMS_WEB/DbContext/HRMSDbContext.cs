using HRMS_WEB.Entities;
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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<TagAttributeAssignement> TagAttributeAssignements { get; set; }
    }
}
