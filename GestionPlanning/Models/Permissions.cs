using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// model des dates de permission ajouté par l'admin (rtt ou pcp)
    /// </summary>
    [Table("permissions")]
    public class Permissions
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// la date de la permission
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date ")]
        [Column("Date", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// le type de permission (rtt ou pcp)
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Type d'absence")]
        [Column("TypePerm", TypeName = "INT")]
        public TypePermEnum TypePerm { get; set; }

        /// <summary>
        /// l'année de cette permission (pour affichage)
        /// </summary>
        [NotMapped]
        public int Annee { get; set; }
    }
}