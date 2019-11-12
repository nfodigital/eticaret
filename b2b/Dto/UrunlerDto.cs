using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2B.Models.Entity;

namespace B2B.Dto
{
    public class UrunlerDto
    {
        public class TekilUrun
        {
            public int UrunId { get; set; }
            public string UrunKodu { get; set; }
            public string UrunAdi { get; set; }
            public string UrunKategorisi { get; set; }
            public string UrunAciklamasi { get; set; }
            public int UrunIndirimTipi { get; set; }
            public string UrunVergiSinifi { get; set; }
            public int UrunVergiOrani { get; set; }
            public decimal UrunStok { get; set; }
            public string UrunAgirlik { get; set; }
            public decimal UrunFiyati { get; set; }
            public decimal Urun_Indirimli_Fiyati { get; set; }
            public int Urun_Indirim_Orani { get; set; }
            public ICollection<PRODUCTS_PHOTO> UrunFotograflari { get; set; }
            public ICollection<PRODUCTS_COMMENTS> UrunYorumlari { get; set; }
            public ICollection<PRODUCTS_SIMILAR> BenzerUrunler { get; set; }
            public ICollection<PRODUCTS_DETAILS_EXTRA> EkstraDetaylar { get; set; }
            public ICollection<PRODUCTS_PROPERTY> UrunOzellikleri { get; set; }
            public ICollection<PAYMENT_TYPES> UrunOdemeSekilleri { get; set; }
            public KampanyaDetaylari Kampanya { get; set; }
            public GonderimDetaylari TeslimatDetaylari { get; set; }
        }
        public class GonderimDetaylari
        {
            public string GonderimSuresi { get; set; }
            public decimal GonderimUcreti { get; set; }
            public string GonderimTipi { get; set; }
            public string Montaj { get; set; }
        }
        public class GarantiKosullari
        {
            public string GarantiKosullarMetni { get; set; }
        }
        public class KampanyaDetaylari
        {
            public string KampanyaAdi { get; set; }
            public int IndirimOrani { get; set; }
            public DateTime KampanyaBaslangic { get; set; }
            public DateTime KampanyaBitis { get; set; }
        }
        public class KampanyaliUrunListesi
        {
            public int UrunId { get; set; }
            public string UrunAdi { get; set; }
            public string UrunFotografi { get; set; }
            public int IndirimOrani { get; set; }
            public decimal UrunFiyat { get; set; }
            public decimal KampanyaliUrunFiyat { get; set; }
        }
        public class OneCikanUrunListesi
        {
            public int UrunId { get; set; }
            public string UrunAdi { get; set; }
            public decimal UrunFiyat { get; set; }
            public int Indirim { get; set; }
            public decimal IndirimliFiyat { get; set; }
            public string UrunFoto { get; set; }
        }

        public class KategoriListele
        {
            public int ID { get; set; }
            public string KategoriBaslik { get; set; }
            public string KategoriSira { get; set; }
            public int KategoriGoster { get; set; }
            public int UstKategorisi { get; set; }
            public ICollection<KategoriListeleAlt> AltKategorileri { get; set; }
        }
        public class KategoriListeleAlt
        {
            public int ID { get; set; }
            public string KategoriBaslik { get; set; }
            public string KategoriSira { get; set; }
            public int KategoriGoster { get; set; }
            public int UstKategorisi { get; set; }
        }
        public class KategoriUrunListele
        {
            public int UrunId { get; set; }
            public string UrunAdi { get; set; }
            public decimal UrunFiyat { get; set; }
            public int Indirim { get; set; }
            public decimal IndirimliFiyat { get; set; }
            public string UrunFoto { get; set; }
            public int IndirimOrani { get; internal set; }
            public decimal KampanyaliUrunFiyat { get; set; }
        }

        public class AramaSonucuUrunleri
        {
            public int UrunId { get; set; }
            public string UrunAdi { get; set; }
            public string UrunFotografi { get; set; }
            public decimal MinSepeteEkleAdeti { get; set; }
            public decimal UrunFiyati { get; set; }
            public decimal Stok { get; set; }
            public string UrunFirsat { get; set; }
            public string UrunKodu { get; set; }
        }

        public class BenzeyenUrunListesi
        {
            public int AsilUrunId { get; set; }
            public int BenzeyenUrunId { get; set; }
            public string BenzeyenUrunBasligi { get; set; }
            public string BenzeyenUrunFotografi { get; set; }
            public decimal BenzeyenUrunFiyati { get; set; }
        }

    }
}