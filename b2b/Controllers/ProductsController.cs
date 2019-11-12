using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using B2B.Models.Entity;
using B2B.Dto;

namespace B2B.Controllers
{
    public partial class ProductsController : BaseController
    {
        CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();
        //
        // GET: /Products/

        public ActionResult ProductList()
        {
            var k = (from item in ctx.Kategoriler.Where(q => q.KategoriGoster == 1 && q.KategoriUst == 0).OrderBy(q => q.KategoriSira).ToList()
                     select new UrunlerDto.KategoriListele()
                     {
                         ID = item.Id,
                         KategoriBaslik = item.KategoriBaslik,
                         KategoriGoster = item.KategoriGoster,
                         KategoriSira = item.KategoriSira,
                         UstKategorisi = item.KategoriUst,
                         AltKategorileri = (from x in ctx.Kategoriler.Where(q => q.KategoriUst == item.Id).OrderBy(q => q.KategoriSira).ToList()
                                            select new UrunlerDto.KategoriListeleAlt()
                                            {
                                                ID = x.Id,
                                                KategoriBaslik = x.KategoriBaslik,
                                                KategoriGoster = x.KategoriGoster,
                                                KategoriSira = x.KategoriSira,
                                                UstKategorisi = x.KategoriUst
                                            }).ToList()
                     }).ToList();
            ViewBag.FavoriUrunler = FavoriUrunleriListele_ClassTaban(3);
            if (k != null)
            {
                ViewBag.KategoriBaslik = k[0].KategoriBaslik;
            }
            return View(k);
        }
        public string KategoriAdiVer(int KategoriId)
        {
            return ctx.Kategoriler.FirstOrDefault(q => q.Id == KategoriId).KategoriBaslik;
        }
        public PartialViewResult ProductDetails(int id)
        {
            PRODUCTS p = ctx.Urunler.FirstOrDefault(q => q.Id == id);
            p.UrunAciklama = Kernel.DecodeHTML(p.UrunAciklama);
            ViewBag.UrunKategoriName = OrtakFonksiyonlar.UrunKategoriNameVer(p.UrunKategorisi);
            // Aktivite _a = new Aktivite(); _a.AktiviteSTR = p.UrunAdi + " ürününü inceliyor"; _a.AktiviteTarihSaat = DateTime.Now; _a.AktiviteTipi = 3; _a.FirmaId = getAktifKullanici().FirmaId; _a.UserId = getAktifKullanici().UserId;
            var SunlaraGozatin = (from item in Kategori_Icinde_UrunBul(p.UrunKategorisi).ToList()
                                  select new UrunlerDto.BenzeyenUrunListesi()
                                  {
                                      AsilUrunId = item.UrunId,
                                      BenzeyenUrunBasligi = item.UrunAdi,
                                      BenzeyenUrunFiyati = item.UrunFiyat,
                                      BenzeyenUrunFotografi = item.UrunFoto,
                                      BenzeyenUrunId = item.UrunId
                                  }).ToList();
            ViewBag.SunlaraGozat = SunlaraGozatin;
            ViewBag.Desc = p.UrunAciklama;
            ViewBag.ogTitle = p.UrunAdi;
            ViewBag.ogImg = "https://cicekgider.com/Upload/imajlar/" + p.UrunFotografi;

            return PartialView(p);
        }
        public string UrunOzellikVer(int? OzellikId)
        {
            return ctx.UrunOzellikleri.Where(q => q.Id == OzellikId).Select(q => q.Ozellik).FirstOrDefault();
        }
        public string UrunAdiVer(int UrunId) // Ürün Adı Ver
        {
            var donendeger = ctx.Urunler.FirstOrDefault(q => q.Id == UrunId);
            if (donendeger != null)
            {
                return donendeger.UrunAdi;
            }
            else
            {
                return "ÜRÜN SİSTEMDEN KALDIRILMIŞ";
            }
        }
        public string UrunKategoriVer(int UstKategoriId)
        {
            return ctx.Kategoriler.Where(q => q.Id == UstKategoriId).Select(q => q.KategoriBaslik).FirstOrDefault();
        }
        public string UrunAgirlikSorgula(int AgirlikId)
        {
            return ctx.UrunlerAgirlikOlculeri.Where(q => q.Id == AgirlikId).Select(q => q.WeightShortCut).FirstOrDefault();
        }
        public string OdemeSekliVer(int odemeSekilId)
        {
            string p = ctx.OdemeSekilleri.Where(q => q.Id == odemeSekilId).Select(q => q.OdemeAdi).FirstOrDefault();
            return p;
        }
        public string FotografVer(int id)
        {
            var donendeger = ctx.Urunler.FirstOrDefault(q => q.Id == id);
            if (donendeger != null)
            {
                return donendeger.UrunFotografi;
            }
            else
            {
                return "ÜRÜN SİSTEMDEN KALDIRILMIŞ";
            }
        }
        public string MarkaVer(int urunId)
        {
            int j = ctx.Urunler.FirstOrDefault(q => q.Id == urunId).MarkaId;
            return ctx.Markalar.FirstOrDefault(q => q.Id == j).Marka;
        }
        public string FiyatVer(int urunId)
        {
            var donendeger = ctx.Urunler.FirstOrDefault(q => q.Id == urunId);
            if (donendeger != null)
            {
                return donendeger.UrunFiyat.ToString();
            }
            else
            {
                return "ÜRÜN SİSTEMDEN KALDIRILMIŞ";
            }
        }
        public string UrunKoduVer(int urunId)
        {
            var donendeger = ctx.Urunler.FirstOrDefault(q => q.Id == urunId);
            if (donendeger != null)
            {
                return donendeger.UrunKodu;
            }
            else
            {
                return "ÜRÜN SİSTEMDEN KALDIRILMIŞ";
            }
        }
        public decimal UrunFiyatVer(int UrunId)
        {
            return ctx.Urunler.Where(q => q.Id == UrunId).FirstOrDefault().UrunFiyat;
        }
        public bool MinSipKontrol(int UrunId, decimal eklenmenIstenenAdet)
        {
            decimal urununMinSipi = ctx.Urunler.Where(q => q.Id == UrunId).FirstOrDefault().MinimumSiparisAdeti;
            if (eklenmenIstenenAdet < urununMinSipi)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public decimal StokVer(int urunId)
        {
            decimal deger = 0;
            var urun = ctx.Urunler.SingleOrDefault(q => q.Id == urunId);
            try
            {
                deger = urun.Stok;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
            return deger;
        }

        public string UrunFotografiVer(int id)
        {
            string donendeger;
            var urun = ctx.Urunler.SingleOrDefault(q => q.Id == id);
            try
            {
                if (System.IO.File.Exists(Server.MapPath("~/Upload/imajlar/" + urun.UrunFotografi)))
                {
                    if (urun.UrunFotografi != "" || urun.UrunFotografi != null)
                        donendeger = urun.UrunFotografi;
                    else
                    {
                        donendeger = "keinFoto.png";
                    }
                }
                else
                {
                    donendeger = "keinFoto.png";
                }
            }
            catch (Exception)
            {
                donendeger = "keinFoto.png";
            }
            return donendeger;
        }

        public decimal IndirimliUrunuHesapla(int indirimId, decimal urunFiyat)
        {
            int indirimOrani = ctx.UrunlerIndirim.Where(q => q.Id == indirimId).FirstOrDefault().IndirimOrani;
            decimal temp = 100 - indirimOrani;
            decimal temp2 = temp / 100;
            return temp2 * urunFiyat;
        }
        public ICollection<PAYMENT_TYPES> OdemeSekliListesiDonustur(ICollection<PRODUCTS_PAYMENT_TYPES> o)
        {
            List<PAYMENT_TYPES> p = new List<PAYMENT_TYPES>();
            foreach (PRODUCTS_PAYMENT_TYPES item in o)
            {
                p.Add(new PAYMENT_TYPES()
                {
                    Id = item.Id,
                    OdemeAdi = OdemeSekliVer(int.Parse(item.OdemeSekli))
                });
            }
            return p;
        }
        public GonderimDetaylari TeslimDetaylariVer(int TeslimDetaylari)
        {
            PRODUCTS_DELIVERY_DETAILS p = ctx.TeslimatDetaylari.Where(q => q.Id == TeslimDetaylari).FirstOrDefault();
            GonderimDetaylari g = new GonderimDetaylari()
            {
                GonderimSuresi = p.TeslimatSuresi,
                GonderimUcreti = p.TeslimatUcreti,
                GonderimTipi = p.TeslimatTuru,
                Montaj = p.MontajTuru
            };
            return g;
        }
        public JsonResult ProductsSimilar(int id)
        {
            var BenzeyenUrunListesi = (from item in ctx.UrunlerBenzer.Where(q => q.UrunId == id).ToList()
                                       select new UrunlerDto.BenzeyenUrunListesi()
                                       {
                                           AsilUrunId = item.UrunId,
                                           BenzeyenUrunBasligi = UrunAdiVer(item.UrunId),
                                           BenzeyenUrunFiyati = UrunFiyatVer(item.UrunId),
                                           BenzeyenUrunFotografi = UrunFotografiVer(item.UrunId),
                                           BenzeyenUrunId = item.BenzeyenUrunId
                                       }).ToList();
            return Json(BenzeyenUrunListesi, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FavoriUrunleriListele(int Adet) // varsayılan adet 1
        {
            if (Adet == null || Adet <= 0) { Adet = 1; }
            var favList = (from item in ctx.Urunler.Where(q => q.OneCikanUrun == 1).ToList()
                           select new UrunlerDto.OneCikanUrunListesi()
                           {
                               Indirim = 0,
                               IndirimliFiyat = 0,
                               UrunAdi = item.UrunAdi,
                               UrunFiyat = item.UrunFiyat,
                               UrunFoto = UrunFotografiVer(item.Id),
                               UrunId = item.Id
                           }).ToList();
            return Json(favList, JsonRequestBehavior.AllowGet);
        }
        public ICollection<UrunlerDto.OneCikanUrunListesi> FavoriUrunleriListele_ClassTaban(int Adet) // varsayılan adet 1
        {
            if (Adet == null || Adet <= 0) { Adet = 1; }
            var favList = (from item in ctx.Urunler.Where(q => q.OneCikanUrun == 1).ToList()
                           select new UrunlerDto.OneCikanUrunListesi()
                           {
                               Indirim = 0,
                               IndirimliFiyat = 0,
                               UrunAdi = item.UrunAdi,
                               UrunFiyat = item.UrunFiyat,
                               UrunFoto = UrunFotografiVer(item.Id),
                               UrunId = item.Id
                           }).ToList();
            return favList;
        }
        public PartialViewResult ProductList_ByCategory(int id)
        {
            var k = (from item in ctx.Kategoriler.Where(q => q.KategoriGoster == 1 && q.KategoriUst == 0).OrderBy(q => q.KategoriSira).ToList()
                     select new UrunlerDto.KategoriListele()
                     {
                         ID = item.Id,
                         KategoriBaslik = item.KategoriBaslik,
                         KategoriGoster = item.KategoriGoster,
                         KategoriSira = item.KategoriSira,
                         UstKategorisi = item.KategoriUst,
                         AltKategorileri = (from x in ctx.Kategoriler.Where(q => q.KategoriUst == item.Id).OrderBy(q => q.KategoriSira).ToList()
                                            select new UrunlerDto.KategoriListeleAlt()
                                            {
                                                ID = x.Id,
                                                KategoriBaslik = x.KategoriBaslik,
                                                KategoriGoster = x.KategoriGoster,
                                                KategoriSira = x.KategoriSira,
                                                UstKategorisi = x.KategoriUst
                                            }).ToList()
                     }).ToList();

            //ICollection<UrunlerDto.KategoriUrunListele> KategoriUrunler = (from item in ctx.Urunler.Where(q => q.UrunKategorisi == id).OrderBy(q => q.Id).ToList()
            //                       select new UrunlerDto.KategoriUrunListele()
            //                    {
            //                        UrunAdi = UrunAdiVer(item.Id),
            //                        UrunFiyat = item.UrunFiyat,
            //                        UrunFoto = UrunFotografiVer(item.Id),
            //                        UrunId = item.Id
            //                    }).ToList();

            List<UrunlerDto.KategoriUrunListele> KategoriUrunler = Kategori_Icinde_UrunBul(id);
            ViewBag.KategoriUrunler = KategoriUrunler;
            return PartialView(k);
        }
        public List<UrunlerDto.KategoriUrunListele> Kategori_Icinde_UrunBul(int catId)
        {
            List<UrunlerDto.KategoriUrunListele> urunListesi = new List<UrunlerDto.KategoriUrunListele>();

            string[] catList;
            foreach (var item in ctx.Urunler.ToList())
            {
                if (item.AlternatifKategoriler != null)
                {
                    catList = item.AlternatifKategoriler.Split(',');

                    foreach (var cat in catList)
                    {
                        if (int.Parse(cat) == catId)
                        {
                            urunListesi.Add(new UrunlerDto.KategoriUrunListele()
                            {
                                UrunAdi = UrunAdiVer(item.Id),
                                UrunFiyat = UrunFiyatVer(item.Id),
                                UrunFoto = UrunFotografiVer(item.Id),
                                UrunId = item.Id
                            });

                        }

                    }
                }

            }

            return urunListesi;
        }
        public ViewResult Search(string q)
        {
            q = Request.Form["q"];
            List<Dto.UrunlerDto.AramaSonucuUrunleri> q_list = new List<UrunlerDto.AramaSonucuUrunleri>();
            try
            {
                q_list = (from x in ctx.Urunler.Where(s => s.UrunAdi.Contains(q)).ToList()
                          select new UrunlerDto.AramaSonucuUrunleri()
                          {
                              UrunAdi = x.UrunAdi,
                              UrunFiyati = x.UrunFiyat,
                              UrunFotografi = x.UrunFotografi,
                              UrunId = x.Id,
                              UrunKodu = x.UrunKodu
                          }).ToList();
                if (q_list.Count == 0)
                {
                    q_list = null;
                    q_list = (from x in ctx.Urunler.Where(s => s.UrunKodu.Contains(q)).ToList()
                              select new UrunlerDto.AramaSonucuUrunleri()
                              {
                                  UrunAdi = x.UrunAdi,
                                  UrunFiyati = x.UrunFiyat,
                                  UrunFotografi = x.UrunFotografi,
                                  UrunId = x.Id,
                                  UrunKodu = x.UrunKodu
                              }).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
            var _k = (from item in ctx.Kategoriler.Where(kat => kat.KategoriGoster == 1 && kat.KategoriUst == 0).OrderBy(kat => kat.KategoriSira).ToList()
                      select new UrunlerDto.KategoriListele()
                      {
                          ID = item.Id,
                          KategoriBaslik = item.KategoriBaslik,
                          KategoriGoster = item.KategoriGoster,
                          KategoriSira = item.KategoriSira,
                          UstKategorisi = item.KategoriUst,
                          AltKategorileri = (from x in ctx.Kategoriler.Where(kat => kat.KategoriUst == item.Id).OrderBy(kat => kat.KategoriSira).ToList()
                                             select new UrunlerDto.KategoriListeleAlt()
                                             {
                                                 ID = x.Id,
                                                 KategoriBaslik = x.KategoriBaslik,
                                                 KategoriGoster = x.KategoriGoster,
                                                 KategoriSira = x.KategoriSira,
                                                 UstKategorisi = x.KategoriUst
                                             }).ToList()
                      }).ToList();
            ViewBag.Kategoriler = _k;
            ViewBag.FavoriUrunler = FavoriUrunleriListele_ClassTaban(3);
            return View(q_list);
        }
        public JsonResult mailtest(string to, string icerik)
        {
            string durum = "";
            try
            {
                Kernel.Mail("info@nfo.com.tr", "yeni sipariş");
                durum = "gönderdim";
            }
            catch (Exception)
            {
                throw;
            }
            return Json(durum, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckNew_Detail()
        {
            YeniSiparisListesi _y = new YeniSiparisListesi();
            try
            {
                var _x = (from i in ctx.Siparisler.Where(q => q.SiparisDurumu == 0).ToList()
                          select new YeniSiparis()
                          {
                              Id = i.Id,
                              Goruldu = 0,
                              SiparisNo = i.SiparisNo,
                              SiparisVeren = "test"
                          });
                _y.NewOrders = new List<YeniSiparis>();
                _y.NewOrders = _x.ToList();
            }
            catch (Exception ex)
            {
                return Json("bir hata oluştu, lütfen şu hatayı bildirin: " + ex.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json(_y.NewOrders, JsonRequestBehavior.AllowGet);
        }
    }

}
