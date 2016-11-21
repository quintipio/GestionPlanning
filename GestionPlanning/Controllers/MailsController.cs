using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Models;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur pour la gestion des mails
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class MailsController : AbstractControl
    {
        /// <summary>
        /// Chage la page du listing des mails
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(Db.Mails.ToList());
        }
        
        /// <summary>
        /// charge la page de modification d'un mail
        /// </summary>
        /// <param name="id">l'id du mail à modifier</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mails = Db.Mails.Find(id);
            if (mails == null)
            {
                return HttpNotFound();
            }
            return View(mails);
        }
        
        /// <summary>
        /// modifie un mail
        /// </summary>
        /// <param name="mails">le mail à modifier</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mails mails)
        {
            if (ModelState.IsValid)
            {
                var mail = Db.Mails.Find(mails.Id);
                mail.Corps = mails.Corps;
                mail.Objet = mail.Objet;
                if (mail != null)
                {
                    Db.Entry(mail).State = EntityState.Modified;
                    Db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(mails);
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
