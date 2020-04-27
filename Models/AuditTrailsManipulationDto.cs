using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    [AuditOperationMustBeDifferent(ErrorMessage = "The Operation and the Message must be different")]
    public class AuditTrailsManipulationDto
    {
        [Required(ErrorMessage = "The operation must be filled")]
        [MaxLength(20)]
        public string Operation { get; set; }

        [MaxLength(200)]
        public virtual string Message { get; set; }

    }
}
