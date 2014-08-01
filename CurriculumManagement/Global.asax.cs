using AutoMapper;
using CurriculumManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace CurriculumManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            Bootstrapper.BootStrap();
            
            
            //Mapper.AssertConfigurationIsValid();
        }

        //OR: Application_PostAuthenticateRequest
        protected void FormsAuthentication_OnAuthenticate(object sender, FormsAuthenticationEventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //Discover user's name            
                        string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;
                        
                        //using (userDbEntities entities = new userDbEntities())
                        //{
                        //    User user = entities.Users.SingleOrDefault(u => u.username == username);
                        //    roles = user.Roles;
                        //}
                        ////let us extract the roles from our own custom cookie
                        var context = new EAFormDBContext();
                        var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                        var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                        //Query UserManager to discover role(s) for this user to store in IPrincipal
                        ApplicationUser appUser = UserManager.FindByName(username);
                        if (appUser == null)
                        {
                            //They've logged in BUT are not in any of our lists
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Logged in user does not have a role. User name = " + username));

                        }
                        else
                        {
                            var delim = "";
                            foreach (var role in appUser.Roles)
                            {
                                roles += delim + role.Role.Name;
                                delim = ";";
                            }
                        }

                        //Set the Pricipal with roles
                        e.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));

                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }

        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

}
}
