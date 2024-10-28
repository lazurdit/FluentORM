using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.Oracle.Base;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Oracle;

public class OracleUpdateTests : UpdateTestsBase<OracleTestBase, OracleConnection, StudentOracleRepository>
{
    public OracleUpdateTests() => TestBase = new OracleTestBase();

    public override OracleTestBase TestBase { get; }
}