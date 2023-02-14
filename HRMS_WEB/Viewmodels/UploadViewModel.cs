using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS_WEB.ViewModels
{
    public class ImageUploadViewModel
    {
        public ImageUploadViewModel()
        {
            
        }
        
        public int SupplierID { get; set; }
        public List<Supplier> SupplierList { get; set; }
    }
}