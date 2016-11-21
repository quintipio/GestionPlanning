using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;
using GestionPlanning.Utils;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur pour l'authentification et la déconnexion
    /// </summary>
    [AllowAnonymous]
    public class AuthController : AbstractControl
    {
        /// <summary>
        /// Charge la page de login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Planning");
            }
            return View();
        }

        /// <summary>
        /// action de login
        /// </summary>
        /// <param name="model">l'utilisateur</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login(Utilisateurs model)
        {
            //si ok
            if (!ModelState.IsValidField("Email") || !ModelState.IsValidField("MotDePasse"))
            {
                return View(model);
            }

            Utilisateurs user = null;
            
            //cas particulier pour un utilisateur en super admin
            if (model.Email == ContexteStatic.MailSuperAdmin && CryptUtils.GenerateHashPassword(model.MotDePasse) == CryptUtils.GenerateHashPassword(ContexteStatic.MotDePassSuperAdmin))
            {
                user = new Utilisateurs
                {
                    Id = 0,
                    Email = ContexteStatic.MailSuperAdmin,
                    Nom = ContexteStatic.NomSuperAdmin,
                    Prenom = ContexteStatic.PrenomSuperAdmin,
                    Etat = EtatEnum.ACTIF,
                    Role = RoleEnum.ADMINISTRATEUR
                };
            }
            else
            {
                using (var db = new ContexteDb())
                {
                    //vérification compte existant
                    int countUser = await db.Utilisateurs.CountAsync(x => x.Email == model.Email && x.Etat == EtatEnum.ACTIF);

                    if (countUser == 1)
                    {
                        //vérification mot de passe
                        var mdp = db.Utilisateurs.First(x => x.Email == model.Email).MotDePasse;

                        //retour de l'utilisateur
                        if (CryptUtils.GenerateHashPassword(model.MotDePasse) == mdp)
                        {
                            user = db.Utilisateurs.First(x => x.Email == model.Email);
                        }
                    }
                }
            }

            //mise en contexte de l'utilisateur
            if (user != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Nom + " " + user.Prenom),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role,user.Role.ToString())
                    }, "ApplicationCookie");

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);

                    return RedirectToAction("Index", "Planning");
                }
            ModelState.AddModelError("MotDePasse","Ces identifiants ne sont pas reconnus");
             return View(model);
        }

        /// <summary>
        /// la déconnexion
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }
    }
}