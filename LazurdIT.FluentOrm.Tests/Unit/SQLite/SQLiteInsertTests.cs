using System.Data.SQLite;
using FluentAssertions;
using LazurdIT.FluentOrm.Tests.TestResources;
using LazurdIT.FluentOrm.Tests.Unit.SQLite.Base;
using LazurdIT.FluentOrm.Tests.Utils;

namespace LazurdIT.FluentOrm.Tests.Unit.SQLite;

public class SQLiteInsertTests : SQLiteTestBase
{
    [Fact(DisplayName = "Test add new record")]
    public void TestInsertStudentModel()
    {
        ToDoBefore();
        StudentSQLiteRepository repository = new();

        using SQLiteConnection connection = new(connectionString);
        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        var student = SQLiteUtils.DefaultStudentsList[0];
        var resultRecord = insertQuery.Execute(student, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");
        resultRecord!.Name.Should().Be(student.Name, $"it should contain the name of {student.Name}, but the result was {resultRecord!.Name}");
        resultRecord!.Age.Should().Be(student.Age, $"Age should be '{student.Age}' but the received is '{resultRecord?.Age}'");

        connection.Close();
    }

    [Fact(DisplayName = "Test add with missing required values record")]
    public void TestInsertStudentModelWithMissingRequiredValues()
    {
        ToDoBefore();

        StudentSQLiteRepository repository = new();

        using SQLiteConnection connection = new(connectionString);
        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        var student = SQLiteUtils.DefaultStudentsList[0];
        student.Name = null!;

        Action act = () => insertQuery.Execute(student, true, connection);

        var exception = act.Should().Throw<SQLiteException>("This action should throw an \"SQLiteException\"")
            .WithMessage("constraint failed\r\nNOT NULL constraint failed: Students.StudentName");

        connection.Close();
    }

    [Fact(DisplayName = "Test add with duplicate unique values")]
    public void TestInsertInvalidStudentModel()
    {
        ToDoBefore();
        StudentSQLiteRepository repository = new();

        using SQLiteConnection connection = new(connectionString);
        connection.Open();

        var insertQuery = repository.Insert();
        var student = SQLiteUtils.DefaultStudentsList[0];

        insertQuery.Execute(student, true, connection);

        Action act = () => insertQuery.Execute(student, true, connection);

        act.Should().Throw<SQLiteException>()
            .WithMessage("constraint failed\r\nUNIQUE constraint failed: Students.StudentId");

        connection.Close();
    }

    [Fact(DisplayName = "Test add new record with null values")]
    public void TestInsertStudentModelWithNullValues()
    {
        ToDoBefore();

        StudentSQLiteRepository repository = new();
        string tempDBFile = Path.GetTempFileName();

        using SQLiteConnection connection = new(connectionString);
        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        var student = SQLiteUtils.DefaultStudentsList[0];
        student.Age = null;

        var resultRecord = insertQuery.Execute(student, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");
        resultRecord!.Name.Should().Be(student.Name, $"it should contain the name of {student.Name}, but the result was {resultRecord!.Name}");
        resultRecord!.Age.Should().Be(student.Age, $"Age should be '{student.Age}' but the received is '{resultRecord?.Age}'");

        connection.Close();
    }
}