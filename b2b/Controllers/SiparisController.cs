using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Entity;
using System.Net;
using RazorEngine;
using RazorEngine.Templating;

namespace B2B.Controllers
{
    public class SiparisController : BaseController
    {
        //
        // GET: /Siparis/
        public ProductsController UrunlerKontrolcusu = new ProductsController();
        public CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();
        public SepetController SepetKontrolcusu = new SepetController();
        public string SiparisDurumver(int DurumId)
        {
            return ctx.SiparisDurumlari.Where(q => q.Aktif == DurumId).FirstOrDefault().SiparisDurumu;
        }
        public ActionResult GecmisSiparisleriListele()
        {
            UserHolder UserInfo = getAktifKullanici();
            //Şu Şekilde de kullanılır beybi
            //Boşuna Döndürme garibi kayıtlar arasında.


            var GecmisSiparis = (from item in ctx.Siparisler.Where(q => q.FirmaId == UserInfo.FirmaId).OrderByDescending(q => q.Id).ToList()
                                 select new GecmisSiparisAyrintilari()
                                 {
                                     Id = item.Id,
                                     SiparisDurumKodu = item.SiparisDurumu,
                                     SiparisKodu = item.SiparisNo,
                                     SiparisTarihi = item.SiparisTarihi,
                                     SiparisToplami = item.SiparisToplami,
                                     SiparisDurum = OrtakFonksiyonlar.SiparisDurumuVer(item.SiparisDurumu),
                                     SiparisUrunleri = CastToSpesificClass_GecmisSiparisUrunler(item.SiparisUrunleri)
                                 }).ToList();

            return View(GecmisSiparis);
        }
        public List<GecmisSiparisUrunler> CastToSpesificClass_GecmisSiparisUrunler(ICollection<ORDER_PRODUCTS> o)
        {
            List<GecmisSiparisUrunler> YeniListe = new List<GecmisSiparisUrunler>();
            foreach (var item in o)
            {
                GecmisSiparisUrunler g = new GecmisSiparisUrunler();
                g.Adet = item.Adet;
                g.Tutar = item.KalemTutar;
                g.UrunBaslik = UrunlerKontrolcusu.UrunAdiVer(item.UrunId);
                g.UrunId = item.UrunId;
                YeniListe.Add(g);
            }

            return YeniListe;
        }
        public ActionResult MarkalariListele()
        {
            ViewBag.Urunler = ctx.Urunler.Count();
            return View();
        }
        public string SiparisNoOlusturucu()
        {
            string random = DateTime.Now.ToShortTimeString();
            Random rnd = new Random();
            int randomizer = rnd.Next(1, 101);
            random = "KK#" + random + randomizer.ToString();
            random = random.Replace(':', 'S');
            return random.Replace('.', 'A');
        }

