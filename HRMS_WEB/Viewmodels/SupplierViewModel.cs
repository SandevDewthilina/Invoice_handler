using HRMS_WEB.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS_WEB.Viewmodels
{
    public class SupplierViewModel
    {
        public SupplierViewModel()
        {
            Templates = new Template[]
            {
                new Template()
                {
                    ID = 1,
                    Content = "",
                    Name = "sandebv"
                }
            };
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public Template[] Templates { get; set; }
        public int[] SelectedIdList { get; set; }
    }
}