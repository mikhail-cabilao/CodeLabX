using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CodeLabX.EntityFramework.Extensions
{
    public static class IdentityExtensions
    {
        public static async Task<object> GetUserIdentityAsync(this HttpContext httpContext)
        {
            var context = httpContext.User;
            var windowsIdentity = (WindowsIdentity)context.Identity;
           
            return await Task.Run(() =>
            {
                return new
                {
                    UserName = context.Identity.Name,
                    Authenticated = context.Identity.IsAuthenticated,
                    Claims = context.Claims.Select(s => $"{s.Type}:{s.Value}").ToArray(),
                    Roles = windowsIdentity.Groups.Translate(typeof(NTAccount)).Select(s => s.Value).ToArray(),
                };
            });
        }
    }
}
