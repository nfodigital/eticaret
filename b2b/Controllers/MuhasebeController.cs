using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Models.Entity;
using B2B.Dto;

namespace B2B.Controllers
{
    public class MuhasebeController : BaseController
    {
        SepetController sController = new SepetController();
        UserHolder UserInfo = new UserHolder();
        public ActionResult Index()
        {
            return View();
        }
        
    }
}
