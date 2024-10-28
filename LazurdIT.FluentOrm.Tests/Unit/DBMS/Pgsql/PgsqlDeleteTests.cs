using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql.Base;
using Npgsql;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql;

public class PgsqlDeleteTests : DeleteTestsBase<PgsqlTestBase, NpgsqlConnection, StudentPgsqlRepository>
{
    public PgsqlDeleteTests() => TestBase = new PgsqlTestBase();

    public override PgsqlTestBase TestBase { get; }
}