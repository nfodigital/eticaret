using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2B.Controllers
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            isAuthorized = false;

            if (httpContext.Session["UserInfo"] != null)
            {
                B2B.Dto.UserHolder p = (B2B.Dto.UserHolder)httpContext.Session["UserInfo"];
                string mevcutYetkiler = p.KullaniciYetkisi;

                if (mevcutYetkiler == "su") //superadmin
                {
                    isAuthorized = true;
                }
            }
            else
            {
                httpContext.Response.Redirect("/Home/");
            }
            return isAuthorized;
        }

    }
}