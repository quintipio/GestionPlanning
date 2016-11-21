using System.Web.Mvc;
using System.Web.Routing;

namespace GestionPlanning
{
    /// <summary>
    /// Bojet pour régler le routage des page. Page apr défaut, le planning des utilisateurs
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Planning", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
