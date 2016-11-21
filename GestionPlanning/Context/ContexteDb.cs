using System.Data.Entity;
using GestionPlanning.App_Start;
using GestionPlanning.Models;

namespace GestionPlanning.Context
{
    /// <summary>
    /// Contexte pour le lien avec la base de donnée
    /// </summary>
    public class ContexteDb : DbContext
    {
        /// <summary>
        /// Constructeur (utilise le paramètre DevConnexion pour al connexion à la base, spécifié dans Web.config)
        /// </summary>
        public ContexteDb()  : base("name=DevConnection")
        {
            Database.SetInitializer(new PlanningDbInitializer());
        }
        
       ///
       /// Les objets en base
       ///
       
        public DbSet<Logs> Logs { get; set; }

        public DbSet<Application> Application { get; set; }

        public DbSet<Utilisateurs>  Utilisateurs { get; set; }
        
        public DbSet<Poles> Poles { get; set; }

        public DbSet<Mails> Mails { get; set; }

        public DbSet<Armes> Armes { get; set; }

        public DbSet<Grades> Grades { get; set; }

        public DbSet<TypeActivites> TypeActivites { get; set; }

        public DbSet<Activites> Activites { get; set; }

        public DbSet<Evenements> Evenements { get; set; }

        public DbSet<EvenementsUtilisateurs> EvenementsUtilisateurs { get; set; }

        public DbSet<DateExceptionRepas> DateExceptionRepas { get; set; }

        public DbSet<Permissions> Permissions { get; set; }

        public DbSet<Couleurs> Couleurs { get; set; }

        /// <summary>
        /// Construit les tables et les clés de la base de donnée si elle n'existe pas
        /// </summary>
        /// <param name="modelBuilder">le modeblBuilder</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //tables de base
            modelBuilder.Entity<Logs>();
            modelBuilder.Entity<Mails>();
            modelBuilder.Entity<Application>();
            modelBuilder.Entity<Couleurs>().HasMany(x => x.TypeActivites);
            modelBuilder.Entity<Poles>().HasMany(x => x.Utilisateurs);
            modelBuilder.Entity<Grades>().HasMany(x => x.Utilisateurs);
            modelBuilder.Entity<Armes>();
            modelBuilder.Entity<Permissions>();

            //type d'activités
            modelBuilder.Entity<TypeActivites>()
                .HasRequired(x =>x.Couleurs)
                .WithMany(x =>x.TypeActivites)
                .HasForeignKey(x=> x.CouleursId);

            //utilisateurs
            modelBuilder.Entity<Utilisateurs>()
               .HasRequired(x => x.Poles)
               .WithMany(x => x.Utilisateurs)
               .HasForeignKey(x => x.PolesId);
            modelBuilder.Entity<Utilisateurs>()
              .HasRequired(x => x.Grades)
              .WithMany(x => x.Utilisateurs)
              .HasForeignKey(x => x.GradesId);
            modelBuilder.Entity<Utilisateurs>().HasMany(x => x.EvenementsUtilisateurs); //test

            //activites
            modelBuilder.Entity<Activites>()
                .HasRequired(x => x.Utilisateurs)
                .WithMany(x => x.Activites)
                .HasForeignKey(x => x.UtilisateurId);
            modelBuilder.Entity<Activites>()
                .HasRequired(x => x.TypeActivites)
                .WithMany(x => x.Activites)
                .HasForeignKey(x => x.TypeActiviteId);

            //evenements
            modelBuilder.Entity<Evenements>()
                .HasRequired(x => x.Createur)
                .WithMany(x => x.EvenementsCreateur)
                .HasForeignKey(x => x.CreateurId);

            //evenementsUtilisateurs
            modelBuilder.Entity<EvenementsUtilisateurs>()
                .HasRequired(x => x.Utilisateurs)
                .WithMany(x => x.EvenementsUtilisateurs)
                .HasForeignKey(x => x.UtilisateursId).WillCascadeOnDelete(false); 
            modelBuilder.Entity<EvenementsUtilisateurs>()
                .HasRequired(x => x.Evenements)
                .WithMany(x => x.EvenementsUtilisateurs)
                .HasForeignKey(x => x.EvenementsId).WillCascadeOnDelete(false); 
            modelBuilder.Entity<EvenementsUtilisateurs>().HasKey(x => new {x.EvenementsId, x.UtilisateursId});
            
            //date Exception Repas
            modelBuilder.Entity<DateExceptionRepas>()
                .HasRequired(x => x.Utilisateur)
                .WithMany(x => x.DateExceptionRepas)
                .HasForeignKey(x => x.UtilisateurId);
            modelBuilder.Entity<DateExceptionRepas>().HasKey(x => new {x.UtilisateurId, x.Date, x.PeriodeRepas});

            //creation
            base.OnModelCreating(modelBuilder);
        }
    }
}
 