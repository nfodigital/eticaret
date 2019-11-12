using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using B2B.Dto;

using B2B.Models.Entity;

namespace B2B.Controllers
{
    public class HesapController : BaseController
    {
        //
        // GET: /Hesap/
        CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();
        ProductsController UrunlerKontrolcusu = new ProductsController();

        public ActionResult HesapBilgileriniGoruntule()
        {
            UserHolder UserInfo = getAktifKullanici();
            var a = getAktifKullanici().FirmaId;

            var Adresler = (from ar in ctx.KullaniciAdresleri.Where(q => q.UserId == a).ToList()
                            select new KullaniciAdresleri
                            {
                                Adres = ar.Adres,
                                AdresTipi = ar.AdresTipi,
                                AdresId = ar.Id,
                                UserId = ar.UserId
                            }
                        ).ToList();

            return View(Adresler);
        }

        public ActionResult HesapOzeti()
        {
            UserHolder UserInfo = getAktifKullanici();
            HesapOzetiDto HDto = new HesapOzetiDto();

            HDto.BakiyeTutar = ctx.Muhasebe.SingleOrDefault(q => q.FirmaId == UserInfo.FirmaId).Bakiye;
            HDto.SiparislerToplamTutar = ctx.Siparisler.Where(q => q.FirmaId == UserInfo.FirmaId).Sum(q => q.SiparisToplami).GetValueOrDefault(0);
            HDto.SiparisToplam = ctx.Siparisler.Where(q => q.FirmaId == UserInfo.FirmaId).Count();
            HDto.VadeTarihiHatirlatici = ctx.VadeTarihleri.Where(q => q.VadeTarihi > DateTime.Now).SingleOrDefault(q => q.FirmaId == UserInfo.FirmaId);
            var Transactions = (from item in ctx.GirdiCiktilar.Where(q => q.FirmaId == UserInfo.FirmaId).OrderByDescending(q => q.Id).ToList()
                                select new HesapOzetiClass()
                                {
                                    IslemAciklamasi = item.Aciklama,
                                    IslemDurum = OrtakFonksiyonlar.GirdiDurumVer(item.GirdiDurum),
                                    IslemTarihi = item.TarihSaat,
                                    IslemId = item.Id,
                                    IslemTutar = item.Tutar,
                                    IslemTipi = item.GirdiTipi
                                }).ToList();
            HDto.HesapOzetiListesi = Transactions;

            return View(HDto);
        }

        public ActionResult FaturaListele(string start, string end)
        {

            DateTime startDate, EndDate;
            DateTime.TryParse(start, out startDate);
            DateTime.TryParse(end, out EndDate);

            UserHolder UserInfo = getAktifKullanici();
            List<ORDERS> Siparisler = new List<ORDERS>();

            if (start != null && end != null)
            {
                Siparisler = ctx.Siparisler.Where(q => q.SadeceTarih > startDate && q.SadeceTarih < EndDate).ToList();
            }
            else
            {
                Siparisler = ctx.Siparisler.Where(q => q.FirmaId == UserInfo.FirmaId).ToList();
            }

            return View(Siparisler);
        }
        public ActionResult FaturaGoster(int Id)
        {
            ORDERS Order = ctx.Siparisler.Where(q => q.Id == Id).FirstOrDefault();
            SiparisAyrintilari Siparis = new SiparisAyrintilari();
            Siparis.SiparisUrunleri = new List<GecmisSiparisUrunler>();

            Siparis.Iskonto = Order.SiparisIskontoOrani;
            Siparis.IskontoTutar = Order.IskontoTutar;
            Siparis.Kdv = 18;
            Siparis.SiparisId = Order.Id;
            Siparis.SiparisKodu = Order.SiparisNo;
            Siparis.SiparisTarihi = Order.SiparisTarihi;
            Siparis.SiparisToplami = Order.SiparisToplami.GetValueOrDefault(0);

            foreach (var item in Order.SiparisUrunleri)
            {
                Siparis.SiparisUrunleri.Add(new GecmisSiparisUrunler()
                {
                    Adet = item.Adet,
                    Tutar = item.KalemTutar,
                    UrunBaslik = UrunlerKontrolcusu.UrunAdiVer(item.UrunId),
                    UrunId = item.UrunId
                });
            }

            return View(Siparis);
        }
        public JsonResult HesapBilgileriniKaydet(int id, string unvan, string yetkili, string telefon)
        {
            MethodStatusDto method = new MethodStatusDto();
            try
            {
                COMPANIES c = ctx.Firmalar.FirstOrDefault(q => q.Id == id);

                c.FirmaUnvani = unvan;
                c.YetkiliKisi = yetkili;
                c.FirmaTelefon = telefon;
                ctx.SaveChanges();
                UserHolder UHolder = (UserHolder)Session["UserInfo"];
                UHolder.FirmaUnvani = c.FirmaUnvani;
                UHolder.KullaniciAdi = c.YetkiliKisi;
                UHolder.TelefonNo = c.FirmaTelefon;
                UHolder.UserId = UHolder.UserId;
                Session["UserInfo"] = UHolder;

                method.Error = "success"; method.ReturnMsg = "Kayıtlı bilgileriniz güncellendi"; method.ReturnValue = 1;
            }
            catch (Exception)
            {
                method.Error = "error"; method.ReturnMsg = "Hata oluştu!"; method.ReturnValue = 0;
            }
            return Json(method, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdresBilgileriniListele()
        {
            var a = getAktifKullanici().FirmaId;
            var Adresler = (from ar in ctx.KullaniciAdresleri.Where(q => q.UserId == a).ToList()
                            select new KullaniciAdresleri
                            {
                                Adres = ar.Adres,
                                AdresTipi = ar.AdresTipi,
                                AdresId = ar.Id,
                                UserId = ar.UserId
                            }
                        ).ToList();

            return Json(Adresler, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult AdresEkle_Page()
        {
            return PartialView();
        }
        public PartialViewResult AdresDuzenle_Page(int id)
        {
            var adres = ctx.KullaniciAdresleri.SingleOrDefault(q=>q.Id==id);
            return PartialView(adres);
        }
        public JsonResult AdresEkleVeyaDuzenle(ADDRESS _a)
        {
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                var a = ctx.KullaniciAdresleri.SingleOrDefault(q => q.Id == _a.Id);
                if (a != null)
                {
                    a.Adres = _a.Adres; a.AdresTipi = "İşyeri"; ctx.SaveChanges(); _m.Error = "success";_m.ReturnMsg = "Adres Bilgileri Düzenlendi."; _m.ReturnValue = a.Id;
                    ctx.SaveChanges();
                }
                else
                {
                    a = new ADDRESS();
                    a.Adres = _a.Adres;
                    a.UserId = getAktifKullanici().UserId;
                    a.AdresTipi = "İşyeri"; ctx.KullaniciAdresleri.Add(a); ctx.SaveChanges();
                    _m.Error = "success"; _m.ReturnMsg = "Yeni Adres Eklendi."; _m.ReturnValue = a.Id;
                }
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = "Hata oluştu:" + ex.Message; _m.ReturnValue = 0;
            }

            return Json(_m, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdresSil(int Id)
        {
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                var _u = ctx.KullaniciAdresleri.SingleOrDefault(q => q.Id == Id);
                ctx.KullaniciAdresleri.Remove(_u); ctx.SaveChanges();
                _m.Error = "success"; _m.ReturnMsg = "Adres silindi"; _m.ReturnValue = 1;
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }
    }
}
