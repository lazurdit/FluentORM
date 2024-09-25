using Npgsql;
using Samples.DAL.Models;
using Samples.DAL.Repositories.PgSql;
using Samples.Utils;

namespace Samples.Samples.Pgsql;

internal class PgsqlGenerateData
{
    private readonly string connectionString;

    public PgsqlGenerateData(string connectionString)
    {
        this.connectionString = connectionString;
    }

    internal void InsertIndiviualRecords(int studentsCount)
    {
        //test time taken to create 10000 students
        Console.WriteLine("Testing Using v2 - Postgresql Individual Records");
        Console.WriteLine($"Preapring Data to create {studentsCount} student");

        List<StudentRecord> studentsList = new();

        var student = new StudentRecord().Random();
        for (int i = 0; i < studentsCount; i++)
        {
            studentsList.Add(student);
        }

        var studentArray = studentsList.ToArray();
        Console.WriteLine($"Creating {studentsCount} student");

        PgsqlStudentRecordRepository students = new();
        NpgsqlConnection connection = new(connectionString);
        var studentsCreate = students.Insert(connection)
                                        .WithFields(s => s.Exclude(f => f.Id));
        var startTime = DateTime.Now;
        Console.Write($"Start: {startTime}");

        for (int i = 0; i < studentsList.Count; i++)
        {
            var newStudent = studentsCreate.Execute(studentArray[i], true);
            Console.WriteLine(newStudent);
        }

        var endTime = DateTime.Now;
        Console.Write($" => {endTime}");
        Console.WriteLine($" : {(endTime - startTime).TotalSeconds} second(s)");
    }
}