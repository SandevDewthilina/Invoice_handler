using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_WEB.Viewmodels
{
    public class RegisterViewModel
    {

        [Required]
        [EmailAddress]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public String ConfirmPassword { get; set; }

        [Required]
        public String PhoneNumber { get; set; }

        [Required]
        public bool AgreedToTerms { get; set; }

        [Required]
        public String Name { get; set; }

    }
}
