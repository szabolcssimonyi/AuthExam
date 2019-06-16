using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AuthExam.Models
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Street { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string HouseNumber { get; set; }

        [ForeignKey("City")]
        public int SelectedCityId { get; set; }
        public virtual CityViewModel City { get; set; }

        public IEnumerable<CityViewModel> Cities { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}