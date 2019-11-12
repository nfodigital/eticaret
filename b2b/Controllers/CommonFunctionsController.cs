using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2B.Models.Entity;
using B2B.Dto;
using System.Web.Mvc;

namespace B2B.Controllers
{
    public class CommonFunctionsController : BaseController
    {
        public string FirmaAdiVer(int FirmaId)
        {
            var DonenDeger = ctx.Firmalar.FirstOrDefault(q => q.Id == FirmaId);
            if(DonenDeger!=null)
            {
                return DonenDeger.FirmaAdi;
            }
            else
            {
                return "Firma Yok/Silinmiş";
            }
        }
        public string KullanicininAdiSoyadi(int UserId)
        {
            var DonenDeger = ctx.Kullanicilar.FirstOrDefault(q => q.Id == UserId);
            if (DonenDeger != null)
            {
                return DonenDeger.Adi;
            }
            else
            {
                return "Kullanıcı Yok/Silinmiş";
            }
        }

        public string UrunKategoriNameVer(int id)
        {
            var DonenDeger = ctx.Kategoriler.FirstOrDefault(q => q.Id == id);
            if(DonenDeger!=null)
            {
                return DonenDeger.KategoriBaslik;
            }
            else
            {
                return "Kategori Yok/Kaldırılmış";
            }
            
        }
        public decimal FirmayaAitTumSiparislerinToplamTutari(int FirmaId)
        {
            return ctx.Siparisler.Where(q => q.FirmaId == FirmaId).Sum(q => q.SiparisToplami).GetValueOrDefault(0);
        }
        public string SiparisDurumuVer(int SiparisDurumu)
        {
            string Durum = SiparisDurumu == 1 ? "Tamamlandı"
                : SiparisDurumu == 2 ? "Onaylandı Kargo Bekliyor"
                : SiparisDurumu == 3 ? "Gönderildi"
                : SiparisDurumu == 4 ? "Ödeme bekliyor"
                : SiparisDurumu == 0 ? "Onay Bekleniyor"
                : "İletişime geçiniz";
            return Durum;
        }
        public string GirdiDurumVer(int DurumId)
        {
            string Durum = DurumId == 1 ? "Tamamlandı"
                : DurumId == 2 ? "Onaylandı, Kargo Bekliyor"
                : DurumId == 3 ? "Gönderildi"
                : DurumId == 4 ? "Ödeme Bekliyor"
                : DurumId == 0 ? "Onay Bekleniyor"
                : "İletişime Geçiniz";
            return Durum;
        }
        public string LabelVer(string DurumId)
        {
            string Durum = DurumId == "Onay Bekleniyor" ? " label-warning"
                : DurumId == "Tamamlandı" ? " label-success"
                : DurumId == "Onaylandı Kargo Bekliyor" ? " label-warning"
                : DurumId == "Gönderildi" ? " label-warning"
                : " label-danger";
            return Durum;
        }
        public void Log(Aktivite Aktivite)
        {
            Kernel.MakeLog(Aktivite);
        }
        public JsonResult GonderimTipleri()
        {
            UserHolder UserInfo = getAktifKullanici();

            List<GONDERI_TIPLERI> Gonderiler = UserInfo.KullaniciYetkisi == "su" ? ctx.GonderiTipleri.OrderBy(q => q.Id).ToList()
                : ctx.GonderiTipleri.Where(q => q.GonderiType != 0).ToList();

            return Json(Gonderiler, JsonRequestBehavior.AllowGet);
        }
        public string EchoCurrent(decimal Amount)
        {
            return string.Format("{0:C}", Amount);
        }


    }
    public class CommonFunctionsForAdminController : BaseController
    {
        [AuthorizeUser(Roles = ("su"))]
        public decimal FirmaBakiyeVer(int FirmaId)
        {
            return ctx.Muhasebe.Where(q => q.FirmaId == FirmaId).FirstOrDefault().Bakiye;
        }
    }

}