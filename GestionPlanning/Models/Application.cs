using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPlanning.Models
{
    /// <summary>
    /// Model des données des paramètres de l'Application à sauvegarder en base
    /// </summary>
    [Table("application")]
    public class Application
    {
        /// <summary>
        /// l'identifiant unique
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// l'adresse mail d'expéditeur des mails que l'Application envoi
        /// </summary>
        [DataType(DataType.Text)]
        [DisplayName("Adresse mail du compte d'envoi")]
        [Column("AdresseMailEnvoyeur", TypeName = "VARCHAR")]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string AdresseMailEnvoyeur { get; set; }

        /// <summary>
        /// le mot de passe du compte mail de l'expéditeurde l'appli
        /// </summary>
        [DataType(DataType.Password)]
        [DisplayName("Mot de passe du compte d'envoi")]
        [Column("MotDePasseMailEnvoyeur", TypeName = "VARCHAR")]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string MotDePasseMailEnvoyeur { get; set; }

        /// <summary>
        /// l'adresse smtp du serveur de l'expéditeur des mails de l'appli
        /// </summary>
        [DataType(DataType.Text)]
        [DisplayName("Adresse SMTP")]
        [Column("AdresseSmtp", TypeName = "VARCHAR")]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string AdresseSmtp { get; set; }

        /// <summary>
        /// Nom de l'Application (variable modifiable uniquement par le super admin)
        /// </summary>
        [DataType(DataType.Text)]
        [DisplayName("Nom de l'Application")]
        [Column("NomAppli", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string NomAppli { get; set; }

        /// <summary>
        /// Nom de l'unité (variable modifiable uniquement par le super admin
        /// </summary>
        [DataType(DataType.Text)]
        [DisplayName("Nom de l'unité")]
        [Column("NomUnite", TypeName = "VARCHAR")]
        [MaxLength(50, ErrorMessage = "Ce champ pdoit faire au maximum 50 caractères")]
        public string NomUnite { get; set; }

        /// <summary>
        /// le port smtp du serveur de l'adresse mail de l'expéditeur
        /// </summary>
        [DisplayName("Port SMTP")]
        [Column("PortSmtp", TypeName = "INT")]
        public int PortSmtp { get; set; }

        /// <summary>
        /// indique si le ssl est actif sur le serveur de mail de l'expéditeur
        /// </summary>
        [DisplayName("Ssl actif ?")]
        [Column("EnableSsl", TypeName = "BIT")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// le fichier de permission qu'un militaire rempli
        /// </summary>
        [DisplayName("Fichier de permission militaire")]
        [Column("FilePermMilitaire", TypeName = "BLOB")]
        public byte[] FilePermMilitaire { get; set; }

        /// <summary>
        /// le fichier de permission qu'un civil rempli
        /// </summary>
        [DisplayName("Fichier de permission civil")]
        [Column("FilePermCivil", TypeName = "BLOB")]
        public byte[] FilePermCivil { get; set; }

        /// <summary>
        /// Variable pour la vue de modification de l'appli,  accès aux variables cachées
        /// </summary>
        [NotMapped]
        public bool IsSUperAdmin { get; set; }
    }
}