using HRMS_WEB.Models;

namespace HRMS_WEB.Entities
{
    public class SupplierTemplateAssignment
    {
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public int TemplateID { get; set; }

        public Supplier Supplier { get; set; }
        public Template Template { get; set; }
    }
}