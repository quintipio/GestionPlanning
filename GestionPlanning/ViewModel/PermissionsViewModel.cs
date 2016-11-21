using System.Collections.Generic;
using GestionPlanning.Models;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien entre la vue et le controleur pour l'administration des permissions
    /// </summary>
    public class PermissionsViewModel
    {
        /// <summary>
        /// L'année en cours d'affichage
        /// </summary>
        public int Annee { get; set; }

        /// <summary>
        /// la liste des permissions (rtt et pcp) créer sur cette année)
        /// </summary>
        public List<Permissions> ListePermissions { get; set; } 
    }
}