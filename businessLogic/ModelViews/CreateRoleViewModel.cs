using DataLayer.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.ModelViews
{
    public class CreateRoleViewModel
    {
        [Required]
        [RolesValidation]
        [Display(Name = "Role Name")]

        public string RoleName { get; set; }
    }


    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
    
}
