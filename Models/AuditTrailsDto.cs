using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    public class AuditTrailsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class AuditTrailsCreateDto : AuditTrailsManipulationDto
    {
    }

    public class AuditTrailsUpdateDto : AuditTrailsManipulationDto
    {
        [Required(ErrorMessage = "The Message description should be supplied")]
        public override string Message { get => base.Message; set => base.Message = value; }
    }

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

