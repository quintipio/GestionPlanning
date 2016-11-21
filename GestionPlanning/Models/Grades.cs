using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des grades des armes
    /// </summary>
    [Table("grades")]
    public class Grades
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le nom du grade
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Nom complet du grade")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// les lettre symbolisant le grade
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Diminutif du grade")]
        [Column("Diminutif", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string Diminutif { get; set; }

        /// <summary>
        /// l'id de l'arme d'appartenance du grade
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("ArmesId", TypeName = "INT")]
        [DisplayName("Arme d'appartenance :")]
        public int ArmesId { get; set; }

        /// <summary>
        /// l'arme d'appartenance du grade
        /// </summary>
        [ForeignKey("ArmesId")]
        [DisplayName("Arme d'appartenance :")]
        public Armes Armes { get; set; }

        /// <summary>
        /// l'état du grade 
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Etat")]
        [Column("Etat", TypeName = "INT")]
        public EtatEnum Etat { get; set; }

        /// <summary>
        /// la liste des utilisateurs associé à ce grade
        /// </summary>
        public virtual ICollection<Utilisateurs> Utilisateurs { get; set; }
    }
}