using DocumentTracking.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.ValidationAttributes
{
    public class SurnameMustBeDifferentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var user = (UsersManipulationDto)validationContext.ObjectInstance;
            if (user.Surname == user.OtherNames)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(UsersManipulationDto) }
                    );
            }

            return ValidationResult.Success;
        }
    }
}
