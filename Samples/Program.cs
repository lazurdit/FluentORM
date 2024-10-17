using System.Data.SqlClient;
using Samples.Utils;
using Samples.DAL.Models;
using Samples.Samples.Pgsql;
using Samples.DAL.Repositories.MsSql;
using Samples.Samples;
using Samples.DAL.Repositories.PgSql;
using MySqlConnector;
using Samples.DAL.Repositories.MySql;
using Samples.Migrations;
using FluentMigrator.Runner;
using Samples.DAL.Repositories.Oracle;
using Oracle.ManagedDataAccess.Client;

namespace Samples;

internal class Program
{
    private const string MsSqlConnectionString = "Data Source=.;Initial Catalog=TopCorp_DB;Integrated Security=false;UID=sa;PWD=P@ssw0rd;TrustServerCertificate=True;";

    private const string PgsqlConnectionString = "Host=localhost:5432;Username=postgres;Password=P@ssw0rd;Database=TopCorp_DB";
    private const string MySqlConnectionString = "Host=localhost;Port=3306;Username=root;Password=P@ssw0rd;Database=TopCorp_DB";

    private const string OracleConnectionString =
    @"User Id=system;Password=oracle;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(COMMUNITY=tcp.world)(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=XE)(SID=SYSTEM)));";

    //private const string OracleConnectionString =
    //@"User Id=system;Password=oracle;Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS =(COMMUNITY = tcp.world)(PROTOCOL = TCP)(Host = localhost)(Port = 1521)))(CONNECT_DATA = (SERVICE_NAME=XE) ));";

    private static void Up()
    {
        ImplementDB.Up(MsSqlConnectionString, mb => mb.AddSqlServer());
        ImplementDB.Up(PgsqlConnectionString, mb => mb.AddPostgres());
        ImplementDB.Up(MySqlConnectionString, mb => mb.AddMySql8());
        ImplementDB.Up(OracleConnectionString, mb => mb.AddOracle12CManaged());
    }

    private static void Down()
    {
        //ImplementDB.Down(MsSqlConnectionString, mb => mb.AddSqlServer());
        //ImplementDB.Down(PgsqlConnectionString, mb => mb.AddPostgres());
        ImplementDB.Down(MySqlConnectionString, mb => mb.AddMySql8());
    }

    private static void Main(string[] _)
    {
        //Up();
        //Down();
        //return;

        //TestStudents();

        var (mssqlStudent, pgsqlStudent, mySqlStudent, oracleStudent) = TestAdd2();

        return;

        //mySqlStudent.mySqlStudent = pgsqlStudent.pgsqlStudent;
        //mySqlStudent.mySqlStudent!.Id = mySqlStudent.mysqlId ?? throw new Exception("cannot get new MySql StudentID");
        oracleStudent = pgsqlStudent;
        oracleStudent!.Id = 7;

        mssqlStudent!.Name += " Updated";
        pgsqlStudent!.Name += " Updated";
        mySqlStudent!.Name += " Updated";
        oracleStudent!.Name += " Updated";
        TestUpdate2(mssqlStudent, pgsqlStudent, mySqlStudent!, oracleStudent);
        Console.WriteLine("Press return to delete records");
        Console.ReadLine();
        TestDelete2(mssqlStudent, pgsqlStudent, mySqlStudent!, oracleStudent);
        //TestDelete2(mssqlStudent.mssqlStudent, pgsqlStudent.pgsqlStudent, mySqlStudent.mySqlStudent!);
    }

