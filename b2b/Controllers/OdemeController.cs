using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2B.Dto;
using B2B.Models.Entity;
using System.Xml;
using System.Net;
using System.IO;
using System.Text;

namespace B2B.Controllers
{
    public class OdemeController : BaseController
    {
        //
        // GET: /Odeme/

        public ActionResult OdemeYap()
        {
            return View();
        }
        public ActionResult FaturaGoster()
        {
            return View();
        }

        public JsonResult OdemeTipleri()
        {
            List<PAYMENT_TYPES> OdemeTipleri = ctx.OdemeSekilleri.Where(q => q.Aktif == 1).OrderBy(q => q.Id).ToList();
            return Json(OdemeTipleri, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HavaleBilgileri()
        {
            return View();
        }
        public B2B.Controllers.Pos.BackResults KrediKartindanCekimYap(PosForm pf)
        {
            B2B.Controllers.Pos.BackResults br = new Pos.BackResults();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement rootNode = xmlDoc.CreateElement("VposRequest");
                xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                xmlDoc.AppendChild(rootNode);

                //eklemek istediğiniz diğer elementleride bu şekilde ekleyebilirsiniz.
                XmlElement merchantNode = xmlDoc.CreateElement("MerchantId");
                XmlElement passwordNode = xmlDoc.CreateElement("Password");
                XmlElement terminalNode = xmlDoc.CreateElement("TerminalNo");
                XmlElement transactionTypeNode = xmlDoc.CreateElement("TransactionType");
                XmlElement transactionIdNode = xmlDoc.CreateElement("TransactionId");
                XmlElement currencyAmountNode = xmlDoc.CreateElement("CurrencyAmount");
                XmlElement currencyCodeNode = xmlDoc.CreateElement("CurrencyCode");
                XmlElement panNode = xmlDoc.CreateElement("Pan");
                XmlElement cvvNode = xmlDoc.CreateElement("Cvv");
                XmlElement expiryNode = xmlDoc.CreateElement("Expiry");
                XmlElement ClientIpNode = xmlDoc.CreateElement("ClientIp");
                XmlElement transactionDeviceSourceNode = xmlDoc.CreateElement("TransactionDeviceSource");

                //yukarıda eklediğimiz node lar için değerleri ekliyoruz.
                XmlText merchantText = xmlDoc.CreateTextNode("000000330520056");
                XmlText passwordtext = xmlDoc.CreateTextNode("G4DR5C54XW");
                XmlText terminalNoText = xmlDoc.CreateTextNode("04102530");
                XmlText transactionTypeText = xmlDoc.CreateTextNode("Sale");
                XmlText transactionIdText = xmlDoc.CreateTextNode(Guid.NewGuid().ToString("N")); //uniqe olacak şekilde düzenleyebilirsiniz.
                XmlText currencyAmountText = xmlDoc.CreateTextNode(pf.tutar.ToString("#.##").Replace(',','.')); //tutarı nokta ile gönderdiğinizden emin olunuz.
                XmlText currencyCodeText = xmlDoc.CreateTextNode("949");
                XmlText panText = xmlDoc.CreateTextNode(pf.kartNumarasi.ToString());
                XmlText cvvText = xmlDoc.CreateTextNode(pf.guvenlikKodu.ToString());
                string ay_str = null;
                if (pf.ay <= 9) { ay_str = "0" + pf.ay.ToString(); }
                XmlText expiryText = xmlDoc.CreateTextNode(pf.yil.ToString() + ay_str);
                XmlText ClientIpText = xmlDoc.CreateTextNode("176.90.15.125");
                XmlText transactionDeviceSourceText = xmlDoc.CreateTextNode("0");

                //nodeları root elementin altına ekliyoruz.
                rootNode.AppendChild(merchantNode);
                rootNode.AppendChild(passwordNode);
                rootNode.AppendChild(terminalNode);
                rootNode.AppendChild(transactionTypeNode);
                rootNode.AppendChild(transactionIdNode);
                rootNode.AppendChild(currencyAmountNode);
                rootNode.AppendChild(currencyCodeNode);
                rootNode.AppendChild(panNode);
                rootNode.AppendChild(cvvNode);
                rootNode.AppendChild(expiryNode);
                rootNode.AppendChild(ClientIpNode);
                rootNode.AppendChild(transactionDeviceSourceNode);

                //nodelar için oluşturduğumuz textleri node lara ekliyoruz.
                merchantNode.AppendChild(merchantText);
                passwordNode.AppendChild(passwordtext);
                terminalNode.AppendChild(terminalNoText);
                transactionTypeNode.AppendChild(transactionTypeText);
                transactionIdNode.AppendChild(transactionIdText);
                currencyAmountNode.AppendChild(currencyAmountText);
                currencyCodeNode.AppendChild(currencyCodeText);
                panNode.AppendChild(panText);
                cvvNode.AppendChild(cvvText);
                expiryNode.AppendChild(expiryText);
                ClientIpNode.AppendChild(ClientIpText);
                transactionDeviceSourceNode.AppendChild(transactionDeviceSourceText);

                string xmlMessage = xmlDoc.OuterXml;

                //oluşturduğumuz xml i vposa gönderiyoruz.
                byte[] dataStream = Encoding.UTF8.GetBytes("prmstr=" + xmlMessage);
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://onlineodeme.vakifbank.com.tr:4443/VposService/v3/Vposreq.aspx");//Vpos adresi
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = dataStream.Length;
                webRequest.KeepAlive = false;
                string responseFromServer = "";

                using (Stream newStream = webRequest.GetRequestStream())
                {
                    newStream.Write(dataStream, 0, dataStream.Length);
                    newStream.Close();
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                    }

                    webResponse.Close();
                }

                if (string.IsNullOrEmpty(responseFromServer))
                {
                    br.hataMesaji = responseFromServer.ToString();
                    return br;
                }
                var xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(responseFromServer);
                var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
                var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDetail");

                string resultCode = xmlResponse.SelectSingleNode("VposResponse/ResultCode").ToString();
                string resultDescription = xmlResponse.SelectSingleNode("VposResponse/ResultDetail").ToString();

                if (resultCodeNode != null)
                {
                    resultCode = resultCodeNode.InnerText;
                    if (resultCode.Trim() == "0000")
                    {
                        br.sonuc = true;
                    }
                }
                if (resultDescriptionNode != null)
                {
                    resultDescription = resultDescriptionNode.InnerText;
                }

                br.hataMesaji = resultDescription;

            }
            catch (Exception ex)
            {
                br.sonuc = false;
                br.hataMesaji = ex.Message.ToString();
            }
            return br;
        }
    }
}
