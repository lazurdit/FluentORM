using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.MsSql.Base;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MsSql;

public class MsSqlUpdateTests : UpdateTestsBase<MsSqlTestBase, SqlConnection, StudentMsSqlRepository>
{
    public MsSqlUpdateTests() => TestBase = new MsSqlTestBase();

    public override MsSqlTestBase TestBase { get; }
}