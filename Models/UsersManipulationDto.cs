using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    [SurnameMustBeDifferentAttribute(ErrorMessage = "The provided Surname should be different from the Other Names")]
    public abstract class UsersManipulationDto
    {
        [Required(ErrorMessage = "The Surname must be specified and should not be more that 20 characters")]
        [MaxLength(20)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "The Other name must be specified and should not be more that 50 characters")]
        [MaxLength(50)]
        public string OtherNames { get; set; }

        [MaxLength(14)]
        [RegularExpression(@"^\([0-9]{14})$", ErrorMessage = "Not a valid phone number")]
        public virtual string PhoneNo { get; set; }

    }
}
