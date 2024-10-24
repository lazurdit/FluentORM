using LazurdIT.FluentOrm.Tests.Utils;

namespace LazurdIT.FluentOrm.Tests.Unit.SQLite.Base;

public class SQLiteTestBase
{
    protected readonly string connectionString;

    public SQLiteTestBase()
    {
        string tempDBFile = Path.GetTempFileName();

        connectionString = $"Data Source={tempDBFile};Version=3;New=True;";
    }

    internal void ToDoBefore()
    {
        SQLiteUtils.ToDoBeforeTest(connectionString);
    }
}