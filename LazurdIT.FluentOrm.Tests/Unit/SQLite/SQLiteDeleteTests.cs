using System.Data.SQLite;
using FluentAssertions;
using LazurdIT.FluentOrm.Tests.TestResources;
using LazurdIT.FluentOrm.Tests.Unit.SQLite.Base;
using LazurdIT.FluentOrm.Tests.Utils;

namespace LazurdIT.FluentOrm.Tests.Unit.SQLite;

public class SQLiteDeleteTests : SQLiteTestBase
{
    [Fact(DisplayName = "Test delete record")]
    public void TestDeleteStudentModel()
    {
        ToDoBefore();
        StudentSQLiteRepository repository = new();
        string tempDBFile = Path.GetTempFileName();

        using SQLiteConnection connection = new(connectionString);
        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));

        var originalStudent = SQLiteUtils.DefaultStudentsList[0];
        var resultRecord = insertQuery.Execute(originalStudent, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");
        resultRecord!.Name.Should().Be(originalStudent.Name, $"it should contain the name of {originalStudent.Name}, but the result was {resultRecord.Name}");
        resultRecord!.Age.Should().Be(originalStudent.Age, $"Age should be '{originalStudent.Age}' but the received is '{resultRecord?.Age}'");

        var deleteQuery = repository.Delete()
            .Where(c => c.Eq(f => f.Id, resultRecord!.Id));

        var count = deleteQuery.Execute(connection);

        count.Should().Be(1, "Students deleted should be 1 ");

        connection.Close();
    }
}