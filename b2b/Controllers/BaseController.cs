using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using B2B.Models.Context;
using B2B.Dto;

namespace B2B.Controllers
{
    public class BaseController : Controller
    {
        public Context ctx;
        public BaseController()
        {
            if (ctx == null)
                ctx = new Context();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //if (Session["UserInfo"] == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            //    {
            //        controller = "Login",
            //        action = "Login"
            //    }));
            //}
        }

        public UserHolder getAktifKullanici()
        {
            UserHolder _kullanici = new UserHolder();
            try
            {
                _kullanici = (UserHolder)Session["UserInfo"];
            }
            catch (Exception ex)
            {
                _kullanici.BayiKodu = "00";
                _kullanici.FirmaAdres = "Misafir Siparişi";
                _kullanici.FirmaId = 1;
                _kullanici.FirmaIskontoOrani = 0;
                _kullanici.FirmaUnvani = "Misafir Kullanıcı";
                _kullanici.KullaniciAdi = "misafir";
                _kullanici.KullaniciYetkisi = "0";
                _kullanici.TelefonNo = "0";
                _kullanici.UserId = 1;
                _kullanici.VergiDairesi = "misafir";
                _kullanici.VergiNo = "000000";
            }
            return _kullanici;
        }
        public SepetDto getSepet()
        {
            return (SepetDto)Session["Sepet"];
        }

    }
}
