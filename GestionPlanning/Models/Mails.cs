using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model pour enregitré les modèles de mails en base
    /// </summary>
    [Table("mails")]
    public class Mails
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le nom associé à ce mail pour le retrouver
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("NomUnique")]
        [Column("NomUnique", TypeName = "VARCHAR")]
        [MaxLength(50,ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string NomUnique { get; set; }

        /// <summary>
        /// l'objet du mail
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Objet")]
        [Column("Objet", TypeName = "VARCHAR")]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string Objet { get; set; }

        /// <summary>
        /// le texte du mail
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Corps")]
        [Column("Corps", TypeName = "TEXT")]
        [MaxLength(10000, ErrorMessage = "Ce champ pdoit faire au maximum 10 000 caractères")]
        public string Corps { get; set; }
    }
}