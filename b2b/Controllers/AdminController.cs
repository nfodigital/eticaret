using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Entity;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Net;
using B2B.OAuth;

namespace B2B.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult Index()
        {
            CommonFunctionsController Cf = new CommonFunctionsController();
            DateTime Dun = DateTime.Today.AddDays(-1);
            DateTime GecenHafta = DateTime.Today.AddDays(-7).Date;
            DateTime GecenAy = DateTime.Today.AddDays(-DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).Date;
            AdminIndex AdminIndex_Classes = new AdminIndex(); AdminIndex_Classes.AdminUyarilari = new List<string>();
            var _Siparisler = ctx.Siparisler.ToList();

            var SonSiparisler = (from item in ctx.Siparisler.Where(q => q.SiparisDurumu == 0).OrderByDescending(q => q.Id).ToList()
                                 select new AdminHomePage_SonSiparisler()
                                 {
                                     Id = item.Id,
                                     SiparisDurumu = item.SiparisDurumu == 0 ? "Onay Bekliyor" : "Onaylandı, Gönderim Bekliyor",
                                     SiparisTarihi = item.SadeceTarih,
                                     SiparisToplami = item.SiparisToplami.GetValueOrDefault(0),
                                     Firma = Cf.FirmaAdiVer(item.FirmaId)
                                 }).ToList();

            AdminIndex_Classes.SonSiparisler = SonSiparisler;
            AdminIndex_Classes.ToplamSatisTutar = _Siparisler.Sum(q => q.SiparisToplami).GetValueOrDefault(0);
            AdminIndex_Classes.AdetSiparis = _Siparisler.Count(q => q.SadeceTarih == DateTime.Today);
            AdminIndex_Classes.MusteriCount = ctx.Firmalar.Count();
            AdminIndex_Classes.TotalVisits = ctx.Firmalar.Sum(q => q.ZiyaretSayisi);

            AdminIndex_Classes.AdetSiparis_BugunWidget = _Siparisler.Count(q => q.SiparisTarihi.ToShortDateString() == DateTime.Today.ToShortDateString());
            AdminIndex_Classes.TutarSiparis_BugunWidget = _Siparisler.Where(q => q.SiparisTarihi.ToShortDateString() == DateTime.Today.ToShortDateString()).Sum(q => q.SiparisToplami).GetValueOrDefault(0);
            AdminIndex_Classes.DunkuSatis_BugunWidget = _Siparisler.Where(q => q.SiparisTarihi.ToShortDateString() == Dun.ToShortDateString()).Sum(q => q.SiparisToplami).GetValueOrDefault(0);
            AdminIndex_Classes.TutarSatis_GecenHafta_BugunWidget = _Siparisler.Where(q => q.SadeceTarih.Date > GecenHafta && q.SadeceTarih.Date < DateTime.Now.Date).Sum(q => q.SiparisToplami).GetValueOrDefault(0);
            AdminIndex_Classes.TutarSatisTutar_GecenAy_BugunWidget = _Siparisler.Where(q => q.SadeceTarih.Date > GecenAy && q.SadeceTarih.Date < DateTime.Now.Date).Sum(q => q.SiparisToplami).GetValueOrDefault(0);

            PRODUCTS _pStokKontrol = ctx.Urunler.Where(q => q.StoktanDus == 1 && q.Stok <= 0).FirstOrDefault();
            if (_pStokKontrol != null) { AdminIndex_Classes.AdminUyarilari.Add("Dikkat! Stoğu BİTMİŞ ürünler mevcut, ürünlere gözatmanız gerekmektedir."); }
            _pStokKontrol = ctx.Urunler.Where(q => q.StoktanDus == 1 && q.Stok < 10).FirstOrDefault();
            if (_pStokKontrol != null) { AdminIndex_Classes.AdminUyarilari.Add("Dikkat! Stoğu AZALAN ürünler mevcut, ürünlere gözatmanız gerekmektedir."); _pStokKontrol = null; }
            return View(AdminIndex_Classes);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult SiparisleriListele()
        {
            CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();
            ProductsController UrunFonksiyonlari = new ProductsController();

            var Siparisler = (from ar in ctx.Siparisler.OrderByDescending(q => q.Id).ToList()
                              select new AdminHomePage_SonSiparisler
                              {
                                  Firma = OrtakFonksiyonlar.FirmaAdiVer(ar.FirmaId),
                                  Id = ar.Id,
                                  SiparisDurumu = OrtakFonksiyonlar.SiparisDurumuVer(ar.SiparisDurumu),
                                  SiparisTarihi = ar.SadeceTarih,
                                  SiparisToplami = ar.SiparisToplami.GetValueOrDefault(0),
                                  UrunlerToplami = ar.SiparisUrunleri.Count(),
                                  SiparisKodu = ar.SiparisNo
                              }).ToList();

            return View(Siparisler);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult SiparisDuzenle(int Id)
        {
            SiparisDuzenleDto SiparisDuzenle = new SiparisDuzenleDto();
            SiparisDuzenle.Siparis = ctx.Siparisler.FirstOrDefault(q => q.Id == Id);
            SiparisDuzenle.SiparisDurumlari = ctx.SiparisDurumlari.ToList();

            return View(SiparisDuzenle);
        }
        //Hesaplar
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult HesaplariListele(string OrderBy)
        {
            AdminHesaplarIndex AdminHesaplarIndex = new AdminHesaplarIndex();

            AdminHesaplarIndex.Firmalar = OrderBy == "BakiyeEksi" ? ctx.Firmalar.OrderBy(q => q.FirmaMuhasebe.FirstOrDefault().Bakiye).ToList()
                : OrderBy == "BakiyeArti" ? ctx.Firmalar.OrderByDescending(q => q.FirmaMuhasebe.FirstOrDefault().Bakiye).ToList()
                : ctx.Firmalar.OrderByDescending(q => q.Id).ToList();

            AdminHesaplarIndex.Aktiviteler = ctx.Aktiviteler.OrderByDescending(q => q.Id).ToList();

            return View(AdminHesaplarIndex);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult HesapDetay(int HesapId)
        {
            COMPANIES Firma = ctx.Firmalar.Where(q => q.Id == HesapId).FirstOrDefault();
            return View(Firma);
        }
        [HttpPost]
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FirmaKaydetveyaGuncelle(COMPANIES C)
        {
            MethodStatusDto Method = new MethodStatusDto();
            var Firma = ctx.Firmalar.SingleOrDefault(q => q.Id == C.Id);
            try
            {
                if (Firma != null)
                {
                    Firma.YetkiliKisi = C.YetkiliKisi; Firma.FirmaAdi = C.FirmaAdi; Firma.FirmaUnvani = C.FirmaUnvani; Firma.Sehir = C.Sehir;
                    Firma.FirmaTelefon = C.FirmaTelefon; Firma.Faks = C.Faks; Firma.FirmaIskonto = C.FirmaIskonto; Firma.CariHesapIzni = C.CariHesapIzni;
                    Firma.VergiDairesi = C.VergiDairesi; Firma.VergiNo = C.VergiNo;
                    ctx.SaveChanges(); Method.Error = "success"; Method.ReturnMsg = "Firma düzenleme başarılı"; Method.ReturnValue = Firma.Id;
                }
                else
                {
                    MUHASEBE MuhasebeKaydi = new MUHASEBE();
                    MuhasebeKaydi.Bakiye = 0;
                    MuhasebeKaydi.FirmaId = C.Id;
                    MuhasebeKaydi.SonGuncelleme = DateTime.Now;
                    C.KayitTarihi = DateTime.Now.Date;
                    C.LastVisit = DateTime.Now;
                    ctx.Firmalar.Add(C);
                    ctx.Muhasebe.Add(MuhasebeKaydi);
                    ctx.SaveChanges();
                    Aktivite _a = new Aktivite(); _a.AktiviteSTR = "Firma kaydı başarılı: " + C.FirmaAdi + " firma kayıt numarası :" + C.Id; _a.AktiviteTarihSaat = DateTime.Now; _a.AktiviteTipi = 1; _a.UserId = 0; _a.FirmaId = C.Id;
                    Kernel.MakeLog(_a);
                    Method.Error = "success";
                    Method.ReturnMsg = "Kayıt Tamamlandı"; Method.ReturnValue = C.Id;
                }
            }
            catch (Exception e)
            {
                Method.Error = "error";
                Method.ReturnMsg = e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult SiparisDurumDegis(int Id, int SiparisDurumu)
        {
            MethodStatusDto JsonMethod = new MethodStatusDto();

            try
            {
                ORDERS Order = ctx.Siparisler.Where(q => q.Id == Id).FirstOrDefault();
                Order.SiparisDurumu = SiparisDurumu;
                ctx.SaveChanges();

                JsonMethod.Error = "0";
                JsonMethod.ReturnMsg = Id + " için Sipariş durumu değiştirildi";
                JsonMethod.ReturnValue = Id;
            }
            catch (Exception e)
            {
                JsonMethod.Error = "1";
                JsonMethod.ReturnMsg = "Hata oluştu" + e.Message;
                JsonMethod.ReturnValue = 0;
            }

            return Json(JsonMethod, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult KullanicilariListele()
        {
            List<USERS> Kullanicilar = ctx.Kullanicilar.ToList();
            return View(Kullanicilar);
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult KullaniciGoster(int id)
        {
            USERS KullaniciBilgileri = ctx.Kullanicilar.Where(q => q.Id == id).FirstOrDefault();
            return PartialView(KullaniciBilgileri);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult KullaniciVer(int UserId)
        {
            USERS Kullanici = ctx.Kullanicilar.Where(q => q.Id == UserId).FirstOrDefault();
            return Json(Kullanici, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FirmalariListele()
        {
            return Json(ctx.Firmalar.ToList(), JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult YeniKullaniciEkle()
        {
            return PartialView();
        }
        [HttpPost]
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult KullaniciEkle(USERS User)
        {
            User.LastLogin = DateTime.Now;
            User.KayitTarihi = DateTime.Now;
            User.KullaniciYetkisi = "1";

            MethodStatusDto Method = new MethodStatusDto();
            try
            {
                ctx.Kullanicilar.Add(User);
                ctx.SaveChanges();
                Method.Error = "success";
                Method.ReturnMsg = "Yeni kullanıcı eklendi.";
                Method.ReturnValue = User.Id;
            }
            catch (Exception e)
            {
                Method.Error = "error";
                Method.ReturnMsg = e.Message;
                Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult KullaniciDuzenle(USERS User)
        {
            MethodStatusDto Method = new MethodStatusDto();
            USERS ExUser = ctx.Kullanicilar.SingleOrDefault(q => q.Id == User.Id);
            try
            {
                ExUser.Adi = User.Adi;
                ExUser.Aktif = User.Aktif;
                ExUser.Eposta = User.Eposta;
                ExUser.FirmaId = User.FirmaId;
                ExUser.USERNAME = User.USERNAME;
                ExUser.PASSWORD = User.PASSWORD;
                ctx.SaveChanges();
                Method.Error = "success";
                Method.ReturnMsg = "Değişiklikler Kaydedildi";
                Method.ReturnValue = User.Id;
            }
            catch (Exception e)
            {
                Method.Error = "error";
                Method.ReturnMsg = e.Message;
                Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult MuhasebeIslemleri_Index(int id, string Filter)
        {

            COMPANIES Company = ctx.Firmalar.SingleOrDefault(q => q.Id == id);
            return View(Company);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult SistemAyarlari()
        {
            return View();
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult UyariYazilari()
        {
            SISTEM_AYARLARI SistemAyarlari = ctx.SistemAyarlari.FirstOrDefault();
            return View(SistemAyarlari);
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult SiparisYazdir(int Id)
        {
            SiparisDuzenleDto SiparisDuzenle = new SiparisDuzenleDto();
            SiparisDuzenle.Siparis = ctx.Siparisler.Where(q => q.Id == Id).FirstOrDefault();
            SiparisDuzenle.SiparisDurumlari = ctx.SiparisDurumlari.ToList();

            return PartialView(SiparisDuzenle);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult UyariEkle(int musteriId, string Mesaj, string validDate, string Update)
        {
            MethodStatusDto Method = new MethodStatusDto();
            UYARILAR Uyari = new UYARILAR();
            DateTime gecerlilikTarihi;
            DateTime.TryParse(validDate, out gecerlilikTarihi);
            try
            {
                if (Update == "false")
                {
                    Uyari.Uyari = Mesaj; Uyari.UyariFirmaId = getAktifKullanici().FirmaId; Uyari.UyariOncelik = 1; Uyari.UyariTipi = "odemeUyarisi";
                    Uyari.GecerlilikTarihi = gecerlilikTarihi;
                    ctx.Uyarilar.Add(Uyari); ctx.SaveChanges();
                    Method.Error = "success"; Method.ReturnMsg = "Ödeme uyarınız '" + Mesaj + "' şeklinde eklenmiştir."; Method.ReturnValue = Uyari.Id;
                }
                else if (Update == "true")
                {
                    Uyari = ctx.Uyarilar.Where(q => q.UyariTipi == "odemeUyarisi" && q.UyariFirmaId == musteriId).FirstOrDefault();
                    Uyari.Uyari = Mesaj; Uyari.GecerlilikTarihi = gecerlilikTarihi;
                    ctx.SaveChanges();
                    Method.Error = "success"; Method.ReturnMsg = "Ödeme uyarınız '" + Mesaj + "' şeklinde güncellenmiştir."; Method.ReturnValue = Uyari.Id;
                }
                else if (Update == "Delete")
                {
                    Uyari = ctx.Uyarilar.Where(q => q.UyariTipi == "odemeUyarisi" && q.UyariFirmaId == musteriId).FirstOrDefault();
                    ctx.Uyarilar.Remove(Uyari); ctx.SaveChanges();
                    Method.Error = "success"; Method.ReturnMsg = "Ödeme uyarınız kaldırılmıştır."; Method.ReturnValue = 0;
                }
            }
            catch (Exception e)
            {
                Method.Error = "error"; Method.ReturnMsg = e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult VadeGunuBelirle(int FirmaId, string vadeTarihi, bool Update, decimal OngorulenMiktar)
        {
            MethodStatusDto Method = new MethodStatusDto();
            DateTime Tarih;
            DateTime.TryParse(vadeTarihi, out Tarih);
            try
            {
                if (Update)
                {
                    VADE_TARIHLERI VadeTarihleri = ctx.VadeTarihleri.Where(q => q.FirmaId == FirmaId).FirstOrDefault();
                    VadeTarihleri.OngorulenMiktar = OngorulenMiktar; VadeTarihleri.VadeTarihi = Tarih; ctx.SaveChanges();
                    Method.Error = "success"; Method.ReturnMsg = "Vade tarihi ve miktarı güncellenmiştir"; Method.ReturnValue = VadeTarihleri.Id;
                }
                else
                {
                    VADE_TARIHLERI VadeTarihleri = new VADE_TARIHLERI();
                    VadeTarihleri.OngorulenMiktar = OngorulenMiktar; VadeTarihleri.VadeTarihi = Tarih; ctx.VadeTarihleri.Add(VadeTarihleri); ctx.SaveChanges();
                    Method.Error = "success"; Method.ReturnMsg = "Bu firma için vade tarihi '" + Tarih + "' olarak belirlenmiştir."; Method.ReturnValue = VadeTarihleri.Id;
                }
            }
            catch (Exception e)
            {
                Method.Error = "error"; Method.ReturnMsg = e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult CariDurumDegis(int id)
        {
            MethodStatusDto MStatus = new MethodStatusDto();
            COMPANIES Company = ctx.Firmalar.SingleOrDefault(q => q.Id == id);
            try
            {
                Company.CariHesapIzni = Company.CariHesapIzni == 1 ? 0 : 1; ctx.SaveChanges();
                MStatus.Error = "success"; MStatus.ReturnMsg = "Cari hesap durumu değiştirildi"; MStatus.ReturnValue = Company.CariHesapIzni;
            }
            catch (Exception e)
            {
                MStatus.Error = "error"; MStatus.ReturnMsg = "Hata oluştu: " + e.Message; MStatus.ReturnValue = 0;
            }
            return Json(MStatus, JsonRequestBehavior.AllowGet);

        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult UrunEklePage()
        {
            return View();
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FileLoad()
        {
            var qqfilename = Request.Files[0];
            SuccessionClass Durum = new SuccessionClass();
            try
            {
                var path = Path.Combine(Server.MapPath("~/Upload/imajlar/"), qqfilename.FileName);
                if (System.IO.File.Exists(path))
                {
                    string extension = Path.GetExtension(qqfilename.FileName);
                    string renamedImage = Path.Combine(Server.MapPath("~/Upload/imajlar/" + System.Guid.NewGuid().ToString("N") + extension));
                    qqfilename.SaveAs(renamedImage);
                }
                else
                {
                    qqfilename.SaveAs(path);
                }
                Durum.success = true; Durum.error = "Başarılı"; Durum.preventRetry = true;
            }
            catch (Exception e)
            {
                Durum.success = false; Durum.error = e.Message; Durum.preventRetry = false;
            }

            return Json(Durum, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FileDelete()
        {
            var r = Request.Form[0];
            SuccessionClass Durum = new SuccessionClass();
            try
            {
                Durum.success = true; Durum.error = "Başarılı"; Durum.preventRetry = true;
            }
            catch (Exception e)
            {
                Durum.success = false; Durum.error = e.Message; Durum.preventRetry = false;
            }

            return Json(Durum, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Fotolar(List<FotografClass> FotograflarListesi)
        {
            return Json(FotograflarListesi, JsonRequestBehavior.AllowGet);
        }
       
        [ValidateInput(false)]
        public JsonResult UrunKaydetveyaYenile(PRODUCTS Product)
        {
            string[] _latest = null;
            string _newStr = Product.AlternatifKategoriler.Substring(1, Product.AlternatifKategoriler.Length-2);
            string _newStr2 = _newStr.Replace('"', ' ').Trim();
            Product.AlternatifKategoriler = _newStr2;
            
            MethodStatusDto Status = new MethodStatusDto();
            try
            {
                
                PRODUCTS Urun = ctx.Urunler.SingleOrDefault(q => q.Id == Product.Id);
                
                if (Urun != null)
                {
                    if (Urun.UrunKodu != Product.UrunKodu && !UrunKoduKontrol(Product.UrunKodu))
                    {
                        throw new ArgumentException("Ürün Kodu kullanımda\nLütfen başka bir ürün kodu deneyiniz.");
                    }
                    else
                    {
                        if (Product.OneCikanUrun == 1) { MakeFav(Urun.Id); } else { DeleteFav(Urun.Id); }


                        Urun.UrunAdi = Product.UrunAdi; Urun.UrunKategorisi = Product.UrunKategorisi;
                        Urun.UrunVergiOrani = Product.UrunVergiOrani; Urun.UrunDurum = Product.UrunDurum;
                        Urun.UrunAgirlik = Product.UrunAgirlik; Urun.UrunAgirlikSinifi = Product.UrunAgirlikSinifi;
                        Urun.UrunFiyat = Product.UrunFiyat; Urun.UrunKodu = Product.UrunKodu; Urun.UrunAciklama = Product.UrunAciklama;
                        Urun.Stok = Product.Stok; Urun.GonderimTipi = Product.GonderimTipi; Urun.IndirimGrubu = Product.IndirimGrubu;
                        Urun.MarkaId = Product.MarkaId; Urun.ModelId = Product.ModelId; Urun.MinimumSiparisAdeti = Product.MinimumSiparisAdeti;
                        Urun.StoktanDus = Product.StoktanDus; Urun.AlternatifKategoriler = _newStr2;

                        Status.Error = "success"; Status.ReturnMsg = "Ürün Düzenleme Başarılı"; Status.ReturnValue = Product.Id;
                        ctx.SaveChanges();

                    }
                }
                else
                {
                    if (!UrunKoduKontrol(Product.UrunKodu) || Product.UrunKodu == "")
                    {
                        throw new ArgumentException("Ürün Kodu kullanımda veya boş\nLütfen başka bir ürün kodu deneyiniz.");
                    }
                    else
                    {
                        Product.UrunFotografi = getFileFromForm_sendFileNameToStr(Request.Files["yeniFoto"]);
                        ctx.Urunler.Add(Product);

                        ctx.SaveChanges();
                        if (Product.OneCikanUrun == 1)
                        {
                            MakeFav(Product.Id);
                        }
                        else
                        {
                            DeleteFav(Product.Id);
                        }
                        Status.Error = "success"; Status.ReturnMsg = "Ürün Ekleme Başarılı"; Status.ReturnValue = Product.Id;
                    }
                }
            }
            catch (Exception e)
            {
                Status.Error = "error"; Status.ReturnMsg = e.Message; Status.ReturnValue = Product.Id;
            }
            return Json(Status, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult UrunleriListele()
        {
            List<PRODUCTS> Urunler = ctx.Urunler.ToList();
            return View(Urunler);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult UrunDuzenlePage(int Id)
        {
            PRODUCTS Urun = ctx.Urunler.SingleOrDefault(q => q.Id == Id);
           // Urun.AlternatifKategoriler = HttpUtility.HtmlDecode(Urun.AlternatifKategoriler);
            Urun.UrunAciklama = Kernel.EncodeHTML(Urun.UrunAciklama);
            ViewBag.UrunModelName = ModelNameVer(Urun.ModelId);
            return View(Urun);
        }
        public string ModelNameVer(int ModelId)
        {
            var Sorgu = ctx.Markalar.SingleOrDefault(q => q.Id == ModelId);
            return Sorgu == null ? "MODEL YOK" : Sorgu.Marka;
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult TumHesaplariDondur(int id, string aksiyon)
        {
            MethodStatusDto Method = new MethodStatusDto();
            try
            {
                COMPANIES Company = ctx.Firmalar.SingleOrDefault(q => q.Id == id);
                Company.Aktif = aksiyon == "dondur" ? 0 : 1;
                ctx.SaveChanges();

                Method.Error = "success";
                Method.ReturnMsg = aksiyon == "dondur" ? "Firmanın tüm hesapları donduruldu" : "Firma aktif edildi";
                Method.ReturnValue = id;

            }
            catch (Exception e)
            {
                Method.Error = "error"; Method.ReturnMsg = e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult KategoriPage()
        {
            return View();
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult MarkaPage()
        {
            List<BRANDS> Brands = ctx.Markalar.ToList();
            return View(Brands);
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult MarkaPageEkle()
        {
            return PartialView();
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult MarkaPageGoster(int Id)
        {
            BRANDS Brand = ctx.Markalar.SingleOrDefault(q => q.Id == Id);
            return PartialView(Brand);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult MarkaVer(int id)
        {
            BRANDS Brand = ctx.Markalar.SingleOrDefault(q => q.Id == id);
            return Json(Brand, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        [HttpPost]
        public JsonResult MarkaKaydetveyaGuncelle(BRANDS MarkaBrand)
        {
            MethodStatusDto MethodStatus = new MethodStatusDto();
            try
            {
                BRANDS ModelMarka = ctx.Markalar.SingleOrDefault(q => q.Id == MarkaBrand.Id);
                if (ModelMarka != null)
                {
                    ModelMarka.Marka = MarkaBrand.Marka; ModelMarka.MarkaAltId = MarkaBrand.MarkaAltId; ctx.SaveChanges();
                    MethodStatus.Error = "success"; MethodStatus.ReturnMsg = "Marka düzenleme başarılı"; MethodStatus.ReturnValue = MarkaBrand.Id;
                }
                else
                {
                    ctx.Markalar.Add(MarkaBrand); ctx.SaveChanges();
                    MethodStatus.Error = "success"; MethodStatus.ReturnMsg = "Marka ekleme başarılı"; MethodStatus.ReturnValue = MarkaBrand.Id;
                }
            }
            catch (Exception e)
            {
                MethodStatus.Error = "error"; MethodStatus.ReturnMsg = e.Message; MethodStatus.ReturnValue = 0;
            }
            return Json(MethodStatus, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult MarkaSil(int id)
        {
            MethodStatusDto Method = new MethodStatusDto();
            try
            {
                BRANDS Brand = ctx.Markalar.SingleOrDefault(q => q.Id == id);
                ctx.Markalar.Remove(Brand); ctx.SaveChanges();

                Method.Error = "success"; Method.ReturnMsg = "Marka Silindi"; Method.ReturnValue = Brand.Id;
            }
            catch (Exception e)
            {
                Method.Error = "error"; Method.ReturnMsg = e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);

        }
        [AuthorizeUser(Roles = ("su"))]
        public bool UrunKoduKontrol(string urunKodu)
        {
            var u_code = ctx.Urunler.FirstOrDefault(q => q.UrunKodu == urunKodu);

            if (u_code != null)
                return false;
            else
            {
                return true;
            }
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult MarkaDenetle(int id)
        {
            MethodStatusDto Method = new MethodStatusDto();
            PRODUCTS pr = ctx.Urunler.Where(q => q.MarkaId == id).SingleOrDefault();
            if (pr != null)
            {
                Method.Error = "error"; Method.ReturnMsg = "Bu markaya bağlı ürünler var\nÜrünleri silmeden bu markayı silemezsiniz.";
                Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult UrunSil(int id)
        {
            var Urun = ctx.Urunler.SingleOrDefault(q => q.Id == id);
            var UrunFotograflari = ctx.UrunFotograflari.Where(q => q.UrunId == id);
            var UrunFav = ctx.OneCikanlar.Where(q => q.UrunId == id);
            MethodStatusDto M = new MethodStatusDto();
            try
            {
                var SepetUrunleri = ctx.Sepet.Where(q => q.UrunId == id).ToList();
                if (SepetUrunleri != null)
                {
                    foreach (var item in SepetUrunleri)
                    {
                        ctx.Sepet.Remove(item);
                    }
                }
                Kernel.DeleteFileFromFileSystem(Server.MapPath(@"\Upload\imajlar\" + Urun.UrunFotografi));
                foreach (var item in UrunFotograflari)
                {
                    Kernel.DeleteFileFromFileSystem(Server.MapPath(@"\Upload\imajlar\" + item.FotoUrl));
                    ctx.UrunFotograflari.Remove(item);
                }
                foreach (var item in UrunFav)
                {
                    ctx.OneCikanlar.Remove(item);
                }
                ctx.Urunler.Remove(Urun);
                ctx.SaveChanges();
                M.Error = "success"; M.ReturnMsg = "Ürün Silindi."; M.ReturnValue = id;
            }
            catch (Exception e)
            {
                M.Error = "error"; M.ReturnMsg = e.Message; M.ReturnValue = 0;
            }
            return Json(M, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult MakeFav(int id)
        {
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                //Kontrol whether active or ///
                var _Control = ctx.Urunler.SingleOrDefault(q => q.Id == id);
                if (_Control.UrunDurum == 1)
                {
                    List<PRODUCTS_FAVORITES> pf = ctx.OneCikanlar.Where(q => q.UrunId == id).ToList();
                    if (pf != null)
                    {
                        foreach (var item in pf)
                        {
                            ctx.OneCikanlar.Remove(item);
                        }
                        _Control.OneCikanUrun = 0;
                    }
                    PRODUCTS_FAVORITES pf_new = new PRODUCTS_FAVORITES();

                    pf_new.EklemeTarihi = DateTime.Now;
                    pf_new.UrunId = id;
                    pf_new.UserId = getAktifKullanici().UserId; _Control.OneCikanUrun = 1;
                    ctx.OneCikanlar.Add(pf_new); ctx.SaveChanges();

                    _m.Error = "success"; _m.ReturnMsg = "Ürün Öne Çıkarıldı"; _m.ReturnValue = pf_new.Id;
                }
                else
                {
                    _m.Error = "error"; _m.ReturnMsg = "Ürün aktif değil. Önce ürünü aktif etmelisiniz."; _m.ReturnValue = 0;
                }
            }
            catch (Exception e)
            {
                _m.Error = "error"; _m.ReturnMsg = e.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult DeleteFav(int id)
        {
            List<PRODUCTS_FAVORITES> p = ctx.OneCikanlar.Where(q => q.UrunId == id).ToList();
            MethodStatusDto _m = new MethodStatusDto();
            foreach (var item in p)
            {
                ctx.OneCikanlar.Remove(item);
                ctx.Urunler.Single(q => q.Id == item.UrunId).OneCikanUrun = 0;
            }
            ctx.SaveChanges();
            _m.Error = "error"; _m.ReturnMsg = "Ürün kampanyadan kaldırıldı."; _m.ReturnValue = -1;
            return Json(_m, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public string ModelinAitOlduguMarka(int id)
        {
            string donenDeger = "Marka";
            var Marka = ctx.Markalar.FirstOrDefault(q => id == q.Id);
            try
            {
                if (id == 0)
                {
                    donenDeger = "Modele Bağlı Değil";
                }
                else
                {
                    donenDeger = Marka.Marka;
                }

            }
            catch (Exception)
            {
                donenDeger = "Marka Yok";
            }
            return donenDeger;
        }
        [AuthorizeUser(Roles = ("su"))]
        public bool GetFavState(int id)
        {
            bool donenDeger = false;
            PRODUCTS_FAVORITES p = ctx.OneCikanlar.FirstOrDefault(q => q.UrunId == id);
            if (p != null)
            {
                donenDeger = true;
            }
            else
            {
                donenDeger = false;
            }
            return donenDeger;
        }
        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult HesapDuzenle(int id)
        {
            var Hesap = ctx.Firmalar.SingleOrDefault(q => q.Id == id);
            return PartialView(Hesap);
        }
        [AuthorizeUser(Roles = ("su"))]
        public string getFileFromForm_sendFileNameToStr(HttpPostedFileBase _file)
        {
            string val = null;
            try
            {
                var getfile = _file;
                if (getfile != null)
                {
                    string getExt = Path.GetExtension(getfile.FileName);
                    string guidRandom = System.Guid.NewGuid().ToString("N") + getExt;
                    var path = Path.Combine(Server.MapPath("~/Upload/imajlar"), getfile.FileName);
                    string renamedImage = Path.Combine(Server.MapPath("~/Upload/imajlar/" + guidRandom));
                    byte[] _b = Kernel.getFileFromInput(getfile);
                    MemoryStream _ms = new MemoryStream(_b);
                    using (System.Drawing.Image _img = Kernel.ResizeImage(System.Drawing.Image.FromStream(_ms), 800, 600))
                    {
                        _img.Save(renamedImage, System.Drawing.Imaging.ImageFormat.Jpeg); val = guidRandom;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
            return val;
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult changeFoto_getintofilesystem()
        {
            MethodStatusDto _m = new MethodStatusDto();

            try
            {
                var getfile = Request.Files[0];
                if (getfile != null)
                {
                    int urunId = int.Parse(Request.Form["urunId"]);
                    string extension = Path.GetExtension(getfile.FileName);
                    string guidRandom = System.Guid.NewGuid().ToString("N") + extension;
                    var path = Path.Combine(Server.MapPath("~/Upload/imajlar/"), getfile.FileName);
                    string renamedImage = Path.Combine(Server.MapPath("~/Upload/imajlar/" + guidRandom));
                    byte[] _b = Kernel.getFileFromInput(getfile);
                    MemoryStream ms = new MemoryStream(_b);
                    using (System.Drawing.Image _ourimage = Kernel.ResizeImage(System.Drawing.Image.FromStream(ms), 800, 600))
                    {
                        _ourimage.Save(renamedImage, System.Drawing.Imaging.ImageFormat.Jpeg); changeProductPhoto_updatefromdatabase(guidRandom, urunId);
                        _m.Error = "success"; _m.ReturnMsg = guidRandom; _m.ReturnValue = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }


            return Json(_m, JsonRequestBehavior.AllowGet);

        }
        [AuthorizeUser(Roles = ("su"))]
        public string urunFotoSil(int urunId)
        {
            string donenDeger = "";
            var urun = ctx.Urunler.SingleOrDefault(q => q.Id == urunId);
            try
            {
                if (System.IO.File.Exists(Server.MapPath("~/Upload/imajlar/" + urun.UrunFotografi)))
                {
                    System.IO.File.Delete(Server.MapPath("~/Upload/imajlar/" + urun.UrunFotografi));
                    donenDeger = "Fotoğraf silindi";
                }
            }
            catch (Exception ex)
            {
                donenDeger = "fotoğraf silinirken hata: " + ex.Message;
            }
            return donenDeger;
        }
        [AuthorizeUser(Roles = ("su"))]
        public string changeProductPhoto_updatefromdatabase(string toWhat, int urunId)
        {
            string donendeger = "";
            try
            {
                var _p = ctx.Urunler.SingleOrDefault(q => q.Id == urunId);
                if (_p != null)
                {
                    _p.UrunFotografi = toWhat; donendeger = _p.UrunFotografi;
                }
                ctx.SaveChanges();
            }
            catch (Exception)
            {
                donendeger = "";
            }
            return donendeger;
        }
        [AuthorizeUser(Roles = ("su"))]
        public ActionResult DuyuruEkle()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult DuyuruEkle(DUYURULAR _d)
        {
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                _d.EklenmeTarihi = DateTime.Now;
                ctx.Duyurular.Add(_d); ctx.SaveChanges(); _m.Error = "success"; _m.ReturnMsg = "Duyuru eklendi"; _m.ReturnValue = _d.Id;
            }
            catch (Exception e)
            {
                _m.Error = "error"; _m.ReturnMsg = e.Message; _m.ReturnValue = 0;
            }

            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult DuyuruSil(int DuyuruId)
        {
            MethodStatusDto _d = new MethodStatusDto();
            try
            {
                var _x = ctx.Duyurular.SingleOrDefault(q => q.Id == DuyuruId);
                if (_x != null)
                {
                    ctx.Duyurular.Remove(_x); ctx.SaveChanges();
                    _d.Error = "success"; _d.ReturnMsg = "Duyuru silindi"; _d.ReturnValue = DuyuruId;
                }
            }
            catch (Exception ex)
            {
                _d.Error = "error"; _d.ReturnMsg = ex.Message; _d.ReturnValue = 0;
            }
            return Json(_d, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FaturasizaDonustur(int orderid, int NakitIskontoOrani)
        {
            SepetController _SepetController = new SepetController();
            MethodStatusDto _m = new MethodStatusDto();
            var _order = ctx.Siparisler.SingleOrDefault(q => q.Id == orderid);
            if (_order != null)
            {
                try
                {
                    if (_order.OdemeTipi == "NAKİT")
                    {
                        decimal yeniTutar = _order.SiparisUrunleri.Sum(q => q.KalemTutar);
                        decimal iskonto = _SepetController.IskontoHesapla(yeniTutar, _order.SiparisIskontoOrani);

                        yeniTutar = yeniTutar - iskonto;
                        TRANSACTIONS _t = new TRANSACTIONS();
                        _t.Aciklama = "Kdv Düşümü " + iskonto + " miktarında"; _t.FirmaId = _order.FirmaId; _t.GirdiDurum = 1;
                        if (NakitIskontoOrani != 0)
                        {
                            decimal nakitIskonto = _SepetController.IskontoHesapla(yeniTutar, NakitIskontoOrani);
                            yeniTutar = yeniTutar - nakitIskonto;
                        }
                        _order.ManuelTutar = 1; _order.SiparisToplami = yeniTutar;


                        _order.KdvOrani = 0; ctx.SaveChanges();
                        _m.Error = "success"; _m.ReturnMsg = "KDV oranı sıfırlandı"; _m.ReturnValue = orderid;
                    }
                    else
                    {
                        _m.Error = "error"; _m.ReturnMsg = "Üzgünüm, Ödeme tipi NAKİT olmayan bir siparişi KDV'siz yapamazsınız."; _m.ReturnValue = 0;
                    }
                }
                catch (Exception ex)
                {
                    _m.Error = "error"; _m.ReturnMsg = "Hata oluştu" + ex.Message; _m.ReturnValue = 0;
                }
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeUser(Roles = ("su"))]
        public JsonResult SiparisiSil(int orderid)
        {
            MethodStatusDto _m = new MethodStatusDto();
            var _order = ctx.Siparisler.Single(q => q.Id == orderid);
            var _orderProducts = ctx.SiparisUrunleri.Where(q => q.SiparisNo == _order.Id);
            var _payments = ctx.SiparisOdemeleri.Where(q => q.OrderId == _order.Id);

            try
            {
                foreach (var item in _orderProducts)
                {
                    ctx.SiparisUrunleri.Remove(item);
                }
                FirmayaOdeme(_payments.Where(q => q.OrderId == _order.Id).Sum(q => q.OdemeTutari), _order.FirmaId, _order.SiparisNo + " sipariş iptal iadesi");
                ctx.Siparisler.Remove(_order);
                ctx.SaveChanges();
                _m.Error = "success"; ; _m.ReturnMsg = "Sipariş silindi"; _m.ReturnValue = orderid;
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult EkstraIskontoUygula(int orderid, int IskontoOrani)
        {
            SepetController _SepetController = new SepetController();
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                if (IskontoYapmayaUygunmu(orderid))
                {
                    var _order = ctx.Siparisler.SingleOrDefault(q => q.Id == orderid);
                    decimal temp_ordersTotal = _order.SiparisUrunleri.Sum(w => w.KalemTutar) - _order.IskontoTutar;


                    if (_order.OdemeTipi == "NAKİT")
                    {
                        decimal iskontoTutar = _SepetController.IskontoHesapla(temp_ordersTotal, IskontoOrani);
                        _order.IskontoTutar = _order.IskontoTutar + iskontoTutar;
                        decimal yeniTutar = (_order.SiparisUrunleri.Sum(q => q.KalemTutar) - _order.IskontoTutar) * decimal.Parse("1,18");

                        _order.SiparisAyrintilari = _order.SiparisAyrintilari + "Nakit ödemeden dolayı " + _order.SiparisNo + " - %" + IskontoOrani + "(" + string.Format("{0:C}", iskontoTutar) + ") iskonto uygulandı. <br>";

                        _order.ManuelTutar = 1; _order.SiparisToplami = yeniTutar; ctx.SaveChanges();
                        _m.Error = "success"; _m.ReturnMsg = "Ekstra %" + IskontoOrani + " iskonto uygulandı."; _m.ReturnValue = orderid;
                    }
                    else
                    {
                        _m.Error = "error"; _m.ReturnMsg = "Üzgünüm, Ödeme tipi NAKİT olmayan bir siparişe ekstra iskonto uygulama izniniz bulunmuyor."; _m.ReturnValue = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public bool IskontoYapmayaUygunmu(int orderid)
        {
            SIPARIS_ODEMELERI _s = ctx.SiparisOdemeleri.FirstOrDefault(q => q.OrderId == orderid);
            if (_s != null)
                throw new ArgumentException("Bu siparişe ilişkin ödeme bulunuyor!\nÖdeme bunulan bir hesaba iskonto uygulayabilmek için ödemeyi silmeniz gerekmektedir.");
            else
                return true;
        }

        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult OdemeEkle_Page()
        {
            return PartialView();
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult SipariseOdemeEkle(SIPARIS_ODEMELERI SiparisOdemesi)
        {
            MethodStatusDto _m = new MethodStatusDto();
            var _order = ctx.Siparisler.Single(q => q.Id == SiparisOdemesi.OrderId);
            try
            {
                if (OdemeEklemeyeMusaitmi(SiparisOdemesi.OrderId, SiparisOdemesi.OdemeTutari))
                {
                    ctx.SiparisOdemeleri.Add(SiparisOdemesi); FirmayaOdeme(SiparisOdemesi.OdemeTutari, _order.FirmaId, "Firmaya " + _order.SiparisNo + " kodlu sipariş kapsamında ödeme"); ctx.SaveChanges();
                    _m.Error = "success"; _m.ReturnMsg = "Ödeme eklendi"; _m.ReturnValue = SiparisOdemesi.Id;
                }
                else
                {
                    var _s = ctx.Siparisler.Single(q => q.Id == SiparisOdemesi.OrderId);
                    decimal _x = _s.SiparisToplami.GetValueOrDefault(0) - _s.SipariseAitOdemeler.Sum(q => q.OdemeTutari);
                    _m.Error = "error"; _m.ReturnMsg = "Ödeme eklendiğinde, sipariş tutarının altına düşüyor\nMuhtemelen fazla ödeme yapmaya çalışıyorsunuz.\nLütfen tutarı gözden geçiriniz.\nBu siparişe en fazla " + string.Format("{0:C}", _x) + " ödeme yapabilirsiniz"; _m.ReturnValue = 0;
                }
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public bool OdemeEklemeyeMusaitmi(int OrderId, decimal DusulecekTutar)
        {
            var _x = true;
            try
            {
                var _order = ctx.Siparisler.Single(q => q.Id == OrderId);
                if (_order.SipariseAitOdemeler.Sum(q => q.OdemeTutari) + DusulecekTutar > _order.SiparisToplami)
                    _x = false;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _x;
        }

        [AuthorizeUser(Roles = ("su"))]
        public decimal FirmayaOdeme(decimal Tutar, int FirmaId, string aciklama)
        {
            var _f = ctx.Firmalar.Single(q => q.Id == FirmaId);
            var _m = ctx.Muhasebe.Single(q => q.FirmaId == FirmaId);

            TRANSACTIONS _t = new TRANSACTIONS();
            try
            {
                _m.Bakiye = _m.Bakiye + Tutar;
                _m.SonGuncelleme = DateTime.Now;
                _m.FirmaId = FirmaId;

                // transactions

                _t.Aciklama = string.Format("{0:N}", Tutar) + " " + aciklama; _t.FirmaId = FirmaId;
                _t.GirdiDurum = 1; _t.GirdiTipi = 0; _t.TarihSaat = DateTime.Now; _t.Tutar = Tutar; _t.UserId = 1;
                ctx.GirdiCiktilar.Add(_t);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _m.Bakiye;
        }

        [AuthorizeUser(Roles = ("su"))]
        public decimal FirmayaBorc(decimal Tutar, int FirmaId, string aciklama)
        {
            var _f = ctx.Firmalar.Single(q => q.Id == FirmaId);
            var _m = ctx.Muhasebe.Single(q => q.FirmaId == FirmaId);

            TRANSACTIONS _t = new TRANSACTIONS();
            try
            {
                _m.Bakiye = _m.Bakiye - Tutar;
                _m.SonGuncelleme = DateTime.Now;
                _m.FirmaId = FirmaId;

                // transactions

                _t.Aciklama = string.Format("{0:N}", Tutar) + " " + aciklama; _t.FirmaId = FirmaId;
                _t.GirdiDurum = 1; _t.GirdiTipi = 1; _t.TarihSaat = DateTime.Now; _t.Tutar = Tutar; _t.UserId = 1;
                ctx.GirdiCiktilar.Add(_t);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _m.Bakiye;
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult OdemeSil(int id)
        {
            MethodStatusDto _m = new MethodStatusDto();
            try
            {
                var _odeme = ctx.SiparisOdemeleri.SingleOrDefault(q => q.Id == id);
                var _firmaId = ctx.Siparisler.SingleOrDefault(q => q.Id == _odeme.OrderId).FirmaId;

                ctx.SiparisOdemeleri.Remove(_odeme);
                FirmayaBorc(_odeme.OdemeTutari, _firmaId, "--");

                ctx.SaveChanges();

                _m.Error = "success"; _m.ReturnMsg = "Ödeme Silindi ve firmanın borç hesabına yazıldı."; _m.ReturnValue = 0;


            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult KullaniciSil(int UserId)
        {
            MethodStatusDto _m = new MethodStatusDto();
            var user = ctx.Kullanicilar.Single(q => q.Id == UserId);

            try
            {
                ctx.Kullanicilar.Remove(user); ctx.SaveChanges(); _m.Error = "success"; _m.ReturnMsg = "Kullanıcı silindi."; _m.ReturnValue = user.Id;
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }

            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public JsonResult FirmaSil(int FirmaId)
        {
            MethodStatusDto _m = new MethodStatusDto();
            var _f = ctx.Firmalar.Single(q => q.Id == FirmaId);
            var _u = ctx.Kullanicilar.Where(q => q.FirmaId == _f.Id);
            try
            {
                if (FirmaSilmeyeMusaitmi(FirmaId)) // firma bakiyesi 0,00 olmalıdır.
                {
                    MUHASEBE _muhasebe = ctx.Muhasebe.Single(q => q.FirmaId == FirmaId);
                    ctx.Muhasebe.Remove(_muhasebe); ctx.Firmalar.Remove(_f);
                    _m.Error = "success"; _m.ReturnMsg = "Firma silindi."; _m.ReturnValue = 0;
                    foreach (var item in _u)
                    {
                        ctx.Kullanicilar.Remove(item);
                    }
                    ctx.SaveChanges();
                }
                else
                {
                    _m.Error = "error"; _m.ReturnMsg = "Firma silinemiyor.\nFirma silmeden önce bakiye sıfırlanması gerekmektedir."; _m.ReturnValue = 0;
                }
            }
            catch (Exception ex)
            {
                _m.Error = "error"; _m.ReturnMsg = ex.Message; _m.ReturnValue = 0;
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(Roles = ("su"))]
        public bool FirmaSilmeyeMusaitmi(int FirmaId)
        {
            bool deger = true;
            try
            {
                MUHASEBE _muhasebe = ctx.Muhasebe.Single(q => q.FirmaId == FirmaId);
                if (_muhasebe.Bakiye != 0)
                {
                    deger = false;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return deger;
        }

        [AuthorizeUser(Roles = ("su"))]
        public PartialViewResult DuyuruListele()
        {
            var _d = ctx.Duyurular.ToList();
            return PartialView(_d);
        }

        [AuthorizeUser(Roles = ("su"))]
        public OAuthClass_GetAccessTokenClass getAccessToken()
        {
            OAuthSonucSinifi _oa = new OAuthSonucSinifi();
            OAuthClass_GetAccessTokenClass _ocs = new OAuthClass_GetAccessTokenClass();
            try
            {
                _oa = _oa._getOauthProperties();
                WebRequest req = HttpWebRequest.Create(_oa.ApiUrl);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                Stream _s = req.GetRequestStream();
                WebResponse res = req.GetResponse();
                string resbody = null;
                using (StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    resbody = sr.ReadToEnd();
                }
                _ocs = JsonConvert.DeserializeObject<OAuthClass_GetAccessTokenClass>(resbody);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _ocs;
        }

        public string getUserId()
        {
            OAuthClass_GetAccessTokenClass _oa = getAccessToken();
            string baseUrl = "https://api.parasut.com/v1/64624/contacts";
            WebRequest _webreq = HttpWebRequest.Create(new Uri(baseUrl));
            _webreq.Headers.Add("Bearer: " + _oa.access_token);
            WebResponse _webres = _webreq.GetResponse();
            Stream _webstream = _webreq.GetRequestStream(); string val = null;
            using (StreamReader _stread = new StreamReader(_webres.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                val = _stread.ReadToEnd();
            }
            return val;
        }
    }
}
