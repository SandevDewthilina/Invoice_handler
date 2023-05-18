using System;
using System.Collections.Generic;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;

namespace HRMS_WEB.Viewmodels
{
    public class UploadViewModel
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedDate { get; set; }
        public string FilePath { get; set; }
        public int SupplierID { get; set; }
        
        public List<Supplier> SupplierList { get; set; }
    }
}