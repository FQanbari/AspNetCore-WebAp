using AspNetCore_WebApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_WebApi.Api.Models
{
    public class UserDto:IValidatableObject
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(500)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public int Age { get; set; }

        public GenderType Gender { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("the name of user can not be test", new[] { nameof(UserName) });
            if (Password.Equals("123456"))
                yield return new ValidationResult("the password can not be 123456", new[] { nameof(Password) });
            if (Gender == GenderType.Male && Age >= 18)
                yield return new ValidationResult("boy can not register", new[] { nameof(Gender),nameof(Age) });
        }
    }
}
