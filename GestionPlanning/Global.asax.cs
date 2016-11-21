using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using GestionPlanning.App_Start;
using GestionPlanning.Context;
using GestionPlanning.Utils;

namespace GestionPlanning
{
    /// <summary>
    /// Classe mère de l'Application
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //initialisation de la base de donnée
            Database.SetInitializer(new MySqlInitializer());

            //sécurité des pages
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            //defaut
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //chargement du contexte à partir de la base
            ContexteApplication.RefreshContexteAppli();
            //chargement des logs
            ContexteApplication.Log = new LogUtils();

        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            // en cas d'erreur (avec stacktrace), on la log en base
            var raisedException = Server.GetLastError();
            var message = raisedException.Source + "---" + raisedException.Message + "---" + raisedException.StackTrace;
            ContexteApplication.Log.Error(message);
        }
    }
}
