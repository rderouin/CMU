using CurriculumManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurriculumManagement.Helpers
{
    public static class ActionLinkHelper
    {
        public static MvcHtmlString If(this MvcHtmlString value, bool evaluation)
        {
            return evaluation ? value : MvcHtmlString.Empty;
        }

        public static bool IsInRole(string roles)
        {
            string[] rolesArray = roles.Split(',');
            var context = new EAFormDBContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            ApplicationUser appUser = UserManager.FindByName(HttpContext.Current.User.Identity.GetUserName());
            if (appUser != null)
            {
                foreach (var roleObj in appUser.Roles)
                {
                    if (rolesArray.Contains(roleObj.Role.Name))
                        return true;

                    //if (roleObj.Role.Name == role)
                    //    return true;
                }
            }
             
            return false;
        }
    }
}