using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B.Models.Entity
{
    [Table("ORDER_PAYMENTS")]
    public partial class SIPARIS_ODEMELERI
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PAYMENT_TYPE")]
        public string OdemeTipi { get; set; }
        [Column("PAYMENT_AMOUNT")]
        public decimal OdemeTutari { get; set; }
        [Column("PAYMENT_FIRMAID")]
        public int OrderId { get; set; }
    }
    [Table("CATEGORIES")]
    public partial class CATEGORIES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("CATEGORY_TITLE")]
        public string KategoriBaslik { get; set; }
        [Column("CATEGORY_ORDER")]
        public string KategoriSira { get; set; }
        [Column("CATEGORY_SUB_ID")]
        public int KategoriUst { get; set; }
        [Column("CATEGORY_SHOW")]
        public int KategoriGoster { get; set; }
    }

    [Table("PRODUCTS")]
    public partial class PRODUCTS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PRODUCT_NAME")]
        public string UrunAdi { get; set; }
        [Column("PRODUCT_CATEGORY")]
        public int UrunKategorisi { get; set; }
        [Column("PRODUCT_TAXCLASS")]
        public int UrunVergiOrani { get; set; }
        [Column("PRODUCT_STATUS")]
        public int UrunDurum { get; set; }
        [Column("PRODUCT_WEIGHT")]
        public decimal? UrunAgirlik { get; set; }
        [Column("PRODUCT_WEIGHT_CLASSID")]
        public int UrunAgirlikSinifi { get; set; }
        [Column("PRODUCT_PRICE")]
        public decimal UrunFiyat { get; set; }
        [Column("PRODUCT_CODE")]
        public string UrunKodu { get; set; }
        [Column("PRODUCT_DESCRIPTION")]
        public string UrunAciklama { get; set; }
        [Column("PRODUCT_STOCK")]
        public decimal Stok { get; set; }
        [Column("PRODUCT_DELIVERY_DETAILS")]
        public int GonderimTipi { get; set; }
        [Column("PRODUCTS_DISCOUNT")]
        public int IndirimGrubu { get; set; }
        [Column("PRODUCT_FAV")]
        public int OneCikanUrun { get; set; }
        [Column("PRODUCT_MINSIP")]
        public decimal MinimumSiparisAdeti { get; set; }
        [Column("PRODUCT_MARKAID")]
        public int MarkaId { get; set; }
        [Column("PRODUCT_PIC")]
        public string UrunFotografi { get; set; }
        [Column("PRODUCT_STOKTANDUS")]
        public int StoktanDus { get; set; }
        [Column("PRODUCT_MODEL")]
        public int ModelId { get; set; }
        [Column("PRODUCTS_YENIURUN")]
        public int? YeniUrun { get; set; }
        [Column("PRODUCTS_ALTERCATS")]
        public string AlternatifKategoriler { get; set; }

        [ForeignKey("UrunId")] // Ürün Yorumlar Tablosundan al
        public virtual ICollection<PRODUCTS_COMMENTS> UrunYorumlari { get; set; }

        [ForeignKey("UrunId")] // Benzeyen Ürünleri getir
        public virtual ICollection<PRODUCTS_SIMILAR> BenzeyenUrunler { get; set; }

        [ForeignKey("UrunId")] // Ürün Fotoğrafları
        public virtual ICollection<PRODUCTS_PHOTO> UrunFotograflari { get; set; }

        [ForeignKey("UrunId")] // Ürün Ödeme Şekilleri
        public virtual ICollection<PRODUCTS_PAYMENT_TYPES> UrunOdemeSekilleri { get; set; }

        [ForeignKey("UrunId")] // Ürün Özellikleri
        public virtual ICollection<PRODUCTS_PROPERTY> UrunOzellikleri { get; set; }
    }

    [Table("ADDRESS")]
    public partial class ADDRESS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("ADDRESS_TEXT")]
        public string Adres { get; set; }
        [Column("ADDRESS_USERID")]
        public int UserId { get; set; }
        [Column("ADDRESS_TYPE")]
        public string AdresTipi { get; set; }
    }

    [Table("PRODUCTS_COMMENTS")]
    public partial class PRODUCTS_COMMENTS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("COMMENT")]
        public string UrunYorumu { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
        [Column("PRODUCT_STARS")]
        public int UrunYildiz { get; set; }
    }

    [Table("PRODUCTS_TAXCLASS")]
    public partial class PRODUCTS_TAXCLASS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TAX_NAME")]
        public string VergiAdi { get; set; }
        [Column("TAX_RATE_PERCENTAGE")]
        public int VergiOrani { get; set; }
    }

    [Table("PRODUCTS_SIMILAR")]
    public partial class PRODUCTS_SIMILAR
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
        [Column("PRODUCT_SIMILAR")]
        public int BenzeyenUrunId { get; set; }
    }

    [Table("PRODUCTS_PHOTO")]
    public partial class PRODUCTS_PHOTO
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PHOTO_URL")]
        public string FotoUrl { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
        [Column("PHOTO_PREVIEW")]
        public bool UrunKucukResim { get; set; }
    }

    [Table("PRODUCTS_DELIVERY_DETAILS")]
    public partial class PRODUCTS_DELIVERY_DETAILS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("DELIVERY_TIME")]
        public string TeslimatSuresi { get; set; }
        [Column("DELIVERY_PRICE")]
        public decimal TeslimatUcreti { get; set; }
        [Column("DELIVERY_TYPE")]
        public string TeslimatTuru { get; set; }
        [Column("DELIVERY_INSTALLMENTS")]
        public string MontajTuru { get; set; }
    }

    [Table("PRODUCTS_WEIGHT")]
    public partial class PRODUCTS_WEIGHT
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("WEIGHT_TITLE")]
        public string Agirlik { get; set; }
        [Column("WEIGHT_SHORTCUT")]
        public string WeightShortCut { get; set; }
    }

    [Table("PRODUCTS_DETAILS_EXTRA")]
    public partial class PRODUCTS_DETAILS_EXTRA
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("DETAIL_DESC")]
        public string DetayAciklamasi { get; set; }
        [Column("DETAIL_TYPE")]
        public string DetayTipi { get; set; }
    }

    [Table("PRODUCTS_TAGS")]
    public partial class PRODUCTS_TAGS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TAG")]
        public string Tag { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
    }

    [Table("PAYMENT_TYPES")]
    public partial class PAYMENT_TYPES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PAYMENT_TITLE")]
        public string OdemeAdi { get; set; }
        [Column("PAYMENT_TYPE")]
        public int OdemeTipi { get; set; }
        [Column("PAYMENT_DESCRIPTION")]
        public string OdemeAciklamasi { get; set; }
        [Column("PAYMENT_ACTIVE")]
        public int Aktif { get; set; }
    }

    [Table("PRODUCTS_PAYMENT_TYPES")]
    public partial class PRODUCTS_PAYMENT_TYPES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PAYMENT_TYPE")]
        public string OdemeSekli { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
    }

    [Table("PRODUCTS_DISCOUNT")]
    public partial class PRODUCTS_DISCOUNT
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("DISCOUNT_RATE")]
        public int IndirimOrani { get; set; }
        [Column("DISCOUNT_TITLE")]
        public string IndirimAdi { get; set; }
        [Column("DISCOUNT_FROM")]
        public DateTime IndirimBaslangic { get; set; }
        [Column("DISCOUNT_UNTIL")]
        public DateTime IndirimBitis { get; set; }
    }

    [Table("PRODUCTS_PROPERTY")]
    public partial class PRODUCTS_PROPERTY
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PROPERTY_TITLE")]
        public string Ozellik { get; set; }
        [Column("PROPERTY_TYPE")]
        public string OzellikTipi { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
        [Column("PRODUCT_STOCK")]
        public int OzellikStok { get; set; }
    }

    [Table("PRODUCTS_FAVORITES")]
    public partial class PRODUCTS_FAVORITES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PRODUCT_ID")]
        public int UrunId { get; set; }
        [Column("DATE_ADDED")]
        public DateTime EklemeTarihi { get; set; }
        [Column("USER_ID")]
        public int UserId { get; set; }
    }

    [Table("USERS")]
    public partial class USERS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("USERNAME")]
        public string USERNAME { get; set; }
        [Column("PASSWORD")]
        public string PASSWORD { get; set; }
        [Column("LASTLOGIN")]
        public DateTime LastLogin { get; set; }
        [Column("ACTIVE")]
        public int Aktif { get; set; }
        [Column("FIRSTNAME")]
        public string Adi { get; set; }
        [Column("LASTNAME")]
        public string Soyadi { get; set; }
        [Column("KAYIT_TARIHI")]
        public DateTime KayitTarihi { get; set; }
        [Column("EPOSTA")]
        public string Eposta { get; set; }
        [Column("COMPANY_ID")]
        public int FirmaId { get; set; }
        [Column("USER_STATUS")]
        public string KullaniciYetkisi { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<ADDRESS> KullaniciAdresleri { get; set; }
    }

    [Table("orders_new")]
    public partial class ORDERS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("ORDER_NUMBER")]
        public string SiparisNo { get; set; }
        [Column("ORDER_BY")]
        public int SiparisVeren { get; set; }
        [Column("ORDER_TOTALPRICE")]
        public decimal? SiparisToplami { get; set; }
        [Column("ORDER_DATE")]
        public DateTime SiparisTarihi { get; set; }
        [Column("ORDER_FIRMAID")]
        public int FirmaId { get; set; }
        [Column("ORDER_STATUS")]
        public int SiparisDurumu { get; set; }
        [Column("ORDER_PAYMENTTYPE")]
        public string OdemeTipi { get; set; }
        [Column("ORDER_JUSTDATE")]
        public DateTime SadeceTarih { get; set; }
        [Column("ORDER_NOTES")]
        public string SiparisNotlari { get; set; }
        [Column("ORDER_ADRES")]
        public string SiparisAdresi { get; set; }
        [Column("ORDER_DISCOUNT")]
        public int SiparisIskontoOrani { get; set; }
        [Column("ORDER_DISCOUNT_AMOUNT")]
        public decimal IskontoTutar { get; set; }
        [Column("ORDER_KDV")]
        public int KdvOrani { get; set; }
        [Column("ORDER_MANUELSUM")]
        public int ManuelTutar { get; set; }
        [Column("ORDER_DESC")]
        public string SiparisAyrintilari { get; set; }
        [Column("ORDER_SIPARISVEREN")]
        public string SiparisiVerenAdSoyad { get; set; }
        [Column("ORDER_PAYMENT")]
        public string ORDER_PAYMENT { get; set; }

        [ForeignKey("SiparisNo")]
        public virtual ICollection<ORDER_PRODUCTS> SiparisUrunleri { get; set; }

        [ForeignKey("OrderId")]
        public virtual ICollection<SIPARIS_ODEMELERI> SipariseAitOdemeler { get; set; }
    }

    [Table("ORDER_PRODUCTS")]
    public partial class ORDER_PRODUCTS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("ORDER_PRODID")]
        public int UrunId { get; set; }
        [Column("ORDER_PIECE")]
        public decimal Adet { get; set; }
        [Column("ORDER_PRICE")]
        public decimal KalemTutar { get; set; }
        [Column("ORDER_ORDERNUMBER")]
        public int SiparisNo { get; set; }
    }

    [Table("COMPANIES")]
    public partial class COMPANIES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("COMPANIES_NAME")]
        public string FirmaAdi { get; set; }
        [Column("COMPANIES_TRADENAME")]
        public string FirmaUnvani { get; set; }
        [Column("COMPANIES_AUTHORITY")]
        public string YetkiliKisi { get; set; }
        [Column("COMPANIES_TELEPHONE")]
        public string FirmaTelefon { get; set; }
        [Column("COMPANIES_RESELLERCODE")]
        public string BayiKodu { get; set; }
        [Column("COMPANIES_DISCOUNT")]
        public int FirmaIskonto { get; set; }
        [Column("COMPANIES_ALLOW_MINUS")]
        public int CariHesapIzni { get; set; }
        [Column("COMPANIES_CITY")]
        public string Sehir { get; set; }
        [Column("COMPANIES_FAX")]
        public string Faks { get; set; }
        [Column("COMPANIES_REGDATE")]
        public DateTime KayitTarihi { get; set; }
        [Column("COMPANIES_VISIT")]
        public int ZiyaretSayisi { get; set; }
        [Column("COMPANIES_LASTVISIT")]
        public DateTime LastVisit { get; set; }
        [Column("COMPANIES_ACTIVE")]
        public int Aktif { get; set; }
        [Column("COMPANIES_TAXCODE")]
        public string VergiNo { get; set; }
        [Column("COMPANIES_TAXCENTER")]
        public string VergiDairesi { get; set; }

        [ForeignKey("FirmaId")]
        public virtual ICollection<USERS> FirmaKullanicilari { get; set; }

        [ForeignKey("FirmaId")]
        public virtual List<MUHASEBE> FirmaMuhasebe { get; set; }

        [ForeignKey("FirmaId")]
        public virtual List<ORDERS> FirmaSiparisleri { get; set; }

        [ForeignKey("FirmaId")]
        public virtual List<TRANSACTIONS> FirmaGirdiCiktilar { get; set; }

        [ForeignKey("UyariFirmaId")]
        public virtual List<UYARILAR> FirmaUyarilari { get; set; }

        [ForeignKey("FirmaId")]
        public virtual List<VADE_TARIHLERI> VadeTarihi { get; set; }

    }

    [Table("BRANDS")]
    public partial class BRANDS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("BRAND_NAME")]
        public string Marka { get; set; }
        [Column("BRAND_SUB_ID")]
        public int MarkaAltId { get; set; }
    }

    [Table("MUHASEBE")]
    public partial class MUHASEBE
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("MUHASEBE_FIRMAID")]
        public int FirmaId { get; set; }
        [Column("MUHASEBE_BAKIYE")]
        public decimal Bakiye { get; set; }
        [Column("MUHASEBE_LASTUPDATE")]
        public DateTime SonGuncelleme { get; set; }
    }

    [Table("ORDER_STATUS")]
    public partial class SIPARIS_DURUMLARI
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("ORDERSTATUS_TITLE")]
        public string SiparisDurumu { get; set; }
        [Column("ORDERSTATUS_ACTIVE")]
        public int Aktif { get; set; }
    }

    [Table("TRANSACTIONS")]
    public partial class TRANSACTIONS
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("PAYMENTS_FIRMAID")]
        public int FirmaId { get; set; }
        [Column("PAYMENTS_TUTAR")]
        public decimal Tutar { get; set; }
        [Column("PAYMENTS_DATE")]
        public DateTime TarihSaat { get; set; }
        [Column("PAYMENTS_ACIKLAMA")]
        public string Aciklama { get; set; }
        [Column("PAYMENTS_USERID")]
        public int UserId { get; set; }
        [Column("PAYMENT_TYPE")]
        public int GirdiTipi { get; set; } // 1 ise girdi 0 ise çıktı
        [Column("PAYMENT_STATUS")]
        public int GirdiDurum { get; set; }

    }

    [Table("DUYURULAR")]
    public partial class DUYURULAR
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("DUYURU_CONT")]
        public string Duyuru { get; set; }
        [Column("DUYURU_TARIH")]
        public DateTime EklenmeTarihi { get; set; }
    }

    [Table("SEPET")]
    public partial class SEPET
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("SEPET_URUNID")]
        public int UrunId { get; set; }
        [Column("SEPET_FIRMAID")]
        public int FirmaId { get; set; }
        [Column("SEPET_USERID")]
        public int UserId { get; set; }
        [Column("SEPET_ADET")]
        public decimal Adet { get; set; }
        [Column("SID")]
        public string SID { get; set; }

    }
    [Table("ACTIVITIES")]
    public partial class ACTIVITIES
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("ACTIVITY_CONT")]
        public string Aktivite { get; set; }
        [Column("ACTIVITY_TYPE")]
        public int AktiviteTipi { get; set; }
        [Column("ACTIVITY_BY")]
        public int AktiviteYapanKullanici { get; set; }
        [Column("ACTIVITIES_DATETIME")]
        public DateTime SaatTarih { get; set; }
        [Column("ACTIVITY_COMPANY")]
        public int AktiviteYapanFirma { get; set; }

    }
    [Table("KARGOLAR")]
    public partial class KARGOLAR
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("KARGO_ADI")]
        public string Kargo { get; set; }
        [Column("KARGO_SIRALAMA")]
        public int Sira { get; set; }
        [Column("KARGO_FIYAT")]
        public decimal Fiyat { get; set; }
    }
    [Table("SENDING_TYPES")]
    public partial class GONDERI_TIPLERI
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("GONDERIM_TIPI_ISIM")]
        public string GonderiTipi { get; set; }
        [Column("GONDERIM_TYPE")]
        public int GonderiType { get; set; }
    }

    [Table("SYSTEM_SETTINGS")]
    public partial class SISTEM_AYARLARI
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("SYSTEM_PAYMENT_DATE_WARNING")]
        public string OdemeYapinizYazisi { get; set; }
    }

    [Table("WARNINGS")]
    public partial class UYARILAR
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("WARNING_TYPE")]
        public string UyariTipi { get; set; }
        [Column("WARNING_PRIORITY")]
        public int UyariOncelik { get; set; }
        [Column("WARNING_CONTENT")]
        public string Uyari { get; set; }
        [Column("WARNING_UNTIL")]
        public DateTime GecerlilikTarihi { get; set; }
        [Column("WARNING_TO")]
        public int UyariFirmaId { get; set; }

    }

    [Table("VADE_TARIHLERI")]
    public partial class VADE_TARIHLERI
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("FIRMAID")]
        public int FirmaId { get; set; }
        [Column("TARIH")]
        public DateTime VadeTarihi { get; set; }
        [Column("ONGORULEN_MIKTAR")]
        public decimal OngorulenMiktar { get; set; }
    }

    [Table("ilce")]
    public partial class ilce
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("il_id")]
        public int il_id { get; set; }
        [Column("ILCE_ADI_BUYUK")]
        public string ilce_adi_buyuk { get; set; }
        [Column("ilce_ad")]
        public string ilce_ad { get; set; }
        [Column("ILCE_ADI_KUCUK")]
        public string ilce_adi_kucuk { get; set; }
    }
    [Table("mahalle")]
    public partial class mahalle
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("il_id")]
        public int il_id { get; set; }
        [Column("ilce_id")]
        public int ilce_id { get; set; }
        [Column("semt_id")]
        public int semt_id { get; set; }
        [Column("MAHALLE_ADI_BUYUK")]
        public string mahalle_adi_buyuk { get; set; }
        [Column("mahalle_ad")]
        public string mahalle_ad { get; set; }
        [Column("MAHALLE_ADI_KUCUK")]
        public string mahalle_ad_kucuk { get; set; }

    }

    [Table("semt")]
    public partial class semt
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IL_ID { get; set; }
        public int ILCE_ID { get; set; }
        public string SEMT_ADI_BUYUK { get; set; }
        public string SEMT_ADI { get; set; }
        public string SEMT_ADI_KUCUK { get; set; }
        public string POSTA_KODU { get; set; }
    }
}