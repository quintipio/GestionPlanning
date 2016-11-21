using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using GestionPlanning.Context;

namespace GestionPlanning.Utils
{
    /// <summary>
    /// Utilitaire pour les mails
    /// </summary>
    public static class MailUtils
    {
        /// <summary>
        /// Envoi un message à une adresse
        /// </summary>
        /// <param name="adresseMail">l'adresse email valide du destinataire</param>
        /// <param name="objet">l'objet du mail</param>
        /// <param name="corps">le corps du message</param>
        public static async Task<bool> SendMail(string adresseMail, string objet, string corps)
        {
            #if DEBUG
            adresseMail = ContexteApplication.Application.AdresseMailEnvoyeur;
            #endif

            try
            {
                var body = "<p>" + corps + "</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(adresseMail));
                message.From = new MailAddress(ContexteApplication.Application.AdresseMailEnvoyeur);
                message.Subject = objet;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ContexteApplication.Application.AdresseMailEnvoyeur,
                        Password = ContexteApplication.Application.MotDePasseMailEnvoyeur
                    };
                    smtp.Credentials = credential;
                    smtp.Host = ContexteApplication.Application.AdresseSmtp;
                    smtp.Port = ContexteApplication.Application.PortSmtp;
                    smtp.EnableSsl = ContexteApplication.Application.EnableSsl;
                    await smtp.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception e)
            {
                ContexteApplication.Log.Error(e.Source+"---"+e.Message + "---"+e.StackTrace);
                return false;
            }
            

        }
    }
}