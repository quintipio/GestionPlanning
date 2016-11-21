using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestionPlanning.ViewModel
{
    /// <summary>
    /// Objet de lien entre la vue et le controleur pour le générateur de fichier de permission
    /// </summary>
    public class GenerateursPermsViewModel
    {
        /// <summary>
        /// L'id de l'activité dont le générateur doit généré la permission
        /// </summary>
        [Required]
        public int IdActivite { get; set; }

        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Téléphone")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(12, ErrorMessage = "Ce champ pdoit faire au maximum 12 caractères")]
        public string Telephone { get; set; }

        /// <summary>
        /// L'adresse de l'utilisateur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Adresse")]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string Adresse { get; set; }

        /// <summary>
        /// le code postal de la ville de l'utilisateur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Code postal")]
        [Range(00000, 99999, ErrorMessage = "Ce champ doit être un code postal")]
        public int CodePostal { get; set; }

        /// <summary>
        /// la ville de l'utilisateur
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Ville")]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "Ce champ pdoit faire au maximum 100 caractères")]
        public string Ville { get; set; }

        /// <summary>
        /// Nombre de jours total avant le décompte
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Nombre de jours de permissions restant")]
        public int NbJoursTotal { get; set; }

        /// <summary>
        /// Nombre de jours à décompte
        /// </summary>
        [Required(ErrorMessage = "Ce champ est requis")]
        [DisplayName("Nombre de jours de permissions décomptés")]
        public int NbJoursDemande { get; set; }

    }
}