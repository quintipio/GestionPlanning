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
    /// Controleur de gesion des grades
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class GradesController : AbstractControl
    {
        /// <summary>
        /// Charge la page d'index des grades
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var grades = Db.Grades.Where(x=> x.Etat == EtatEnum.ACTIF).Include(g => g.Armes);
            return View(grades.ToList());
        }

        /// <summary>
        /// Charge la page des détails d'un grade
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grades = Db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            return View(grades);
        }

        /// <summary>
        /// Charge la page de création d'un grade
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.ArmesId = new SelectList(Db.Armes, "Id", "Nom");
            return View(new Grades {Etat = EtatEnum.ACTIF});
        }
        
        /// <summary>
        /// Créer un grade
        /// </summary>
        /// <param name="grades">le grade à créer</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Grades grades)
        {
            if (ModelState.IsValid)
            {
                grades.Etat = EtatEnum.ACTIF;
                Db.Grades.Add(grades);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArmesId = new SelectList(Db.Armes, "Id", "Nom", grades.ArmesId);
            return View(grades);
        }

        /// <summary>
        /// Charge la page de modification d'un grade
        /// </summary>
        /// <param name="id">l'id du grade à modifier</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grades = Db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArmesId = new SelectList(Db.Armes, "Id", "Nom", grades.ArmesId);
            return View(grades);
        }
        
        /// <summary>
        /// Modifie un grade
        /// </summary>
        /// <param name="grades">le grade modifié</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nom,Diminutif,ArmesId,Etat")] Grades grades)
        {
            if (ModelState.IsValid)
            {
                grades.Etat = EtatEnum.ACTIF;
                Db.Entry(grades).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArmesId = new SelectList(Db.Armes, "Id", "Nom", grades.ArmesId);
            return View(grades);
        }

        /// <summary>
        /// charge la page pour supprimer un grade
        /// </summary>
        /// <param name="id">l'id du grade à supprimer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grades = Db.Grades.Find(id);
            if (grades == null)
            {
                return HttpNotFound();
            }
            return View(grades);
        }

        /// <summary>
        /// Périme un grade
        /// </summary>
        /// <param name="id">l'id du grade à périmer</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var grades = Db.Grades.Find(id);
            grades.Etat = EtatEnum.PERIME;
            Db.Entry(grades).State = EntityState.Modified;
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
