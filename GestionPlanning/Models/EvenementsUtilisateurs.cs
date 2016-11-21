using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model pour faire le entre un évènement et ses invités (avec leurs réponses)
    /// </summary>
    [Table("evenementsutilisateurs")]
    public class EvenementsUtilisateurs
    {
        /// <summary>
        /// l'identifiant de l'utilisateur
        /// </summary>
        public int UtilisateursId { get; set; }

        /// <summary>
        /// l'utilisateur
        /// </summary>
        [ForeignKey("UtilisateursId")]
        public Utilisateurs Utilisateurs { get; set; }
        
        /// <summary>
        /// l'id de l'évènement de l'invité
        /// </summary>
        public int EvenementsId { get; set; }

        /// <summary>
        /// l'évènement de l'invité
        /// </summary>
        [ForeignKey("EvenementsId")]
        public Evenements Evenements { get; set; }

        /// <summary>
        /// le commentaire de l'invité
        /// </summary>
        [DataType(DataType.MultilineText)]
        [DisplayName("Commentaire")]
        [Column("Commentaire", TypeName = "VARCHAR")]
        [MaxLength(500, ErrorMessage = "Ce champ pdoit faire au maximum 500 caractères")]
        public string Commentaire { get; set; }
        
        /// <summary>
        /// la présence ou non de l'invité
        /// </summary>
        [DisplayName("Est présent")]
        [Column("IsPresent", TypeName = "BIT")]
        public bool? IsPresent { get; set; }

        /// <summary>
        /// la date sélectionné par l'utilisateur
        /// </summary>
        [DisplayName("Date sélectionnée")]
        [Column("DateSelected", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateSelected { get; set; }

        /// <summary>
        /// la demi journée sélectionné par l'utilisateur
        /// </summary>
        [DisplayName("Demi journée sélectionné")]
        [Column("DemiJourneeSelected", TypeName = "INT")]
        public DemiJourneeEnum? DemiJourneeSelected { get; set; }

    }
}