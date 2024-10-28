using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql.Base;
using Npgsql;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql;

public class PgsqlInsertTests : InsertTestsBase<PgsqlTestBase, NpgsqlConnection, StudentPgsqlRepository>
{
    public PgsqlInsertTests() => TestBase = new PgsqlTestBase();

    public override PgsqlTestBase TestBase { get; }
}