        public ActionResult SiparisIncele(int Id)
        {
            ORDERS Order = ctx.Siparisler.Where(q => q.Id == Id).FirstOrDefault();
            SiparisAyrintilari Siparis = new SiparisAyrintilari();
            Siparis.SiparisUrunleri = new List<GecmisSiparisUrunler>();

            Siparis.Iskonto = Order.SiparisIskontoOrani;
            Siparis.Kdv = Order.KdvOrani;
            Siparis.SiparisId = Order.Id;
            Siparis.SiparisKodu = Order.SiparisNo;
            Siparis.SiparisTarihi = Order.SiparisTarihi;
            Siparis.SiparisToplami = Order.SiparisToplami.GetValueOrDefault(0);
            Siparis.IskontoTutar = Order.IskontoTutar;

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

        public JsonResult MarkaVer()
        {
            List<BRANDS> Brands = ctx.Markalar.Where(q => q.MarkaAltId == 0).OrderBy(q => q.Marka).ToList();
            return Json(Brands, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ModelVer(int MarkaId)
        {
            var Modeller = (from item in ctx.Markalar.Where(q => q.MarkaAltId == MarkaId).OrderByDescending(q => q.Id).ToList()
                            select new BRANDS()
                            {
                                Id = item.Id,
                                Marka = item.Marka,
                                MarkaAltId = item.MarkaAltId
                            }).ToList();
            return Json(Modeller, JsonRequestBehavior.AllowGet);
        }
        public JsonResult KategoriVer()
        {
            List<CATEGORIES> Kategoriler = ctx.Kategoriler.OrderBy(q => q.KategoriSira).ToList();
            return Json(Kategoriler, JsonRequestBehavior.AllowGet);
        }
        public bool EksiBakiyeIzinKontrol(int firmaId)
        {
            MuhasebeController Muhasebe = new MuhasebeController();
            bool Izin = false;
            COMPANIES Company = ctx.Firmalar.Where(q => q.Id == firmaId).FirstOrDefault();
            if (KalanBakiyeSorgula(firmaId, 1) < 0)
            {
                if (Company.CariHesapIzni == 1)
                {
                    Izin = true;
                }
                else
                {
                    Izin = false;
                }
            }
            else
            {
                Izin = true;
            }
            return Izin;
        }
        public decimal KalanBakiyeSorgula(int firmaId, int kalanHesaplansinmi)
        {
            SepetController _sepetC = new SepetController();
            decimal donenDeger;
            MUHASEBE m = ctx.Muhasebe.FirstOrDefault(q => q.FirmaId == firmaId);

            if (kalanHesaplansinmi == 0)
            {
                donenDeger = m.Bakiye;
            }
            else
            {
                SepetDto s = _sepetC.sepetAl();
                decimal sepetToplam = 0;
                sepetToplam = s.Urunler.Sum(q => (q.UrunFiyat) * q.Adet) + s.Urunler.Sum(q => (q.UrunFiyat * 18 / 100) * q.Adet);
                donenDeger = m.Bakiye - sepetToplam;
            }
            return donenDeger;
        }
        public JsonResult YeniSiparisEkle(string odemeTipi, string AliciAdSoyad, string SiparisiVeren, string SiparisNotlari, string adres)
        {
            MethodStatusDto m = new MethodStatusDto();
            UserHolder UserInfo = getAktifKullanici();
            SepetController _sepetc = new SepetController();
            /*/ Ürün Sepeti döngüsü stok kontrolü
    /*/
            try
            {
                if (isAvailable(_sepetc.sepetAl()))
                {
                    if (odemeTipi == "CARİ")
                    {
                        if (EksiBakiyeIzinKontrol(UserInfo.FirmaId))
                        {
                            try
                            {
                                SiparisEkle(odemeTipi, AliciAdSoyad, SiparisiVeren, SiparisNotlari, adres, _sepetc.sepetAl(), "0");
                                m.Error = "success";
                                m.ReturnMsg = "Siparişiniz başarıyla oluşturuldu.";
                                m.ReturnValue = 1;
                            }
                            catch (Exception e)
                            {
                                m.Error = "error";
                                m.ReturnMsg = e.Message;
                                m.ReturnValue = 0;
                            }
                        }
                        else
                        {
                            m.Error = "BakiyeYetersiz";
                            m.ReturnMsg = "Bakiyeniz Yetersiz. Lütfen hesabınıza para yükleyiniz";
                            m.ReturnValue = 0;
                        }
                    }
                    else if (odemeTipi == "HAVALE")
                    {
                        try
                        {
                            SiparisEkle(odemeTipi, AliciAdSoyad, SiparisiVeren, SiparisNotlari, adres, _sepetc.sepetAl(),"0");
                            m.Error = "HAVALE";
                            m.ReturnMsg = "Siparişiniz başarıyla oluşturuldu";
                            m.ReturnValue = 1;
                        }
                        catch (Exception e)
                        {
                            m.Error = "error";
                            m.ReturnMsg = e.InnerException.Message;
                            m.ReturnValue = 0;
                        }

                    }
                    else if (odemeTipi == "KREDİ KARTI")
                    {
                        m.Error = "KK";
                        m.ReturnMsg = "Ödeme sayfasına yönlendiriliyorsunuz.";
                        m.ReturnValue = 1;
                    }
                    else if (odemeTipi == "NAKİT")
                    {
                        SiparisEkle(odemeTipi, AliciAdSoyad, SiparisNotlari, SiparisiVeren, adres, _sepetc.sepetAl(),"0");
                        m.Error = "NAKİT";
                        m.ReturnMsg = "Siparişiniz başarıyla oluşturuldu";
                        m.ReturnValue = 1;
                    }
                }
            }
            catch (Exception e)
            {
                m.Error = "error"; m.ReturnMsg = e.Message; m.ReturnValue = 0;
            }
            return Json(m, JsonRequestBehavior.AllowGet);
        }
        public bool SiparisEkle(string odemeTipi, string AliciAdSoyad, string SiparisVerenAdSoyad, string siparisNotlari, string adres, SepetDto sepet, string uniq)
        {

            bool sonuc = false;
            MethodStatusDto m = new MethodStatusDto();
            try
            {
                UserHolder UserInfo = getAktifKullanici();
                ORDERS o = new ORDERS();

                SepetHesapAraci SepetHesabi = SepetKontrolcusu.YeniSiparisIcinSepetHesabi(sepet, UserInfo, 18);

                string siparisNo = SiparisNoOlusturucu();

                o.SiparisNo = siparisNo;
                o.SiparisTarihi = DateTime.Now;

                List<SepetUrunlerDto> sepetUrunleri = new List<SepetUrunlerDto>();

                ORDERS Siparis = new ORDERS();
                Siparis.SiparisNo = siparisNo;
                Siparis.SiparisTarihi = DateTime.Now;
                Siparis.SiparisToplami = SepetHesabi.GenelTutar;
                Siparis.IskontoTutar = SepetHesabi.IskontoTutari;
                Siparis.SiparisVeren = UserInfo.UserId;
                Siparis.KdvOrani = 18;
                Siparis.FirmaId = UserInfo.FirmaId;
                Siparis.SiparisDurumu = 0;
                Siparis.OdemeTipi = odemeTipi;
                Siparis.SadeceTarih = DateTime.Today;
                Siparis.SiparisNotlari = "Alıcı Adı Soyadı: <b> " + AliciAdSoyad + "</b><br>Not: " + siparisNotlari;
                Siparis.SiparisiVerenAdSoyad = SiparisVerenAdSoyad;
                Siparis.SiparisAdresi = adres;
                Siparis.SiparisIskontoOrani = SepetHesabi.IskontoOrani;
                Siparis.ORDER_PAYMENT = uniq;
                ctx.Siparisler.Add(Siparis);
                ctx.SaveChanges();

                foreach (var item in sepet.Urunler)
                {
                    ORDER_PRODUCTS OrderProds = new ORDER_PRODUCTS();
                    OrderProds.Adet = item.Adet;
                    OrderProds.KalemTutar = item.UrunFiyat * item.Adet;
                    OrderProds.UrunId = item.UrunId;
                    OrderProds.SiparisNo = Siparis.Id;

                    //
                    //
                    //
                    //stoktan düşme kontrol ve action
                    if (isStoktanDus(item.UrunId))
                        StoktanDus(item.UrunId, item.Adet);

                    ctx.SiparisUrunleri.Add(OrderProds);
                }
                ctx.SaveChanges();

                /*/ aktiviteyi logla /*/
                Aktivite Aktivite = new Aktivite();
                Aktivite.AktiviteSTR = "" + UserInfo.FirmaUnvani + " yeni bir sipariş oluşturdu.";
                Aktivite.AktiviteTipi = 5; Aktivite.FirmaId = UserInfo.FirmaId;
                Aktivite.AktiviteTarihSaat = DateTime.Now;
                Aktivite.UserId = getAktifKullanici().UserId;
                OrtakFonksiyonlar.Log(Aktivite);
                /*/ aktiviteyi logla /*/

                TRANSACTIONS Transaction = new TRANSACTIONS();
                Transaction.Aciklama = siparisNo + " sipariş kodlu alışveriş.";
                Transaction.FirmaId = UserInfo.FirmaId;
                Transaction.GirdiTipi = 0;
                Transaction.Tutar = SepetHesabi.GenelTutar;
                Transaction.TarihSaat = DateTime.Now;
                Transaction.UserId = UserInfo.UserId;
                ctx.GirdiCiktilar.Add(Transaction);
                ctx.SaveChanges();
                //
                // Bakiye'den düş
                MUHASEBE Muhasebe = ctx.Muhasebe.Single(q => q.FirmaId == UserInfo.FirmaId);
                Muhasebe.Bakiye = Muhasebe.Bakiye - SepetHesabi.GenelTutar;
                Muhasebe.SonGuncelleme = DateTime.Now;
                ctx.SaveChanges();
                sonuc = true;
                //send email
                try
                {
                    
                }
                catch (Exception exc)
                {
                    //problem yok devam sadece mail atılamadı
                    /*/ hatayı logla /*/
                    Aktivite errorLog = new Aktivite(); errorLog.AktiviteSTR = "hata: " + exc.Message.ToString(); errorLog.AktiviteTipi = 1; errorLog.FirmaId = 1; errorLog.AktiviteTarihSaat = DateTime.Now; errorLog.UserId = getAktifKullanici().UserId; OrtakFonksiyonlar.Log(Aktivite);
                    /*/ hatayı logla /*/
                }
            }
            catch (Exception exstr)
            {
                /*/ hatayı logla /*/
                Aktivite Aktivite = new Aktivite();
                Aktivite.AktiviteSTR = "hata: " + exstr.Message.ToString();
                Aktivite.AktiviteTipi = 1; Aktivite.FirmaId = 1;
                Aktivite.AktiviteTarihSaat = DateTime.Now;
                Aktivite.UserId = getAktifKullanici().UserId;
                OrtakFonksiyonlar.Log(Aktivite);
                /*/ hatayı logla /*/
            }
            return sonuc;
        }
        public bool isStoktanDus(int urunId)
        {
            return ctx.Urunler.SingleOrDefault(q => q.Id == urunId).StoktanDus == 1 ? true : false;
        }
        public void StoktanDus(int urunId, decimal dusulecekAdet)
        {
            PRODUCTS Urun = ctx.Urunler.SingleOrDefault(q => q.Id == urunId);
            Urun.Stok = Urun.Stok - dusulecekAdet;
            ctx.SaveChanges();
        }
        public bool isAvailable(SepetDto Sepet)
        {
            bool donenDeger = true;
            try
            {
                foreach (var item in Sepet.Urunler)
                {
                    if (UrunlerKontrolcusu.StokVer(item.UrunId) - item.Adet < 0)
                    {
                        donenDeger = false; throw new ArgumentException("Sepetinizdeki bazı ürünler için stok yetersiz.\n" + UrunlerKontrolcusu.UrunAdiVer(item.UrunId));
                    }
                }
            }
            catch (Exception e)
            {
                donenDeger = false; throw new ArgumentException("Üzgünüz: " + e.Message);
            }
            return donenDeger;
        }
        public ActionResult MarkayaGoreSiparis()
        {
            MarkayaGoreAramaSinifi Sinif = new MarkayaGoreAramaSinifi();
            Sinif.Markalar = ctx.Markalar.ToList();

            if (Request.QueryString["Marka"] != null)
            {
                int s = int.Parse(Request.QueryString["Marka"]);
                Sinif.Urunler = ctx.Urunler.Where(q => q.MarkaId == s).ToList();
            }
            else
            {
                Sinif.Urunler = ctx.Urunler.Take(15).ToList();
            }
            return View(Sinif);
        }
        public ActionResult SiparisEmailTemplate(int Id)
        {
            SiparisAyrintilari Siparis = new SiparisAyrintilari();
            try
            {
                ORDERS Order = ctx.Siparisler.Where(q => q.Id == Id).FirstOrDefault();
                Siparis.SiparisUrunleri = new List<GecmisSiparisUrunler>();

                Siparis.Iskonto = Order.SiparisIskontoOrani;
                Siparis.Kdv = Order.KdvOrani;
                Siparis.SiparisId = Order.Id;
                Siparis.SiparisKodu = Order.SiparisNo;
                Siparis.SiparisTarihi = Order.SiparisTarihi;
                Siparis.SiparisToplami = Order.SiparisToplami.GetValueOrDefault(0);
                Siparis.IskontoTutar = Order.IskontoTutar;

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

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return View(Siparis);
        }
        public ActionResult Basarili()
        {
            return View();
        }
        public ActionResult Problem()
        {
            return View();
        }
    }
}
