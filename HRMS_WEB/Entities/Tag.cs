using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class Tag
    {
        public int ID { get; set; }
        public String Name { get; set; }
        [NotMapped]
        public List<Attribute> Attributes { get; set; }
    }
}
