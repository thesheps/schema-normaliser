using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using Dapper;
using Data;
using FluentMigrator;

namespace Migrations
{
    [Migration(3)]
    public class NormaliseData : Migration
    {
        public override void Up()
        {
            Create.Table("FormType")
                .WithColumn("FormTypeId").AsInt32().PrimaryKey().Identity()
                .WithColumn("Description").AsString();

            Create.Table("FieldType")
                .WithColumn("FieldTypeId").AsInt32().PrimaryKey().Identity()
                .WithColumn("FormTypeId").AsInt32().ForeignKey("FormType", "FormTypeId")
                .WithColumn("Description").AsString();

            Create.Table("Form")
                .WithColumn("FormId").AsInt32().PrimaryKey().Identity()
                .WithColumn("FormTypeId").AsInt32().ForeignKey("FormType", "FormTypeId");

            Create.Table("Field")
                .WithColumn("FieldId").AsInt32().PrimaryKey().Identity()
                .WithColumn("FieldTypeId").AsInt32().ForeignKey("FieldType", "FieldTypeId")
                .WithColumn("FormId").AsInt32().ForeignKey("Form", "FormId")
                .WithColumn("Value").AsString();

            Execute.WithConnection(Normalise);
        }

        public override void Down()
        {
            Delete.Table("Field");
            Delete.Table("Form");
            Delete.Table("FieldType");
            Delete.Table("FormType");
        }

        private static void Normalise(IDbConnection cnn, IDbTransaction transaction)
        {
            cnn.Execute("INSERT INTO FormType (Description) VALUES ('ChangeRequest')", null, transaction);
            cnn.Execute(@"INSERT INTO FieldType (FormTypeId, Description) VALUES (1, 'Alpha')", null, transaction);
            cnn.Execute(@"INSERT INTO FieldType (FormTypeId, Description) VALUES (1, 'Bravo')", null, transaction);
            cnn.Execute(@"INSERT INTO FieldType (FormTypeId, Description) VALUES (1, 'Charlie')", null, transaction);
            cnn.Execute(@"INSERT INTO FieldType (FormTypeId, Description) VALUES (1, 'Delta')", null, transaction);

            var rows = cnn.Query<TestData>("SELECT * FROM Test", null, transaction);

            foreach (var row in rows)
            {
                cnn.Execute(@"INSERT INTO Form (FormTypeId) VALUES (1)", null, transaction);
                var formId = cnn.Query<int>("SELECT CAST(@@IDENTITY AS INT)", null, transaction).First();

                cnn.Execute(@"INSERT INTO Field (FieldTypeId, FormId, Value) VALUES (1, @formId, @value)", new { formId, value = row.Alpha }, transaction);
                cnn.Execute(@"INSERT INTO Field (FieldTypeId, FormId, Value) VALUES (2, @formId, @value)", new { formId, value = row.Bravo }, transaction);
                cnn.Execute(@"INSERT INTO Field (FieldTypeId, FormId, Value) VALUES (3, @formId, @value)", new { formId, value = row.Charlie }, transaction);
                cnn.Execute(@"INSERT INTO Field (FieldTypeId, FormId, Value) VALUES (4, @formId, @value)", new { formId, value = row.Delta }, transaction);
            }

            var results = cnn.Query(@"SELECT * FROM Form fo
                                      INNER JOIN Field fi ON fo.FormId = fi.FormId
                                      INNER JOIN FieldType ft ON ft.FieldTypeId = fi.FieldTypeId
                                      WHERE fo.FormId = 1", null, transaction).ToDictionary(
                row => row.Description,
                row => row.Value);
        }
    }
}