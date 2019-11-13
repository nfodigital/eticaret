using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Context;
using B2B.Models.Entity;
using System.Net;
using System.Web.Script.Serialization;


namespace B2B.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public string sid()
        {
            try
            {
                return Request.Cookies["usersid"].Value.ToString();
            }
            catch (Exception)
            {

                return Session.SessionID;
            }
        }
        private Context ctx;
        public CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();
        public ActionResult Login()
        {
            var _u = Request.Cookies["userId"];
            if (_u != null)
            {
                Response.Redirect("/Login/SuspendedLogin");
            }
            return View();
        }

        public JsonResult JustControl(string username, string password)
        {
            ctx = new Context();
            USERS u = ctx.Kullanicilar.FirstOrDefault(q => q.USERNAME == username && q.PASSWORD == password);
            MethodStatusDto method = new MethodStatusDto();
            if (u != null && u.Aktif == 1)
            {
                method.Error = "success";
                method.ReturnMsg = "Giriş Başarılı";
                method.ReturnValue = 0;
            }
            return Json(method);
        }

        public JsonResult DoLogin(string username, string password)
        {
            MethodStatusDto method = new MethodStatusDto();

            if (username == "" || password == "")
            {
                username = Request.QueryString["username"];
                password = Request.QueryString["password"];
            }
            try
            {
                ProductsController ProductControl = new ProductsController();
                ctx = new Context();

                USERS u = ctx.Kullanicilar.FirstOrDefault(q => q.USERNAME == username && q.PASSWORD == password);

                if (u != null && u.Aktif == 1)
                {
                    COMPANIES _c = ctx.Firmalar.FirstOrDefault(q => q.Id == u.FirmaId); method.Error = "success"; method.ReturnMsg = "Giriş Başarılı, yönlendirilirken lütfen bekleyiniz";

                    if (u.KullaniciYetkisi == "su")
                    {
                        method.ReturnValue = 999;
                    }
                    else
                    {
                        method.ReturnValue = 1;
                    }
                    COMPANIES c = ctx.Firmalar.SingleOrDefault(q => q.Id == u.FirmaId);


                    c.LastVisit = DateTime.Now;
                    c.ZiyaretSayisi = c.ZiyaretSayisi + 1;

                    // setting login values
                    UserHolder UserInfo = new UserHolder();
                    UserInfo.FirmaUnvani = c.FirmaUnvani;
                    UserInfo.FirmaIskontoOrani = c.FirmaIskonto;
                    UserInfo.TelefonNo = c.FirmaTelefon;
                    UserInfo.KullaniciAdi = c.YetkiliKisi;
                    UserInfo.BayiKodu = c.BayiKodu;
                    UserInfo.UserId = u.Id;
                    UserInfo.FirmaId = c.Id;
                    UserInfo.VergiDairesi = c.VergiDairesi;

                    UserInfo.VergiNo = c.VergiNo;
                    UserInfo.KullaniciYetkisi = u.KullaniciYetkisi;

                    Session["UserInfo"] = UserInfo;
                    var _cookie = new HttpCookie("userId"); _cookie.Expires = DateTime.Now.AddDays(365);
                    _cookie["userId"] = UserInfo.UserId.ToString(); Response.Cookies.Add(_cookie);

                    // Sepeti Yerine koyalım

                    List<SEPET> Sepet = ctx.Sepet.Where(q => q.FirmaId == UserInfo.FirmaId).ToList();

                    if (Sepet != null)
                    {
                        SepetDto SepettekiUrunler = new SepetDto();
                        List<SepetUrunlerDto> Urun = new List<SepetUrunlerDto>();

                        foreach (var item in Sepet)
                        {
                            Urun.Add(new SepetUrunlerDto()
                            {
                                UrunId = item.UrunId,
                                Adet = item.Adet,
                                Fotograf = ProductControl.UrunFotografiVer(item.UrunId),
                                Marka = ProductControl.MarkaVer(item.UrunId),
                                UrunBaslik = ProductControl.UrunAdiVer(item.UrunId),
                                UrunFiyat = ProductControl.UrunFiyatVer(item.UrunId),
                                UrunKodu = ProductControl.UrunKoduVer(item.UrunId)
                            });
                        }
                        SepettekiUrunler.Urunler = Urun;
                        SepettekiUrunler.SepetUrunId = 1;
                        SepettekiUrunler.SessionId = sid();

                        Session["Sepet"] = SepettekiUrunler;
                        foreach (var item in Sepet)
                        {
                            ctx.Sepet.Remove(item);
                        }
                        ctx.SaveChanges();
                        Aktivite _a = new Aktivite(); _a.AktiviteSTR = "Sisteme giriş yapıldı."; _a.FirmaId = UserInfo.FirmaId; _a.AktiviteTarihSaat = DateTime.Now; _a.AktiviteTipi = 2; _a.UserId = UserInfo.UserId;
                        Kernel.MakeLog(_a);
                    }
                }
                else
                {
                    method.Error = "error";
                    method.ReturnMsg = "Giriş başarısız.\nLütfen Bayi Kodu ve Şifrenizi gözden geçiriniz.\nProsedür gereği oluşabilecek durumlardan dolayı hesabınız aktif de olmayabilir";
                    method.ReturnValue = 0;
                }
            }
            catch (Exception ex)
            {
                method.Error = "error"; method.ReturnMsg = ex.Message; method.ReturnValue = 0;
            }

            return Json(method, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LogOut()
        {
            SepetDto SepetSession = (SepetDto)Session["Sepet"];
            UserHolder UserInfo = (UserHolder)Session["UserInfo"];
            ctx = new Context();
            if (SepetSession != null)
            {
                foreach (var item in SepetSession.Urunler)
                {
                    SEPET SepetUrunleri = new SEPET();
                    SepetUrunleri.FirmaId = UserInfo.FirmaId;
                    SepetUrunleri.UrunId = item.UrunId;
                    SepetUrunleri.UserId = UserInfo.UserId;
                    SepetUrunleri.Adet = item.Adet;

                    ctx.Sepet.Add(SepetUrunleri);

                }
            }
            ctx.SaveChanges();
            var _cookie = Request.Cookies["userId"];
            _cookie.Expires = DateTime.Now.AddDays(-1); Response.Cookies.Add(_cookie);
            Session["Sepet"] = null;
            Session["UserInfo"] = null;
            Response.Redirect("/Login/Login");
            return Json("Çıkış yapıldı.", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecoverPwd()
        {
            return View();
        }
        public ActionResult SuspendedLogin()
        {
            USERS _x = new USERS();
            var _u = Request.Cookies["userId"];
            try
            {
                ctx = new Context();
                int UserId = int.Parse(_u["userId"]);
                _x = ctx.Kullanicilar.FirstOrDefault(q => q.Id == UserId);
            }
            catch (Exception)
            {
              //  Response.Redirect("/Login/Login");
            }
            return View(_x);
        }
    }
}
