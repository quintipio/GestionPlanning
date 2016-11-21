using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;
using GestionPlanning.Utils;
using GestionPlanning.ViewModel;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des permissions personalisé par l'admin
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class PermissionsController : AbstractControl
    {

        #region Acceuil
       

        /// <summary>
        /// Charge la page de listing des permissions
        /// </summary>
        /// <param name="year">l'année des permissions à afficher</param>
        /// <returns></returns>
        public ActionResult Index(int? year)
        {
            if (year == null)
            {
                year = DateTime.Now.Year;
            }

            if ( year < ContexteStatic.MinYearPermission)
            {
                return RedirectToAction("Index", DateTime.Now.Year);
            }

            var dateMax = new DateTime(year.Value, 12, 31);
            var dateMin = new DateTime(year.Value, 01, 01);

            var viewModel = new PermissionsViewModel
            {
                Annee = year.Value,
                ListePermissions =
                    Db.Permissions.Where(
                        x =>
                            x.Date <= dateMax &&
                            x.Date >= dateMin).ToList()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Charge les permissions de l'année suivante
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index"), SubmitButtonSelector(Name = "NextYear")]
        [ValidateAntiForgeryToken]
        public ActionResult NextYear()
        {
            int annee;
            var viewModel = new PermissionsViewModel();
            if (int.TryParse(Request.Form["inputAnnee"], out annee) && annee >= ContexteStatic.MinYearPermission)
            {
                viewModel.Annee = ++annee;
            }
            else
            {
                viewModel.Annee = DateTime.Now.Year;
            }

            var dateMax = new DateTime(annee, 12, 31);
            var dateMin = new DateTime(annee, 01, 01);

            viewModel.ListePermissions =
                Db.Permissions.Where(
                    x =>
                        x.Date <= dateMax &&
                        x.Date >= dateMin).ToList();


            return View(viewModel);
        }

        /// <summary>
        /// Charge les permissions de l'année précédente
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Index"), SubmitButtonSelector(Name = "PreviousYear")]
        [ValidateAntiForgeryToken]
        public ActionResult PreviousYear()
        {
            int annee;
            var viewModel = new PermissionsViewModel();
            if (int.TryParse(Request.Form["inputAnnee"], out annee) && annee > ContexteStatic.MinYearPermission)
            {
                viewModel.Annee = --annee;
            }
            else
            {
                viewModel.Annee = DateTime.Now.Year;
            }

             var dateMax = new DateTime(annee, 12, 31);
            var dateMin = new DateTime(annee, 01, 01);

            viewModel.ListePermissions =
               Db.Permissions.Where(
                   x =>
                       x.Date <= dateMax &&
                       x.Date >= dateMin).ToList();

            return View(viewModel);
        }

        #endregion


        #region Création
        /// <summary>
        /// Charge la page pour créer une nouvelle date de perm
        /// </summary>
        /// <param name="year">l'année à laquelle appartiendra la permission</param>
        /// <returns></returns>
        public ActionResult Create(int? year)
        {
            if (year == null || year < ContexteStatic.MinYearPermission)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var perm = new Permissions
            {
                Annee = year.Value,
                TypePerm = TypePermEnum.PCP
               
            };
            return View(perm);
        }

        /// <summary>
        /// Créer une permission
        /// </summary>
        /// <param name="permission">la permission à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Permissions permission)
        {
            permission.Annee = (permission.Annee < ContexteStatic.MinYearPermission) ? DateTime.Now.Year : permission.Annee;
            permission.Date = new DateTime(permission.Annee, permission.Date.Month, permission.Date.Day);

            if (permission.TypePerm != TypePermEnum.PCP && permission.TypePerm != TypePermEnum.RTT)
            {
                ModelState.AddModelError("TypePerm", "Le type de permission n'est pas valide");
            }

            if (Db.Permissions.Count(x => x.TypePerm == permission.TypePerm && x.Date == permission.Date) > 0)
            {
                ModelState.AddModelError("Date", "Cette date est déjà présente");
            }

            if (ModelState.IsValid)
            {
                Db.Permissions.Add(permission);
                Db.SaveChanges();
                return RedirectToAction("Index", permission.Annee);
            }
            return View(permission);
        }
        #endregion


        #region Suppression

        /// <summary>
        /// Charge la page de supression d'une permission
        /// </summary>
        /// <param name="id">l'id de la permission à supprimer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var perms = Db.Permissions.Find(id);
            if (perms == null)
            {
                return HttpNotFound();
            }
            return View(perms);
        }

        /// <summary>
        /// Supprime une permission
        /// </summary>
        /// <param name="id">l'id de la permission à supprimer</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var perms = Db.Permissions.Find(id);
            Db.Permissions.Remove(perms);
             Db.SaveChanges();
            return RedirectToAction("Index",perms.Date.Year);
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