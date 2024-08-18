using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(20)]
        public string? Address { get; set; }
        [Range(0, 60)]
        public int? Age { get; set; }
    }
}
