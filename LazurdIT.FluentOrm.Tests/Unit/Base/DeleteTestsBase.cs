using System.Data.Common;
using FluentAssertions;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Tests.TestResources.Models;
using LazurdIT.FluentOrm.Tests.Utils;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;

namespace LazurdIT.FluentOrm.Tests.Unit.Base;

public abstract class DeleteTestsBase<TBase, TConnection, TRepository> where TBase : ITestBase<TConnection, TRepository> where TConnection : DbConnection, new() where TRepository : IFluentRepository<StudentModel>
{
    public abstract TBase TestBase { get; }

    [Fact(DisplayName = "Test delete record")]
    public void TestDeleteStudentModel()
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
        var deleteQuery = repository.Delete()
            .Where(c => c.Eq(f => f.Id, resultRecord!.Id));

        var count = deleteQuery.Execute(connection);

        count.Should().Be(1, $"Students deleted should be 1 , got {count}");

        connection.Close();
    }
}