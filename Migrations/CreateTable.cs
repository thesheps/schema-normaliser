using FluentMigrator;

namespace Migrations
{
    [Migration(1)]
    public class CreateTable : Migration
    {
        public override void Up()
        {
            Create.Table("Test")
                .WithColumn("Alpha").AsString()
                .WithColumn("Bravo").AsString()
                .WithColumn("Charlie").AsString()
                .WithColumn("Delta").AsString();
        }

        public override void Down()
        {
            Delete.Table("Test");
        }
    }
}