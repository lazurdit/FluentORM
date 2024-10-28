using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.SQLite.Base;

public class SQLiteTestBase : ITestBase<SQLiteConnection, StudentSQLiteRepository>
{
    public string NewConnectionString() => $"Data Source={Path.GetTempFileName()};Version=3;New=True;";

    public SQLiteConnection NewConnection(string? connectionString) => new(connectionString ?? NewConnectionString());

    public StudentSQLiteRepository NewStudentsRepository()
    => new();

    public void ToDoBefore(string? connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using SQLiteCommand checkTableCmd = new();
        using SQLiteConnection connection = NewConnection(connectionString);
        connection.Open();
        checkTableCmd.Connection = connection;
        checkTableCmd.CommandText = @"
                        SELECT name FROM sqlite_master WHERE type='table' AND name='Students';
                    ";

        var tableExists = checkTableCmd.ExecuteScalar() != null;

        if (tableExists)
        {
            // If the table exists, delete all records and reset auto-increment counter
            using (var clearTableCmd = new SQLiteCommand("DELETE FROM Students;", connection))
            {
                clearTableCmd.ExecuteNonQuery();
            }
            using var resetCounterCmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name='Students';", connection);
            resetCounterCmd.ExecuteNonQuery();
        }
        else
        {
            // If the table does not exist, create it
            using SQLiteCommand createTableCmd = new();
            createTableCmd.Connection = connection;
            createTableCmd.CommandText = @"
                                CREATE TABLE Students (
                                    StudentId INTEGER PRIMARY KEY AUTOINCREMENT,
                                    StudentName TEXT NOT NULL UNIQUE,
                                    StudentAge INTEGER NULL
                                );
                            ";
            createTableCmd.ExecuteNonQuery();
        }
        connection.Close();
    }
}