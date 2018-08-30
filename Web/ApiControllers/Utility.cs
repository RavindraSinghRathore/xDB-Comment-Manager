using System;
using System.Net.Mail;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Web.ApiControllers
{
    public static class Utility
    {
        public static void SendMail(string emailFrom, string emailTo, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage(emailFrom, emailTo);
                SmtpClient client = new SmtpClient();
                if (Settings.GetSetting("SMTP_PORT") != string.Empty)
                {
                    client.Port = int.Parse(Settings.GetSetting("SMTP_PORT"));
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                if (Settings.GetSetting("SMTP_HOST_ADDRESS") != string.Empty)
                {
                    client.Host = Settings.GetSetting("SMTP_HOST_ADDRESS");
                }
                mail.Subject = subject;
                mail.Body = body;
                if (client.Port.ToString() != "" && client.Host != null)
                {
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, new Exception());
            }
        }
    }
}