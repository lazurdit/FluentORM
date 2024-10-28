using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Tests.TestResources.Models;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Tests.Utils.TestBase;

public interface ITestBase<TConnection, TRepository> where TConnection : DbConnection where TRepository : IFluentRepository<StudentModel>
{
    void ToDoBefore(string? connectionString);

    TConnection NewConnection(string? connectionString);

    TRepository NewStudentsRepository();

    string NewConnectionString();
}