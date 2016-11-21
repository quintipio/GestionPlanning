using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionPlanning.Enum;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model de l'utilisateur
    /// </summary>
    [Table("utilisateurs")]
    public class Utilisateurs
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// l'adresse mail de l'utilisateur
        /// </summary>
        [Required(ErrorMessage ="Cechamp est requis")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Ce champ doit être une adresse mail")]
        [DisplayName("Adresse mail")]
        [Index("IX_EmailUser", 1, IsUnique = true)]
        [Column("Email", TypeName = "VARCHAR")]
        [MaxLength(100,ErrorMessage = "Ce champ doit faire au maximum 100 caractères")]
        public string Email { get; set; }

        /// <summary>
        /// le hash SHA256 de l'utilisateur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Password)]
        [DisplayName("Mot de passe")]
        [Column("MotDePasse", TypeName = "VARCHAR")]
        [MinLength(8, ErrorMessage = "Ce champ doit faire au minimum 8 caractères")]
        [MaxLength(500, ErrorMessage = "Ce champ doit faire au maximum 500 caractères")]
        public string MotDePasse { get; set; }

        /// <summary>
        /// Le nom
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Nom")]
        [Column("Nom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ doit faire au maximum 50 caractères")]
        public string Nom { get; set; }

        /// <summary>
        /// le prénom
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("Prénom")]
        [Column("Prenom", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ doit faire au maximum 50 caractères")]
        public string Prenom { get; set; }

        /// <summary>
        /// le matricule
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.Text)]
        [DisplayName("NID / Alliance")]
        [Column("Nid", TypeName = "VARCHAR")]
        [MaxLength(30, ErrorMessage = "Ce champ doit faire au maximum 30 caractères")]
        public string Nid { get; set; }

        /// <summary>
        /// le numéro de téléphone
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Ce champ doit contenir un numéro de téléphone valide")]
        [DisplayName("PNIA")]
        [Column("Pnia", TypeName = "VARCHAR")]
        [MaxLength(10, ErrorMessage = "Ce champ doit faire au maximum 10 caractères")]
        public string Pnia { get; set; }

        /// <summary>
        /// l'état de l'utilisateur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Etat")]
        [Column("Etat", TypeName = "INT")]
        public EtatEnum Etat { get; set; }
        
        /// <summary>
        /// son grade
        /// </summary>
        [ForeignKey("GradesId")]
        [DisplayName("Grade")]
        public virtual Grades Grades { get; set; }

        /// <summary>
        /// son grade
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [Column("GradesId", TypeName = "INT")]
        [DisplayName("Grade")]
        public int GradesId { get; set; }
        
        /// <summary>
        /// son role
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Role")]
        [Column("Role", TypeName = "INT")]
        public RoleEnum Role { get; set; }

        /// <summary>
        /// son pole
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Pole")]
        public int PolesId { get; set; }

        /// <summary>
        /// son pole
        /// </summary>
        [ForeignKey("PolesId")]
        [DisplayName("Pole")]
        public virtual Poles Poles { get; set; }

        /// <summary>
        /// Telephone
        /// </summary>
        [DataType(DataType.PhoneNumber)]
        [MaxLength(12, ErrorMessage = "Ce champ doit faire au maximum 12 caractères")]
        [Column("Telephone", TypeName = "VARCHAR")]
        public string Telephone { get; set; }

        /// <summary>
        /// champ utile pour la génération des feuilles de perm (adresse)
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "Ce champ doit faire au maximum 100 caractères")]
        [Column("Adresse", TypeName = "VARCHAR")]
        public string Adresse { get; set; }

        /// <summary>
        ///  champ utile pour la génération des feuilles de perm (code postal)
        /// </summary>
        [Column("CodePostal", TypeName = "INT")]
        public int CodePostal { get; set; }

        /// <summary>
        ///  champ utile pour la génération des feuilles de perm (la ville)
        /// </summary>
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "Ce champ doit faire au maximum 100 caractères")]
        [Column("Ville", TypeName = "VARCHAR")]
        public string Ville { get; set; }

        /// <summary>
        ///  champ utile pour la génération des feuilles de perm (le nombre de jours restant sur le solde)
        /// </summary>
        [Column("NbJoursPermsRestant", TypeName = "INT")]
        public int NbJoursPermsRestant { get; set; }


        /// <summary>
        /// les activités de cet utilisateur
        /// </summary>
        public virtual ICollection<Activites> Activites { get; set; }


        /// <summary>
        /// les invitations aux évènement de cet utilisateur
        /// </summary>
        public virtual ICollection<EvenementsUtilisateurs> EvenementsUtilisateurs { get; set; }

        /// <summary>
        /// les évènements cré par cet utilisateur
        /// </summary>
        public virtual ICollection<Evenements> EvenementsCreateur { get; set; }

        /// <summary>
        /// les exceptions de repas de cet utilisateur
        /// </summary>
        public virtual ICollection<DateExceptionRepas> DateExceptionRepas { get; set; }

        /// <summary>
        /// pour affichage uniquement (le choix des armes)
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Arme")]
        [NotMapped]
        public int ArmesId { get; set; }
    }
}