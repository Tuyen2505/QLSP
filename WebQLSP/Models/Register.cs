using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebQLSP.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}