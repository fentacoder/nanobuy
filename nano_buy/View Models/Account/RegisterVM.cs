using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NanoShop.Web.VM.Account
{
    public class RegisterVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action:"IsEmailInUse", controller:"Account")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
