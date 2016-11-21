using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using GestionPlanning.Abstract;
using GestionPlanning.Context;
using GestionPlanning.Models;
using GestionPlanning.Utils;

namespace GestionPlanning.Controllers
{
    /// <summary>
    /// Controleur des paramètres appli
    /// </summary>
    [Authorize(Roles = "ADMINISTRATEUR")]
    public class ApplicationsController : AbstractControl
    {
        /// <summary>
        /// Charge le apge de modification des paramètres de l'appli
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            var application = Db.Application.Find(1);
            if (application == null)
            {
                return HttpNotFound();
            }

            application.IsSUperAdmin = GetIdUtilisateurConnecte() == 0;
            return View(application);
        }
        
        /// <summary>
        /// modifie les paramètres de l'appli et recharge le contexte appli
        /// </summary>
        /// <param name="application">les nouveaux paramètres</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit"), SubmitButtonSelector(Name = "SubmitApp")]
        public ActionResult Edit(Application application)
        {
            if (ModelState.IsValid)
            {
                //insertion des nouvelles valeurs en base
                var app = Db.Application.Find(1);
                app.AdresseMailEnvoyeur = application.AdresseMailEnvoyeur;
                app.AdresseSmtp = application.AdresseSmtp;
                app.EnableSsl = application.EnableSsl;
                app.MotDePasseMailEnvoyeur = application.MotDePasseMailEnvoyeur;
                app.PortSmtp = application.PortSmtp;
                //control si super admin
                if (GetIdUtilisateurConnecte() == 0)
                {
                    app.NomUnite = application.NomUnite;
                    app.NomAppli = application.NomAppli;
                }
                Db.Entry(app).State = EntityState.Modified;
                Db.SaveChanges();
                //rechargement du contexte
                ContexteApplication.RefreshContexteAppli();
            }

            application.IsSUperAdmin = GetIdUtilisateurConnecte() == 0;
            return View(application);
        }

        /// <summary>
        /// Charge un fichier de permission pour militaire en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit"), SubmitButtonSelector(Name = "SubmitFileMili")]
        public ActionResult UploadFileMilitaire()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (!ContexteStatic.ListeExtensionUploadAutorise.Contains(StringUtils.GetExtension(file.FileName)))
                {
                    ModelState.AddModelError("fileMili", "Ce type de fichier n'est pas autorisé");
                    return View(Db.Application.Find(1));
                }

                using (var inputStream = file.InputStream)
                {
                    var memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }

                    var app = Db.Application.Find(1);
                    app.FilePermMilitaire = memoryStream.ToArray();
                    Db.Entry(app).State = EntityState.Modified;
                    Db.SaveChanges();
                    ;
                    ContexteApplication.RefreshContexteAppli();
                }
            }
            var application = Db.Application.Find(1);
            application.IsSUperAdmin = GetIdUtilisateurConnecte() == 0;
            ModelState.AddModelError("fileMili", "Fichier chargé !");
            return View(application);

        }

        /// <summary>
        /// Charge un fichier de permission pour civil en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit"), SubmitButtonSelector(Name = "SubmitFileCivil")]
        public ActionResult UploadFileCivil()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (!ContexteStatic.ListeExtensionUploadAutorise.Contains(StringUtils.GetExtension(file.FileName)))
                {
                    ModelState.AddModelError("fileCivil", "Ce type de fichier n'est pas autorisé");
                    return View(Db.Application.Find(1));
                }

                using (var inputStream = file.InputStream)
                {
                    var memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }

                    var app = Db.Application.Find(1);
                    app.FilePermCivil = memoryStream.ToArray();
                    Db.Entry(app).State = EntityState.Modified;
                    Db.SaveChanges();
                    ContexteApplication.RefreshContexteAppli();
                }
            }

            var application = Db.Application.Find(1);
            application.IsSUperAdmin = GetIdUtilisateurConnecte() == 0;
            ModelState.AddModelError("fileCivil", "Fichier chargé !");
            return View(application);
        }


        #region Logs
        /// <summary>
        /// Charge la page des logs de l'appli
        /// </summary>
        /// <returns></returns>
        public ActionResult Logs()
        {
            var dateMin = DateUtils.GetAujourdhui().AddDays(-30);
            return View(Db.Logs.Where(x=>x.Date >= dateMin).OrderByDescending(x => x.Date).ToList());
        }

        /// <summary>
        /// Efface les logs en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Logs"), SubmitButtonSelector(Name = "DeleteLogs")]
        public ActionResult DeleteLogs()
        {
            var listeLog = Db.Logs.ToList();
            Db.Logs.RemoveRange(listeLog);
            Db.SaveChanges();
            return View(Db.Logs.ToList());
        }

        /// <summary>
        /// Génère un fichier contenant les logs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Logs"), SubmitButtonSelector(Name = "DownloadLogs")]
        public ActionResult DownloadLogs()
        {
            var listeLog = Db.Logs.ToList();
            var chaine = listeLog.Aggregate("", (current, log) => current + (DateUtils.GetDateFormat(log.Date, "yyyy-MM-dd-HH:mm:ss") + " --- " + log.Type + " --- " + log.Texte + "\r\n"));
            byte[] byteArray = Encoding.UTF8.GetBytes(chaine);
            var mem = new MemoryStream(byteArray);
            return File(mem.ToArray(), "Application/octet-stream", "logs.txt");
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
