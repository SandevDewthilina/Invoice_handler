using HRMS_WEB.Models;

namespace HRMS_WEB.Viewmodels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Template[] Templates { get; set; }
    }
}