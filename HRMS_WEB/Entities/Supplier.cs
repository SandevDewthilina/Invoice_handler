using System.Collections.Generic;

namespace HRMS_WEB.Models
{
    public class Supplier
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Template> Templates { get; set; }
    }
}