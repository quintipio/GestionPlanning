using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des poles du personnel
    /// </summary>
    [Table("poles")]
    public class Poles
    {
        /// <summary>
        /// l'identifant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le nom du pole
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Nom du pôle")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ doit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// l'tétat du pole
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Etat")]
        [Column("Etat", TypeName = "INT")]
        public EtatEnum Etat { get; set; }

        /// <summary>
        /// la lsite des utilisateurs du pole
        /// </summary>
        public virtual ICollection<Utilisateurs> Utilisateurs { get; set; }
    }
}