using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Enum;
using GestionPlanning.Models;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur pour le gestion des poles
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class PolesController : AbstractControl
    {

        /// <summary>
        /// Charge la page d'index des poles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Db.Poles.Where(x => x.Etat == EtatEnum.ACTIF).ToList());
        }

        /// <summary>
        /// CHarge la page de détails d'un pole
        /// </summary>
        /// <param name="id">l'id du pole</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var poles = Db.Poles.Find(id);
            if (poles == null)
            {
                return HttpNotFound();
            }
            return View(poles);
        }

        /// <summary>
        /// Charge la page de création d'un pole
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var pole = new Poles
            {
                Etat = EtatEnum.ACTIF
            };
            return View(pole);
        }

        /// <summary>
        /// Créer un pole
        /// </summary>
        /// <param name="poles">le pole à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Poles poles)
        {
            if (ModelState.IsValid)
            {
                poles.Etat = EtatEnum.ACTIF;
                Db.Poles.Add(poles);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(poles);
        }

        /// <summary>
        /// Charge la page de modification d'un pole
        /// </summary>
        /// <param name="id">l'id du pole à modifier</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var poles = Db.Poles.Find(id);
            if (poles == null)
            {
                return HttpNotFound();
            }
            return View(poles);
        }

        /// <summary>
        /// Modifie un pole
        /// </summary>
        /// <param name="poles">le pole à modifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Poles poles)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(poles).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(poles);
        }

        /// <summary>
        /// Charge la page de supression d'un pole
        /// </summary>
        /// <param name="id">l'id du pole à supprimer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Poles poles = Db.Poles.Find(id);
            if (poles == null)
            {
                return HttpNotFound();
            }
            return View(poles);
        }

        /// <summary>
        /// Supprime un pole
        /// </summary>
        /// <param name="id">le pole à périmer</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var poles = Db.Poles.Find(id);
            poles.Etat = EtatEnum.PERIME;
            Db.Entry(poles).State = EntityState.Modified;
            Db.SaveChanges();
            return RedirectToAction("Index");
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