    private static (StudentRecord? mssqlStudent, StudentRecord? pgsqlStudent, StudentRecord? mySqlStudent, StudentRecord? oracleStudent) TestAdd2()
    {
        var student = new StudentRecord().Random();
        Console.WriteLine(student);
        Console.WriteLine();

        Console.WriteLine("Generating student in MsSql");
        var sqlRepository = new MsSqlStudentRecordRepository();
        var sqlConnection = new SqlConnection(MsSqlConnectionString);
        var mssqlStudent = WriteTester.TestAddStudent(sqlRepository, student, sqlConnection);
        Console.WriteLine($"Done: new Identity: {mssqlStudent}");
        Console.WriteLine();

        Console.WriteLine("Generating student in PgSql");
        var pgsqlRepository = new PgsqlStudentRecordRepository();
        var pgConnection = new Npgsql.NpgsqlConnection(PgsqlConnectionString);
        var pgsqlStudent = WriteTester.TestAddStudent(pgsqlRepository, student, pgConnection);
        Console.WriteLine($"Done: new Identity: {pgsqlStudent}");
        Console.WriteLine();

        Console.WriteLine("Generating student in MySql");
        var mySqlRepository = new MySqlStudentRecordRepository();
        var mySqlConnection = new MySqlConnection(MySqlConnectionString);
        var mySqlStudent = WriteTester.TestAddStudent(mySqlRepository, student, mySqlConnection);
        Console.WriteLine($"Done: new Identity: {mySqlStudent}");
        Console.WriteLine();

        Console.WriteLine("Generating student in Oracle");
        var oracleRepository = new OracleStudentRecordRepository();
        var oracleConnection = new OracleConnection(OracleConnectionString);
        var oracleStudent = WriteTester.TestAddStudent(oracleRepository, student, oracleConnection);
        Console.WriteLine($"Done: new Identity: {oracleStudent}");
        Console.WriteLine();

        return (mssqlStudent, pgsqlStudent, oracleStudent, oracleStudent);
    }

    private static void TestUpdate2(StudentRecord mssqlStudent, StudentRecord pgsqlStudent, StudentRecord mySqlStudent, StudentRecord oracleStudent)
    {
        //Console.WriteLine(mssqlStudent);
        //Console.WriteLine();
        //Console.WriteLine("Updating student in MsSql");
        //var sqlRepository = new MsSqlStudentRecordRepository();
        //var sqlConnection = new SqlConnection(MsSqlConnectionString);
        //int? count = WriteTester.TestUpdateStudent(sqlRepository, mssqlStudent, sqlConnection);
        //Console.WriteLine($"Done: count updated: {count}");
        //Console.WriteLine();

        //Console.WriteLine(pgsqlStudent);
        //Console.WriteLine("Updating student in PgSql");
        //var pgsqlRepository = new PgsqlStudentRecordRepository();
        //var pgConnection = new Npgsql.NpgsqlConnection(PgsqlConnectionString);
        //count = WriteTester.TestUpdateStudent(pgsqlRepository, pgsqlStudent, pgConnection);
        //Console.WriteLine($"Done: count updated: {count}");
        //Console.WriteLine();

        //Console.WriteLine(mySqlStudent);
        //Console.WriteLine("Updating student in MySql");
        //var mySqlRepository = new MySqlStudentRecordRepository();
        //var mySqlConnection = new MySqlConnection(MySqlConnectionString);
        //count = WriteTester.TestUpdateStudent(mySqlRepository, mySqlStudent, mySqlConnection);
        //Console.WriteLine($"Done: count updated: {count}");
        //Console.WriteLine();

        Console.WriteLine(oracleStudent);
        Console.WriteLine("Updating student in Oracle");
        var oracleRepository = new OracleStudentRecordRepository();
        var oracleConnection = new OracleConnection(OracleConnectionString);
        int? count = WriteTester.TestUpdateStudent(oracleRepository, oracleStudent, oracleConnection);
        Console.WriteLine($"Done: count updated: {count}");
        Console.WriteLine();
    }

    private static void TestDelete2(StudentRecord mssqlStudent, StudentRecord pgsqlStudent, StudentRecord mySqlStudent, StudentRecord oracleStudent)
    {
        Console.WriteLine();

        //Console.WriteLine("Deleting student in MsSql");
        //var sqlRepository = new MsSqlStudentRecordRepository();
        //var sqlConnection = new SqlConnection(MsSqlConnectionString);
        //int? count = WriteTester.TestDeleteStudent(sqlRepository, mssqlStudent.Id, sqlConnection);
        //Console.WriteLine($"Done: count deleted: {count}");
        //Console.WriteLine();

        //Console.WriteLine("Deleting student in PgSql");
        //var pgsqlRepository = new PgsqlStudentRecordRepository();
        //var pgConnection = new Npgsql.NpgsqlConnection(PgsqlConnectionString);
        //count = WriteTester.TestDeleteStudent(pgsqlRepository, pgsqlStudent.Id, pgConnection);
        //Console.WriteLine($"Done: count deleted: {count}");
        //Console.WriteLine();

        //Console.WriteLine("Deleting student in MySql");
        //var mySqlRepository = new MySqlStudentRecordRepository();
        //var mySqlConnection = new MySqlConnection(MySqlConnectionString);
        //count = WriteTester.TestDeleteStudent(mySqlRepository, mySqlStudent.Id, mySqlConnection);
        //Console.WriteLine($"Done: count updated: {count}");
        //Console.WriteLine();

        Console.WriteLine("Deleting student in Oracle");
        var oracleRepository = new OracleStudentRecordRepository();
        var oracleConnection = new OracleConnection(OracleConnectionString);
        int? count = WriteTester.TestDeleteStudent(oracleRepository, (long)oracleStudent.Id, oracleConnection);
        Console.WriteLine($"Done: count updated: {count}");
        Console.WriteLine();
    }

