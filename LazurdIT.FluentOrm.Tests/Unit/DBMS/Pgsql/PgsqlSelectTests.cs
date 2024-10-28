using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql.Base;
using Npgsql;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql;

public class PgsqlSelectTests : SelectTestsBase<PgsqlTestBase, NpgsqlConnection, StudentPgsqlRepository>
{
    public PgsqlSelectTests() => TestBase = new PgsqlTestBase();

    public override PgsqlTestBase TestBase { get; }
}