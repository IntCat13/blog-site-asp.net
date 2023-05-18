using System;
using System.Security.Claims;

using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Project.Areas.Permissons
{

    public static class Permissions
    {
        private static string[] Admins = {
        "admin@email.com"
        };

        public static bool IsHasPermissons(this ClaimsPrincipal user)
        {
            return Admins.Contains(user.Identity?.Name);
        }
    }
}
