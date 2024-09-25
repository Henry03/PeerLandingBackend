using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqRegisterUserDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30, ErrorMessage = "Name cannot exceed 30 characters")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(30, ErrorMessage = "Role cannot exceed 30 characters")]
        public string Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal? Balance { get; set; }
    }
}
