using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [RegularExpression("([a-zA-Z]+)")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [RegularExpression("([a-zA-Z]+)")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
