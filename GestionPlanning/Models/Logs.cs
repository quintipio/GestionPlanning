using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model pour enregistrer les lgos en base
    /// </summary>
    [Table("logs")]
    public class Logs
    {
        /// <summary>
        /// l'idnetifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// la date du log
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Date")]
        [Column("Date", TypeName = "DATETIME")]
        [DataType(DataType.DateTime,ErrorMessage = "Ce champ doit être une date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd-HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// le niveua du log
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Type")]
        [Column("Type", TypeName = "INT")]
        public TypeLogEnum Type { get; set; }

        /// <summary>
        /// le texte à enregistrer
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Texte")]
        [Column("Texte", TypeName = "TEXT")]
        [DataType(DataType.MultilineText)]
        public string Texte { get; set; }
    }
}