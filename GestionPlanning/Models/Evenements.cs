using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des évènements à enregistrer en base (sortie cohésion, resto....)
    /// </summary>
    [Table("evenements")]
    public class Evenements
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le titre de l'évènement
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Titre")]
        [Column("Titre", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string Titre { get; set; }

        /// <summary>
        /// la première date de l'évènement
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date (obligatoire)")]
        [Column("Date", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// la demi journée où aura lieu cet évènement
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Demi journée")]
        [Column("DemiJournee", TypeName = "INT")]
        public DemiJourneeEnum DemiJournee { get; set; }

        /// <summary>
        /// la dexuième date de l'évènement
        /// </summary>
        [DisplayName("Deuxième date (optionnelle)")]
        [Column("DateB", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateB { get; set; }

        /// <summary>
        /// la demi journée de la deuxième date
        /// </summary>
        [DisplayName("Demi journée")]
        [Column("DemiJourneeB", TypeName = "INT")]
        public DemiJourneeEnum? DemiJourneeB { get; set; }

        /// <summary>
        /// la trosième date de l'évènement
        /// </summary>
        [DisplayName("Troisième date (optionnelle) ")]
        [Column("DateC", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateC { get; set; }

        /// <summary>
        /// la demi journée de la troisième date
        /// </summary>
        [DisplayName("Demi journée")]
        [Column("DemiJourneeC", TypeName = "INT")]
        public DemiJourneeEnum? DemiJourneeC { get; set; }

        /// <summary>
        /// la date limite de réponse de l'évènement
        /// </summary>
        [DisplayName("Date limite de réponse ")]
        [Column("DateVerrou", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateVerrou { get; set; }

        
        /// <summary>
        /// un commentaire éventuel de l'organisateur
        /// </summary>
        [DataType(DataType.MultilineText)]
        [DisplayName("Commentaire")]
        [Column("Commentaire", TypeName = "VARCHAR")]
        [MaxLength(500, ErrorMessage = "Ce champ pdoit faire au maximum 500 caractères")]
        public string Commentaire { get; set; }

        /// <summary>
        /// l'id du créateur de l'évènement
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("CreateurId", TypeName = "INT")]
        public int CreateurId { get; set; }

        /// <summary>
        /// l'objet du créateur
        /// </summary>
        [ForeignKey("CreateurId")]
        [DisplayName("Créateur")]
        public virtual Utilisateurs Createur { get; set; }
        
        /// <summary>
        /// la liste des utilisateurs associé à cet évènement
        /// </summary>
        public virtual ICollection<EvenementsUtilisateurs> EvenementsUtilisateurs { get; set; }


        ////////POUR LA CONSULTATION D'UN EVENEMENT
        /// <summary>
        /// Pour la consultation d'un utilisateur, indique le numéro de la date sélectionné 1= Date, 2=DateB, 3=DateC
        /// </summary>
        [NotMapped]
        public int SelectedDate { get; set; }

        /// <summary>
        /// Pour la consultation d'un utilisateur, indique si cet utilisateur qui le consulte est présent
        /// </summary>
        [NotMapped]
        public bool? IsUtilisateurPresent { get; set; }

        /// <summary>
        /// Pour la consultation d'un utilisateur, indique le commentaire de l'utilisateur qui consulte
        /// </summary>

        [NotMapped]
        [MaxLength(500)]
        public string CommentaireUtilisateur { get; set; }

    }
}