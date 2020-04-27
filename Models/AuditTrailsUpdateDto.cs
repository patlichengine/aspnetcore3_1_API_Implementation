using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    public class AuditTrailsUpdateDto : AuditTrailsManipulationDto
    {
        [Required(ErrorMessage = "The Message description should be supplied")]
        public override string Message { get => base.Message; set => base.Message = value; }
    }
}
