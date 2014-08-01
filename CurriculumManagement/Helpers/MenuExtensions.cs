using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurriculumManagement.Helpers
{
    public static class MenuExtensions
    {
        public static MvcHtmlString MenuItem(
        this HtmlHelper htmlHelper,
        //string text,
        string action,
        string controller)
        {
            var liOpeningTag = "<li";
            //var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                //li.AddCssClass("active");
                liOpeningTag += " class='active'";
            }
            liOpeningTag += ">";
            //li.InnerHtml = htmlHelper.ActionLink(text, action, controller).ToHtmlString();
            //return MvcHtmlString.Create(li.ToString());
            return MvcHtmlString.Create(liOpeningTag);
        }
    }
}