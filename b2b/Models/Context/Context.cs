using System.Data.Entity;
using B2B.Models.Entity;

namespace B2B.Models.Context
{
    public partial class Context : DbContext
    {

        static Context()
        {
            Database.SetInitializer<Context>(null);
        }

        public Context()
            : base("Name=Context")
        {
        }

        public DbSet<ADDRESS> KullaniciAdresleri { get; set; }
        public DbSet<CATEGORIES> Kategoriler { get; set; }
        public DbSet<PAYMENT_TYPES> OdemeSekilleri { get; set; }
        public DbSet<PRODUCTS> Urunler { get; set; }
        public DbSet<PRODUCTS_COMMENTS> UrunYorumlari { get; set; }
        public DbSet<PRODUCTS_DELIVERY_DETAILS> TeslimatDetaylari { get; set; }
        public DbSet<PRODUCTS_DETAILS_EXTRA> Urunler_Ekstra_Detaylar { get; set; }
        public DbSet<PRODUCTS_DISCOUNT> UrunlerIndirim { get; set; }
        public DbSet<PRODUCTS_PAYMENT_TYPES> UrunlerOdemeSekilleri { get; set; }
        public DbSet<PRODUCTS_PHOTO> UrunFotograflari { get; set; }
        public DbSet<PRODUCTS_PROPERTY> UrunOzellikleri { get; set; }
        public DbSet<PRODUCTS_SIMILAR> UrunlerBenzer { get; set; }
        public DbSet<PRODUCTS_TAGS> UrunEtiketleri { get; set; }
        public DbSet<PRODUCTS_TAXCLASS> UrunlerVergiSinifi { get; set; }
        public DbSet<PRODUCTS_WEIGHT> UrunlerAgirlikOlculeri { get; set; }
        public DbSet<USERS> Kullanicilar { get; set; }
        public DbSet<PRODUCTS_FAVORITES> OneCikanlar { get; set; }
        public DbSet<ORDERS> Siparisler { get; set; }
        public DbSet<ORDER_PRODUCTS> SiparisUrunleri { get; set; }
        public DbSet<SIPARIS_DURUMLARI> SiparisDurumlari { get; set; }
        public DbSet<COMPANIES> Firmalar { get; set; }
        public DbSet<BRANDS> Markalar { get; set; }
        public DbSet<MUHASEBE> Muhasebe { get; set; }
        public DbSet<TRANSACTIONS> GirdiCiktilar { get; set; }
        public DbSet<DUYURULAR> Duyurular { get; set; }
        public DbSet<SEPET> Sepet { get; set; }
        public DbSet<ACTIVITIES> Aktiviteler { get; set; }
        public DbSet<GONDERI_TIPLERI> GonderiTipleri { get; set; }
        public DbSet<KARGOLAR> Kargolar { get; set; }
        public DbSet<SISTEM_AYARLARI> SistemAyarlari { get; set; }
        public DbSet<UYARILAR> Uyarilar { get; set; }
        public DbSet<VADE_TARIHLERI> VadeTarihleri { get; set; }
        public DbSet<SIPARIS_ODEMELERI> SiparisOdemeleri { get; set; }
        public DbSet<ilce> ilce { get; set; }
        public DbSet<mahalle> mahalle { get; set; }
        public DbSet<semt> semt { get; set; }
    }
}
