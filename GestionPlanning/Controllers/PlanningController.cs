using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Enum;
using GestionPlanning.Utils;
using GestionPlanning.ViewModel;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur du calendrier
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR,UTILISATEUR")]
    [AllowAnonymous]
    public class PlanningController : AbstractControl
    {
        /// <summary>
        /// Retourne le viewModel à afficher à partir de la date
        /// </summary>
        /// <param name="date">la date de début</param>
        /// <returns>l'objet</returns>
        private UserDateEventViewModel GetData(DateTime date)
        {
            var dateFinMois = DateUtils.GetEndMoisDate(date);
            var dateDebut = new DateTime(date.Year, date.Month, 1);
            var listeArme = Db.Armes.ToList();

            //chargement des données de bases (les activités existante, les utilisateurs, la date du jour)
            var data = new UserDateEventViewModel
            {
                IdUserEnCours = (User?.Identity != null && User.Identity.IsAuthenticated)?GetIdUtilisateurConnecte():0,
                ListeTypesActivites =  Db.TypeActivites.Where(x => x.Etat == EtatEnum.ACTIF).ToList(),
                ListeUtilisateurs = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF)
                                                    .OrderBy(x => x.Poles.Nom)
                                                    .ThenBy(x=> x.Nom)
                                                    .ThenBy(x => x.Prenom)
                                                    .ToList(),
                DateEnCours = date,
                Annee = date.Year.ToString(),
                Mois = DateUtils.GetNomMois(date)
            };
            
            //chargement des activités, des armes et des évènements
            foreach (var utilisateur in data.ListeUtilisateurs)
            {
                utilisateur.Activites = Db.Activites.Where(x => x.UtilisateurId == utilisateur.Id && x.DateDebut>= dateDebut && x.DateFin <= dateFinMois).ToList();
                utilisateur.EvenementsUtilisateurs =  Db.EvenementsUtilisateurs.Where(x => x.UtilisateursId == utilisateur.Id && x.IsPresent.HasValue && x.IsPresent.Value && x.DateSelected.HasValue && x.DateSelected.Value <= dateFinMois && x.DateSelected.Value >= dateDebut).Include(x=>x.Evenements).ToList();
                utilisateur.Grades.Armes = listeArme.First(x => x.Id == utilisateur.Grades.ArmesId);
            }

            //chargement des jours (avec indication si jour férié ou non)
            var listePcp = Db.Permissions.Where(x => x.Date >= dateDebut && x.Date <= dateFinMois && x.TypePerm == TypePermEnum.PCP).Select(x => x.Date).ToList();
            var listeRtt = Db.Permissions.Where(x => x.Date >= dateDebut && x.Date <= dateFinMois && x.TypePerm == TypePermEnum.RTT).Select(x => x.Date).ToList();
            data.ListeJours = new List<JourViewModel>();
            for (var i = 1; i <= dateFinMois.Day; i++)
            {
                var dateBoucle = new DateTime(date.Year, date.Month, i);
                data.ListeJours.Add(new JourViewModel
                {
                    Date = new DateTime(date.Year,date.Month,i),
                    LettreJour = DateUtils.GetJourDiminutif(new DateTime(date.Year,date.Month,i)),
                    IsFerie = DateUtils.IsFerie(dateBoucle) || DateUtils.IsWeekEnd(dateBoucle),
                    IsRtt = listeRtt.Contains(dateBoucle),
                    IsPcp = listePcp.Contains(dateBoucle)
                });
            }
            return data;
        }


        #region page principale
        /// <summary>
        /// Charge la apge du planning
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(GetData(DateTime.Today));
        }

        /// <summary>
        /// Charge le planning du mois suivant
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index"), SubmitButtonSelector(Name="NextMonth")]
        public ActionResult NextMonth()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);
            var data =
                GetData(date.Month >= 12
                    ? new DateTime(date.Year + 1, 1, 1)
                    : new DateTime(date.Year, date.Month + 1, 1));
            return View(data);
        }
        
        /// <summary>
        /// Charge le planning du mois précédent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index"), SubmitButtonSelector(Name = "PreviousMonth")]
        public ActionResult PreviousMonth()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);
            var data =
                GetData(date.Month <= 1
                    ? new DateTime(date.Year - 1, 12, 1)
                    : new DateTime(date.Year, date.Month - 1, 1));
            return View(data);
        }

        #endregion
    }
}