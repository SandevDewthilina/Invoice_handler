using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Entities
{
    public class TagAttributeAssignement
    {
        public int ID { get; set; }
        public int TagId { get; set; }
        public int AttributeId { get; set; }
        public Tag Tag { get; set; }
        public Attribute Attribute { get; set; }
    }
}
