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
    /// Controleur pour la gestion des armes des utilisateurs
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class ArmesController : AbstractControl
    {
        /// <summary>
        /// Chage la page de la liste des armes
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Db.Armes.ToList());
        }

        /// <summary>
        /// Charge la page des détails d'une arme
        /// </summary>
        /// <param name="id">l'id d'une arme</param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var armes = Db.Armes.Find(id);
            if (armes == null)
            {
                return HttpNotFound();
            }
            return View(armes);
        }

        /// <summary>
        /// Charge la page de création d'une arme
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new Armes {Etat = EtatEnum.ACTIF});
        }
        
        /// <summary>
        /// Créer une nouelle arme
        /// </summary>
        /// <param name="armes">la nouvelle arme</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Armes armes)
        {
            if (ModelState.IsValid)
            {
                //sauvegarde en base
                armes.Etat = EtatEnum.ACTIF;
                Db.Armes.Add(armes);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(armes);
        }

        /// <summary>
        /// CHarge la page des édition d'une arme
        /// </summary>
        /// <param name="id">l'id de l'arme à modifier</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var armes = Db.Armes.Find(id);
            if (armes == null)
            {
                return HttpNotFound();
            }
            return View(armes);
        }
        
        /// <summary>
        /// modifie une arme
        /// </summary>
        /// <param name="armes">l'amre modifiée</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Armes armes)
        {
            if (ModelState.IsValid)
            {
                armes.Etat = EtatEnum.ACTIF;
                Db.Entry(armes).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(armes);
        }

        /// <summary>
        /// charge la page de suppression d'une arme
        /// </summary>
        /// <param name="id">l'id de l'arme à supprimer</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var armes = Db.Armes.Find(id);
            if (armes == null)
            {
                return HttpNotFound();
            }
            return View(armes);
        }

        /// <summary>
        /// Supprime une arme en base (la périme)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var armes = Db.Armes.Find(id);
            armes.Etat = EtatEnum.PERIME;
            Db.Entry(armes).State = EntityState.Modified;
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
