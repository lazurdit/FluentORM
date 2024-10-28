using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MsSql.Base;

public class MsSqlTestBase : ITestBase<SqlConnection, StudentMsSqlRepository>
{
    public string NewConnectionString() => $"Server=localhost;Database=Fluent_Test;Trusted_Connection=false;TrustServerCertificate=true;UID=sa;PWD=P@ssw0rd";

    public SqlConnection NewConnection(string? connectionString) => new(connectionString ?? NewConnectionString());

    public StudentMsSqlRepository NewStudentsRepository()
    => new();

    public void ToDoBefore(string? connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using SqlConnection connection = NewConnection(connectionString);
        connection.Open();

        using SqlCommand checkTableCmd = new(@"
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                               WHERE TABLE_NAME = 'Students')
                    BEGIN
                        SELECT 1
                    END
                    ELSE
                    BEGIN
                        SELECT 0
                    END", connection);

        bool tableExists = (int)checkTableCmd.ExecuteScalar() == 1;

        if (tableExists)
        {
            // If the table exists, delete all records and reset the identity column
            using (var clearTableCmd = new SqlCommand("DELETE FROM Students;", connection))
            {
                clearTableCmd.ExecuteNonQuery();
            }

            // Reset the identity column
            using var resetCounterCmd = new SqlCommand("DBCC CHECKIDENT ('Students', RESEED, 0);", connection);
            resetCounterCmd.ExecuteNonQuery();
        }
        else
        {
            // If the table does not exist, create it
            using SqlCommand createTableCmd = new(@"
                        CREATE TABLE Students (
                            StudentId BIGINT IDENTITY(1,1) PRIMARY KEY,
                            StudentName NVARCHAR(100) NOT NULL UNIQUE,
                            StudentAge BIGINT NULL
                        );", connection);
            createTableCmd.ExecuteNonQuery();
        }

        connection.Close();
    }
}