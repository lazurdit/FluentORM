using FluentMigrator;
using FluentMigrator.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Migrations;

[Migration(20180430121800)]
public class AddClassTable : Migration
{
    public override void Up()
    {
        IfDatabase(ProcessorId.MySql8)
              .Create.Table("Tbl_Class")
              .WithColumn("Fld_Class_Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("Fld_Class_Name").AsString(255).NotNullable()
              .WithColumn("fld_class_created_at").AsDateTime().Nullable()
              .WithColumn("Fld_Class_Updated_At").AsDateTime().Nullable();

        OracleExtensions.Identity(IfDatabase("Oracle12cManaged")
              .Create.Table("Tbl_Class")
              .WithColumn("Fld_Class_Id").AsInt64(), OracleGenerationType.Always, 1, 1).PrimaryKey()
              .WithColumn("Fld_Class_Name").AsString(255).NotNullable()
              .WithColumn("fld_class_created_at").AsDateTime().Nullable()
              .WithColumn("Fld_Class_Updated_At").AsDateTime().Nullable();

        IfDatabase(t => t != ProcessorId.MySql8 && t != "Oracle12cManaged")
            .Create.Table("Tbl_Class")
            .WithColumn("Fld_Class_Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Fld_Class_Name").AsString(255).NotNullable()
            .WithColumn("fld_class_created_at").AsDateTimeOffset().Nullable()
            .WithColumn("Fld_Class_Updated_At").AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Tbl_Class");
    }
}

[Migration(20180430121801)]
public class AddStudentTable : Migration
{
    public override void Up()
    {
        IfDatabase(ProcessorId.MySql8)
              .Create.Table("Tbl_Student")
              .WithColumn("Fld_Student_Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("Fld_Class_Id").AsInt64().Nullable().ForeignKey("Tbl_Class", "Fld_Class_Id")
              .WithColumn("Fld_Student_Name").AsString(255).NotNullable()
              .WithColumn("fld_Student_created_at").AsDateTime().Nullable()
              .WithColumn("Fld_Student_Updated_At").AsDateTime().Nullable();

        IfDatabase(t => t != ProcessorId.MySql8)
           .Create.Table("Tbl_Student")
              .WithColumn("Fld_Student_Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("Fld_Class_Id").AsInt64().Nullable().ForeignKey("Tbl_Class", "Fld_Class_Id")
              .WithColumn("Fld_Student_Name").AsString(255).NotNullable()
              .WithColumn("fld_Student_created_at").AsDateTime().Nullable()
              .WithColumn("Fld_Student_Updated_At").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Tbl_Student");
    }
}

[Migration(20180430121802)]
public class AddInstructorTable : Migration
{
    public override void Up()
    {
        Create.Table("Tbl_Instructor")
          .WithColumn("Fld_Instructor_Id").AsInt64().PrimaryKey().Identity()
          .WithColumn("Fld_Class_Id").AsInt64().Nullable().ForeignKey("Tbl_Class", "Fld_Class_Id")
          .WithColumn("Fld_Instructor_Name").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Tbl_Instructor");
    }
}