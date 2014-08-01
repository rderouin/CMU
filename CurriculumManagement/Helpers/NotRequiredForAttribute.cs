using CurriculumManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CurriculumManagement.Helpers
{
    public class NotRequiredForAttribute: ValidationAttribute
    {
        public string Roles { get; set; }
        
        public override bool IsValid(object value)
        {
            IPrincipal user = HttpContext.Current.User;
            var RolesArray = Roles.Split(',');
            if (RolesArray.Length > 0)
            {
                var context = new EAFormDBContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                ApplicationUser appUser = UserManager.FindByName(user.Identity.Name);
                if (appUser == null || appUser.Roles == null)
                    return false;

                foreach (var roleObj in appUser.Roles)
                {
                    foreach (var passedInRole in RolesArray)
                        if (roleObj.Role.Name == passedInRole)
                            return true;
                }

                return false;
            }

            return false;
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