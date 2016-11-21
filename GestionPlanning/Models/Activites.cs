using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des activités des utilisateurs
    /// </summary>
    [Table("activites")]
    public class Activites
    {
        /// <summary>
        /// Identifiat unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// l'identifiant de l'utilisateur associé
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("UtilisateurId", TypeName = "INT")]
        public int UtilisateurId { get; set; }

        /// <summary>
        /// l'objet utilisateur associé
        /// </summary>
        [ForeignKey("UtilisateurId")]
        [DisplayName("Utilisateur")]
        public virtual Utilisateurs Utilisateurs { get; set; }


        /// <summary>
        /// l'identifiant du type d'activité de cette activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("TypeActiviteId", TypeName = "INT")]
        public int TypeActiviteId { get; set; }

        /// <summary>
        /// le type d'activité de cette activité
        /// </summary>
        [ForeignKey("TypeActiviteId")]
        [DisplayName("Type de l'activité")]
        public virtual TypeActivites TypeActivites { get; set; }

        /// <summary>
        /// une liste des types d'activités disponibles (pour la vue)
        /// </summary>
        [NotMapped]
        public virtual List<TypeActivites> ListeTypeActivites { get; set; }
        
        /// <summary>
        /// la date de début de l'activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date de début ")]
        [Column("DateDebut", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateDebut { get; set; }

        /// <summary>
        /// la demi journée d edébut de l'activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Demi journée")]
        [Column("DemiJourneeDebut", TypeName = "INT")]
        public DemiJourneeEnum DemiJourneeDebut { get; set; }

        /// <summary>
        /// la date de fin de l'activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date de fin ")]
        [Column("DateFin", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFin { get; set; }

        /// <summary>
        ///  la demi journée de fin de l'activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Demi journée")]
        [Column("DemiJourneeFin", TypeName = "INT")]
        public DemiJourneeEnum DemiJourneeFin { get; set; }


        /// <summary>
        /// le commentaire de l'utilisateur sur l'activité
        /// </summary>
        [DataType(DataType.Text)]
        [DisplayName("Commentaire ")]
        [Column("Commentaire", TypeName = "VARCHAR")]
        [MaxLength(500, ErrorMessage = "Ce champ pdoit faire au maximum 500 caractères")]
        public string Commentaire { get; set; }

    }
}