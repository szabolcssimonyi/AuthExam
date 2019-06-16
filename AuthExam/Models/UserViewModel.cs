using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AuthExam.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "VerifyEmail", controller: "User")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(36, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public List<AddressViewModel> Addresses { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public List<CityViewModel> Cities { get; set; }
    }
}