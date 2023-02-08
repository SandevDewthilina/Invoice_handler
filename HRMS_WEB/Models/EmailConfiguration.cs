using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Models
{
    public class EmailConfiguration
    {
        public String From { get; set; }
        public String SmtpServer { get; set; }
        public int Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
    }
}
