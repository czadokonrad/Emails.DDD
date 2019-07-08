using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Emails.DAL.Migrations;

namespace Emails.DAL.IntegrationTests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void SetUpDatabase()
        {
            DestroyLocalDatabse();
            CreateLocalDabase();
        }

        private static void CreateLocalDabase()
        {
            ExecuteSqlCommand(Master,
                $@"CREATE DATABASE [Emails2016] ON (NAME = 'Emails2016',
                   FILENAME = '{FileName}')");

            var migration = new MigrateDatabaseToLatestVersion
                <EmailsContext, Configuration>();



            migration.InitializeDatabase(new EmailsContext());
        }

        [OneTimeTearDown]
        public void TearDownDatabase()
        {
            DestroyLocalDatabse();
        }

        private static void DestroyLocalDatabse()
        {
            var fileNames = ExecuteSqlQuery(Master,
                            @"SELECT [physical_name] FROM [sys].[master_files]
                  WHERE [database_id] = DB_ID('Emails2016')",
                            row => (string)row["physical_name"]);

            if (!fileNames.Any()) return;

            ExecuteSqlCommand(Master,
                @"ALTER DATABASE [Emails2016] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                EXEC sp_detach_db 'Emails2016'");

            fileNames.ForEach(File.Delete);


        }

        private static void ExecuteSqlCommand(SqlConnectionStringBuilder connectionStringBuilder, string commandText)
        {
            using(var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<T> ExecuteSqlQuery<T>(SqlConnectionStringBuilder connectionStringBuilder, string queryText,
            Func<SqlDataReader, T> read)
        {
            var result = new List<T>();

            using(var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }

            return result;
        }


        private static SqlConnectionStringBuilder Master =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true
            };

        private static string FileName => Path.Combine(
            Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location),
            "Emails2016.mdf");
    }
}
