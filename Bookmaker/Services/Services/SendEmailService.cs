using System.Net;
using System.Net.Mail;

namespace Bookmaker.Services
{
    class SendEmailService : IService
    {
        #region Const
        private const string APP_EMAIL = "Bookmaker1357@gmail.com";
        private const string APP_EMAIL_PASSWORD = "Bookmaker**5";
        private const string APP_EMAIL_NAME = "Bookmaker";
        private const string HOST = "smtp.gmail.com";
        private const int PORT = 587;
        #endregion
        public object Execute(object obj)
        {
            (string email, string body, string subject)? EmailData = obj as (string email, string body, string subject)?;
            if (obj != null)
            {
                MailAddress from = new MailAddress(APP_EMAIL, APP_EMAIL_NAME);
                MailAddress to = new MailAddress(EmailData.Value.email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = EmailData.Value.subject;
                m.Body = EmailData.Value.body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(HOST, PORT);
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(APP_EMAIL, APP_EMAIL_PASSWORD);
                smtp.EnableSsl = true;
                smtp.Send(m);
                return true;
            }
            return false;
        }
    }
}
