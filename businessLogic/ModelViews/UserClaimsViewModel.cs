using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace businessLogic.ModelViews
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }
        public string ?UserName { get; set; }
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
