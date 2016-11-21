using System.Data.Entity;
using GestionPlanning.Context;
using GestionPlanning.Enum;
using GestionPlanning.Models;

namespace GestionPlanning.App_Start
{
    /// <summary>
    /// Objet pour initialiser les données lors de la création de la base : CreateDatabaseIfNotExists créer la base de donnée si elle n'existe pas
    /// </summary>
    public class PlanningDbInitializer : CreateDatabaseIfNotExists<ContexteDb>
    {
        protected override void Seed(ContexteDb context)
        {
            //REFERENTIEL OBLIGATOIRE
            //Application
            context.Application.Add(new Application
            {
                AdresseMailEnvoyeur = "monmail@mail.fr",
                AdresseSmtp = "smtp-mail.mail.com",
                MotDePasseMailEnvoyeur = "MotDePasse",
                PortSmtp = 587,
                NomAppli = "OrgaP",
                NomUnite = "NomUnite",
                EnableSsl = true
            });

            //mails
            context.Mails.Add(new Mails
            {
                NomUnique = ContexteStatic.EnvoiInvitEvenement,
                Objet = "Invitation à un évènement",
                Corps =
                    "Vous avez été invité par le %NOM_CREATEUR% à l'évènement %TITRE_EVEN% sur %APPLI%, veuillez vous connectez <a href='%ADRESSE%'>ici</a> pour répondre à l'invitation avant le %DATELIM%."
            });
            context.Mails.Add(new Mails
            {
                NomUnique = ContexteStatic.EnvoiDesactivationEvenement,
                Objet = "Supression d'un évènement",
                Corps =
                    "L'évènement %NOMEVEN% à été désactivé, votre invitation à donc été annulé."
            });
            context.Mails.Add(new Mails
            {
                NomUnique = ContexteStatic.EnvoiCreationCompte,
                Objet = "Création de votre compte",
                Corps =
                    "Votre compte sur %NOMAPPLI% a été crée. Pour vous connectez veuillez utiliser ces identifiants : <br/> Login : %LOGIN% <br/> Mot de passe : %MOTDEPASSE%"
            });

            //couleurs obligatoires
            context.Couleurs.Add(new Couleurs {Nom = "Rouge", CodeCouleur = "B1221C" });
            context.Couleurs.Add(new Couleurs {Nom = "Orange", CodeCouleur = "FF5B2B" });
            context.Couleurs.Add(new Couleurs {Nom = "Violet", CodeCouleur = "853894" });
            context.Couleurs.Add(new Couleurs {Nom = "Vert foncé", CodeCouleur = "677E52" });
            context.Couleurs.Add(new Couleurs {Nom = "Vert clair", CodeCouleur = "B0CC99" });
            context.Couleurs.Add(new Couleurs {Nom = "Bleu clair", CodeCouleur = "5EB6DD" });
            context.Couleurs.Add(new Couleurs {Nom = "Bleu foncé", CodeCouleur = "046380" });
            context.Couleurs.Add(new Couleurs {Nom = "Rose", CodeCouleur = "F5BFE0" });
            context.Couleurs.Add(new Couleurs {Nom = "Gris", CodeCouleur = "CCC6AD" });
            context.Couleurs.Add(new Couleurs {Nom = "Jaune", CodeCouleur = "E8CC06" });
            context.Couleurs.Add(new Couleurs {Nom = "Marron", CodeCouleur = "52251C" });

            //activités obligatoires
            context.TypeActivites.Add(new TypeActivites { Nom = "Permissions", Etat = EtatEnum.ACTIF, CouleursId = 1,ModifierRepas = true});

            //armes obligatoires
            context.Armes.Add(new Armes { Nom = "Entreprise extérieure", Etat = EtatEnum.ACTIF, IsPcp = false, IsRtt = false });

            //grades obligatoire
            
            //exterieur
            context.Grades.Add(new Grades { Nom = "Monsieur", Diminutif = "M", ArmesId = 6, Etat = EtatEnum.ACTIF });
            context.Grades.Add(new Grades { Nom = "Madame", Diminutif = "Mme", ArmesId = 6, Etat = EtatEnum.ACTIF });

            base.Seed(context);
        }

    }
}