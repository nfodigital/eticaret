using B2B.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace B2B.Controllers
{
    public class Pos : BaseController
    {
        public static string RandomNumber()
        {
            Random r = new Random();
            string strRsayi = r.Next(1, 10000000).ToString() + String.Format("{0:T}", DateTime.Now).Replace(":", string.Empty);
            return strRsayi;
        }
        public static string GetIp()
        {
            if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["remote_addr"]))
            {
                return "127.0.0.1";
            }
            else
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["remote_addr"].ToString();
            }
        }
        public BackResults VakifBank(PosForm pf)
        {
            // Banka bilgileri.
            string kullanici = "0001";
            string sifre = "G4DR5C54XW";
            string uyeno = "000000330520056";
            string posno = "04102530";
            string xcip = "xxx";

            BackResults br = new BackResults();

            try
            {
                byte[] b = new byte[5000];
                string sonuc;
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-9");

                String tutarcevir = pf.tutar.ToString();
                tutarcevir = tutarcevir.Replace(".", "");
                tutarcevir = tutarcevir.Replace(",", "");
                tutarcevir = String.Format("{0:0000000000.00}", Convert.ToInt32(tutarcevir)).Replace(",", "");

                string taksitcevir = "";

                if (pf.taksit == -1)
                {
                    taksitcevir = "00";
                }
                else
                {
                    taksitcevir = String.Format("{0:00}", pf.taksit);
                }

                String yilcevir = pf.yil.ToString();
                yilcevir = yilcevir.Substring(2, 2);

                string aycevir = string.Format("{0:00}", pf.ay);

                string provizyonMesaji = "kullanici=" + kullanici + "&sifre=" + sifre + "&islem=PRO&uyeno=" + uyeno + "&posno=" + posno + "&kkno=" + pf.kartNumarasi + "&gectar=" + yilcevir + aycevir + "&cvc=" + string.Format("{0:000}", pf.guvenlikKodu) + "&tutar=" + tutarcevir + "&provno=000000&taksits=" + taksitcevir + "&islemyeri=I&uyeref=" + RandomNumber() + "&vbref=0&khip=" + GetIp() + "&xcip=" + xcip;

                b.Initialize();
                b = Encoding.ASCII.GetBytes(provizyonMesaji);

                WebRequest h1 = (WebRequest)HttpWebRequest.Create("https://subesiz.vakifbank.com.tr/vpos724v3/?" + provizyonMesaji);
                h1.Method = "GET";

                WebResponse wr = h1.GetResponse();
                Stream s2 = wr.GetResponseStream();

                byte[] buffer = new byte[10000];
                int len = 0, r = 1;
                while (r > 0)
                {
                    r = s2.Read(buffer, len, 10000 - len);
                    len += r;
                }
                s2.Close();
                sonuc = encoding.GetString(buffer, 0, len).Replace("\r", "").Replace("\n", "");

                String gelenonaykodu, gelenrefkodu;
                XmlNode node = null;
                XmlDocument _msgTemplate = new XmlDocument();
                _msgTemplate.LoadXml(sonuc);
                node = _msgTemplate.SelectSingleNode("//Cevap/Msg/Kod");
                gelenonaykodu = node.InnerText.ToString();

                if (gelenonaykodu == "00")
                {
                    node = _msgTemplate.SelectSingleNode("//Cevap/Msg/ProvNo");
                    gelenrefkodu = node.InnerText.ToString();
                    br.sonuc = true;
                    br.referansNo = gelenrefkodu;
                }
                else
                {
                    br.sonuc = false;
                    br.hataMesaji = "";
                    br.hataKodu = gelenonaykodu;
                }
            }
            catch (Exception e)
            {
                br.sonuc = false;
                br.hataMesaji = br.sistemHatasi;
            }
            return br;
        }
        public class BackResults
        {
            public string sistemHatasi = "Bankayla bağlantı kurulamadı!, lütfen daha sonra tekrar deneyiniz!";

            public bool sonuc { get; set; }
            public string hataMesaji { get; set; }
            public string hataKodu { get; set; }
            // Bankadan geri dönen değerler

            public string code { get; set; }
            public string groupId { get; set; }
            public string transId { get; set; }
            public string referansNo { get; set; }
        }
    }
}