using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PortalRoemmers.Helpers
{
    public class EmailClass
    {
        public async Task<bool> SendEmailAsync(string email, string messages, string subject, string strAdjunto,string correo,string clave)
        {
            Attachment archAdjun = default(Attachment);
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(correo);  // replace with sender's email id 
            message.Subject = subject;//titulo
            message.Body = messages;//mensaje
            message.IsBodyHtml = true;
            if (System.IO.File.Exists(strAdjunto))
            {
                archAdjun = new Attachment(strAdjunto);
                message.Attachments.Add(archAdjun);
            }

            MailAddress bcc = new MailAddress(ConstCorreo.CC_CORREO);
            message.Bcc.Add(bcc);

            using (var smtp = new SmtpClient())
            {
                smtp.Host = ConstCorreo.HOST_OFFICE; //"smtp.gmail.com"; //smtp-mail.outlook.com
                smtp.Port = ConstCorreo.PUERTO;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;

                var credential = new NetworkCredential
                {
                    UserName = correo,     //ConstCorreo.CORREO,  // replace with sender's email id 
                    Password = clave       //ConstCorreo.CLAVE_CORREO  // replace with password 
                };
                smtp.Credentials = credential;

                await smtp.SendMailAsync(message);
                return true;
            }
        }


        

    }
}