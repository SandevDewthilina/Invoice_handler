using System;

namespace HRMS_WEB.Models
{
    public class Upload
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }
        public string FilePath { get; set; }
        public int SupplierID { get; set; }

        public Supplier Supplier { get; set; }
    }
}