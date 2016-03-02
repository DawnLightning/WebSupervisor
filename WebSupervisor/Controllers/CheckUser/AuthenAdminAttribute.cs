using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace WebSupervisor.Controllers.CheckUser
{
    public class AuthenAdminAttribute : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //这个方法是在Action执行之前调用
            var user = filterContext.HttpContext.Session["AdminUser"];
            if (user == null)
            {
                //filterContext.HttpContext.Response.Redirect("/Account/Logon");
                var Url = new UrlHelper(filterContext.RequestContext);
                var url = Url.Action("Login", "Account", new { area = "" });
                filterContext.Result = new RedirectResult(url);
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //这个方法是在Action执行之后调用
        }
    }
}