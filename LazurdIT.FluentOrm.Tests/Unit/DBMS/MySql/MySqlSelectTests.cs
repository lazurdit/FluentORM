using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.MySql.Base;
using MySqlConnector;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MySql;

public class MySqlSelectTests : SelectTestsBase<MySqlTestBase, MySqlConnection, StudentMySqlRepository>
{
    public MySqlSelectTests() => TestBase = new MySqlTestBase();

    public override MySqlTestBase TestBase { get; }
}