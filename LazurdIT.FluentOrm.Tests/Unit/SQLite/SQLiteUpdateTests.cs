using System.Data.SQLite;
using FluentAssertions;
using LazurdIT.FluentOrm.Tests.TestResources;
using LazurdIT.FluentOrm.Tests.Unit.SQLite.Base;
using LazurdIT.FluentOrm.Tests.Utils;

namespace LazurdIT.FluentOrm.Tests.Unit.SQLite;

public class SQLiteUpdateTests : SQLiteTestBase
{
    [Fact(DisplayName = "Test Update record")]
    public void TestUpdateStudentModel()
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
        resultRecord!.Name.Should().Be(originalStudent.Name, $"it should contain the name of {originalStudent.Name}, but the result was {resultRecord!.Name}");
        resultRecord!.Age.Should().Be(originalStudent.Age, $"Age should be '{originalStudent.Age}' but the received is '{resultRecord?.Age}'");

        var updatedStudent = new StudentModel
        {
            Id = resultRecord!.Id,
            Name = "Updated Name",
            Age = (originalStudent.Age ?? 0) + 100
        };

        var updateQuery = repository.Update()
            .WithFields(f => f.Exclude(f => f.Id))
            .Where(m => m.Eq(f => f.Id, updatedStudent.Id));

        var updatedRecord = updateQuery.Execute(updatedStudent, connection);

        var selectQuery = repository.Select().Where(m => m.Eq(f => f.Id, updatedStudent.Id));
        var selectedRecord = selectQuery.Execute(connection).ToList()[0];
        selectedRecord.Should().NotBeNull("The record should not be null");
        selectedRecord!.Id.Should().Be(updatedStudent.Id, $"The record should Have the Id: {updatedStudent.Id} but the received is: {selectedRecord.Id}");
        selectedRecord!.Name.Should().Be(updatedStudent.Name, $"it should contain the name of {updatedStudent.Name}, but the result was {selectedRecord!.Name}");
        selectedRecord!.Age.Should().Be(updatedStudent.Age, $"Age should be '{updatedStudent.Age}' but the received is '{selectedRecord?.Age}'");

        connection.Close();
    }
}