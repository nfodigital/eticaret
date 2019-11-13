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
            if (httpContext.Request.Cookies["userId"] != null) //superadmin
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}