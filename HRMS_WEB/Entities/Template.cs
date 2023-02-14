using System;

namespace HRMS_WEB.Models
{
    public class Template
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int SupplierID { get; set; }

        public Supplier Supplier { get; set; }
    }
}