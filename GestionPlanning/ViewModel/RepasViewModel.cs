using System;
using System.Collections.Generic;
using GestionPlanning.Models;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien entre la vue et le controleur pour la gestion des repas
    /// </summary>
    public class RepasViewModel
    {
        #region listing
        /// <summary>
        /// un utilisateur (correspond  à une ligne du tableau de listing)
        /// </summary>
        public Utilisateurs User { get; set; }

        /// <summary>
        /// une liste d'utilisateurs avec leurs repas associés
        /// </summary>
        public List<RepasViewModel> Listing  { get; set; }

        #endregion

        #region Compteur
        
        /// Pour le listing, indique pour chaque le repas le nombre de personnage maneant à ce repas

        public int NbRepasLundiMidi { get; set; }

        public int NbRepasLundiSoir { get; set; }

        public int NbRepasMardiMidi { get; set; }

        public int NbRepasMardiSoir { get; set; }
        public int NbRepasMercrediMidi { get; set; }

        public int NbRepasMercrediSoir { get; set; }
        public int NbRepasJeudiMidi { get; set; }

        public int NbRepasJeudiSoir { get; set; }

        public int NbRepasVendrediMidi { get; set; }
        
        #endregion

        #region gestion
        /// <summary>
        /// Date de début de semaine d'une page
        /// </summary>
        public DateTime DateDeb { get; set; }

        /// <summary>
        /// indique si les champs doivent être cliquable ou non
        /// </summary>
        public bool IsDisable { get; set; }

        /// <summary>
        /// mois et année en toute lettre de la date de début d'une page
        /// </summary>
        public string MoisAnneeDateDeb { get; set; }

        /// <summary>
        /// la date de fin de semaine des repas de la page
        /// </summary>
        public DateTime DateFin { get; set; }

        /// <summary>
        /// mois et année en toute lettre de la date de fin d'une page
        /// </summary>
        public string MoisAnneeDateFin { get; set; }
        
        /// Indique pour chaque repas la présence ou non

        public bool IsLundiMidi { get; set; }

        public bool IsLundiSoir { get; set; }

        public bool IsMardiMidi { get; set; }

        public bool IsMardiSoir { get; set; }

        public bool IsMercrediMidi { get; set; }

        public bool IsMercrediSoir { get; set; }

        public bool IsJeudiMidi { get; set; }

        public bool IsJeudiSoir { get; set; }

        public bool IsVendrediMidi { get; set; }

        #endregion
    }
}