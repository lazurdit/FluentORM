using System.Data.Common;
using FluentAssertions;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Tests.TestResources.Models;
using LazurdIT.FluentOrm.Tests.Utils;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;

namespace LazurdIT.FluentOrm.Tests.Unit.Base;

public abstract class UpdateTestsBase<TBase, TConnection, TRepository> where TBase : ITestBase<TConnection, TRepository> where TConnection : DbConnection, new() where TRepository : IFluentRepository<StudentModel>
{
    public abstract TBase TestBase { get; }

    [Fact(DisplayName = "Test update record")]
    public void UpdateData()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);
        using var connection = TestBase.NewConnection(connectionString);

        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));

        var originalStudent = SampleData.DefaultStudentsList[0];
        var resultRecord = insertQuery.Execute(originalStudent, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();

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

        //Adds a fix to pgsql test
        connection.Close();
        connection.Open();

        var selectQuery = repository.Select(connection)
                            .Where(m => m.Eq(f => f.Id, updatedStudent.Id));
        var selectedRecord = selectQuery.Execute(connection).ToList()[0];
        selectedRecord.Should().NotBeNull("The record should not be null");
        selectedRecord!.Id.Should().Be(updatedStudent.Id, $"The record should Have the Id: {updatedStudent.Id} but the received is: {selectedRecord.Id}");
        selectedRecord!.Name.Should().Be(updatedStudent.Name, $"it should contain the name of {updatedStudent.Name}, but the result was {selectedRecord!.Name}");
        selectedRecord!.Age.Should().Be(updatedStudent.Age, $"Age should be '{updatedStudent.Age}' but the received is '{selectedRecord?.Age}'");

        connection.Close();
    }
}