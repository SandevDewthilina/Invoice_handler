using System.Collections.Generic;
using HRMS_WEB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS_WEB.Viewmodels
{
    public class SupplierViewModel
    {
        public SupplierViewModel()
        {
            Templates = new List<Template>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Template> Templates { get; set; }
        public int[] AlreadySelectedIdList { get; set; }
        public int[] NewlySelectedIdList { get; set; }
    }
}