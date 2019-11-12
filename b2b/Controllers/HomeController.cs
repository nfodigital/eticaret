using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Entity;

namespace B2B.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        ProductsController UrunlerKontrolcusu = new ProductsController();
        public ActionResult Index()
        {
            HomePageClasses HomePage = new HomePageClasses();
            CommonFunctionsController OrtakFonksiyonlar = new CommonFunctionsController();

            /*/
            Sırasıyla Yüklenecekler
            1. Duyurular
            2. Üst Bilgiler
            /*/

            var HomePageDuyurular = (from ar in ctx.Duyurular.OrderByDescending(q => q.Id).ToList()
                                     select new DuyurularDto
                                     {
                                         Duyuru = ar.Duyuru,
                                         DuyuruId = ar.Id,
                                         DuyuruTarihi = ar.EklenmeTarihi
                                     }
            ).ToList();
            /*/
             * Favori Ürün 1 adet
            /*/

            UrunlerDto.OneCikanUrunListesi fav = new UrunlerDto.OneCikanUrunListesi();
            PRODUCTS_FAVORITES fav_table = new PRODUCTS_FAVORITES(); fav_table = ctx.OneCikanlar.OrderByDescending(q=>q.Id).FirstOrDefault();

            fav.UrunAdi = UrunlerKontrolcusu.UrunAdiVer(fav_table.UrunId);
            fav.UrunFiyat = UrunlerKontrolcusu.UrunFiyatVer(fav_table.UrunId);
            //fav.UrunFoto = UrunlerKontrolcusu.UrunFotografiVer(fav_table.UrunId);
            fav.UrunFoto = UrunlerKontrolcusu.FotografVer(fav_table.UrunId);
            fav.UrunId = fav_table.UrunId;
            HomePage.FavoriUrun = fav;

            /*/
             * İndirimdeki ürünleri listele
             * /*/

            var Indirim = (from indirim in ctx.Urunler.OrderByDescending(q=>q.Id).Take(25).ToList()
                           select new IndirimdekiUrunler
                           {
                               urunAdi = indirim.UrunAdi,
                               urunFiyat = indirim.UrunFiyat,
                               urunFoto = indirim.UrunFotografi,
                               urunId = indirim.Id
                           }).ToList();

            var Yeni = (from y in ctx.Urunler.Where(q => q.YeniUrun == 1).OrderByDescending(q=>q.Id).ToList()
                        select new YeniUrunler
                        {
                            urunAdi = y.UrunAdi,
                            urunFiyat = y.UrunFiyat,
                            urunFoto = y.UrunFotografi,
                            urunId = y.Id
                        }).ToList();

            var Uygun = (from o in ctx.Urunler.Where(q => q.UrunFiyat <= 70).ToList()
                               select new UygunFiyat
                               {
                                   urunAdi = o.UrunAdi,
                                   urunFiyat = o.UrunFiyat,
                                   urunFoto = o.UrunFotografi,
                                   urunId = o.Id
                               }).ToList();

            HomePage.Indirim = Indirim;
            HomePage.YeniUrunler = Yeni;
            HomePage.UygunFiyatlar = Uygun;
            ViewBag.Desc = "Mersin çiçek siparişi sektöründe en başarılı çiçek sipariş hizmeti veren şirketimiz, bir tıkla sipariş!";

            return View(HomePage);
        }
        public string SessionRefresh()
        {
            string _val = "ok";
            try
            {
                if (getAktifKullanici() == null)
                {
                    _val = "exited";
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("error refreshing session" + ex.Message);
            }
            return _val;
        }

        public ActionResult Gizlilik()
        {
            return View();
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Teslimat()
        {
            return View();
        }

        public ActionResult Mesafeli()
        {
            return View();
        }

    }
}
