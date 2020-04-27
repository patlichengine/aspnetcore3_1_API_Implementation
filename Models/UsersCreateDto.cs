using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    public class UsersCreateDto : UsersManipulationDto //: IValidatableObject
    {
        [Required]
        [MaxLength(150)] 
        [EmailAddress(ErrorMessage ="Enter a valid Email Address")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if(Surname == OtherNames)
        //    {
        //        yield return new ValidationResult(
        //            "The provided Surname should be different from the Other Names",
        //            new[] { "UsersCreateDto" }
        //            );
        //    }
        //}
    }

}
