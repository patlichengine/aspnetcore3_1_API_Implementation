using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{

    public class UsersUpdateDto : UsersManipulationDto //: IValidatableObject
    {
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

        [Required(ErrorMessage = "The Phone number must be specified")]
        public override string PhoneNo { get => base.PhoneNo; set => base.PhoneNo = value; }
    }

}
