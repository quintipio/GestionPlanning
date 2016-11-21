using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Enum;
using GestionPlanning.Models;
using GestionPlanning.Utils;
using GestionPlanning.ViewModel;
using Novacode;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur de la gestion des repas
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR,UTILISATEUR")]
    public class RepasController : AbstractControl
    {
        #region méthodes privées
        /// <summary>
        /// Génère un viewModel par défaut pour les repas
        /// </summary>
        /// <param name="dateDebut">la date de début de semaine à afficher</param>
        /// <returns>le viewModel pret</returns>
        private RepasViewModel GenerateViewModelDefaut(DateTime dateDebut)
        {
            var lundi = new DateTime(dateDebut.AddDays(DayOfWeek.Monday - dateDebut.DayOfWeek).Year, dateDebut.AddDays(DayOfWeek.Monday - dateDebut.DayOfWeek).Month, dateDebut.AddDays(DayOfWeek.Monday - dateDebut.DayOfWeek).Day);
            var vendredi = lundi.AddDays(4);

            var listePcp = Db.Permissions.Where(x => x.Date >= lundi && x.Date <= vendredi && x.TypePerm == TypePermEnum.PCP).Select(x => x.Date).ToList();
            var listeRtt = Db.Permissions.Where(x => x.Date >= lundi && x.Date <= vendredi && x.TypePerm == TypePermEnum.RTT).Select(x => x.Date).ToList();

            var IsLundiMidiA = DateUtils.IsFerie(lundi);
            var IsLundiMidiB = listeRtt.Contains(lundi);
            var IsLundiMidiC = listePcp.Contains(lundi);

            var IsMardiMidiA = DateUtils.IsFerie(lundi.AddDays(1));
            var IsMardiMidiB = listeRtt.Contains(lundi.AddDays(1));
            var IsMardiMidiC = listePcp.Contains(lundi.AddDays(1));

            var viewModel = new RepasViewModel
            {
                //les dates
                IsDisable = lundi <= DateUtils.GetAujourdhui(),
                DateDeb = lundi,
                MoisAnneeDateDeb = " "+DateUtils.GetNomMois(lundi)+" "+lundi.Year,

                DateFin = vendredi,
                MoisAnneeDateFin = " " + DateUtils.GetNomMois(vendredi) + " " + vendredi.Year,

                //la présence par défaut (en prenant en compte les jours fériés)
                IsLundiMidi = (DateUtils.IsFerie(lundi) || (listeRtt.Contains(lundi) && listePcp.Contains(lundi)))?false:true,
                IsLundiSoir = false,

                IsMardiMidi = (DateUtils.IsFerie(lundi.AddDays(1)) || (listeRtt.Contains(lundi.AddDays(1)) && listePcp.Contains(lundi.AddDays(1)))) ? false : true,
                IsMardiSoir = false,

                IsMercrediMidi = (DateUtils.IsFerie(lundi.AddDays(2)) || (listeRtt.Contains(lundi.AddDays(2)) && listePcp.Contains(lundi.AddDays(2)))) ? false : true,
                IsMercrediSoir = false,

                IsJeudiMidi = (DateUtils.IsFerie(lundi.AddDays(3)) || (listeRtt.Contains(lundi.AddDays(3)) && listePcp.Contains(lundi.AddDays(3)))) ? false : true,
                IsJeudiSoir = false,

                IsVendrediMidi = false
            };
            return viewModel;
        }

        /// <summary>
        /// Récupère les particularité d'une semaine de repas pour un utilisateur
        /// </summary>
        /// <param name="viewModel">le viewModel à modifier</param>
        /// <returns>le viewModel modifié</returns>
        private RepasViewModel RecupDonneesSemainePerso(RepasViewModel viewModel)
        {
            //charge les exceptions
            var idUser = GetIdUtilisateurConnecte();
            var listeException =
                Db.DateExceptionRepas.Where(
                    x => x.UtilisateurId == idUser && x.Date <= viewModel.DateFin && x.Date >= viewModel.DateDeb)
                    .ToList();

            //les applique au planning
            foreach (var exception in listeException)
            {
                switch (exception.Date.DayOfWeek)
                {
                     case DayOfWeek.Monday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsLundiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsLundiSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Tuesday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsMardiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsMardiSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Wednesday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsMercrediMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsMercrediSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Thursday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsJeudiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsJeudiSoir = exception.IsPresent;
                        }
                        break;
                        

                    case DayOfWeek.Friday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsVendrediMidi = exception.IsPresent;
                        }
                        break;
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Génère les exception d'un viewModel
        /// </summary>
        /// <param name="viewModel">le viewModel à modifié</param>
        /// <param name="listeException">les exccetions à inclure</param>
        /// <returns>le viewMoel modifié</returns>
        private RepasViewModel GenerateRepasSemaine(RepasViewModel viewModel, ICollection<DateExceptionRepas> listeException)
        {
            foreach (var exception in listeException)
            {
                switch (exception.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsLundiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsLundiSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Tuesday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsMardiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsMardiSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Wednesday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsMercrediMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsMercrediSoir = exception.IsPresent;
                        }
                        break;

                    case DayOfWeek.Thursday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsJeudiMidi = exception.IsPresent;
                        }

                        if (exception.PeriodeRepas == PeriodeRepasEnum.SOIR)
                        {
                            viewModel.IsJeudiSoir = exception.IsPresent;
                        }
                        break;


                    case DayOfWeek.Friday:
                        if (exception.PeriodeRepas == PeriodeRepasEnum.MIDI)
                        {
                            viewModel.IsVendrediMidi = exception.IsPresent;
                        }
                        break;
                }
            }

            return viewModel;
        }

        #endregion


        #region Gestion du planning perso
        /// <summary>
        /// Charge la page du planning du mess d'un utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult PlanningMessPerso()
        {
            var viewModel = GenerateViewModelDefaut(DateUtils.GetAujourdhui());
            viewModel = RecupDonneesSemainePerso(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Charge le planning perso de la semaine précédente
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PlanningMessPerso"), SubmitButtonSelector(Name = "PreviousWeek")]
        public ActionResult PreviousWeek()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);

            date = date.AddDays(-7);
            var viewModel = GenerateViewModelDefaut(date);
            viewModel = RecupDonneesSemainePerso(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Charge le planning perso de la semaine suivante
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PlanningMessPerso"), SubmitButtonSelector(Name = "NextWeek")]
        public ActionResult NextWeek()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);

            date = date.AddDays(7);
            var viewModel = GenerateViewModelDefaut(date);
            viewModel = RecupDonneesSemainePerso(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Modifie la présence d'un repas
        /// </summary>
        /// <param name="input">le repas et sa présence</param>
        /// <param name="date">la date de la modification</param>
        public void AjaxModifRepas(string input, string date)
        {
            var dataSplit = input.Split('=');
            var dateConvert = DateUtils.StringEnDate(date);

            if (dataSplit.Length == 2 && dateConvert.DayOfWeek == DayOfWeek.Monday)
            {
                DateTime dateModif;
                PeriodeRepasEnum momentRepas;
                switch (dataSplit[0])
                {
                    case "LundiMidi":
                        dateModif = dateConvert;
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                    case "LundiSoir":
                        dateModif = dateConvert;
                        momentRepas = PeriodeRepasEnum.SOIR;
                        break;

                    case "MardiMidi":
                        dateModif = dateConvert.AddDays(1);
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                    case "MardiSoir":
                        dateModif = dateConvert.AddDays(1);
                        momentRepas = PeriodeRepasEnum.SOIR;
                        break;

                    case "MercrediMidi":
                        dateModif = dateConvert.AddDays(2);
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                    case "MercrediSoir":
                        dateModif = dateConvert.AddDays(2);
                        momentRepas = PeriodeRepasEnum.SOIR;
                        break;

                    case "JeudiMidi":
                        dateModif = dateConvert.AddDays(3);
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                    case "JeudiSoir":
                        dateModif = dateConvert.AddDays(3);
                        momentRepas = PeriodeRepasEnum.SOIR;
                        break;

                    case "VendrediMidi":
                        dateModif = dateConvert.AddDays(4);
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                    default:
                        dateModif = dateConvert;
                        momentRepas = PeriodeRepasEnum.MIDI;
                        break;

                }

                //enregistreent en base ou supression de la base
                var momentToSearch = new DateExceptionRepas
                {
                    Date = dateModif,
                    IsPresent = dataSplit[1] == "1",
                    PeriodeRepas = momentRepas,
                    UtilisateurId = GetIdUtilisateurConnecte()
                };

                if (
                    Db.DateExceptionRepas.Count(
                        x =>
                            x.Date == momentToSearch.Date && x.PeriodeRepas == momentToSearch.PeriodeRepas &&
                            x.UtilisateurId == momentToSearch.UtilisateurId) > 0)
                {
                    var toRemove = Db.DateExceptionRepas.First(
                        x =>
                            x.Date == momentToSearch.Date && x.PeriodeRepas == momentToSearch.PeriodeRepas &&
                            x.UtilisateurId == momentToSearch.UtilisateurId);
                    Db.DateExceptionRepas.Remove(toRemove);
                    Db.SaveChanges();
                }
                else
                {
                    Db.DateExceptionRepas.Add(momentToSearch);
                    Db.SaveChanges();
                }
            }
        }
        #endregion


        #region Listing

        /// <summary>
        /// Génère les stats de présence d'un repas du personnel sur une semaine
        /// </summary>
        /// <param name="viewModel">le viewModel à modifié</param>
        /// <returns>le viewModel modifié</returns>
        private RepasViewModel CompteurRepas(RepasViewModel viewModel)
        {
            viewModel.NbRepasLundiMidi = viewModel.Listing.Count(x => x.IsLundiMidi);
            viewModel.NbRepasLundiSoir = viewModel.Listing.Count(x => x.IsLundiSoir);
            viewModel.NbRepasMardiMidi = viewModel.Listing.Count(x => x.IsMardiMidi);
            viewModel.NbRepasMardiSoir = viewModel.Listing.Count(x => x.IsMardiSoir);
            viewModel.NbRepasMercrediMidi = viewModel.Listing.Count(x => x.IsMercrediMidi);
            viewModel.NbRepasMercrediSoir = viewModel.Listing.Count(x => x.IsMercrediSoir);
            viewModel.NbRepasJeudiMidi = viewModel.Listing.Count(x => x.IsJeudiMidi);
            viewModel.NbRepasJeudiSoir = viewModel.Listing.Count(x => x.IsJeudiSoir);
            viewModel.NbRepasVendrediMidi = viewModel.Listing.Count(x => x.IsVendrediMidi);
            return viewModel;
        }

        /// <summary>
        /// Génère la page du listing des repas d'une semaine
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ListingRepas()
        {
            var listeUtil = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).OrderBy(x=> x.Nom).ThenBy(x=> x.Prenom).ToArray();
            var liste = new List<RepasViewModel>();
            foreach (var user in listeUtil)
            {
                var viewModel = GenerateViewModelDefaut(DateUtils.GetMaintenant());
                viewModel.User = user;
                viewModel = GenerateRepasSemaine(viewModel, user.DateExceptionRepas.Where(x=>x.Date <= viewModel.DateFin && x.Date>=viewModel.DateDeb).ToList());
                liste.Add(viewModel);
            }
            var viewModelToSend = GenerateViewModelDefaut(DateUtils.GetMaintenant());
            viewModelToSend.Listing = liste;
            viewModelToSend = CompteurRepas(viewModelToSend);
            return View(viewModelToSend);
        }

        /// <summary>
        /// Charge le lisnting des repas de la semaine précédente
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ListingRepas"), SubmitButtonSelector(Name = "PreviousWeek")]
        [AllowAnonymous]
        public ActionResult PreviousWeekListing()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);
            date = date.AddDays(-7);

            var listeUtil = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).OrderBy(x => x.Nom).ThenBy(x => x.Prenom).ToArray();
            var liste = new List<RepasViewModel>();
            foreach (var user in listeUtil)
            {
                var viewModel = GenerateViewModelDefaut(date);
                viewModel.User = user;
                viewModel = GenerateRepasSemaine(viewModel, user.DateExceptionRepas.Where(x => x.Date <= viewModel.DateFin && x.Date >= viewModel.DateDeb).ToList());
                liste.Add(viewModel);
            }
            var viewModelToSend = GenerateViewModelDefaut(date);
            viewModelToSend.Listing = liste;
            viewModelToSend = CompteurRepas(viewModelToSend);
            return View(viewModelToSend);
        }

        /// <summary>
        /// Charge le listing des repas de la semaine suivante
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ListingRepas"), SubmitButtonSelector(Name = "NextWeek")]
        [AllowAnonymous]
        public ActionResult NextWeekListing()
        {
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);
            date = date.AddDays(7);

            var listeUtil = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).OrderBy(x => x.Nom).ThenBy(x => x.Prenom).ToArray();
            var liste = new List<RepasViewModel>();
            foreach (var user in listeUtil)
            {
                var viewModel = GenerateViewModelDefaut(date);
                viewModel.User = user;
                viewModel = GenerateRepasSemaine(viewModel, user.DateExceptionRepas.Where(x => x.Date <= viewModel.DateFin && x.Date >= viewModel.DateDeb).ToList());
                liste.Add(viewModel);
            }
            var viewModelToSend = GenerateViewModelDefaut(date);
            viewModelToSend.Listing = liste;
            viewModelToSend = CompteurRepas(viewModelToSend);
            return View(viewModelToSend);
        }

        /// <summary>
        /// Télécharge en format docx le listing des repas d'une semaine
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ListingRepas"), SubmitButtonSelector(Name = "DownloadDoc")]
        [AllowAnonymous]
        public ActionResult DownloadDoc()
        {
            //récupération des infos
            var date = DateUtils.StringEnDate(Request.Form["inputDate"]);
            var listeUtil = Db.Utilisateurs.Where(x => x.Etat == EtatEnum.ACTIF).OrderBy(x => x.Nom).ThenBy(x => x.Prenom).ToArray();
            var liste = new List<RepasViewModel>();
            foreach (var user in listeUtil)
            {
                var viewModel = GenerateViewModelDefaut(date);
                viewModel.User = user;
                viewModel = GenerateRepasSemaine(viewModel, user.DateExceptionRepas.Where(x => x.Date <= viewModel.DateFin && x.Date >= viewModel.DateDeb).ToList());
                liste.Add(viewModel);
            }
            var viewModelToSend = GenerateViewModelDefaut(date);
            viewModelToSend.Listing = liste;
            viewModelToSend = CompteurRepas(viewModelToSend);

            //mise dans le document
            var mem = new MemoryStream();
            var document = DocX.Create(mem);
            var t = document.AddTable(viewModelToSend.Listing.Count+2, 10);
            var par = document.InsertParagraph();
            par.Append("Du "+viewModelToSend.DateDeb.Day+" "+viewModelToSend.MoisAnneeDateDeb+" au "+ viewModelToSend.DateFin.Day + " " + viewModelToSend.MoisAnneeDateFin).Font(new FontFamily("Times New Roman")).FontSize(16).Bold();

            t.Alignment = Alignment.center;
            t.Design = TableDesign.MediumGrid2Accent1;

            t.Rows[0].Cells[0].Paragraphs.First().Append("Personnel");
            t.Rows[0].Cells[0].Width = 150;
            t.Rows[0].Cells[1].Paragraphs.First().Append("Lundi midi");
            t.Rows[0].Cells[1].Width = 100;
            t.Rows[0].Cells[2].Paragraphs.First().Append("Lundi soir");
            t.Rows[0].Cells[2].Width = 100;
            t.Rows[0].Cells[3].Paragraphs.First().Append("Mardi midi");
            t.Rows[0].Cells[3].Width = 100;
            t.Rows[0].Cells[4].Paragraphs.First().Append("Mardi soir");
            t.Rows[0].Cells[4].Width = 100;
            t.Rows[0].Cells[5].Paragraphs.First().Append("Mercredi midi");
            t.Rows[0].Cells[5].Width = 100;
            t.Rows[0].Cells[6].Paragraphs.First().Append("Mercredi soir");
            t.Rows[0].Cells[6].Width = 100;
            t.Rows[0].Cells[7].Paragraphs.First().Append("Jeudi midi");
            t.Rows[0].Cells[7].Width = 100;
            t.Rows[0].Cells[8].Paragraphs.First().Append("Jeudi soir");
            t.Rows[0].Cells[8].Width = 100;
            t.Rows[0].Cells[9].Paragraphs.First().Append("Vendredi midi");
            t.Rows[0].Cells[9].Width = 100;
            var i = 1;
            foreach (var repasViewModel in viewModelToSend.Listing)
            {
                t.Rows[i].Cells[0].Paragraphs.First().Append(repasViewModel.User.Nom+" "+ repasViewModel.User.Prenom);
                t.Rows[i].Cells[0].Width = 150;
                t.Rows[i].Cells[1].Paragraphs.First().Append(repasViewModel.IsLundiMidi ? "X" : " ");
                t.Rows[i].Cells[1].Width = 100;
                t.Rows[i].Cells[2].Paragraphs.First().Append(repasViewModel.IsLundiSoir ? "X" : " ");
                t.Rows[i].Cells[2].Width = 100;
                t.Rows[i].Cells[3].Paragraphs.First().Append(repasViewModel.IsMardiMidi ? "X" : " ");
                t.Rows[i].Cells[3].Width = 100;
                t.Rows[i].Cells[4].Paragraphs.First().Append(repasViewModel.IsMardiSoir ? "X" : " ");
                t.Rows[i].Cells[4].Width = 100;
                t.Rows[i].Cells[5].Paragraphs.First().Append(repasViewModel.IsMercrediMidi ? "X" : " ");
                t.Rows[i].Cells[5].Width = 100;
                t.Rows[i].Cells[6].Paragraphs.First().Append(repasViewModel.IsMercrediSoir ? "X" : " ");
                t.Rows[i].Cells[6].Width = 100;
                t.Rows[i].Cells[7].Paragraphs.First().Append(repasViewModel.IsJeudiMidi ? "X" : " ");
                t.Rows[i].Cells[7].Width = 100;
                t.Rows[i].Cells[8].Paragraphs.First().Append(repasViewModel.IsJeudiSoir ? "X" : " ");
                t.Rows[i].Cells[8].Width = 100;
                t.Rows[i].Cells[9].Paragraphs.First().Append(repasViewModel.IsVendrediMidi ? "X" : " ");
                t.Rows[i].Cells[9].Width = 100;
                i++;
            }
            t.Rows[i].Cells[0].Paragraphs.First().Append("Compteur");
            t.Rows[i].Cells[0].Width = 150;
            t.Rows[i].Cells[1].Paragraphs.First().Append(viewModelToSend.NbRepasLundiMidi.ToString());
            t.Rows[i].Cells[1].Width = 100;
            t.Rows[i].Cells[2].Paragraphs.First().Append(viewModelToSend.NbRepasLundiSoir.ToString());
            t.Rows[i].Cells[2].Width = 100;
            t.Rows[i].Cells[3].Paragraphs.First().Append(viewModelToSend.NbRepasMardiMidi.ToString());
            t.Rows[i].Cells[3].Width = 100;
            t.Rows[i].Cells[4].Paragraphs.First().Append(viewModelToSend.NbRepasMardiSoir.ToString());
            t.Rows[i].Cells[4].Width = 100;
            t.Rows[i].Cells[5].Paragraphs.First().Append(viewModelToSend.NbRepasMercrediMidi.ToString());
            t.Rows[i].Cells[5].Width = 100;
            t.Rows[i].Cells[6].Paragraphs.First().Append(viewModelToSend.NbRepasMercrediSoir.ToString());
            t.Rows[i].Cells[6].Width = 100;
            t.Rows[i].Cells[7].Paragraphs.First().Append(viewModelToSend.NbRepasJeudiMidi.ToString());
            t.Rows[i].Cells[7].Width = 100;
            t.Rows[i].Cells[8].Paragraphs.First().Append(viewModelToSend.NbRepasJeudiSoir.ToString());
            t.Rows[i].Cells[8].Width = 100;
            t.Rows[i].Cells[9].Paragraphs.First().Append(viewModelToSend.NbRepasVendrediMidi.ToString());
            t.Rows[i].Cells[9].Width = 100;
            
            document.InsertTable(t);

            document.Save();

            return File(mem.ToArray(), "Application/octet-stream", "ListingRepas.docx");

        }

        #endregion
    }
}