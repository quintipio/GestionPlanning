using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Controleur pour la gestion des activités des utilisateurs
    /// </summary>
    [Authorize(Roles = "UTILISATEUR,ADMINISTRATEUR")]
    public class ActivitesController : AbstractControl
    {

        #region méthodes privés

        /// <summary>
        /// efface toute les particularité des repas compris entre deux dates
        /// </summary>
        /// <param name="dateDebut">la date de début</param>
        /// <param name="dateFin">la date de fin</param>
        private void RemiseEtatNormalRepas(DateTime dateDebut, DateTime dateFin)
        {
            var idUser = GetIdUtilisateurConnecte();
            //récupère les dates en base de l'utilisateurs pour les effacer
            var listeAnomalies = Db.DateExceptionRepas.Where(x => x.Date <= dateFin && x.Date >= dateDebut && x.UtilisateurId == idUser).ToList();
            Db.DateExceptionRepas.RemoveRange(listeAnomalies);
            Db.SaveChanges();
        }

        /// <summary>
        /// Supprime tout les repas compris entre deux dates
        /// </summary>
        /// <param name="dateDebut">la date de début</param>
        /// <param name="dateFin">la date de fin</param>
        private void SupprimeRepas(DateTime dateDebut, DateTime dateFin)
        {
            RemiseEtatNormalRepas(dateDebut, dateFin);

            //on réucpère les dates de cette semaine pour éviter de pointer sur des dates devant être bloqué
            var lundiEnCours = DateUtils.GetAujourdhui().AddDays(DayOfWeek.Monday - DateUtils.GetAujourdhui().DayOfWeek);
            var vendrediEnCours = lundiEnCours.AddDays(4);

            var dateDeb = dateDebut;
            var dateF = dateFin.AddDays(1);
            if (dateDeb <= dateFin)
            {
                //ajoute les repas du midi aux exceptions (en supposant que toute les exceptions ont été supprimées avant)
                do
                {
                    if ((dateDeb.DayOfWeek == DayOfWeek.Monday || dateDeb.DayOfWeek == DayOfWeek.Tuesday ||
                        dateDeb.DayOfWeek == DayOfWeek.Wednesday || dateDeb.DayOfWeek == DayOfWeek.Thursday) &&
                        dateDeb > vendrediEnCours)
                    {
                        var repas = new DateExceptionRepas
                        {
                            UtilisateurId = GetIdUtilisateurConnecte(),
                            Date = dateDeb,
                            IsPresent = false,
                            PeriodeRepas = PeriodeRepasEnum.MIDI
                        };
                        Db.DateExceptionRepas.Add(repas);
                    }
                       
                    dateDeb = dateDeb.AddDays(1);
                } while (dateDeb != dateF);
                Db.SaveChanges();

            }

        }

        private bool CheckAutresActivitesSurDate(DateTime dateDebut, DemiJourneeEnum demiJourneeDebut, DateTime dateFin,
            DemiJourneeEnum demiJourneeFin, int idUtilisateur,int? idActivite)
        {
            if (idActivite.HasValue)
            {
                return Db.Activites.Count(x => x.UtilisateurId == idUtilisateur && x.Id != idActivite.Value && (
                (dateDebut > x.DateDebut && dateDebut < x.DateFin) ||
                (dateFin > x.DateDebut && dateFin < x.DateFin) ||
                (dateDebut == x.DateDebut && x.DemiJourneeDebut == DemiJourneeEnum.APRESMIDI && demiJourneeDebut == DemiJourneeEnum.MATIN) ||
                (dateDebut == x.DateDebut && x.DemiJourneeDebut == demiJourneeDebut) ||
                (dateFin == x.DateDebut && x.DemiJourneeDebut == DemiJourneeEnum.MATIN && demiJourneeFin == DemiJourneeEnum.APRESMIDI) ||
                (dateFin == x.DateDebut && x.DemiJourneeDebut == demiJourneeFin) ||
                (dateDebut == x.DateFin && x.DemiJourneeFin == DemiJourneeEnum.APRESMIDI && demiJourneeDebut == DemiJourneeEnum.MATIN) ||
                (dateDebut == x.DateFin && x.DemiJourneeFin == demiJourneeDebut) ||
                (dateFin == x.DateFin && x.DemiJourneeFin == demiJourneeFin)
                )) > 0;
            }
                return Db.Activites.Count(x => x.UtilisateurId == idUtilisateur && (
                 (dateDebut > x.DateDebut && dateDebut < x.DateFin) ||
                 (dateFin > x.DateDebut && dateFin < x.DateFin) ||
                 (dateDebut == x.DateDebut && x.DemiJourneeDebut == DemiJourneeEnum.APRESMIDI && demiJourneeDebut == DemiJourneeEnum.MATIN) ||
                (dateDebut == x.DateDebut && x.DemiJourneeDebut == demiJourneeDebut) ||
                (dateFin == x.DateDebut && x.DemiJourneeDebut == DemiJourneeEnum.MATIN && demiJourneeFin == DemiJourneeEnum.APRESMIDI) ||
                (dateFin == x.DateDebut && x.DemiJourneeDebut == demiJourneeFin) ||
                (dateDebut == x.DateFin && x.DemiJourneeFin == DemiJourneeEnum.APRESMIDI && demiJourneeDebut == DemiJourneeEnum.MATIN) ||
                (dateDebut == x.DateFin && x.DemiJourneeFin == demiJourneeDebut) ||
                (dateFin == x.DateFin && x.DemiJourneeFin == demiJourneeFin)
                 )) > 0;
        }

        #endregion

        /// <summary>
        /// Charge la page des activités d'un utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var id = GetIdUtilisateurConnecte();
            //renvoi les activités non passées
            var aujourdhui = DateUtils.GetAujourdhui();
            return View(Db.Activites.Where(x => x.UtilisateurId == id && x.DateFin >= aujourdhui).ToList());
        }
        

        /// <summary>
        /// Charge le page d'une création d'activité d'un utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.TypeActiviteId = new SelectList(Db.TypeActivites.Where(x => x.Etat == EtatEnum.ACTIF), "Id", "Nom");
            return View(new Activites {Id = GetIdUtilisateurConnecte(), DemiJourneeDebut = DemiJourneeEnum.MATIN,DateDebut = DateUtils.GetAujourdhui(), DateFin = DateUtils.GetAujourdhui(), DemiJourneeFin = DemiJourneeEnum.APRESMIDI, TypeActiviteId = ContexteStatic.IdActivitePermission});
        }
        
        /// <summary>
        /// Créer une activité
        /// </summary>
        /// <param name="activites">l'activité à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Activites activites)
        {
            if (activites.DateDebut > activites.DateFin)
            {
                ModelState.AddModelError("DateFin", "La date de début doit être inférieure à la date de fin");
            }

            if (activites.DateDebut < DateUtils.GetAujourdhui())
            {
                ModelState.AddModelError("DateDebut", "La date de début doit être au moins supérieur à la date du jour");
            }

            if (activites.DateDebut == activites.DateFin && activites.DemiJourneeFin == DemiJourneeEnum.MATIN &&
                activites.DemiJourneeDebut == DemiJourneeEnum.APRESMIDI)
            {
                ModelState.AddModelError("DemiJourneeDebut", "Incohérence entre la demi journée de début et la demi journée de fin");
            }

            if (CheckAutresActivitesSurDate(activites.DateDebut, activites.DemiJourneeDebut, activites.DateFin,
                activites.DemiJourneeFin, GetIdUtilisateurConnecte(),null))
            {
                ModelState.AddModelError("DateDebut", "Une activité est déjà enregistré entre ces deux dates");
            }

            if (ModelState.IsValid)
            {
                //sauvegarde en base
                activites.UtilisateurId = GetIdUtilisateurConnecte();
                Db.Activites.Add(activites);
                Db.SaveChanges();

                //changement des repas si le type d'activité le permet
                var typeActi = Db.TypeActivites.Find(activites.TypeActiviteId);
                if (typeActi.ModifierRepas)
                {
                    SupprimeRepas(activites.DateDebut, activites.DateFin);
                }

                return RedirectToAction("Index");
            }

            ViewBag.TypeActiviteId = new SelectList(Db.TypeActivites.Where(x => x.Etat == EtatEnum.ACTIF), "Id", "Nom", activites.TypeActiviteId);
            return View(activites);
        }

        /// <summary>
        /// Charge la page de modification d'une activité
        /// </summary>
        /// <param name="id">l'id de l'activité</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activites = Db.Activites.Find(id);
            if (activites == null || (activites != null && activites.UtilisateurId != GetIdUtilisateurConnecte()))
            {
                return HttpNotFound();
            }
            ViewBag.TypeActiviteId = new SelectList(Db.TypeActivites.Where(x => x.Etat == EtatEnum.ACTIF), "Id", "Nom", activites.TypeActiviteId);
            return View(activites);
        }
        
        /// <summary>
        /// Modifie une activité
        /// </summary>
        /// <param name="activites"> l'activité à modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Activites activites)
        {
            int idUser = GetIdUtilisateurConnecte();
            
            if (ModelState.IsValid)
            {
                if (activites.DateDebut > activites.DateFin)
                {
                    ModelState.AddModelError("DateFin", "La date de début doit être inférieure à la date de fin");
                }

                if (activites.DateDebut < DateUtils.GetAujourdhui())
                {
                    ModelState.AddModelError("DateDebut", "La date de début doit être au moins supérieur à la date du jour");
                }

                if (activites.DateDebut == activites.DateFin && activites.DemiJourneeFin == DemiJourneeEnum.MATIN &&
                activites.DemiJourneeDebut == DemiJourneeEnum.APRESMIDI)
                {
                    ModelState.AddModelError("DemiJourneeDebut", "Incohérence entre la demi journée de début et la demi journée de fin");
                }


                if (CheckAutresActivitesSurDate(activites.DateDebut, activites.DemiJourneeDebut, activites.DateFin,
                activites.DemiJourneeFin, GetIdUtilisateurConnecte(), activites.Id))
                {
                    ModelState.AddModelError("dateDebutAct", "Une activité est déjà enregistré entre ces deux dates");
                }

                if (ModelState.IsValid && Db.Activites.Count(x => x.Id == activites.Id && x.UtilisateurId == idUser) > 0)
                {
                    var oldActivite = Db.Activites.Find(activites.Id);
                    
                    //remise en état normal des repas de l'ancienne activité
                    if (oldActivite.TypeActivites.ModifierRepas)
                    {
                        RemiseEtatNormalRepas(oldActivite.DateDebut, oldActivite.DateFin);
                    }

                    //suppression des repas de la nouvelle activité
                    var type = Db.TypeActivites.Find(activites.TypeActiviteId);
                    if (type.ModifierRepas)
                    {
                        SupprimeRepas(activites.DateDebut, activites.DateFin);
                    }

                    //mise en base
                    oldActivite.DateDebut = activites.DateDebut;
                    oldActivite.DateFin = activites.DateFin;
                    oldActivite.TypeActiviteId = activites.TypeActiviteId;
                    oldActivite.DemiJourneeDebut = activites.DemiJourneeDebut;
                    oldActivite.DemiJourneeFin = activites.DemiJourneeFin;
                    oldActivite.Commentaire = activites.Commentaire;
                    Db.Entry(oldActivite).State = EntityState.Modified;
                    Db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            
            ViewBag.TypeActiviteId = new SelectList(Db.TypeActivites.Where(x => x.Etat == EtatEnum.ACTIF), "Id", "Nom", activites.TypeActiviteId);
            return View(activites);
        }

        /// <summary>
        /// Charge le page pour effcer une activité
        /// </summary>
        /// <param name="id">l'id de l'activité à effacer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var activites = Db.Activites.Find(id);
            if (activites == null || (activites != null && activites.UtilisateurId != GetIdUtilisateurConnecte()))
            {
                return HttpNotFound();
            }

            //suppression de la base
            Db.Activites.Remove(activites);
            Db.SaveChanges();

            //remise en état des repas
            //changement des repas si le type d'activité le permet
            var typeActi = Db.TypeActivites.Find(activites.TypeActiviteId);
            if (typeActi.ModifierRepas)
            {
                RemiseEtatNormalRepas(activites.DateDebut, activites.DateFin);
            }
           
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Créer la page pour la génération du fichier de perm
        /// </summary>
        /// <param name="id">l'id de l'activité qui doit être une permission</param>
        /// <returns></returns>
        public ActionResult GeneratePerm(int? id)
        {
            //Controle
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var activites = Db.Activites.Find(id);
            if (activites == null || (activites != null && activites.UtilisateurId != GetIdUtilisateurConnecte()) || (activites != null & activites.TypeActiviteId != ContexteStatic.IdActivitePermission))
            {
                return HttpNotFound();
            }

            //récupération des données
            var idUser = GetIdUtilisateurConnecte();
            var user = Db.Utilisateurs.Find(idUser);
            var arme = Db.Armes.Find(user.Grades.ArmesId);
            var isSensiblePcp = arme.IsPcp;
            var isSensibleRtt = arme.IsRtt;
            
            //décompte de jours de perms (en prenant en compte week end jour férié, pcp et rtt)
            var compteurJours = 0;
            var dateDeb = activites.DateDebut;
            var dateF = activites.DateFin.AddDays(1);
            var listePcp = Db.Permissions.Where(x => x.Date >= activites.DateDebut && x.Date <= activites.DateFin && x.TypePerm == TypePermEnum.PCP).Select(x => x.Date).ToList();
            var listeRtt = Db.Permissions.Where(x => x.Date >= activites.DateDebut && x.Date <= activites.DateFin && x.TypePerm == TypePermEnum.RTT).Select(x => x.Date).ToList();
            
            do
            {
                if (!DateUtils.IsFerie(dateDeb) && !DateUtils.IsWeekEnd(dateDeb) &&
                    !(isSensiblePcp && listePcp.Contains(dateDeb)) && !(isSensibleRtt && listeRtt.Contains(dateDeb)))
                { 
                    compteurJours++;
                }
               dateDeb =  dateDeb.AddDays(1);
            } while (dateDeb != dateF);

            return View(new GenerateursPermsViewModel { IdActivite = id.Value, CodePostal = user.CodePostal, NbJoursTotal = user.NbJoursPermsRestant, Adresse = user.Adresse, Ville = user.Ville, NbJoursDemande = compteurJours, Telephone = user.Telephone});
        }

        /// <summary>
        /// Génère un fichier de permmission à partir du model en base
        /// </summary>
        /// <param name="infosPerms">les infos supplémentaire à inclure dans le fichier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeneratePerm(GenerateursPermsViewModel infosPerms)
        {
            var activites = Db.Activites.Find(infosPerms.IdActivite);
            if (activites == null || (activites != null && activites.UtilisateurId != GetIdUtilisateurConnecte()) || (activites != null & activites.TypeActiviteId != ContexteStatic.IdActivitePermission))
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                //récupérationdes infos nécéssaire en base
                var idUser = GetIdUtilisateurConnecte();
                var user = Db.Utilisateurs.Find(idUser);
                var arme = Db.Armes.Find(user.Grades.ArmesId);

                //si c'est un militaire ou un civil le fichier n'est pas le meme (détecté si la personne utilise des rtt ou des pcp)
                MemoryStream streamFile;
                if (arme.IsPcp)
                {
                    if (ContexteApplication.Application.FilePermMilitaire != null)
                    {
                        streamFile = new MemoryStream(ContexteApplication.Application.FilePermMilitaire);
                    }
                    else
                    {
                        ModelState.AddModelError("erreurAucunFichier", "Le fichier de permission n'est pas présent");
                        return View(infosPerms);
                    }
                }
                else
                {
                    if (ContexteApplication.Application.FilePermCivil != null)
                    {
                        streamFile = new MemoryStream(ContexteApplication.Application.FilePermCivil);
                    }
                    else
                    {
                        ModelState.AddModelError("erreurAucunFichier", "Le fichier de permission n'est pas présent");
                        return View(infosPerms);
                    }
                }
                var file = DocX.Load(streamFile);
                
                //remplacement des variables
                file.ReplaceText("INSERT_NAME", user.Grades.Diminutif + " " + user.Nom + " " + user.Prenom);
                file.ReplaceText("INSERT_TODAY", DateUtils.GetDateJour());
                file.ReplaceText("INSERT_NID", user.Nid);
                file.ReplaceText("INSERT_PNIA", user.Pnia);
                file.ReplaceText("INSERT_UNITE", ContexteApplication.Application.NomUnite);

                file.ReplaceText("INSERT_ADRESSE", infosPerms.Adresse);
                file.ReplaceText("INSERT_VILLE", infosPerms.Ville);
                file.ReplaceText("INSERT_CODEPOSTAL", infosPerms.CodePostal.ToString());
                file.ReplaceText("INSERT_PAYS", "France");
                file.ReplaceText("INSERT_TELEPHONE", infosPerms.Telephone);
                file.ReplaceText("INSERT_NBJOURSTOTAL", infosPerms.NbJoursTotal.ToString());
                file.ReplaceText("INSERT_NBJOURSDECOMPTE", infosPerms.NbJoursDemande.ToString());
                file.ReplaceText("INSERT_NBJOURSRESTANT", (infosPerms.NbJoursTotal - infosPerms.NbJoursDemande).ToString());

                file.ReplaceText("INSERT_DATEDEB", DateUtils.GetDateFormat(activites.DateDebut, "yyyy-MM-dd"));
                file.ReplaceText("INSERT_DATEFIN", DateUtils.GetDateFormat(activites.DateFin, "yyyy-MM-dd"));

                var mem = new MemoryStream();
                file.SaveAs(mem);

                //sauvegarde des nouvelles données en base
                user.Telephone = infosPerms.Telephone;
                user.Adresse = infosPerms.Adresse;
                user.CodePostal = infosPerms.CodePostal;
                user.Ville = infosPerms.Ville;
                user.NbJoursPermsRestant = infosPerms.NbJoursTotal - infosPerms.NbJoursDemande;
                Db.Entry(user).State = EntityState.Modified;
                Db.SaveChanges();

                return File(mem.ToArray(), "Application/octet-stream", user.Nid+".docx");
            }
            return View(infosPerms);
        }


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
