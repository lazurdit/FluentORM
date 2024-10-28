using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using Oracle.ManagedDataAccess.Client;
using FluentMigrator.Runner;
using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Utils.Migrations;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Oracle.Base;

public class OracleTestBase : ITestBase<OracleConnection, StudentOracleRepository>
{
    public string NewConnectionString() => @"User Id=system;Password=oracle;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(COMMUNITY=tcp.world)(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=XE)(SID=SYSTEM)));";

    public OracleConnection NewConnection(string? connectionString) => new(connectionString ?? NewConnectionString());

    public StudentOracleRepository NewStudentsRepository()
    => new();

    public void ToDoBefore(string? connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using OracleConnection connection = NewConnection(connectionString ?? NewConnectionString());
        connection.Open();
        ImplementDB.Down(connectionString ?? NewConnectionString(), mb => mb.AddOracle12CManaged());

        ImplementDB.Up(connectionString ?? NewConnectionString(), mb => mb.AddOracle12CManaged());
        Console.WriteLine("Done");
    }
}