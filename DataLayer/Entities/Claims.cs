using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DataLayer.Entities
{
    public class Claims
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim(AuthenticationConstants.Identity.Create, AuthenticationConstants.Identity.Create),
            new Claim(AuthenticationConstants.Identity.Edit, AuthenticationConstants.Identity.Edit),
            new Claim(AuthenticationConstants.Identity.Delete, AuthenticationConstants.Identity.Delete),
            new Claim(AuthenticationConstants.Claims.Manage, AuthenticationConstants.Claims.Manage)
        };
        
    }

    public class AuthenticationConstants
    {
        public static class Identity
        {
            public const string Create = "CreateIdentity";
            public const string Edit = "EditIdentity";
            public const string Delete = "DeleteIdentity";
            public const string Manage = "ManageIdentity";
        }

        public static class Claims
        {
            public const string Manage = "ManageClaims";
        }


    }
}
