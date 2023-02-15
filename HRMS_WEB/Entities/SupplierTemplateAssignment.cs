namespace HRMS_WEB.Models
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