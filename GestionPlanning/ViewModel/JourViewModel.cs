using System;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien netre la vue et lecontroleur pour le planning pour représenter un jour
    /// </summary>
    public class JourViewModel
    {
        /// <summary>
        /// La date du jour
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// la lettre de la semaine du jour
        /// </summary>
        public string LettreJour { get; set; }

        /// <summary>
        /// Indique si c'est un jour férié
        /// </summary>
        public bool IsFerie { get; set; }
        
        /// <summary>
        /// indique si c'est un RTT
        /// </summary>
        public bool IsRtt { get; set; }

        /// <summary>
        /// Indique si c'est un pcp
        /// </summary>
        public bool IsPcp { get; set; }

    }
}