using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des types d'activités (permission, service....) seule permission ne peut être supprimé
    /// </summary>
    [Table("typeactivites")]
    public class TypeActivites
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le nom du type activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Libelle")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ doit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// est ce que cette activité, si elle est utilisé, le supprime du planning des repas
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Modifie le repas")]
        [Column("ModifierRepas", TypeName = "BIT")]
        public bool ModifierRepas { get; set; }

        /// <summary>
        /// l'état de ce type d'activité
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Etat")]
        [Column("Etat", TypeName = "INT")]
        public EtatEnum Etat { get; set; }

        /// <summary>
        /// la couleur associé
        /// </summary>
        [ForeignKey("CouleursId")]
        [DisplayName("Couleur d'affichage")]
        public virtual Couleurs Couleurs { get; set; }

        /// <summary>
        /// l'id de la couleur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("CouleursId", TypeName = "INT")]
        [DisplayName("Couleur d'affichage")]
        public int CouleursId { get; set; }

        /// <summary>
        /// les activités des utilisateurs associé à ce type
        /// </summary>
        public virtual ICollection<Activites> Activites { get; set; }
    }
}