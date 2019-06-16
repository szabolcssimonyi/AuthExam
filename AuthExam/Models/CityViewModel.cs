using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuthExam.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public ICollection<AddressViewModel> Addresses { get; set; }
    }
}