using Sqlite.SqliteAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace FantasyCricket.Models
{
    public class UserRegistration
    
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Invalid User Registration Information")]
        [StringLength(10, MinimumLength = 6)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Invalid User Registration Information")]
        [DataType(DataType.Password)]
        [StringLength(12,MinimumLength = 6)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,12}$", ErrorMessage = "Invalid User Registration Information")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Invalid User Registration Information")]
        public string DisplayName { get; set; }



    }
}
