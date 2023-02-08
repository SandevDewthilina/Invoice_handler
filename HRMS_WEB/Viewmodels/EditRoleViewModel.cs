using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public String Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public String RoleName { get; set; }

        public List<String> Users { get; set; }
    }
}
