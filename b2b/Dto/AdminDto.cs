using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2B.Models.Entity;

namespace B2B.Dto
{
    public class AdminIndex
    {
        public decimal ToplamSatisTutar { get; set; }
        public int AdetSiparis { get; set; }
        public int MusteriCount { get; set; }
        public int TotalVisits { get; set; }
        public List<string> AdminUyarilari { get; set; }
        

        public int AdetSiparis_BugunWidget { get; set; }
        public decimal TutarSiparis_BugunWidget { get; set; }
        public decimal DunkuSatis_BugunWidget { get; set; }
        public decimal TutarSatis_GecenHafta_BugunWidget { get; set; }
        public decimal TutarSatisTutar_GecenAy_BugunWidget { get; set; }
        public List<AdminHomePage_SonSiparisler> SonSiparisler { get; set; }
        public List<UyarilarDTO> Uyarilar { get; set; }
    }
    public class AdminHomePage_SonSiparisler
    {
        public int Id { get; set; }
        public string Firma { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string SiparisDurumu { get; set; }
        public decimal SiparisToplami { get; set; }
        public int UrunlerToplami { get; set; }
        public string SiparisKodu { get; set; }
    }
    public class AdminHesaplarIndex
    {
        public List<COMPANIES> Firmalar { get; set; }
        public List<ACTIVITIES> Aktiviteler { get; set; }
    }
    public class SiparisDuzenleDto
    {
        public ORDERS Siparis { get; set; }
        public List<SIPARIS_DURUMLARI> SiparisDurumlari { get; set; }
    }
    public class SuccessionClass
    {
        public bool success { get; set; }
        public string error { get; set; }
        public bool preventRetry { get; set; }
    }
    public class SiparisOdemeleriClass
    {
        public List<decimal> OncekiOdemeler { get; set; }
        public decimal SiparisToplamTutari { get; set; }
    }
}