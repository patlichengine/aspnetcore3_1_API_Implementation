using DocumentTracking.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Models
{
    public class UsersDto
    {
        public Guid Id { get; set; }
        public string Lpno { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? RankId { get; set; }
        public Guid? UserGroupId { get; set; }
        public bool? IsAccessGranted { get; set; }
        public bool? IsFirstLogin { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Gender { get; set; }
    }

    public class UsersFullDto
    {
        public Guid Id { get; set; }
        public string Lpno { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string PhoneNo { get; set; }
        public Guid? DepartmentId { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? RankId { get; set; }
        public Guid? UserGroupId { get; set; }
        public bool? IsAccessGranted { get; set; }
        public bool? IsFirstLogin { get; set; }
        public bool? IsLocked { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Gender { get; set; }
    }

    public class UsersCreateDto : UsersManipulationDto //: IValidatableObject
    {
        [Required]
        [MaxLength(150)]
        [EmailAddress(ErrorMessage = "Enter a valid Email Address")]
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
