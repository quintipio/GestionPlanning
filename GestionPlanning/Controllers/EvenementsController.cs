using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;
using GestionPlanning.Utils;
using GestionPlanning.ViewModel;
using Novacode;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des évènements
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR,UTILISATEUR")]
    public class EvenementsController : AbstractControl
    {
        /// <summary>
        /// Génère un viewmodel adapté à la création / modification
        /// </summary>
        /// <param name="evenement"> l'évènement ViewModel si il existe déjà, sinon null</param>
        /// <returns>the even</returns>
        private EvenementViewModel GenerateViewModelCreate(EvenementViewModel evenement)
        {
            var even = evenement ?? new EvenementViewModel ();

            if (evenement?.EvenementsCre != null)
            {
                even.EvenementsCre = evenement.EvenementsCre;
            }
            else
            {
                even.EvenementsCre = new Evenements { DemiJournee = DemiJourneeEnum.MATIN, DateVerrou = null, Date = DateUtils.GetAujourdhui() };
            }

            //chargement des poles sélectionables
            even.PoleItems = Db.Poles.Where(x => x.Etat == EtatEnum.ACTIF).ToList();

            //chargement des utilisateurs sélectionable
            even.UtilisateursItems = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).OrderBy(x => x.Nom).ThenBy(x => x.Prenom).ToList();
            return even;
        }

        /// <summary>
        /// Génère un view model pour l'affichage de la liste des évènements auquel l'utilisateur participe ou à crée
        /// </summary>
        /// <returns>the even</returns>
        public EvenementViewModel GenerateEvenementsParticipe()
        {
            var dateLim = DateUtils.GetAujourdhui();
            //on ne prend pas les évènements passé depuis 3 jours
            dateLim = dateLim.AddDays(-3);
            var even  = new EvenementViewModel();
            var id = GetIdUtilisateurConnecte();
            even.EvenementsPersoCreer = Db.Evenements.Where(x => x.CreateurId == id && x.Date > dateLim).ToList();
            even.EvenementsParticipe = (from e in Db.Evenements.Where(x => x.Date > dateLim)
                join eu in Db.EvenementsUtilisateurs
                on e.Id equals eu.EvenementsId
                where eu.UtilisateursId == id
                select e ).ToList();
            return even;
        }



        /// <summary>
        /// Charge la page des évènements d'un utilisateur
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MesEvenements()
        {
            return View(GenerateEvenementsParticipe());
        }


        #region Création d'évènements
        /// <summary>
        /// Charge la page de création d'un évènement
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            return View(GenerateViewModelCreate(null));
        }

        /// <summary>
        /// Créer un évènement à partir du viewModel
        /// </summary>
        /// <param name="evenementCreer">le viewModel contenant les infos pour créer l'évènement</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EvenementViewModel evenementCreer)
        {
            if (ModelState.IsValid)
            {
                //controle de la dateA
                if (evenementCreer.EvenementsCre.Date < DateUtils.GetAujourdhui())
                {
                    ModelState.AddModelError("EvenementsCre.Date", "La date doit être supérieur à la date du jour");
                }

                if (evenementCreer.EvenementsCre.DemiJournee != DemiJourneeEnum.APRESMIDI && evenementCreer.EvenementsCre.DemiJournee != DemiJourneeEnum.MATIN)
                {
                    ModelState.AddModelError("EvenementsCre.DemiJournee", "Il est nécéssaire de préciser la demi journée");
                }

                //controle de la dateB
                if (evenementCreer.EvenementsCre.DateB != null)
                {
                    if (evenementCreer.EvenementsCre.DemiJourneeB != DemiJourneeEnum.APRESMIDI && evenementCreer.EvenementsCre.DemiJourneeB != DemiJourneeEnum.MATIN)
                    {
                        ModelState.AddModelError("EvenementsCre.DemiJourneeB", "Il est nécéssaire de préciser la demi journée");
                    }

                    if (evenementCreer.EvenementsCre.DateB < DateUtils.GetAujourdhui())
                    {
                        ModelState.AddModelError("EvenementsCre.DateB", "La date doit être supérieur à la date du jour");
                    }

                    if (evenementCreer.EvenementsCre.DateB == evenementCreer.EvenementsCre.Date &&
                        evenementCreer.EvenementsCre.DemiJourneeB == evenementCreer.EvenementsCre.DemiJournee)
                    {
                        ModelState.AddModelError("EvenementsCre.DateB", "cette date est identique à la première date");
                    }
                }

                //controle de la dateC
                if (evenementCreer.EvenementsCre.DateC != null)
                {
                    if (evenementCreer.EvenementsCre.DemiJourneeC != DemiJourneeEnum.APRESMIDI && evenementCreer.EvenementsCre.DemiJourneeC != DemiJourneeEnum.MATIN)
                    {
                        ModelState.AddModelError("EvenementsCre.DemiJourneeC", "Il est nécéssaire de préciser la demi journée");
                    }

                    if (evenementCreer.EvenementsCre.DateC < DateUtils.GetAujourdhui())
                    {
                        ModelState.AddModelError("EvenementsCre.DateC", "La date doit être supérieur à la date du jour");
                    }

                    if ((evenementCreer.EvenementsCre.DateC == evenementCreer.EvenementsCre.Date && evenementCreer.EvenementsCre.DemiJourneeC == evenementCreer.EvenementsCre.DemiJournee) ||
                        (evenementCreer.EvenementsCre.DateB != null && evenementCreer.EvenementsCre.DemiJourneeB != null &&
                         evenementCreer.EvenementsCre.DateC == evenementCreer.EvenementsCre.DateB && evenementCreer.EvenementsCre.DemiJourneeC == evenementCreer.EvenementsCre.DemiJourneeB))
                    {
                        ModelState.AddModelError("EvenementsCre.DateC", "cette date est identique à une des dates ci dessus");
                    }
                }


                //control de la date de verrouillage
                if (evenementCreer.EvenementsCre.DateVerrou != null)
                {
                    if (evenementCreer.EvenementsCre.DateVerrou <= DateUtils.GetAujourdhui() ||
                        evenementCreer.EvenementsCre.DateVerrou > evenementCreer.EvenementsCre.Date ||
                        (evenementCreer.EvenementsCre.DateB != null && evenementCreer.EvenementsCre.DateVerrou > evenementCreer.EvenementsCre.DateB) ||
                        (evenementCreer.EvenementsCre.DateC != null && evenementCreer.EvenementsCre.DateVerrou > evenementCreer.EvenementsCre.DateC) )
                    {
                        ModelState.AddModelError("EvenementsCre.DateVerrou", "La date limite est incorrecte");
                    }
                }

                //recup des utilisateurs / poles sélectionnés
                var selectedIdsPole = Request.Form.GetValues("PolesIdChecked");
                var selectedIdsUser = Request.Form.GetValues("UtilisateursIdChecked");

                //control de la sélection du personnel
                if (!StringUtils.CheckStringArrayContainsInt(selectedIdsPole) && !StringUtils.CheckStringArrayContainsInt(selectedIdsUser))
                {
                    ModelState.AddModelError("selectionError", "Aucune personne de sélectionné");
                }

                 if (ModelState.IsValid)
                 {
                     var selectedUserToSend = Request.Form["radioSelect"] == "0";
                    //si la date de verrouillage est null c'est que c'est la meme que la date de l'évènement (la plus grande des trois possibles)
                    evenementCreer.EvenementsCre.DateVerrou = (evenementCreer.EvenementsCre.DateVerrou.HasValue)
                         ? evenementCreer.EvenementsCre.DateVerrou
                         : DateUtils.GetPlusGrandeDate(evenementCreer.EvenementsCre.Date, evenementCreer.EvenementsCre.DateB, evenementCreer.EvenementsCre.DateC);
                    
                    //récup de l'évènement
                    var even = evenementCreer.EvenementsCre;
                    even.CreateurId = GetIdUtilisateurConnecte();
                    Db.Evenements.Add(even);

                    //récup des utilisateurs à inviter
                    List<Utilisateurs> listeUser;
                    if (selectedUserToSend)
                    {
                        //récup de la lsite des utilisateurs sélectionné
                        var listeInt = new List<int>();
                        foreach (var value in selectedIdsUser)
                        {
                            int i;
                            if(int.TryParse(value, out i))
                            {
                                listeInt.Add(i);
                            }
                        }

                        listeUser = Db.Utilisateurs.Where(
                            x => listeInt.Contains(x.Id)).ToList();
                    }
                    else
                    {
                        //récup de la lsite des utilisateurs sélectionné par poles
                        var listeInt = new List<int>();
                        foreach (var value in selectedIdsPole)
                        {
                            int i;
                            if (int.TryParse(value, out i))
                            {
                                listeInt.Add(i);
                            }
                        }
                        listeUser = Db.Utilisateurs.Where(x => listeInt.Contains(x.PolesId)).ToList();
                    }

                    //si l'utilisateur qui le crée n'est pas dedans, il est rajouté automatiquement
                    if (listeUser.Count(x => x.Id == GetIdUtilisateurConnecte()) == 0)
                    {
                        listeUser.Add(Db.Utilisateurs.Find(GetIdUtilisateurConnecte()));
                    }

                    //pour chaque invité, envoi d'un mail
                    var mail = Db.Mails.First(x => x.NomUnique == ContexteStatic.EnvoiInvitEvenement);
                    var corpsMessage = StringUtils.CopyString(mail.Corps);
                    foreach (var user in listeUser)
                    {
                        var evenUser = new EvenementsUtilisateurs()
                        {
                            Utilisateurs = user,
                            Evenements = even,
                            Commentaire = null,
                            IsPresent = null
                        };
                        Db.EvenementsUtilisateurs.Add(evenUser);

                        var id = GetIdUtilisateurConnecte();
                        var userCre = Db.Utilisateurs.Find(id);
                        corpsMessage = corpsMessage.Replace("%TITRE_EVEN%", even.Titre);
                        corpsMessage = corpsMessage.Replace("%ADRESSE%",
                            $"{Request.Url.Scheme}://{Request.Url.Authority}{Url.Content("~")}" +
                            "Evenements/MesEvenements");
                        corpsMessage = corpsMessage.Replace("%APPLI%", ContexteApplication.Application.NomAppli);
                        corpsMessage = corpsMessage.Replace("%NOM_CREATEUR%",  userCre.Grades.Diminutif + " " + userCre.Nom + " " + userCre.Prenom);
                        corpsMessage = corpsMessage.Replace("%DATELIM%",  DateUtils.GetDateFormat((even.DateVerrou.HasValue)? even.DateVerrou.Value:even.Date, "yyyy-MM-dd"));

                        //envoi du mail
                        await MailUtils.SendMail(user.Email, mail.Objet, corpsMessage);

                    }
                    Db.SaveChanges();
                    return RedirectToAction("MesEvenements", "Evenements");
                }
            }
            return View(GenerateViewModelCreate(evenementCreer));
        }
        #endregion



        #region Edition
        /// <summary>
        /// Chargement de la apge de modification d'un évènement
        /// </summary>
        /// <param name="id">l'id de l'évènement à modifier</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var even = Db.Evenements.Find(id);

            if (even == null || (even != null && even.CreateurId != GetIdUtilisateurConnecte()))
            {
                return HttpNotFound();
            }

            foreach (var ev in even.EvenementsUtilisateurs)
            {
                ev.Utilisateurs = Db.Utilisateurs.Find(ev.UtilisateursId);
            }

            //pour masquer la date de verrouillage si c'est la même que la date de l'évènement
            if (DateUtils.GetPlusGrandeDate(even.Date, even.DateB,even.DateC) == even.DateVerrou)
            {
                even.DateVerrou = null;
            }
            return View(even);
        }

        /// <summary>
        /// Modifie un évnement
        /// </summary>
        /// <param name="evenement">l'évènement à modifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit"), SubmitButtonSelector(Name = "ModifierEven")]
        public ActionResult ModifierEven(Evenements evenement)
        {
            if (ModelState.IsValid)
            {
                //controle de la dateA
                if (evenement.Date < DateUtils.GetAujourdhui())
                {
                    ModelState.AddModelError("Date", "La date doit être supérieur à la date du jour");
                }

                if (evenement.DemiJournee != DemiJourneeEnum.APRESMIDI && evenement.DemiJournee != DemiJourneeEnum.MATIN)
                {
                    ModelState.AddModelError("DemiJournee", "Il est nécéssaire de préciser la demi journée");
                }

                //controle de la dateB
                if (evenement.DateB != null)
                {
                    if (evenement.DemiJourneeB != DemiJourneeEnum.APRESMIDI && evenement.DemiJourneeB != DemiJourneeEnum.MATIN)
                    {
                        ModelState.AddModelError("DemiJourneeB", "Il est nécéssaire de préciser la demi journée");
                    }

                    if (evenement.DateB < DateUtils.GetAujourdhui())
                    {
                        ModelState.AddModelError("DateB", "La date doit être supérieur à la date du jour");
                    }

                    if (evenement.DateB == evenement.Date &&
                        evenement.DemiJourneeB == evenement.DemiJournee)
                    {
                        ModelState.AddModelError("DateB", "cette date est identique à la première date");
                    }
                }

                //controle de la dateC
                if (evenement.DateC != null)
                {
                    if (evenement.DemiJourneeC != DemiJourneeEnum.APRESMIDI && evenement.DemiJourneeC != DemiJourneeEnum.MATIN)
                    {
                        ModelState.AddModelError("DemiJourneeC", "Il est nécéssaire de préciser la demi journée");
                    }

                    if (evenement.DateC < DateUtils.GetAujourdhui())
                    {
                        ModelState.AddModelError("DateC", "La date doit être supérieur à la date du jour");
                    }

                    if ((evenement.DateC == evenement.Date && evenement.DemiJourneeC == evenement.DemiJournee) ||
                        (evenement.DateB != null && evenement.DemiJourneeB != null &&
                         evenement.DateC == evenement.DateB && evenement.DemiJourneeC == evenement.DemiJourneeB))
                    {
                        ModelState.AddModelError("DateC", "cette date est identique à une des dates ci dessus");
                    }
                }


                //control de la date de verrouillage
                if (evenement.DateVerrou != null)
                {
                    if (evenement.DateVerrou <= DateUtils.GetAujourdhui() ||
                        evenement.DateVerrou > evenement.Date ||
                        (evenement.DateB != null && evenement.DateVerrou > evenement.DateB) ||
                        (evenement.DateC != null && evenement.DateVerrou > evenement.DateC))
                    {
                        ModelState.AddModelError("DateVerrou", "La date limite est incorrecte");
                    }
                }

                var oldEven = Db.Evenements.Find(evenement.Id);
                if (oldEven.DateB != null && evenement.DateB == null)
                {
                    ModelState.AddModelError("DateB", "Ce champ ne peut être vide");
                }

                if (oldEven.DateC != null && evenement.DateC == null)
                {
                    ModelState.AddModelError("DateB", "Ce champ ne peut être vide");
                }

                if (ModelState.IsValid)
                {
                    if (evenement.DateVerrou == null)
                    {
                        evenement.DateVerrou = DateUtils.GetPlusGrandeDate(evenement.Date, evenement.DateB, evenement.DateC);
                    }

                    var even = Db.Evenements.Find(evenement.Id);

                    //si le propriétair est bien la personne connecté
                    if (even.CreateurId == GetIdUtilisateurConnecte())
                    {
                        //on récupère l'ancien évènement et tout les utilisateurs ayant déjà choisi une date venant d'être modifié, retrouve leurs choix  modifié
                        var listeEvenUser = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == even.Id);
                        foreach (var evenUser in listeEvenUser)
                        {
                            if (evenUser.DateSelected == even.Date && evenUser.DemiJourneeSelected == even.DemiJournee)
                            {
                                evenUser.DateSelected = evenement.Date;
                                evenUser.DemiJourneeSelected = evenement.DemiJournee;
                            }

                            if (evenUser.DateSelected == even.DateB && evenUser.DemiJourneeSelected == even.DemiJourneeB)
                            {
                                evenUser.DateSelected = evenement.DateB;
                                evenUser.DemiJourneeSelected = evenement.DemiJourneeB;
                            }

                            if (evenUser.DateSelected == even.DateC && evenUser.DemiJourneeSelected == even.DemiJourneeC)
                            {
                                evenUser.DateSelected = evenement.DateC;
                                evenUser.DemiJourneeSelected = evenement.DemiJourneeC;
                            }
                        }

                        //sauvegarde en base du nouvel évènement
                        even.Titre = evenement.Titre;
                        even.Date = evenement.Date;
                        even.DemiJournee = evenement.DemiJournee;
                        even.DateB = evenement.DateB;
                        even.DemiJourneeB = evenement.DemiJourneeB;
                        even.DateC = evenement.DateC;
                        even.DemiJourneeC = evenement.DemiJourneeC;
                        even.DateVerrou = evenement.DateVerrou;
                        even.Commentaire = evenement.Commentaire;
                        Db.Entry(even).State = EntityState.Modified;
                        Db.SaveChanges();
                    }

                    return RedirectToAction("MesEvenements", "Evenements");
                }
            }
            evenement.EvenementsUtilisateurs = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == evenement.Id).ToList();
            foreach (var ev in evenement.EvenementsUtilisateurs)
            {
                ev.Utilisateurs = Db.Utilisateurs.Find(ev.UtilisateursId);
            }

            return View(evenement);
        }

        /// <summary>
        /// Supprime un évnement
        /// </summary>
        /// <param name="id">l'id de l'évènement à supprimerr</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit"), SubmitButtonSelector(Name = "SupprimerEven")]
        public async Task<ActionResult> SupprimerEven(Evenements evenement)
        {
            var idUSer = GetIdUtilisateurConnecte();
            //Controle que l'évènement appartiennent à la personne qui le supprime
            if(Db.Evenements.Count(x => x.Id == evenement.Id && x.CreateurId == idUSer) > 0)
            {
                //préparation du mail
                var mail = Db.Mails.First(x => x.NomUnique == ContexteStatic.EnvoiDesactivationEvenement);
                var corpsMessage = StringUtils.CopyString(mail.Corps);
                corpsMessage = corpsMessage.Replace("%NOMEVEN%", evenement.Titre);

                //suppression des liens évènements et utilisateur
                var listeEvenUser = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == evenement.Id).Include(x => x.Utilisateurs).ToList();
                foreach (var evenementsUtilisateurs in listeEvenUser)
                {
                    await MailUtils.SendMail(evenementsUtilisateurs.Utilisateurs.Email, mail.Objet, corpsMessage);
                    Db.EvenementsUtilisateurs.Remove(evenementsUtilisateurs);
                }

                var ev = Db.Evenements.Find(evenement.Id);
                //supression de l'évènement
                Db.Evenements.Remove(ev);
                Db.SaveChanges();

                return RedirectToAction("MesEvenements");
            }
            return View(evenement);
        }
        
        #endregion


        #region Consultation
        /// <summary>
        /// Charge la pagede consultation d'un évènement
        /// </summary>
        /// <param name="id">l'id de l'évènement</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Consult(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var even = Db.Evenements.Find(id);
            
            if (even == null)
            {
                return HttpNotFound();
            }
            
            even.EvenementsUtilisateurs = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == id)
                .Include(x => x.Utilisateurs)
                .OrderBy(x => x.DateSelected)
                .ThenBy(x => x.DemiJourneeSelected)
                .ThenBy(x => x.Utilisateurs.Nom)
                .ThenBy(x => x.Utilisateurs.Prenom).ToList();

            //les invités
            /*foreach (var ev in even.EvenementsUtilisateurs)
            {
                ev.Utilisateurs = Db.Utilisateurs.Find(ev.UtilisateursId);
                if (ev.UtilisateursId == GetIdUtilisateurConnecte())
                {
                    even.IsUtilisateurPresent = ev.IsPresent;
                    even.CommentaireUtilisateur = ev.Commentaire;
                    even.SelectedDate = 1;
                }
            }*/
            var myId = GetIdUtilisateurConnecte();
            var ev = even.EvenementsUtilisateurs.First(x => x.UtilisateursId == myId);
            even.IsUtilisateurPresent = ev.IsPresent;
            even.CommentaireUtilisateur = ev.Commentaire;
            //sléection de la date sur la page
            if (ev.DateSelected != null)
            {
                if (ev.DateSelected == even.Date && ev.DemiJourneeSelected == even.DemiJournee)
                {
                    even.SelectedDate = 1;
                }
                else if (ev.DateSelected == even.DateB && ev.DemiJourneeSelected == even.DemiJourneeB)
                {
                    even.SelectedDate = 2;
                }
                else if (ev.DateSelected == even.DateC && ev.DemiJourneeSelected == even.DemiJourneeC)
                {
                    even.SelectedDate = 3;
                }
                else
                {
                    even.SelectedDate = 1;
                }
            }
            else
            {
                even.SelectedDate = 1;
            }

            return View(even);
        }

        /// <summary>
        /// Modifie l'invitation pour utilisateur
        /// </summary>
        /// <param name="even">l'évènement concerné</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Consult"), SubmitButtonSelector(Name = "ValidConsult")]
        public ActionResult Consult(Evenements even)
        {
            var error = false;
            if (even.Commentaire != null && even.CommentaireUtilisateur.Length >= 500)
            {
                ModelState.AddModelError("CommentairePresence", "Ce champs est trop long");
                error = true;
            }
            if (even.IsUtilisateurPresent == null)
            {
                ModelState.AddModelError("IsPresentPresence", "Ce champs n'est pas rempli");
                error = true;
            }
            if (even.SelectedDate < 1 || even.SelectedDate > 3)
            {
                ModelState.AddModelError("IsDateSelect", "Ce champs n'est pas rempli");
                error = true;
            }

            if (!error)
            {
                var myId = GetIdUtilisateurConnecte();
                var userEvenList = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == even.Id && x.UtilisateursId == myId).ToList();
                if (userEvenList.Count > 0)
                {
                    var userEven = userEvenList[0];
                    var evenOri = Db.Evenements.Find(even.Id);
                    switch (even.SelectedDate)
                    {
                        case 1:
                            userEven.DateSelected = evenOri.Date;
                            userEven.DemiJourneeSelected = evenOri.DemiJournee;
                            break;

                        case 2:
                            userEven.DateSelected = evenOri.DateB;
                            userEven.DemiJourneeSelected = evenOri.DemiJourneeB;
                            break;

                        case 3:
                            userEven.DateSelected = evenOri.DateC;
                            userEven.DemiJourneeSelected = evenOri.DemiJourneeC;
                            break;

                        default:
                            userEven.DateSelected = evenOri.Date;
                            userEven.DemiJourneeSelected = evenOri.DemiJournee;
                            break;

                    }
                    userEven.IsPresent = even.IsUtilisateurPresent;
                    userEven.Commentaire = even.CommentaireUtilisateur;
                    Db.Entry(userEven).State = EntityState.Modified;
                    Db.SaveChanges();
                    return RedirectToAction("MesEvenements", "Evenements");
                }
            }

            even = Db.Evenements.Find(even.Id);
            even.EvenementsUtilisateurs = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == even.Id)
                .Include(x => x.Utilisateurs)
                .OrderBy(x => x.DateSelected)
                .ThenBy(x => x.DemiJourneeSelected)
                .ThenBy(x => x.Utilisateurs.Nom)
                .ThenBy(x => x.Utilisateurs.Prenom).ToList();

            return View(even);
        }

        /// <summary>
        /// Modifie l'invitation pour utilisateur
        /// </summary>
        /// <param name="even">l'évènement concerné</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Consult"), SubmitButtonSelector(Name = "DownloadConsult")]
        public ActionResult Download(Evenements evenement)
        {
            
            var even = Db.Evenements.Find(evenement.Id);

            if (even == null)
            {
                return HttpNotFound();
            }

            //les invités
            even.EvenementsUtilisateurs = Db.EvenementsUtilisateurs.Where(x => x.EvenementsId == even.Id)
               .Include(x => x.Utilisateurs)
               .OrderBy(x => x.DateSelected)
               .ThenBy(x => x.DemiJourneeSelected)
               .ThenBy(x => x.Utilisateurs.Nom)
               .ThenBy(x => x.Utilisateurs.Prenom).ToList();

            var mem = new MemoryStream();
            var document = DocX.Create(mem);
            var parTitre = document.InsertParagraph();
            parTitre.Append(even.Titre);
            var parDate = document.InsertParagraph();
            parDate.Append("Date 1 : "+DateUtils.GetDateFormat(even.Date,"yyyy-MM-dd")+" "+even.DemiJournee);
            if (even.DateB != null)
            {
                var parDateB = document.InsertParagraph();
                parDateB.Append("Date 2 : " + DateUtils.GetDateFormat(even.DateB.Value, "yyyy-MM-dd") + " " + even.DemiJourneeB);
            }
            if (even.DateC != null)
            {
                var parDateC = document.InsertParagraph();
                parDateC.Append("Date 3 : " + DateUtils.GetDateFormat(even.DateC.Value, "yyyy-MM-dd") + " " + even.DemiJourneeC);
            }
            var parCreateur = document.InsertParagraph();
            parCreateur.Append("Par le "+even.Createur.Grades.Diminutif+" "+even.Createur.Nom+" "+even.Createur.Prenom);
            var parCommentaire = document.InsertParagraph();
            parCommentaire.Append(even.Commentaire);
            var t = document.AddTable(even.EvenementsUtilisateurs.Count + 2, 4);
            t.Alignment = Alignment.center;
            t.Design = TableDesign.MediumGrid2Accent1;

            t.Rows[0].Cells[0].Paragraphs.First().Append("Personnel");
            t.Rows[0].Cells[1].Paragraphs.First().Append("Présence");
            t.Rows[0].Cells[2].Paragraphs.First().Append("Date");
            t.Rows[0].Cells[3].Paragraphs.First().Append("Commentaire");

            var oui = 0;
            var non = 0;
            var i = 1;
            foreach (var evenementsUtilisateur in even.EvenementsUtilisateurs)
            {
                t.Rows[i].Cells[0].Paragraphs.First().Append(evenementsUtilisateur.Utilisateurs.Nom+" "+ evenementsUtilisateur.Utilisateurs.Prenom);
                t.Rows[i].Cells[1].Paragraphs.First().Append(evenementsUtilisateur.DateSelected.HasValue ? DateUtils.GetDateFormat(evenementsUtilisateur.DateSelected.Value,"yyyy-MM-dd")+" - "+evenementsUtilisateur.DemiJourneeSelected : " ");
                t.Rows[i].Cells[2].Paragraphs.First().Append(evenementsUtilisateur.IsPresent.HasValue? (evenementsUtilisateur.IsPresent.Value ? "Oui" : "Non") : "");
                t.Rows[i].Cells[3].Paragraphs.First().Append(evenementsUtilisateur.Commentaire);
                if (evenementsUtilisateur.IsPresent.HasValue)
                {
                    if (evenementsUtilisateur.IsPresent.Value)
                    {
                        oui++;
                    }
                    else
                    {
                        non++;
                    }
                }
                i++;
            }
            t.Rows[i].Cells[2].Paragraphs.First().Append("Oui :"+oui+" -- Non :"+non);

            document.InsertTable(t);

            document.Save();

            return File(mem.ToArray(), "Application/octet-stream", even.Titre+".docx");
        }

        #endregion
        }
}