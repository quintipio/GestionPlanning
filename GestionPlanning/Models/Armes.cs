using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des armes du personnel (armée de terre, marine....)
    /// </summary>
    [Table("armes")]
    public class Armes
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// le nom de l'arme
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Nom")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// l'tat de l'arme
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Etat")]
        [Column("Etat", TypeName = "INT")]
        public EtatEnum Etat { get; set; }

        /// <summary>
        /// est ce que l'arme prend en compte les rtt
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Sensible aux RTT")]
        [Column("IsRtt", TypeName = "BIT")]
        public bool IsRtt { get; set; }

        /// <summary>
        /// est ce que l'arme prend en compte les pcp
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Sensible aux PCP")]
        [Column("IsPcp", TypeName = "BIT")]
        public bool IsPcp { get; set; }
        
        /// <summary>
        /// la liste des grades associé à cette arme
        /// </summary>
        public virtual ICollection<Grades> Grades { get; set; }
    }
}