    private static void TestGenerate()
    {
        var recordsCount = 100;
        GenerateTester generateTester = new(MsSqlConnectionString);
        PgsqlGenerateData generateData = new(PgsqlConnectionString);
        generateTester.TV2Single1(recordsCount);
        Console.WriteLine();
        generateData.InsertIndiviualRecords(recordsCount);
        Console.WriteLine();
        generateTester.TV2Bulk(recordsCount);
        Console.WriteLine();
    }

    private static void TestStudents()
    {
        SqlConnection connection = new(MsSqlConnectionString);
        connection.Open();
        SqlConnection connector = new(MsSqlConnectionString);

        connector.Open();
        MsSqlClassRecordRepository classRecordCollection = new();

        //var x = classRecordCollection.IsUsedBy(nameof(StudentRecord), cm => cm.Eq(f => f.Id, 1), connection);

        long id = 5;
        bool found = false;
        found = classRecordCollection.IsUsedByAnyOutRelation(cm => cm.Eq(f => f.Id, id), connection);

        Console.WriteLine($"{nameof(StudentRecord)} with value {id} is{(found ? "" : " not")} used and can{(found ? "not" : "")} be deleted");

        id = 1;
        found = classRecordCollection.IsUsedByAnyOutRelation(cm => cm.Eq(f => f.Id, id), connection);

        Console.WriteLine($"{nameof(StudentRecord)} with value {id} is{(found ? "" : " not")} used and can{(found ? "not" : "")} be deleted");

        id = 5;
        found = classRecordCollection.IsUsedByRelation(nameof(StudentRecord), cm => cm.Eq(f => f.Id, id), connection);

        Console.WriteLine($"{nameof(StudentRecord)} with value {id} is{(found ? "" : " not")} used in students and can{(found ? "not" : "")} be deleted");

        id = 4;
        found = classRecordCollection.IsUsedByAnyOutRelation(cm => cm.Eq(f => f.Id, id), connection);

        Console.WriteLine($"{nameof(StudentRecord)} with value {id} is{(found ? "" : " not")} used and can{(found ? "not" : "")} be deleted");

        MsSqlStudentRecordRepository studentRecordCollection = new();

        var updateQuery1 = studentRecordCollection.Update()
               .WithFields(fm => fm.ExcludeAll().FromFieldExpression(f => f.ClassId, "1"));
        var count1 = updateQuery1.Execute(connection);
        Console.WriteLine($"Updated All To Class 1 : {count1}");

        var updateQuery = studentRecordCollection.Update()
      .WithFields(fm => fm.FromField(f => f.Name)
                        .FromFieldExpression(f => f.ClassId, "iif(%  >1 , 1 , %)")

      //   .FromField(f => f.ClassId)
      ).Where(wm => wm.Eq(f => f.Id, 1));

        Console.WriteLine("pre update");
        var count = updateQuery.Execute(new StudentRecord()
        {
            Id = 1,
            ClassId = 1,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "John Doe 33"
        }, connection);
        Console.WriteLine($"{count} record(s) updated");

        //for (int i = 0; i < result.count; i++)
        //{
        //    var student = result.Item2(i);
        //    Console.WriteLine("pre update");
        //    var result = updateQuery.Execute(new()
        //    {
        //        Id = 1,
        //        ClassId = 1,
        //        CreatedAt = System.DateTime.Now,
        //        UpdatedAt = System.DateTime.Now,
        //        Name = "John Doe 2"
        //    }, connection, false, true);
        //    Console.WriteLine("post update");
        //}

        /**/

        ReadAllTester.TV2Page(connector, 25, 10);

        ReadAllTester.TV2AggregatePage(connector, 25, 10);

        ReadAllTester.TV2(connector);
    }
}