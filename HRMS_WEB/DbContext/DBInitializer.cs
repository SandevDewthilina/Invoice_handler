using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.DbContext
{
    public class DBInitializer
    {
        public static void Initialize(HRMSDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
