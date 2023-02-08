using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Attribute
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Datatype { get; set; }
        public String DefaultValue { get; set; }
    }
}
