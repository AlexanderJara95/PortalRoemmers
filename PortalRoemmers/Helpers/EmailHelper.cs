using MimeKit;


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

   


    }
}