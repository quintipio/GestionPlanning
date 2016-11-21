using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Le model des couleurs disponible pour les différents type d'activité (non modifiable par l'utilisateur)
    /// </summary>
    [Table("couleurs")]
    public class Couleurs
    {
        /// <summary>
        /// l'identifant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///  le nom de la couleur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Nom de la couleur")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// le code couleur (6 caractères en hexa)
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Code de la couleur")]
        [Column("CodeCouleur", TypeName = "VARCHAR")]
        [MaxLength(6, ErrorMessage = "Ce champ pdoit faire au maximum 6 caractères")]
        [MinLength(6,ErrorMessage = "Ce champ doit faire au minimum 8 caractères")]
        public string CodeCouleur { get; set; }

        /// <summary>
        /// la liste des types d'activités associé à cette couleur
        /// </summary>
        public virtual ICollection<TypeActivites> TypeActivites { get; set; }
        
    }
}