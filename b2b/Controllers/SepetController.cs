using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Entity;
using Iyzipay.Model;
using Iyzipay.Request;

namespace B2B.Controllers
{
    public class SepetController : BaseController
    {
        //
        // GET: /Sepet/
        ProductsController UrunlerFonksiyonlari = new ProductsController();
        SepetHesapAraci _SepetHesapAraci = new SepetHesapAraci();
        public string sid()
        {
            try
            {
                return Request.Cookies["usersid"].Value;
            }
            catch (Exception)
            {

                return Session.SessionID;
            }
        }

        public JsonResult ToplamUrun()
        {
            try
            {
                var sessionid = sid();
                return Json(ctx.Sepet.Count(q => q.SID == sessionid).ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult Index()
        {
            SepetDto s = new SepetDto();
            s = sepetAl();
            return PartialView(s);
        }
        public PartialViewResult SepetGoster()
        {
            SepetDto _s = sepetAl();
            return PartialView(_s);
        }
        public bool isAvailable(int urunId, decimal adet)
        {
            bool donenDeger = false;
            var urun = ctx.Urunler.SingleOrDefault(q => q.Id == urunId);
            if (urun != null)
            {
                try
                {
                    if (urun.StoktanDus != 0 && urun.UrunDurum == 1)
                    {
                        // IsActive 
                        if (urun.UrunDurum == 1 && urun.Stok > 0 && UrunlerFonksiyonlari.StokVer(urunId) - adet >= 0)
                        {
                            // herşey normal 
                            donenDeger = true;
                        }
                        else
                        {
                            throw new ArgumentException("Ürün sepete eklenemiyor.\nEklemeye çalıştığınız ürün stok sınırını aşıyor. Bu ürün için en fazla " + UrunlerFonksiyonlari.StokVer(urunId) + " sipariş verebilirsiniz.");
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.Message);
                }
            }
            else
            {
                throw new ArgumentException("Ürün bulunamıyor.");
            }
            return donenDeger;
        }

        public JsonResult SepeteAt(int urunId, decimal adet, int? ozellik)
        {
            SepetiBosalt();
            MethodStatusDto MethodStatus = new MethodStatusDto();
            try
            {
                if (isAvailable(urunId, adet))
                {
                    if (UrunlerFonksiyonlari.MinSipKontrol(urunId, adet))
                    {
                        SepetDto s = new SepetDto();
                        s = sepetAl();
                        if (s.Urunler.Count == 0)
                        {
                            s.SessionId = sid();
                            s.Urunler = new List<SepetUrunlerDto>();

                            s.Urunler.Add(new SepetUrunlerDto()
                            {
                                Adet = adet,
                                UrunId = urunId,
                                UrunBaslik = UrunlerFonksiyonlari.UrunAdiVer(urunId),
                                OzellikId = ozellik,
                                Ozelligi = UrunlerFonksiyonlari.UrunOzellikVer(ozellik),
                                Fotograf = UrunlerFonksiyonlari.FotografVer(urunId),
                                Marka = UrunlerFonksiyonlari.MarkaVer(urunId),
                                UrunKodu = UrunlerFonksiyonlari.UrunKoduVer(urunId),
                                UrunFiyat = UrunlerFonksiyonlari.UrunFiyatVer(urunId)
                            });
                            MethodStatus.Error = "success";
                            MethodStatus.ReturnMsg = "Ürünler sepetinize eklendi.";
                            MethodStatus.ReturnValue = urunId;

                            foreach (var item in s.Urunler)
                            {
                                SEPET _temp = new SEPET();
                                _temp.Adet = item.Adet;
                                _temp.FirmaId = 1;
                                _temp.SID = sid();
                                _temp.UrunId = item.UrunId;
                                _temp.UserId = 1;
                                ctx.Sepet.Add(_temp);
                            }
                            ctx.SaveChanges();
                        }
                        else
                        {
                            s = sepetAl();
                            var sessionid = sid();
                            if (s.Urunler.Where(q => q.UrunId == urunId && q.OzellikId == ozellik).Count() > 0)
                            {
                                foreach (var item in s.Urunler.Where(q => q.UrunId == urunId && q.OzellikId == ozellik))
                                {
                                    SEPET _temp = new SEPET();
                                    _temp = ctx.Sepet.FirstOrDefault(q => q.SID == sessionid && q.UrunId == item.UrunId);
                                    _temp.Adet = _temp.Adet + adet;
                                    item.Adet = item.Adet + adet;
                                    ctx.Sepet.Add(_temp);
                                }
                                ctx.SaveChanges();
                                MethodStatus.Error = "success";
                                MethodStatus.ReturnMsg = "Ürün adedi artırıldı.";
                                MethodStatus.ReturnValue = urunId;
                            }
                            else
                            {
                                s.Urunler.Add(new SepetUrunlerDto()
                                {
                                    Adet = adet,
                                    UrunId = urunId,
                                    UrunBaslik = UrunlerFonksiyonlari.UrunAdiVer(urunId),
                                    OzellikId = ozellik,
                                    Ozelligi = UrunlerFonksiyonlari.UrunOzellikVer(ozellik),
                                    Fotograf = UrunlerFonksiyonlari.FotografVer(urunId),
                                    Marka = UrunlerFonksiyonlari.MarkaVer(urunId),
                                    UrunKodu = UrunlerFonksiyonlari.UrunKoduVer(urunId),
                                    UrunFiyat = UrunlerFonksiyonlari.UrunFiyatVer(urunId)
                                });
                                SEPET _temp = new SEPET();
                                ctx.Sepet.Add(new SEPET()
                                {
                                    Adet = adet,
                                    FirmaId = 1,
                                    SID = sid(),
                                    UrunId = urunId,
                                    UserId = 1,
                                });
                                ctx.SaveChanges();
                                MethodStatus.Error = "success";
                                MethodStatus.ReturnMsg = "Ürünler sepetinize eklendi";
                                MethodStatus.ReturnValue = urunId;
                            }
                        }
                    }
                    else
                    {
                        MethodStatus.Error = "error";
                        MethodStatus.ReturnMsg = "Minimum sipariş adetinin altında sipariş veremezsiniz";
                        MethodStatus.ReturnValue = 0;
                    }
                }
            }
            catch (Exception e)
            {
                MethodStatus.Error = "error"; MethodStatus.ReturnMsg = e.Message; MethodStatus.ReturnValue = 0;
            }

            return Json(MethodStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SepetiGoster()
        {
            if (sepetAl().Urunler.Count == 0)
            {
                return Json(sepetAl(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                SepetDto s = new SepetDto();
                return Json(s, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SepettenSil(int urunId)
        {
            MethodStatusDto Method = new MethodStatusDto();

            if (sepetAl().Urunler.Count > 0)
            {
                SepetDto s = sepetAl();
                List<SepetUrunlerDto> list = s.Urunler.ToList();
                list.RemoveAll(x => x.UrunId == urunId);
                s.Urunler = list;
                Session["Sepet"] = s;
                Method.Error = "success";
                Method.ReturnMsg = "Ürün Sepetten Silindi.";
                Method.ReturnValue = urunId;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SepetiBosalt()
        {
            var sessionid = sid();
            List<SEPET> _s = new List<SEPET>();
            _s = ctx.Sepet.Where(q => q.SID == sessionid).ToList();
            ctx.Sepet.RemoveRange(_s);
            ctx.SaveChanges();
            MethodStatusDto m = new MethodStatusDto();
            m.ReturnMsg = "Sepet Boşaltıldı";
            m.ReturnValue = 0;
            m.Error = "success";
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public string SepetToplamVer(int KdvTutarBulunsunmu)
        {
            if (sepetAl().Urunler.Count > 0)
            {
                SepetDto s = sepetAl();
                if (KdvTutarBulunsunmu == 0)
                {
                    return string.Format("{0:C}", s.Urunler.Sum(q => (q.UrunFiyat) * q.Adet));
                }
                else
                {
                    return string.Format("{0:C}", s.Urunler.Sum(q => (q.UrunFiyat * 18 / 100) * q.Adet));
                }
            }
            else
            {
                return "0";
            }
        }
        public string SepetGenelToplamVer()
        {
            if (sepetAl().Urunler.Count > 0)
            {
                SepetDto s = (SepetDto)Session["Sepet"];
                decimal sepetToplam = 0;
                sepetToplam = s.Urunler.Sum(q => (q.UrunFiyat) * q.Adet) + s.Urunler.Sum(q => (q.UrunFiyat * 18 / 100) * q.Adet);
                return string.Format("{0:C}", sepetToplam);
            }
            else
            {
                return "0";
            }
        }
        public decimal SepetGenelToplamVerDecimalCinsinden(bool kdvDahilEdilsinmi, SepetDto Sepet)
        {
            decimal sepetToplam = 0;
            if (Sepet != null)
            {
                if (kdvDahilEdilsinmi == false)
                {
                    sepetToplam = Sepet.Urunler.Sum(q => q.UrunFiyat * q.Adet);
                }
                else
                {
                    decimal tempToplam = Sepet.Urunler.Sum(q => q.UrunFiyat * q.Adet) * 18 / 100;
                    sepetToplam = Sepet.Urunler.Sum(q => q.UrunFiyat * q.Adet) + tempToplam;
                }
            }
            else
            {
                throw new ArgumentNullException("Sepet boş");
            }
            return sepetToplam;
        }
        public decimal SepetToplamVerDecimalCinsinden(SepetDto Sepet)
        {
            decimal sepetToplam = 0;
            if (Sepet != null)
            {
                sepetToplam = Sepet.Urunler.Sum(q => q.UrunFiyat * q.Adet);
            }
            else
            {
                throw new ArgumentNullException("Sepetiniz boş görünüyor");
            }
            return sepetToplam;
        }

        public JsonResult SepetHesabi()
        {
            SepetDto Sepet = sepetAl();
            UserHolder UserInfo = getAktifKullanici();
            _SepetHesapAraci = new SepetHesapAraci();

            _SepetHesapAraci.ToplamTutar = SepetToplamVerDecimalCinsinden(Sepet);
            //_SepetHesapAraci.IskontoOrani = UserInfo.FirmaIskontoOrani;
            //_SepetHesapAraci.IskontoTutari = IskontoHesapla(SepetToplamVerDecimalCinsinden(Sepet), UserInfo.FirmaIskontoOrani);
            _SepetHesapAraci.AraToplam = _SepetHesapAraci.ToplamTutar - _SepetHesapAraci.IskontoTutari;
            _SepetHesapAraci.KdvOrani = 18;
            _SepetHesapAraci.KdvTutari = (_SepetHesapAraci.ToplamTutar - _SepetHesapAraci.IskontoTutari) * 18 / 100;
            _SepetHesapAraci.GenelTutar = SepetToplamVerDecimalCinsinden(Sepet) - _SepetHesapAraci.IskontoTutari + _SepetHesapAraci.KdvTutari;

            return Json(_SepetHesapAraci, JsonRequestBehavior.AllowGet);
        }
        public SepetHesapAraci YeniSiparisIcinSepetHesabi(SepetDto Sepet, UserHolder UserInfo, int KdvOrani)
        {
            _SepetHesapAraci = new SepetHesapAraci();

            _SepetHesapAraci.ToplamTutar = SepetToplamVerDecimalCinsinden(Sepet);
            _SepetHesapAraci.IskontoOrani = UserInfo.FirmaIskontoOrani;
            _SepetHesapAraci.IskontoTutari = IskontoHesapla(SepetToplamVerDecimalCinsinden(Sepet), UserInfo.FirmaIskontoOrani);
            _SepetHesapAraci.AraToplam = _SepetHesapAraci.ToplamTutar - _SepetHesapAraci.IskontoTutari;
            _SepetHesapAraci.KdvOrani = 18;
            _SepetHesapAraci.KdvTutari = (_SepetHesapAraci.ToplamTutar - _SepetHesapAraci.IskontoTutari) * 18 / 100;
            _SepetHesapAraci.GenelTutar = _SepetHesapAraci.ToplamTutar - _SepetHesapAraci.IskontoTutari + _SepetHesapAraci.KdvTutari;

            return _SepetHesapAraci;
        }

        public decimal IskontoHesapla(decimal tutar, int Oran)
        {
            return tutar * Oran / 100;
        }
        public JsonResult SiparisiTekrarla(int siparisNo)
        {
            MethodStatusDto Method = new MethodStatusDto();
            try
            {
                SepetDto Sepet = new SepetDto();
                List<SepetUrunlerDto> SepetUrunleri = new List<SepetUrunlerDto>();

                List<ORDER_PRODUCTS> SepeteAtilacakUrunler = ctx.SiparisUrunleri.Where(q => q.SiparisNo == siparisNo).ToList();
                foreach (var item in SepeteAtilacakUrunler)
                {
                    SepeteAt(item.UrunId, item.Adet, 0);
                }
                Sepet.Urunler = SepetUrunleri;
                SepetDto S = sepetAl();
            }
            catch (Exception e)
            {
                Method.Error = "error"; Method.ReturnMsg = "Sipariş tekrarlanırken bir hata oluştu\nMuhtemelen önceden verdiğiniz ürün artık ürün kataloğunda bulunmuyor.\nSistem yöneticisine şu mesajı iletin: " + e.Message; Method.ReturnValue = 0;
            }
            return Json(Method, JsonRequestBehavior.AllowGet);
        }
        public string SepetiYenile()
        {
            SepetDto Sepet = sepetAl();
            string Yazdir = "";
            if (Yazdir.isNumeric())
                return "Değil";
            if (Sepet != null)
            {
                foreach (var item in Sepet.Urunler)
                {
                    Yazdir += @"<!-- list item--> " +
                        "<a href='javascript: void(0);' class='list - group - item'>" +
                        "<div class='media'>" +
                        "<div class='pull-left p-r-10'>" +
                        "<em class='fa fa-caret-right fa-2x text-primary'></em>" +
                        "</div>" +
                        "<div class='media-body'>" +
                        "<h5 class='media-heading'>" + item.UrunBaslik + "</h5>" +
                        "<p class='m-0'>" +
                        "<small>" + item.Adet + " adet " + item.UrunKodu + "</small>" +
                        "</p>" +
                        "</div>" +
                        "</div>" +
                        "</a>";
                }
                Yazdir += "<span id='RefreshUrun' style='display:none'>" + Sepet.Urunler.Count() + "</span>";
            }
            else
            {
                Yazdir = @"<!-- list item-->" +
                                            "< a href = 'javascript:void(0);' class='list-group-item'>" +
                                                "<div class='media'>" +
                                                    "<div class='pull-left p-r-10'>" +
                                                        "<em class='fa fa-caret-right fa-2x text-primary'></em>" +
                                                    "</div>" +
                                                    "<div class='media-body'>" +
                                                        "<h5 class='media-heading'>Sepetiniz Boş</h5>" +
                                                        "<p class='m-0'>" +
                                                            "<small>Sepetinize ürün ekleyiniz.</small>" +
                                                        "</p>" +
                                                    "</div>" +
                                                "</div>" +
                                            "</a>";
                Yazdir += "<span id='RefreshUrun' style='display:none'>0</span>";
            }
            return Yazdir;
        }

        public List<Iyzipay.Model.BasketItem> SepetUrunleri()
        {
            var _s = sepetAl();
            decimal tempfiyat;
            List<Iyzipay.Model.BasketItem> _items = new List<BasketItem>();
            foreach (var item in _s.Urunler)
            {
                tempfiyat = (item.UrunFiyat * item.Adet) * 18 / 100;
                tempfiyat = item.UrunFiyat + tempfiyat;
                _items.Add(new BasketItem()
                {
                    Category1 = "Çiçek",
                    Category2 = "",
                    Id = item.UrunId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Name = item.UrunBaslik,
                    Price = tempfiyat.ToString().Replace(",", ".")
                }); ;
            }

            return _items;
        }
        public Iyzipay.Options getIyziOptions()
        {
            Iyzipay.Options options = new Iyzipay.Options();
            options.ApiKey = "HgsF11XBqB86seU29Kv80hHvfgKsjZ3t";
            options.SecretKey = "MVtf0Ny7bLOwukZ8rh3dZiTo0OaEd7sO";
            options.BaseUrl = "https://api.iyzipay.com";

            
            //options.ApiKey = "sandbox-3rHpqIv8uvy3rofE6e8SGMbAhHKAloOr";
            //options.SecretKey = "sandbox-zhp0x5PsQRh5ri8xk1RiOzgickEMBHDt";
            //options.BaseUrl = "https://sandbox-api.iyzipay.com";
            return options;
        }

        public LastSectionDto veri = new LastSectionDto();
        public string body = "";

        public PartialViewResult ThreeDPage(string body)
        {
            ViewBag.body = body;
            return PartialView();
        }

        public ActionResult callBack(string status, string mdStatus, string conversationData, string paymentId, string conversationId)
        {
            var xy = Request;
            if (mdStatus == "1" && status == "success")
            {
                var x = ctx.Siparisler.FirstOrDefault(q => q.ORDER_PAYMENT == conversationId);
                x.SiparisDurumu = 1;
                CreateThreedsPaymentRequest request = new CreateThreedsPaymentRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = conversationId;
                request.PaymentId = paymentId;
                request.ConversationData = conversationData;

                Iyzipay.Options options = new Iyzipay.Options();
                options = getIyziOptions();
                ThreedsPayment threedsPayment = ThreedsPayment.Create(request, options);
                Kernel.Mail("gidercicek@gmail.com", "Yeni sipariş: " + x.SiparisNo + " - " + string.Format("{0:C}", x.SiparisToplami) + " tutarında");
                ctx.SaveChanges();
                Response.Redirect("/Siparis/Basarili");
            }
            else
            {
                string donenDeger = "";
                switch (mdStatus)
                {
                    case "0":
                        donenDeger = "3-D Secure imzası geçersiz veya doğrulama";
                        break;
                    case "2":
                        donenDeger = "Kart sahibi veya bankası sisteme kayıtlı değil";
                        break;
                    case "3":
                        donenDeger = "Kartın bankası sisteme kayıtlı değil";
                        break;
                    case "5":
                        donenDeger = "Doğrulama yapılamıyor";
                        break;
                    case "6":
                        donenDeger = "3-D Secure hatası";
                        break;
                    case "7":
                        donenDeger = "Sistem hatası";
                        break;
                    case "8":
                        donenDeger = "Bilinmeyen kart no";
                        break;
                    case "4":
                        donenDeger = "Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş";
                        break;
                    default:
                        donenDeger = "Bilinmeyen bir hata oluştu";
                        break;
                }
                Response.Redirect("/Siparis/Problem/?m=" + donenDeger);
            }
            return View();
        }

        public string sayiSalla()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        //3d odeme methodu
        public JsonResult odemeYap(LastSectionDto datas)
        {
            MethodStatusDto _m = new MethodStatusDto();
            SiparisController _siparis = new SiparisController();
            datas.PosBilgileri.tutar = Convert.ToDecimal(SepetGenelToplamVerDecimalCinsinden(true, sepetAl()));
            datas.BasketItems = SepetUrunleri();
            try
            {

                Iyzipay.Options _o = new Iyzipay.Options();
                _o = getIyziOptions();

                CreatePaymentRequest request = new CreatePaymentRequest();
                request.Locale = Locale.TR.ToString();
                request.ConversationId = sayiSalla(); //burada
                request.Price = datas.PosBilgileri.tutar.ToString().Replace(",", ".");
                request.PaidPrice = datas.PosBilgileri.tutar.ToString().Replace(",", ".");
                request.Currency = Currency.TRY.ToString();
                request.Installment = 1;
                request.BasketId = sepetAl().SessionId;
                request.PaymentChannel = PaymentChannel.WEB.ToString();
                request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
                request.CallbackUrl = "http://www.cicekgider.com/Sepet/Callback"; //3d sayfasından sonra gideceği url

                PaymentCard paymentCard = new PaymentCard();
                paymentCard.CardHolderName = datas.PosBilgileri.kartSahibi;
                paymentCard.CardNumber = datas.PosBilgileri.kartNumarasi.ToString();
                paymentCard.ExpireMonth = datas.PosBilgileri.ay.ToString();
                paymentCard.ExpireYear = datas.PosBilgileri.yil.ToString();
                paymentCard.Cvc = datas.PosBilgileri.guvenlikKodu.ToString();
                paymentCard.RegisterCard = 0;
                request.PaymentCard = paymentCard;

                Buyer buyer = new Buyer();
                buyer.Id = Session.SessionID;
                buyer.Name = datas.SiparisVerenAdSoyad;
                buyer.Surname = datas.SiparisVerenAdSoyad;
                buyer.GsmNumber = datas.SiparisVerenTelefon;
                buyer.Email = "email@email.com";
                buyer.IdentityNumber = "00000";
                buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                buyer.RegistrationAddress = datas.AcikAdres;
                buyer.Ip = Request.UserHostAddress;
                buyer.City = "Mersin";
                buyer.Country = "Turkey";
                buyer.ZipCode = "33010";
                request.Buyer = buyer;

                Address shippingAddress = new Address();
                shippingAddress.ContactName = datas.AliciAdSoyad;
                shippingAddress.City = "Mersin";
                shippingAddress.Country = "Turkey";
                shippingAddress.Description = "Alıcı Kişi " + datas.AliciAdSoyad;
                shippingAddress.ZipCode = "33010";
                request.ShippingAddress = shippingAddress;

                Address billingAddress = new Address();
                billingAddress.ContactName = datas.AliciAdSoyad;
                billingAddress.City = "Mersin";
                billingAddress.Country = "Turkey";
                billingAddress.Description = datas.AcikAdres;
                billingAddress.ZipCode = "0";
                request.BillingAddress = billingAddress;
                List<BasketItem> basketItems = new List<BasketItem>();
                //al
                basketItems = datas.BasketItems;
                //ver
                request.BasketItems = basketItems;


                ThreedsInitialize threedsInitialize = ThreedsInitialize.Create(request, _o); //3d init
                // Payment payment = Payment.Create(request, _o);
                if (threedsInitialize.Status == "success") //3d init isteği başarılı ise,sayfa henüz açılmadı
                {

                    var sepet = sepetAl();
                    _siparis.SiparisEkle("Kredi Kartı", datas.AliciAdSoyad, datas.SiparisVerenAdSoyad + " - Telefon: " + datas.SiparisVerenTelefon, datas.Notlar, datas.AcikAdres + " Tarif: " + datas.AdresTarifi, sepet, request.ConversationId);
                    body = threedsInitialize.HtmlContent; //3d sayfasının htmli , viewda ekrana bastırılacak
                    veri = datas;
                    _m.ReturnMsg = body;
                    _m.Error = "success";
                    _m.ReturnValue = 0;
                    //    Response.Redirect("~/Sepet/ThreeDPage");
                    //_m.ReturnMsg = odemeReturnMsg;
                }
                else
                {
                    _m.ReturnMsg = threedsInitialize.ErrorMessage;
                }

            }
            catch (Exception)
            {
                _m.ReturnMsg = "Bir iç hata oluştu. Daha sonra tekrar deneyiniz.";
            }
            return Json(_m, JsonRequestBehavior.AllowGet);
        }


        public JsonResult LastSection(LastSectionDto _veriler)
        {
            Pos.BackResults br = new Pos.BackResults();
            OdemeController _odeme = new OdemeController();
            _veriler.PosBilgileri.tutar = Convert.ToDecimal(SepetGenelToplamVerDecimalCinsinden(true, sepetAl()));
            _veriler.BasketItems = SepetUrunleri();
            try
            {
                MethodStatusDto _m = new MethodStatusDto();
                var sepet = sepetAl();
                // br = _odeme.KrediKartindanCekimYap(_veriler.PosBilgileri);
            //    _m = iyziOdemeAl(_veriler);
                if (_m.ReturnMsg == "success")
                {
                    SiparisController _siparis = new SiparisController();
                    try
                    {
                        //_siparis.SiparisEkle("Kredi Kartı", _veriler.AliciAdSoyad, _veriler.SiparisVerenAdSoyad + "-Telefon: " + _veriler.SiparisVerenTelefon, _veriler.Notlar, _veriler.AcikAdres + " Tarif: " + _veriler.AdresTarifi, sepet);
                        _veriler.Success = true;
                    }
                    catch (Exception excep)
                    {
                        Kernel.Mail("gidercicek@gmail.com", "Yeni siparişiniz var.<br>Fakat sistemde oluşan bir hatadan ötürü bu veritabanına işlenemedi<br>Lütfen şu bilgiyi yazılım departmanına bildirin--- <br>" + excep.InnerException.Message);
                        Kernel.Mail("ismetsinar@hotmail.com", "Yeni siparişiniz var.<br>Fakat sistemde oluşan bir hatadan ötürü bu veritabanına işlenemedi<br>Lütfen şu bilgiyi yazılım departmanına bildirin--- <br>" + excep.InnerException.Message);
                    }
                }
                else
                {
                    _veriler.Success = false;
                    _veriler.resultDesc = _m.ReturnMsg;
                }
            }
            catch (Exception)
            {
                _veriler.Success = false;
            }
            return Json(_veriler, JsonRequestBehavior.AllowGet);
        }
        public SepetDto sepetAl()
        {
            SepetDto _r = new SepetDto();
            try
            {
                List<SEPET> _s = new List<SEPET>();
                string sessionid = sid();
                _s = ctx.Sepet.Where(q => q.SID == sessionid).ToList();
                _r.SepetUrunId = 1;
                _r.SessionId = sessionid;
                _r.Urunler = new List<SepetUrunlerDto>();
                foreach (var item in _s)
                {
                    _r.Urunler.Add(new SepetUrunlerDto()
                    {
                        Adet = item.Adet,
                        Fotograf = UrunlerFonksiyonlari.UrunFotografiVer(item.UrunId),
                        Marka = UrunlerFonksiyonlari.MarkaVer(item.UrunId),
                        UrunId = item.UrunId,
                        UrunBaslik = UrunlerFonksiyonlari.UrunAdiVer(item.UrunId),
                        UrunFiyat = UrunlerFonksiyonlari.UrunFiyatVer(item.UrunId),
                        UrunKodu = UrunlerFonksiyonlari.UrunKoduVer(item.UrunId),
                        Ozelligi = UrunlerFonksiyonlari.UrunOzellikVer(item.UrunId)
                    });
                }
            }

            catch (Exception)
            {
                throw new Exception("Sepetiniz boş görünüyor");
            }
            return _r;
        }
    }
}
