using System.Collections.Generic;
using System.Web.Mvc;
using GestionPlanning.Models;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien entre le controleur et la vue pour les évènements
    /// </summary>
    public class EvenementViewModel
    {
        /*Création  d'un évènement*/
        
        /// <summary>
        /// L'évènement crée
        /// </summary>
        public Evenements EvenementsCre { get; set; }
        
        /// <summary>
        /// la liste des utilisateurs sélectionable dasn la vue
        /// </summary>
        public List<Utilisateurs> UtilisateursItems { get; set; }
        
        /// <summary>
        /// la lsite des poles sélectionables dans la vue
        /// </summary>
        public List<Poles> PoleItems { get; set; }




        /*Mes évènements (index)*/
        public List<Evenements> EvenementsPersoCreer { get; set; }

        public List<Evenements> EvenementsParticipe { get; set; }
    }
}