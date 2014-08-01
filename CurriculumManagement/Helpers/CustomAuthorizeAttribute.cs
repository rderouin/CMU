using CurriculumManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace CurriculumManagement.Helpers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //We had to override this method because the .NET CAS client does not
        //support the "User.IsInRole" method
        //
        //TO DO: remove this functionality and go back to User.IsInRole once the CAS
        //client has been written to support this. This is inefficient because it queries the DB
        //everytime.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            var _usersSplit = SplitString(this.Users);
            var _rolesSplit = SplitString(this.Roles);

            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (_rolesSplit.Length > 0)
            {
                var context = new EAFormDBContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                ApplicationUser appUser = UserManager.FindByName(user.Identity.Name); //(HttpContext.Current.User.Identity.GetUserName());
                if (appUser != null)
                {
                    foreach (var roleObj in appUser.Roles)
                    {
                        foreach (var passedInRole in _rolesSplit)
                            if (roleObj.Role.Name == passedInRole)
                                return true;
                    }
                }

                return false;
            }
            
            return true;

        }

        private static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }


}