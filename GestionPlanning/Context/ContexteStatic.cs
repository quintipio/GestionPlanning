using System.Collections.Generic;

namespace GestionPlanning.Context
{
    /// <summary>
    /// Classe static contenant les informations imuables de l'appli
    /// </summary>
    public static class ContexteStatic
    {
        /// <summary>
        /// Numéro de version en cours
        /// </summary>
        public const string Version = "0.8.0.0";

        /// <summary>
        /// Année minimale pour laquelle on peut mettre une permission
        /// </summary>
        public const int MinYearPermission = 2010;

        /// <summary>
        /// nom unique pour retrouver le mail d'envoi des invitations à un évènement
        /// </summary>
        public const string EnvoiInvitEvenement = "EnvoiInvitEvenement";

        /// <summary>
        ///  nom unique pour retrouver le mail d'envoi des désactivations d'un évènement
        /// </summary>
        public const string EnvoiDesactivationEvenement = "EnvoiDesactivationEvenement";

        /// <summary>
        ///  nom unique pour retrouver le mail d'envoi des créations de compte
        /// </summary>
        public const string EnvoiCreationCompte = "EnvoiCreationCompte";

        /// <summary>
        /// L'id des permissions dans type activités
        /// </summary>
        public const int IdActivitePermission = 1;

        /// <summary>
        /// la liste des extensions autorisé à l'upload des fichiers de perm
        /// </summary>
        public static readonly List<string> ListeExtensionUploadAutorise = new List<string> {"docx"};

        /// <summary>
        /// Le nom d'utilisateur du super admin
        /// </summary>
        public const string NomSuperAdmin = "SUPER";

        /// <summary>
        /// Le prénom du super admin
        /// </summary>
        public const string PrenomSuperAdmin = "Lama";

        /// <summary>
        /// le mail du superadmin(login)
        /// </summary>
        public const string MailSuperAdmin = "superlama@.lama.fr";

        /// <summary>
        /// le mot de passe du super Admin
        /// </summary>
        public const string MotDePassSuperAdmin = "Erjy.0:FKn,23é";
    }
}