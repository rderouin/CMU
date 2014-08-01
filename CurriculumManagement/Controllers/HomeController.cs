using CurriculumManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CurriculumManagement.Controllers
{
    public class HomeController : Controller
    {
        //[CustomAuthorize(Roles = "Program Assistant,Admin,Curriculum Materials Coordinator")]
        public ActionResult Index()
        {
            // Get the Web application configuration.
            //System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/aspnetTest");

            //// Get the external Authentication section.
            //AuthenticationSection authenticationSection = (AuthenticationSection)configuration.GetSection("system.web/authentication");

            //// Get the external Forms section .
            //FormsAuthenticationConfiguration formsAuthentication = authenticationSection.Forms;

            //ViewBag.LoginURL = formsAuthentication.LoginUrl + "?TARGET=" + Server.HtmlEncode(Request.Url.AbsoluteUri);
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "For assistance, please contact technical support below:";

            return View();
        }

    }
}