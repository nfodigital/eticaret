using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2B.Dto;
using B2B.Models.Entity;
using System.Net.Mail;
using B2B.Models.Context;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

namespace B2B.Controllers
{
    public static class Kernel
    {
        const string fromEmail = "info@neutronsatis.com";
        const string fromPass = "xBc210dctb24";
        const int smtpPort = 587;
        const string mailHost = "mail.neutronsatis.com";

        public static Context ctx;
        public static bool isNumeric(this string value)
        {
            int i = 0;
            return int.TryParse(value, out i);
        }

        public static string Mail(string To, string Icerik)
        {
            string durum = "";
            try
            {
                MailMessage Mailing = new MailMessage("iletisim@cicekgider.com", To);
                Mailing.To.Add(To);
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.cicekgider.com";
                smtp.Credentials = new System.Net.NetworkCredential("iletisim@cicekgider.com", "Xbc210.i");
                Mailing.Subject = "yeni sipariş";
                Mailing.Body = Icerik;
                smtp.Send(Mailing); 
            }
            catch (Exception)
            {

            }
            return durum;
        }

        public static Image ResizeImage(Image sourceImage, int maxWidth, int maxHeight)
        {
            // Determine which ratio is greater, the width or height, and use
            // this to calculate the new width and height. Effectually constrains
            // the proportions of the resized image to the proportions of the original.
            double xRatio = (double)sourceImage.Width / maxWidth;
            double yRatio = (double)sourceImage.Height / maxHeight;
            double ratioToResizeImage = Math.Max(xRatio, yRatio);
            int newWidth = (int)Math.Floor(sourceImage.Width / ratioToResizeImage);
            int newHeight = (int)Math.Floor(sourceImage.Height / ratioToResizeImage);

            // Create new image canvas -- use maxWidth and maxHeight in this function call if you wish
            // to set the exact dimensions of the output image.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);

            // Render the new image, using a graphic object
            using (Graphics newGraphic = Graphics.FromImage(newImage))
            {
                // Set the background color to be transparent (can change this to any color)
                newGraphic.Clear(Color.Transparent);

                // Set the method of scaling to use -- HighQualityBicubic is said to have the best quality
                newGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Apply the transformation onto the new graphic
                Rectangle sourceDimensions = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                Rectangle destinationDimensions = new Rectangle(0, 0, newWidth, newHeight);
                newGraphic.DrawImage(sourceImage, destinationDimensions, sourceDimensions, GraphicsUnit.Pixel);
            }

            // Image has been modified by all the references to it's related graphic above. Return changes.

            return newImage;
        }

        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
                return false;
            }

        }
        public static byte[] getFileFromInput(HttpPostedFileBase file)
        {
            byte[] fileData = null;
            try
            {

                using (BinaryReader _readbin = new BinaryReader(file.InputStream))
                {
                    fileData = _readbin.ReadBytes(file.ContentLength);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("hata oluştu: " + ex.Message);
            }
            return fileData;
        }
        public static string DecodeHTML(string _input)
        {
            string _output = null;
            try
            {
                _output = HttpUtility.HtmlDecode(_input);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _output;
        }
        public static string EncodeHTML(string _input)
        {
            string _output = null;
            try
            {
                _output = HttpUtility.HtmlEncode(_input);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return _output;
        }
        public static string DeleteFileFromFileSystem(string filePath)
        {
            var str = "";
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); str = "file deleted";
                }
                else
                {
                    str = "hata, dosya sisteminde böyle bir dosya bulunamadı";
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return str;
        }
        public static bool FirmaDosyasiOlustur(Aktivite _C, string _logType)
        {
            string log_create = null;
            switch (_logType)
            {
                case "activity":
                    log_create = "_activity.log"; break;
                default:
                    log_create = "_activity.log";
                    break;
            }
            var _donendeger = true;
            try
            {
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("/Companies/" + _C.FirmaId.ToString())))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("/Companies/" + _C.FirmaId.ToString()));
                    using (File.Create(HttpContext.Current.Server.MapPath("/Companies/" + _C.FirmaId.ToString() + "/" + log_create)))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                _donendeger = false;
                throw new ArgumentException(ex.Message);
            }
            return _donendeger;
        }
        public static bool MakeLog(Aktivite Firma)
        {
            FirmaDosyasiOlustur(Firma, "activity");
            var donendeger = true;
            try
            {
                File.AppendAllText(HttpContext.Current.Server.MapPath("/Companies/" + Firma.FirmaId.ToString() + "/_activity.log"), "Log tip: " + Firma.AktiviteTipi + " " + Firma.AktiviteSTR + " " + Firma.AktiviteTarihSaat + " Kullanıcı: " + Firma.UserId + Environment.NewLine);
            }
            catch (Exception EX)
            {
                donendeger = false;
                throw new ArgumentException(EX.Message);
            }
            return donendeger;
        }
        public static void sendEmail(string to, string Subject, string templateBody)
        {
            try
            {
                MailMessage _mail = new MailMessage();
                _mail.To.Add(to);
                _mail.From = new MailAddress(fromEmail);
                _mail.Body = templateBody;
                SmtpClient _smtpClient = new SmtpClient();
                _smtpClient.Credentials = new System.Net.NetworkCredential(fromEmail, fromPass);
                _smtpClient.Port = smtpPort;
                _smtpClient.UseDefaultCredentials = false;
                _smtpClient.Host = mailHost;
                _mail.Subject = Subject;
                _mail.IsBodyHtml = true;

                _smtpClient.Send(_mail);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

    }
}