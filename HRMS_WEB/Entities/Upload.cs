using System;
using HRMS_WEB.Models;

namespace HRMS_WEB.Entities
{
    public class Upload
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }
        public string FilePath { get; set; }
        public int SupplierID { get; set; }
        public string DocumentName { get; set; }

        public Supplier Supplier { get; set; }
    }
}