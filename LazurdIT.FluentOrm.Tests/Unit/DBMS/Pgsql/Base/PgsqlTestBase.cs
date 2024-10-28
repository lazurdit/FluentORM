using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using Npgsql;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.Pgsql.Base;

public class PgsqlTestBase : ITestBase<NpgsqlConnection, StudentPgsqlRepository>
{
    public string NewConnectionString() => "Host=localhost:5432;Username=postgres;Password=P@ssw0rd;Database=Fluent_Test";

    public NpgsqlConnection NewConnection(string? connectionString) => new(connectionString ?? NewConnectionString());

    public StudentPgsqlRepository NewStudentsRepository() => new();

    public void ToDoBefore(string? connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using NpgsqlConnection connection = NewConnection(connectionString);
        connection.Open();

        using NpgsqlCommand checkTableCmd = new NpgsqlCommand(@"
                        SELECT to_regclass('public.Students')::TEXT;", connection);

        var tableExists = checkTableCmd.ExecuteScalar() != DBNull.Value;

        if (tableExists)
        {
            // If the table exists, delete all records and reset auto-increment sequence
            using (var clearTableCmd = new NpgsqlCommand("DELETE FROM Students;", connection))
            {
                clearTableCmd.ExecuteNonQuery();
            }

            // Reset the sequence associated with the StudentId column
            using var resetCounterCmd = new NpgsqlCommand("ALTER SEQUENCE students_studentid_seq RESTART WITH 1;", connection);
            resetCounterCmd.ExecuteNonQuery();
        }
        else
        {
            // If the table does not exist, create it with a serial (auto-increment) primary key
            using NpgsqlCommand createTableCmd = new NpgsqlCommand(@"
                        CREATE TABLE Students (
                            StudentId BIGSERIAL PRIMARY KEY,
                            StudentName TEXT NOT NULL UNIQUE,
                            StudentAge bigint NULL
                        );", connection);
            createTableCmd.ExecuteNonQuery();
        }

        connection.Close();
    }
}