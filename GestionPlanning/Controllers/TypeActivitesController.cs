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
    /// Controleur de la gestion des types d'activités
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class TypeActivitesController : AbstractControl
    {

        /// <summary>
        /// charge la page d'index des types d'activités
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var typeActivites = Db.TypeActivites.Include(t => t.Couleurs);
            return View(typeActivites.Where(x=>x.Etat == EtatEnum.ACTIF).ToList());
        }

        /// <summary>
        /// Charge la page de détails d'un type d'activité
        /// </summary>
        /// <param name="id">l'id du type à afficher</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var typeActivites = Db.TypeActivites.Find(id);
            if (typeActivites == null)
            {
                return HttpNotFound();
            }
            return View(typeActivites);
        }

        /// <summary>
        /// Charge la page de création d'un type d'activité
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.CouleursId = new SelectList(Db.Couleurs, "Id", "Nom");
            return View(new TypeActivites {Etat = EtatEnum.ACTIF});
        }

        /// <summary>
        /// Créer un type d'activité
        /// </summary>
        /// <param name="typeActivites">le type d'activité à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TypeActivites typeActivites)
        {
            if (ModelState.IsValid)
            {
                typeActivites.Etat = EtatEnum.ACTIF;
                Db.TypeActivites.Add(typeActivites);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CouleursId = new SelectList(Db.Couleurs, "Id", "Nom", typeActivites.CouleursId);
            return View(typeActivites);
        }

        /// <summary>
        /// Charge la page de modification d'un tpye d'activité
        /// </summary>
        /// <param name="id">l'id du type d'activité à modifié</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var typeActivites = Db.TypeActivites.Find(id);
            if (typeActivites == null)
            {
                return HttpNotFound();
            }
            ViewBag.CouleursId = new SelectList(Db.Couleurs, "Id", "Nom", typeActivites.CouleursId);
            return View(typeActivites);
        }
        
        /// <summary>
        /// Modifie un type d'activité
        /// </summary>
        /// <param name="typeActivites">le type d'activité modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TypeActivites typeActivites)
        {
            if (ModelState.IsValid)
            {
                typeActivites.Etat = EtatEnum.ACTIF;
                Db.Entry(typeActivites).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CouleursId = new SelectList(Db.Couleurs, "Id", "Nom", typeActivites.CouleursId);
            return View(typeActivites);
        }

        /// <summary>
        /// Charge la page de suppresion d'un type d'activité
        /// </summary>
        /// <param name="id">l'id du type d'activité à supprimer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var typeActivites = Db.TypeActivites.Find(id);
            if (typeActivites == null)
            {
                return HttpNotFound();
            }
            return View(typeActivites);
        }

        /// <summary>
        /// périme un type d'activité
        /// </summary>
        /// <param name="id">l'id du type d'activité à périmer</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var typeActivites = Db.TypeActivites.Find(id);
            typeActivites.Etat = EtatEnum.PERIME;
            Db.Entry(typeActivites).State = EntityState.Modified;
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
