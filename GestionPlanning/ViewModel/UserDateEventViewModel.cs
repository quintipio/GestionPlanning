using System;
using System.Collections.Generic;
using GestionPlanning.Models;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien entre la vue et le controleur pour l'affichage du planning
    /// </summary>
    public class UserDateEventViewModel
    {
        /// <summary>
        /// l'id de l'utilisateur en cours de consultation
        /// </summary>
        public int IdUserEnCours { get; set; }

        /// <summary>
        /// L'année en cours de consultation
        /// </summary>
        public string Annee { get; set; }

        /// <summary>
        /// le mois en cours de consultation
        /// </summary>
        public string Mois { get; set; }

        /// <summary>
        /// le premier jour du mois en cours de consultation
        /// </summary>
        public DateTime DateEnCours { get; set; }

        /// <summary>
        /// la liste des jours à afficher
        /// </summary>
        public List<JourViewModel> ListeJours { get; set; } 

        /// <summary>
        /// la liste des utilisateurs à afficher
        /// </summary>
        public List<Utilisateurs> ListeUtilisateurs { get; set; } 

        /// <summary>
        /// la liste des typs d'actvités à afficher
        /// </summary>
        public List<TypeActivites> ListeTypesActivites { get; set; } 
    }
}