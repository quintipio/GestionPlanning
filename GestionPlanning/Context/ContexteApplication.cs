using GestionPlanning.Models;
using GestionPlanning.Utils;

namespace GestionPlanning.Context
{
    /// <summary>
    /// OBjet d'une durée de vie de l'Application
    /// </summary>
    public static class ContexteApplication
    {
        /// <summary>
        /// les données de l'Application en base (doit être rafraichi à chaque modification de la page Edit du controlleur Application, ou au chargement de l'appli)
        /// </summary>
        public static Application Application { get; private set; }

        /// <summary>
        /// Accès pour enregistrer les logs (chargé au démarrage de l'appli)
        /// </summary>
        public static LogUtils Log { get; set; }

        /// <summary>
        /// Rafraichi en mémoire toute les données du contexte
        /// </summary>
        public static void RefreshContexteAppli()
        {
            var db = new ContexteDb();
            Application = db.Application.Find(1);
        }
    }
}