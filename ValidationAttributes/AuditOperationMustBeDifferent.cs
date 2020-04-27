using DocumentTracking.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.ValidationAttributes
{
    public class AuditOperationMustBeDifferent : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var user = (AuditTrailsManipulationDto)validationContext.ObjectInstance;
            if (user.Operation == user.Operation)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(AuditTrailsManipulationDto) }
                    );
            }

            return ValidationResult.Success;
        }
    }
}
