using System.Data.SqlClient;
using Samples.DAL.Models;
using Samples.DAL.Repositories.MsSql;

namespace Samples.Utils;

internal class GenerateTester
{
    private readonly string connectionString;

    public GenerateTester(string connectionString)
    {
        this.connectionString = connectionString;
    }

    internal void TV2Single1(int studentsCount)
    {
        //test time taken to create 10000 students
        Console.WriteLine("Testing Using v2 - Single Records - 1");
        Console.WriteLine($"Preapring Data to create {studentsCount} student");

        List<StudentRecord> studentsList = new();

        var student = new StudentRecord().Random();
        for (int i = 0; i < studentsCount; i++)
        {
            studentsList.Add(student);
        }

        var studentArray = studentsList.ToArray();
        Console.WriteLine($"Creating {studentsCount} student");

        SqlConnection connection = new(connectionString);
        connection.Open();
        var startTime = DateTime.Now;
        MsSqlStudentRecordRepository students = new();

        var studentsCreate = students.Insert(connection)
                                        .WithFields(s => s.Exclude(f => f.Id));
        Console.Write($"Start: {startTime}");

        for (int i = 0; i < studentsList.Count; i++)
        {
            studentsCreate.Execute(studentArray[i]);
        }

        var endTime = DateTime.Now;
        Console.Write($" => {endTime}");
        Console.WriteLine($" : {(endTime - startTime).TotalSeconds} second(s)");
    }

    internal void TV2Bulk(int studentsCount)
    {
        //test time taken to create 10000 students
        Console.WriteLine("Testing Using v2 - Bulk");
        Console.WriteLine($"Preapring Data to create {studentsCount} student");

        List<StudentRecord> studentsList = new();

        var student = new StudentRecord().Random();
        for (int i = 0; i < studentsCount; i++)
        {
            studentsList.Add(student);
        }

        var studentArray = studentsList.ToArray();
        Console.WriteLine($"Creating {studentsCount} student");

        SqlConnection connector = new(connectionString);

        MsSqlStudentRecordRepository students = new();
        SqlConnection connection = new(connectionString);
        var studentsCreate = students.Insert(connection)
                                        .WithFields(s => s.Exclude(f => f.Id)
                                                            .Exclude(f => f.ClassId));

        var startTime = DateTime.Now;
        Console.Write($"Start: {startTime}");

        studentsCreate.InsertBulk(studentArray);

        var endTime = DateTime.Now;
        Console.Write($" => {endTime}");
        Console.WriteLine($" : {(endTime - startTime).TotalSeconds} second(s)");
    }
}