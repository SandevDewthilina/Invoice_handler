using System;
using System.Collections.Generic;
using System.Linq;
using HRMS_WEB.Entities;
using HRMS_WEB.Models;

namespace HRMS_WEB.Viewmodels
{
    public class TemplateViewModel
    {
        public TemplateViewModel()
        {}

        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string SupplierID { get; set; }
        public List<Supplier> Suppliers { get; set; }
    }
}