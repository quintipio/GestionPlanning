using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;
using GestionPlanning.Utils;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur de la gestion des utilisateurs
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
    public class UtilisateursController : AbstractControl
    {

        #region administration

        /// <summary>
        /// Charge la page de listing des utilisateurs
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Index()
        {
            var utilisateurs = Db.Utilisateurs.Where(x=> x.Etat == EtatEnum.ACTIF).OrderBy(x => x.Nom).ThenBy(x => x.Prenom).Include(u => u.Grades).Include(u => u.Poles);
            return View(utilisateurs.ToList());
        }

        /// <summary>
        /// CHarge la page de détail des utilisateurs
        /// </summary>
        /// <param name="id">l'id de l'utilisateur à consulter</param>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var utilisateurs = Db.Utilisateurs.Find(id);
            if (utilisateurs == null)
            {
                return HttpNotFound();
            }
            return View(utilisateurs);
        }

        /// <summary>
        /// Charge la page de création d'un utilisateur
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Create()
        {
            ViewBag.GradesId = new SelectList(Db.Grades.Where(x => x.Etat == EtatEnum.ACTIF && x.ArmesId == 1).ToList(), "Id", "Nom");
            ViewBag.ArmesId = new SelectList(Db.Armes.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom");
            ViewBag.PolesId = new SelectList(Db.Poles.Where(x =>x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom");
            return View(new Utilisateurs {Etat = EtatEnum.ACTIF, Role = RoleEnum.UTILISATEUR, MotDePasse = CryptUtils.GeneratePassword(9,true,true,true)});
        }

        /// <summary>
        /// Chang ela liste des grades lors d'un changement d'arme
        /// </summary>
        /// <param name="id">l'id de la nouvelle arme</param>
        /// <returns></returns>
        public ActionResult ChangeArme(int id)
        {
            var liste = Db.Grades.Where(x => x.ArmesId == id && x.Etat == EtatEnum.ACTIF).Select(x => new { x.Id, x.Nom }).ToList();
            return Json(liste);
        }
        
        /// <summary>
        /// Créer un utilisateur
        /// </summary>
        /// <param name="utilisateurs">l'utilisateur à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR")]
        public async Task<ActionResult> Create(Utilisateurs utilisateurs)
        {
            if (Db.Utilisateurs.Count(x => x.Email == utilisateurs.Email && x.Etat == EtatEnum.ACTIF) > 0)
            {
                ModelState.AddModelError("Email", "Cette adresse est déjà présente!");
            }

            if (ModelState.IsValid)
            {
                Utilisateurs user;
                var isAjout = false;
                //si l'adresse existe en base c'est que c'est un utilisateur périmé, donc on le réactive
                if(Db.Utilisateurs.Count(x => x.Email == utilisateurs.Email) > 0)
                {
                    user = Db.Utilisateurs.First(x => x.Email == utilisateurs.Email);
                }
                else
                {
                    user = Db.Utilisateurs.Create();
                    isAjout = true;
                }

                var motDePasse = CryptUtils.GeneratePassword(9, true, true, true);
                user.MotDePasse = CryptUtils.GenerateHashPassword(motDePasse);
                user.Email = utilisateurs.Email.ToLower();
                user.Nom = StringUtils.FirstLetterUpper(utilisateurs.Nom.ToLower());
                user.Prenom = StringUtils.FirstLetterUpper(utilisateurs.Prenom.ToLower());
                user.Nid = utilisateurs.Nid;
                user.Pnia = utilisateurs.Pnia;
                user.Role = utilisateurs.Role;
                user.GradesId = utilisateurs.GradesId;
                user.PolesId = utilisateurs.PolesId;
                user.Etat = EtatEnum.ACTIF;

                if (isAjout)
                {
                    Db.Entry(user).State = EntityState.Added;
                    Db.Utilisateurs.Add(user);
                }
                else
                {
                    Db.Entry(user).State = EntityState.Modified;
                }
                Db.SaveChanges();

                //envoi du mail
                var mail = Db.Mails.First(x => x.NomUnique == ContexteStatic.EnvoiCreationCompte);
                var corpsMessage = StringUtils.CopyString(mail.Corps);
                corpsMessage = corpsMessage.Replace("%NOMAPPLI%", ContexteApplication.Application.NomAppli);
                corpsMessage = corpsMessage.Replace("%LOGIN%", user.Email);
                corpsMessage = corpsMessage.Replace("%MOTDEPASSE%", motDePasse);
                await MailUtils.SendMail(user.Email, mail.Objet, corpsMessage);

                return RedirectToAction("Index");
            }

            ViewBag.GradesId = new SelectList(Db.Grades.Where(x => x.Etat == EtatEnum.ACTIF && x.ArmesId == utilisateurs.ArmesId).ToList(), "Id", "Nom");
            ViewBag.ArmesId = new SelectList(Db.Armes.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom");
            ViewBag.PolesId = new SelectList(Db.Poles.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom");
            return View(utilisateurs);
        }

        /// <summary>
        /// Charge la page de modification d'un utilisateur
        /// </summary>
        /// <param name="id">l'id de l'utilisateur à modifier</param>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var utilisateurs = Db.Utilisateurs.Find(id);
            if (utilisateurs == null)
            {
                return HttpNotFound();
            }

            utilisateurs.ArmesId = utilisateurs.Grades.ArmesId;
            ViewBag.ArmesId = new SelectList(Db.Armes.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom",utilisateurs.ArmesId);
            ViewBag.GradesId = new SelectList(Db.Grades.Where(x => x.Etat == EtatEnum.ACTIF && x.ArmesId == utilisateurs.ArmesId).ToList(), "Id", "Nom", utilisateurs.GradesId);
            ViewBag.PolesId = new SelectList(Db.Poles.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom", utilisateurs.PolesId);
            return View(utilisateurs);
        }
        
        /// <summary>
        /// Modifie un utilisateur en base
        /// </summary>
        /// <param name="utilisateurs"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Edit(Utilisateurs utilisateurs)
        {
            if (Db.Utilisateurs.Count(x => x.Email == utilisateurs.Email && x.Id != utilisateurs.Id) > 0)
            {
                ModelState.AddModelError("Email", "Cette adresse est déjà présente!");
            }


            if (ModelState.IsValid)
            {
                var user = Db.Utilisateurs.Find(utilisateurs.Id);

                if (user != null)
                {
                    user.Email = utilisateurs.Email;
                    user.GradesId = utilisateurs.GradesId;
                    user.Role = utilisateurs.Role;
                    user.Nom = utilisateurs.Nom;
                    user.Prenom = utilisateurs.Prenom;
                    user.PolesId = utilisateurs.PolesId;
                    user.Nid = utilisateurs.Nid;
                    user.Pnia = utilisateurs.Pnia;
                    
                    Db.Entry(user).State = EntityState.Modified;
                    Db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Nom", "Cet utilisateur est introuvable");
                }
            }
            ViewBag.GradesId = new SelectList(Db.Grades.Where(x => x.Etat == EtatEnum.ACTIF && x.ArmesId == utilisateurs.ArmesId).ToList(), "Id", "Nom");
            ViewBag.ArmesId = new SelectList(Db.Armes.Where(x => x.Etat == EtatEnum.ACTIF).ToList(), "Id", "Nom");
            ViewBag.PolesId = new SelectList(Db.Poles, "Id", "Nom", utilisateurs.PolesId);
            return View(utilisateurs);
        }

        /// <summary>
        /// Charge la page de supression d'un utilisateur
        /// </summary>
        /// <param name="id">l'id de l'utilisateur à supprimer</param>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var utilisateurs = Db.Utilisateurs.Find(id);
            if (utilisateurs == null)
            {
                return HttpNotFound();
            }
            return View(utilisateurs);
        }

        /// <summary>
        /// Périme un utilisateur en base
        /// </summary>
        /// <param name="id">l'id de l'utilisateur à périmer</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult DeleteConfirmed(int id)
        {
            var utilisateurs = Db.Utilisateurs.Find(id);
            utilisateurs.Etat = EtatEnum.PERIME;
            Db.Entry(utilisateurs).State = EntityState.Modified;
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion


        #region Gestion mot de passe utilisateur par admin
        /// <summary>
        /// Charge la page de modification d'un mot de passe
        /// </summary>
        /// <param name="id">l'id de l'utiliiatuer dont le mot de passe va changer</param>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var utilisateurs = Db.Utilisateurs.Find(id);
            if (utilisateurs == null)
            {
                return HttpNotFound();
            }
            return View(utilisateurs);
        }

        /// <summary>
        /// modifie un mdp d'un utilisateur
        /// </summary>
        /// <param name="utilisateurs">l'utilisateur dont on souhaite modifier le mot de passer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR")]
        public ActionResult ChangePassword(Utilisateurs utilisateurs)
        {
            if (utilisateurs.MotDePasse != null && !CryptUtils.IsValidMotDePasse(utilisateurs.MotDePasse))
            {
                ModelState.AddModelError("MotDePasse", "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial");
            }

            if (ModelState.IsValid)
            {
                var user = Db.Utilisateurs.Find(utilisateurs.Id);

                if (user != null)
                {
                    user.MotDePasse = CryptUtils.GenerateHashPassword(utilisateurs.MotDePasse);
                    Db.Entry(user).State = EntityState.Modified;
                    Db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("MotDePasse", "Cet utilisateur est introuvable");
                }

                return RedirectToAction("Index");
            }
            return View(utilisateurs);
        }

        #endregion

        #region modifications personnelles

        /// <summary>
        /// Charge la page de modification des données de son propre compte
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
        public ActionResult ChangeInfoPerso()
        {
            var id = GetIdUtilisateurConnecte();
            if (id != -1)
            {
                var user = Db.Utilisateurs.Find(id);
                return View(user);
            }
            return RedirectToAction("Index", "Planning");
        }


        /// <summary>
        /// modifie les données personnelles de l'utilisateur
        /// </summary>
        /// <param name="utilisateurs">l'utilisateur à modifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
        public ActionResult ChangeInfoPerso(Utilisateurs utilisateurs)
        {
            if (ModelState.IsValidField("Email"))
            {
                //sécurité pour éviter de se baser sur les infos provenant du client
                var id = GetIdUtilisateurConnecte();
                if (id != -1)
                {
                    var user = Db.Utilisateurs.Find(id);
                    
                    if (Db.Utilisateurs.Count(x => x.Email == utilisateurs.Email && x.Id != id) > 0)
                    {
                        ModelState.AddModelError("Email", "Cette adresse est déjà présente");
                        return View(utilisateurs);
                    }

                    //user.Email = utilisateurs.Email;
                    //user.Nid = utilisateurs.Nid;
                    user.Pnia = utilisateurs.Pnia;
                    Db.Entry(user).State = EntityState.Modified;
                    Db.SaveChanges();
                    return RedirectToAction("Index", "Planning");
                }
            }
            return View(utilisateurs);
        }

        /// <summary>
        /// Accès  à la page de modification du mot de passe perso
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
        public ActionResult ChangePasswordPerso()
        {
            var id = GetIdUtilisateurConnecte();
            if (id != -1)
            {
                var user = Db.Utilisateurs.First(x => x.Id == id);
                return View(user);
            }
            return RedirectToAction("Index", "Planning");
        }


        /// <summary>
        /// modifie le mot de passe d'un utilisateur
        /// </summary>
        /// <param name="utilisateurs">l'utilisateur à modifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
        public ActionResult ChangePasswordPerso(Utilisateurs utilisateurs)
        {
            if (ModelState.IsValidField("MotDePasse"))
            {
                string mdpA = Request.Form["mdpA"];
                string mdpB = Request.Form["mdpB"];

                var id = GetIdUtilisateurConnecte();

                if (id != -1)
                {
                    var user = Db.Utilisateurs.First(x => x.Id == id);

                    //controle de l'ancien mot de passe
                    var error = false;

                    if (string.IsNullOrWhiteSpace(mdpA))
                    {
                        ModelState.AddModelError("mdpA", "Ce champs est vide");
                        error = true;
                    }

                    if (string.IsNullOrWhiteSpace(mdpB))
                    {
                        ModelState.AddModelError("mdpB", "Ce champs est vide");
                        error = true;
                    }

                    if (!error)
                    {
                        if (user.MotDePasse != CryptUtils.GenerateHashPassword(utilisateurs.MotDePasse))
                        {
                            ModelState.AddModelError("MotDePasse", "L'ancien mot de passe ne correspond pas");
                            error = true;
                        }

                        if (!CryptUtils.IsValidMotDePasse(mdpA))
                        {
                            ModelState.AddModelError("mdpA", "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre et un caractère spécial");
                            error = true;
                        }

                        if (mdpA != mdpB)
                        {
                            ModelState.AddModelError("mdpB", "Les mots de passe ne correspondent pas");
                            error = true;
                        }
                        else
                        {
                            if (mdpA.Length < 8)
                            {
                                ModelState.AddModelError("mdpB", "Le mot de passe doit faire au moins 8 caractères");
                                error = true;
                            }
                            else
                            {
                                if (mdpA == utilisateurs.MotDePasse)
                                {
                                    ModelState.AddModelError("mdpA", "Le mot de passe est identique à l'ancien");
                                    error = true;
                                }
                            }
                        }

                        if (!error)
                        {
                            user.MotDePasse = CryptUtils.GenerateHashPassword(mdpA);
                            Db.Entry(user).State = EntityState.Modified;
                            Db.SaveChanges();
                            return RedirectToAction("ChangeInfoPerso");
                        }
                    }

                }
            }
            return View(utilisateurs);
        }
        #endregion

        #region annuaire
        /// <summary>
        /// Charge la page de l'annuaire
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ADMINISTRATEUR, UTILISATEUR")]
        [AllowAnonymous]
        public ActionResult Annuaire()
        {
            return View(Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).ToList().OrderBy(x => x.Nom).ThenBy(x => x.Prenom));
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
