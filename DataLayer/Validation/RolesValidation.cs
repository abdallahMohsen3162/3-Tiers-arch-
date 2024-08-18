using DataLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Validation
{
    public class RolesValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var roleManager = validationContext.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
            if (roleManager.RoleExistsAsync(value.ToString()).Result)
            {
                return new ValidationResult($"Role '{value}' already exists.");
            }
            return ValidationResult.Success;
        }
    }

}
