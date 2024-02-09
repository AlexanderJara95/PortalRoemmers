using MimeKit;
using System.Collections.Generic;

namespace PortalRoemmers.Helpers
{
    public class EmailHelper
    {
        bool correcto = false;

            public bool SendEmail(string toEmail, string messages, string subject, string correo, string clave)//, string strAdjunto, string correo, string clave
               {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(correo, correo));
                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Bcc.Add(new MailboxAddress(ConstCorreo.CC_CORREO, ConstCorreo.CC_CORREO));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = messages };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(ConstCorreo.HOST_OFFICE, 587);
                    client.Authenticate(correo, clave);
                    client.Send(message);
                    client.Disconnect(true);
                    correcto = true;
                }
                return correcto;
            }
            public bool SendEmailCC(string toEmail, List<string> ccEmails, string messages, string subject, string correo, string clave)
            {
                bool correcto = false;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(correo, correo));
                message.To.Add(new MailboxAddress(toEmail, toEmail));

                // Agregar destinatarios en copia (Cc)
                foreach (var ccEmail in ccEmails)
                {
                    message.Cc.Add(new MailboxAddress(ccEmail, ccEmail));
                }

                message.Subject = subject;
                message.Body = new TextPart("html") { Text = messages };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(ConstCorreo.HOST_OFFICE, 587);
                    client.Authenticate(correo, clave);
                    client.Send(message);
                    client.Disconnect(true);
                    correcto = true;
                }

                return correcto;
            }





    }
}