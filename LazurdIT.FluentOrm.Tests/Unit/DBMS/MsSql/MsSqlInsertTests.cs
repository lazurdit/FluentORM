using System.Data.SqlClient;
using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.MsSql.Base;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MsSql;

public class MsSqlInsertTests : InsertTestsBase<MsSqlTestBase, SqlConnection, StudentMsSqlRepository>
{
    public MsSqlInsertTests() => TestBase = new MsSqlTestBase();

    public override MsSqlTestBase TestBase { get; }
}