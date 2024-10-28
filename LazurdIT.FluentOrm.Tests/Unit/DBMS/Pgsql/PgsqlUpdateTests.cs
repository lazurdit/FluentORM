using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql.Base;
using Npgsql;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql;

public class PgsqlUpdateTests : UpdateTestsBase<PgsqlTestBase, NpgsqlConnection, StudentPgsqlRepository>
{
    public PgsqlUpdateTests() => TestBase = new PgsqlTestBase();

    public override PgsqlTestBase TestBase { get; }
}