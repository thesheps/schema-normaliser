using FluentMigrator;

namespace Migrations
{
    [Migration(2)]
    public class InsertData : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Test").Row(new { Alpha = "Test1", Bravo = "Test1", Charlie = "Test1", Delta = "Test1" });
            Insert.IntoTable("Test").Row(new { Alpha = "Test2", Bravo = "Test2", Charlie = "Test2", Delta = "Test2" });
            Insert.IntoTable("Test").Row(new { Alpha = "Test3", Bravo = "Test3", Charlie = "Test3", Delta = "Test3" });
            Insert.IntoTable("Test").Row(new { Alpha = "Test4", Bravo = "Test4", Charlie = "Test4", Delta = "Test4" });
            Insert.IntoTable("Test").Row(new { Alpha = "Test5", Bravo = "Test5", Charlie = "Test5", Delta = "Test5" });
        }

        public override void Down()
        {
        }
    }
}