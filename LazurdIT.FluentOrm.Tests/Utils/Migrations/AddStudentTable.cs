using FluentMigrator;
using FluentMigrator.Oracle;

namespace LazurdIT.FluentOrm.Tests.Utils.Migrations;

[Migration(202410272256)]
public class AddStudentTable : Migration
{
    public override void Up()
    {
        OracleExtensions.Identity(IfDatabase("Oracle12cManaged")
           .Create.Table("Students")
           .WithColumn("StudentId").AsInt64(), OracleGenerationType.Always, 1, 1).PrimaryKey()
            .WithColumn("StudentName").AsString(255).NotNullable().Unique()
              .WithColumn("StudentAge").AsInt64().Nullable();

        IfDatabase(t => t != "Oracle12cManaged")
            .Create.Table("Students")
             .WithColumn("StudentId").AsInt64().PrimaryKey().Identity()
              .WithColumn("StudentName").AsString(255).NotNullable().Unique()
              .WithColumn("StudentAge").AsInt64().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Students");
    }
}