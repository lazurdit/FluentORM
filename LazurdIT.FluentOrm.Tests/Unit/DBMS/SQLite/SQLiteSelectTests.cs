using System.Data.SQLite;
using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.SQLite.Base;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.SQLite;

public class SQLiteSelectTests : SelectTestsBase<SQLiteTestBase, SQLiteConnection, StudentSQLiteRepository>
{
    public SQLiteSelectTests() => TestBase = new SQLiteTestBase();

    public override SQLiteTestBase TestBase { get; }
}