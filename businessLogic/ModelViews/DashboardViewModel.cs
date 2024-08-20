

using System.ComponentModel.DataAnnotations;
using DataLayer.Entities;
using businessLogic.ModelViews;
using Microsoft.AspNetCore.Http;

namespace businessLogic.ModelViews
{
    public class DashboardViewModel
    {
        public ProfileViewModel[] ?Users { get; set; }
    }


    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Address { get; set; }

        [Required]
        public int Age { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public IFormFile? ProfileImage { get; set; }
        [Required]
        public string RoleId { get; set; } 
    }

    public class EditViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string ?Address { get; set; }

        public int Age { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string RoleId { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
