using HRMS_WEB.Entities;

namespace HRMS_WEB.Models
{
    public class UploadData
    {
        public int ID { get; set; }
        public int UploadID { get; set; }
        public int? DetectedTemplateID { get; set; }
        public string FieldJson { get; set; }
        public string TableJson { get; set; }
        public bool SupplierConfirmed { get; set; }

        public Upload Upload { get; set; }
        public Template DetectedTemplate { get; set; }
    }
}