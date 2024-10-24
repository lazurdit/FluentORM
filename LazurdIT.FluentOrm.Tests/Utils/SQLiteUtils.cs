using LazurdIT.FluentOrm.Tests.TestResources;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.Tests.Utils;

internal static class SQLiteUtils
{
    internal static List<StudentModel> DefaultStudentsList => new()
    {
        new ()
        {
            Name = "John Doe",
            Age = 25
        },
        new ()
        {
            Name = "Jane Doe",
            Age = 23
        },
        new()
        {
            Name = "Jack Doe",
            Age = 21
        }
    };

    internal static void ToDoBeforeTest(string connectionString)
    {
        // Step 1: Check if the Students table exists and reset it if it does
        using SQLiteCommand checkTableCmd = new();
        using SQLiteConnection connection = new(connectionString);
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
                                    StudentName TEXT NOT NULL,
                                    StudentAge INTEGER NULL
                                );
                            ";
            createTableCmd.ExecuteNonQuery();
        }
        connection.Close();
    }
}