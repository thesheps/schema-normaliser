using Migrations;
using NUnit.Framework;

namespace Tests
{
    public class MigrationTest
    {
        [Test]
        public void Test()
        {
            Runner.SqlCompact("Test").Down();
            Runner.SqlCompact("Test").Up();
        }
    }
}
