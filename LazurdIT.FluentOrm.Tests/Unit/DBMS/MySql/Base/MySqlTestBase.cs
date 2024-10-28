using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using MySqlConnector;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MySql.Base;

public class MySqlTestBase : ITestBase<MySqlConnection, StudentMySqlRepository>
{
    public string NewConnectionString() => "server=127.0.0.1;uid=root;pwd=P@ssw0rd;database=Fluent_Test";

    public MySqlConnection NewConnection(string? connectionString) => new(connectionString ?? NewConnectionString());

    public StudentMySqlRepository NewStudentsRepository()
    => new();

    public void ToDoBefore(string? connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using MySqlConnection connection = NewConnection(connectionString);
        connection.Open();

        using MySqlCommand checkTableCmd = new MySqlCommand(@"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'Fluent_Test'
                    AND TABLE_NAME = 'Students';", connection);

        bool tableExists = (Convert.ToInt32(checkTableCmd.ExecuteScalar()) > 0);

        if (tableExists)
        {
            // If the table exists, delete all records
            using (var clearTableCmd = new MySqlCommand("DELETE FROM Students;", connection))
            {
                clearTableCmd.ExecuteNonQuery();
            }

            // Reset the AUTO_INCREMENT counter to 1
            using var resetCounterCmd = new MySqlCommand("ALTER TABLE Students AUTO_INCREMENT = 1;", connection);
            resetCounterCmd.ExecuteNonQuery();
        }
        else
        {
            // If the table does not exist, create it
            using MySqlCommand createTableCmd = new MySqlCommand(@"
                        CREATE TABLE Students (
                            StudentId BIGINT AUTO_INCREMENT PRIMARY KEY,
                            StudentName VARCHAR(100) NOT NULL UNIQUE,
                            StudentAge BIGINT NULL
                        );", connection);
            createTableCmd.ExecuteNonQuery();
        }

        connection.Close();
    }
}