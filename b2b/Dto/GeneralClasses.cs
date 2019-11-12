using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2B.Models.Entity;

namespace B2B.Dto
{
    public class UserHolder
    {
        public int UserId { get; set; }
        public string KullaniciAdi { get; set; }
        public string FirmaUnvani { get; set; }
        public int FirmaIskontoOrani { get; set; }
        public int FirmaId { get; set; }
        public string TelefonNo { get; set; }
        public string BayiKodu { get; set; }
        public string KullaniciYetkisi { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string FirmaAdres { get; set; }
    }
    public class Sepet
    {
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public int Kdv { get; set; }
        public decimal Fiyat { get; set; }
    }
    public class GonderimDetaylari
    {
        public string GonderimSuresi { get; set; }
        public decimal GonderimUcreti { get; set; }
        public string GonderimTipi { get; set; }
        public string Montaj { get; set; }
    }
    public class SepetDto
    {
        public int SepetUrunId { get; set; }
        public List<SepetUrunlerDto> Urunler { get; set; }
        public string SessionId { get; set; }
    }
    public class SepetUrunlerDto
    {
        public int UrunId { get; set; }
        public string UrunBaslik { get; set; }
        public decimal Adet { get; set; }
        public int? OzellikId { get; set; }
        public string Ozelligi { get; set; }
        public string UrunKodu { get; set; }
        public string Marka { get; set; }
        public string Fotograf { get; set; }
        public decimal UrunFiyat { get; set; }
    }
    public class KullaniciAdresleri
    {
        public int UserId { get; set; }
        public int AdresId { get; set; }
        public string Adres { get; set; }
        public string AdresTipi { get; set; }
    }
    public class DuyurularDto
    {
        public int DuyuruId { get; set; }
        public string Duyuru { get; set; }
        public DateTime DuyuruTarihi { get; set; }
    }
    public class HomePageHeaderData
    {
        public decimal siparislerinToplamTutari { get; set; }
        public int ToplamSiparisAdeti { get; set; }
        public int UrunKategorileriSayisi { get; set; }
        public int BayiAgiSayisi { get; set; }
        public string uid { get; set; }
        public string pw { get; set; }
    }
    public class HomePageClasses
    {
        public List<DuyurularDto> Duyurular { get; set; }
        public List<HomePageSonSiparisler> SonSiparisler { get; set; }
        public HomePageHeaderData UstBilgiler { get; set; }
        public List<UyarilarDTO> Uyarilar { get; set; }
        public UrunlerDto.OneCikanUrunListesi FavoriUrun { get; set; }
        public ICollection<IndirimdekiUrunler> Indirim { get; set; }
        public ICollection<YeniUrunler> YeniUrunler { get; set; }
        public ICollection<UygunFiyat> UygunFiyatlar { get; set; }

    }
    public class UyarilarDTO
    {
        public int Id { get; set; }
        public string UyariTip { get; set; }
        public int UyariOncelik { get; set; }
        public string Uyari { get; set; }
        public DateTime GecerlilikTarihi { get; set; }
    }
    public class HomePageSonSiparisler
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string SiparisKodu { get; set; }
        public decimal ToplamTutar { get; set; }
        public string UruneAitBirFotograf { get; set; }
        public string SiparisDurumu { get; set; }
    }
    public class HesapOzetiClass
    {
        public int IslemId { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string IslemAciklamasi { get; set; }
        public string IslemDurum { get; set; }
        public decimal IslemTutar { get; set; }
        public int IslemTipi { get; set; }
    }
    public class HesapOzetiDto
    {
        public int SiparisToplam { get; set; }
        public decimal SiparislerToplamTutar { get; set; }
        public decimal BakiyeTutar { get; set; }
        public string ToplamZiyaret { get; set; }
        public List<HesapOzetiClass> HesapOzetiListesi { get; set; }
        public VADE_TARIHLERI VadeTarihiHatirlatici { get; set; }
    }
    public class SepetHesapAraci
    {
        public decimal ToplamTutar { get; set; }
        public decimal AraToplam { get; set; }
        public int KdvOrani { get; set; }
        public decimal KdvTutari { get; set; }
        public int IskontoOrani { get; set; }
        public decimal IskontoTutari { get; set; }
        public decimal GenelTutar { get; set; }
        public decimal IskontoluGenelTutar { get; set; }
    }
    public class GecmisSiparisUrunler
    {
        public int UrunId { get; set; }
        public string UrunBaslik { get; set; }
        public decimal Adet { get; set; }
        public decimal Tutar { get; set; }
    }
    public class GecmisSiparisAyrintilari
    {
        public int Id { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string SiparisKodu { get; set; }
        public int SiparisDurumKodu { get; set; }
        public string SiparisDurum { get; set; }
        public decimal? SiparisToplami { get; set; }
        public List<GecmisSiparisUrunler> SiparisUrunleri { get; set; }

    }
    public class SiparisAyrintilari
    {
        public int SiparisId { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string SiparisKodu { get; set; }
        public decimal SiparisToplami { get; set; }
        public int Kdv { get; set; }
        public int Iskonto { get; set; }
        public decimal IskontoTutar { get; set; }
        public List<GecmisSiparisUrunler> SiparisUrunleri { get; set; }
    }
    public class FotografClass
    {
        public int FotoId { get; set; }
        public string FileName { get; set; }
    }
    public class MarkayaGoreAramaSinifi
    {
        public List<BRANDS> Markalar { get; set; }
        public List<PRODUCTS> Urunler { get; set; }
    }
    public class SearchOptions
    {
        public int? PRODUCT_CATEGORY { get; set; }
        public int? PRODUCT_MARKAID { get; set; }
        public int? PRODUCT_MODELID { get; set; }
        public string PRODUCT_NAME { get; set; }
    }
    public class Aktivite
    {
        public int FirmaId { get; set; }
        public int UserId { get; set; }
        public string AktiviteSTR { get; set; }
        public DateTime AktiviteTarihSaat { get; set; }
        public int AktiviteTipi { get; set; }
    }
    public class IndirimdekiUrunler
    {
        public int urunId { get; set; }
        public decimal urunFiyat { get; set; }
        public string urunFoto { get; set; }
        public string urunAdi { get; set; }
    }
    public class YeniUrunler
    {
        public int urunId { get; set; }
        public decimal urunFiyat { get; set; }
        public string urunFoto { get; set; }
        public string urunAdi { get; set; }
    }
    public class UygunFiyat
    {
        public int urunId { get; set; }
        public decimal urunFiyat { get; set; }
        public string urunFoto { get; set; }
        public string urunAdi { get; set; }
    }
    public class PosForm
    {
        public string kartSahibi { get; set; }
        public long kartNumarasi { get; set; }
        public int guvenlikKodu { get; set; }
        public int ay { get; set; }
        public int yil { get; set; }
        public decimal tutar { get; set; }
        public int taksit { get; set; }
    }
    public class LastSectionDto
    {
        public PosForm PosBilgileri { get; set; }
        public string GonderenAdSoyad { get; set; }
        public string TeslimTarihSaat { get; set; }
        public string AliciAdSoyad { get; set; }
        public string Sehir { get; set; }
        public string AcikAdres { get; set; }
        public string AdresTarifi { get; set; }
        public string Notlar { get; set; }
        public bool Success { get; set; }
        public string resultDesc { get; set; }
        public string SiparisVerenAdSoyad { get; set; }
        public string SiparisVerenTelefon { get; set; }
        public List<Iyzipay.Model.BasketItem> BasketItems { get; set; }
    }
    public class YeniSiparis
    {
        public int Id { get; set; }
        public int Goruldu { get; set; }
        public string SiparisNo { get; set; }
        public string SiparisVeren { get; set; }
    }
    public class YeniSiparisListesi
    {
        public ICollection<YeniSiparis> NewOrders { get; set; }
    }

}