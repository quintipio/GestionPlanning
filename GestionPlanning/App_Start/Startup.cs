using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace GestionPlanning.App_Start
{
    /// <summary>
    /// Classe pour le démarrage de l'appli
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //paramètre les sessions utilisateurs (par cookie, page de connexion, timeout des sessions à 10 minutes)
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/auth/login"),
                ExpireTimeSpan = TimeSpan.FromSeconds(600)
            });

            
        }

    }
}