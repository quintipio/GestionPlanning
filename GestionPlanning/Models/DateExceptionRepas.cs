using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// model pour enregistrer toute date de repas de personnel sortant de la normale
    /// </summary>
    [Table("dateexceptionrepas")]
    public class DateExceptionRepas
    {
        /// <summary>
        /// l'id de l'utilisateur concerné
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("UtilisateurId", TypeName = "INT")]
        public int UtilisateurId { get; set; }

        /// <summary>
        /// l'objet utilisateur concerné
        /// </summary>
        [ForeignKey("UtilisateurId")]
        [DisplayName("Personne")]
        public virtual Utilisateurs Utilisateur { get; set; }

        /// <summary>
        /// la date d'enregistrement de la particularité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date")]
        [Column("Date", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// le repas à modifé
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Période")]
        [Column("Periode", TypeName = "INT")]
        public PeriodeRepasEnum PeriodeRepas { get; set; }

        /// <summary>
        /// la présence ou non à ce repas
        /// </summary>
        [DisplayName("Présence")]
        [Column("IsPresent", TypeName = "BIT")]
        public bool IsPresent { get; set; }
    }
}