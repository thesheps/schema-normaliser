﻿using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;

namespace Migrations
{
    public class Runner
    {
        public static Runner SqlServer(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            return new Runner(GetRunner(connectionString, new SqlServer2008ProcessorFactory()));
        }

        public static Runner SqlCompact(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            if (!File.Exists(new SqlCeConnectionStringBuilder(connectionString).DataSource))
                new SqlCeEngine(connectionString).CreateDatabase();

            return new Runner(GetRunner(connectionString, new SqlServerCeProcessorFactory()));
        }

        public void Up()
        {
            _runner.MigrateUp();
        }

        public void Down()
        {
            _runner.MigrateDown(0);
        }

        private Runner(MigrationRunner runner)
        {
            _runner = runner;
        }

        private static MigrationRunner GetRunner(string connectionString, IMigrationProcessorFactory factory)
        {
            var announcer = new NullAnnouncer();
            var assembly = Assembly.GetExecutingAssembly();
            var migrationContext = new RunnerContext(announcer) { Namespace = "Migrations" };
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var processor = factory.Create(connectionString, announcer, options);

            return new MigrationRunner(assembly, migrationContext, processor);
        }

        private class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public int Timeout { get; set; }
            public string ProviderSwitches => string.Empty;
        }

        private readonly MigrationRunner _runner;
    }
}