using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using GestionPlanning.Context;

namespace GestionPlanning.Abstract
{
    /// <summary>
    /// Couche abtraite des controleurs afin de retrouver des outils commun à tous les controleurs
    /// </summary>
    public abstract class AbstractControl : Controller
    {
        /// <summary>
        /// Connexion à la base de donnée
        /// </summary>
        protected readonly ContexteDb Db = new ContexteDb();

        /// <summary>
        /// Retourne l'id de l'utilisateur connecté (ou -1 si aucun)
        /// </summary>
        /// <returns>l'id de l'utilisateurs connecté</returns>
        protected int GetIdUtilisateurConnecte()
        {
            if (System.Web.HttpContext.Current.GetOwinContext().Authentication.User.Identity.IsAuthenticated)
            {
                var id = System.Web.HttpContext.Current.GetOwinContext()
                    .Authentication.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)
                    .Value;
                int idint;
                int.TryParse(id, out idint);
                return idint;
            }
            return -1;
        }
    }
}