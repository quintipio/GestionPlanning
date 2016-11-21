namespace GestionPlanning.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GestionPlanning.Context.ContexteDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            // register mysql code generator
            SetSqlGenerator("MySql.Data.MySqlClient",
                new MySql.Data.Entity.MySqlMigrationSqlGenerator());

            SetHistoryContextFactory("MySql.Data.MySqlClient",
            (conn, schema) => new MySqlHistoryContext(conn, schema));

        }

        protected override void Seed(GestionPlanning.Context.ContexteDb context)
        {
        }
    }
}
