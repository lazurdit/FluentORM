using System.Data.Common;
using FluentAssertions;
using System.Data.SQLite;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Tests.TestResources.Models;
using LazurdIT.FluentOrm.Tests.Utils;
using LazurdIT.FluentOrm.Tests.Utils.TestBase;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.Tests.Unit.Base;

public abstract class InsertTestsBase<TBase, TConnection, TRepository> where TBase : ITestBase<TConnection, TRepository> where TConnection : DbConnection, new() where TRepository : IFluentRepository<StudentModel>
{
    public abstract TBase TestBase { get; }

    [Fact(DisplayName = "Test add new record")]
    public void InsertData()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);
        using var connection = TestBase.NewConnection(connectionString);

        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        var student = SampleData.DefaultStudentsList[0];
        var resultRecord = insertQuery.Execute(student, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");
        resultRecord!.Name.Should().Be(student.Name, $"it should contain the name of {student.Name}, but the result was {resultRecord!.Name}");
        resultRecord!.Age.Should().Be(student.Age, $"Age should be '{student.Age}' but the received is '{resultRecord?.Age}'");

        connection.Close();
    }

    [Fact(DisplayName = "Test add with missing required values record")]
    public void InsertWithMissingRequiredValues()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);
        using var connection = TestBase.NewConnection(connectionString);

        connection.Open();

        var insertQuery = repository.Insert().WithFields(f => f.Exclude(f => f.Id));
        var student = SampleData.DefaultStudentsList[0];
        student.Name = null!;

        Action act = () => insertQuery.Execute(student, true, connection);

        if (connection is SQLiteConnection)
            act.Should().Throw<SQLiteException>()
                .WithMessage("*NOT NULL constraint failed*");
        else if (connection is SqlConnection)
            act.Should().Throw<SqlException>()
                .WithMessage("*column does not allow nulls*");
        else if (connection is NpgsqlConnection)
            act.Should().Throw<Npgsql.PostgresException>()
                .WithMessage("*null value in column*");
        else if (connection is MySqlConnection)
            act.Should().Throw<MySqlException>()
                .WithMessage("*Column*cannot be null*");
        else if (connection is OracleConnection)
            act.Should().Throw<OracleException>()
                .WithMessage("*cannot insert NULL into*");
        else
            act.Should().Throw<Exception>()
              .WithMessage("*Unknown Message*");

        connection.Close();
    }

    [Fact(DisplayName = "Test add with duplicate unique values")]
    public void InsertInvalidData()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);
        using var connection = TestBase.NewConnection(connectionString);

        connection.Open();

        var insertQuery = repository.Insert()
            .WithFields(m => m.Exclude(f => f.Id));

        var student = SampleData.DefaultStudentsList[0];

        //insert student for first time
        insertQuery.Execute(student, true, connection);

        // fix for pgsql
        connection.Close();
        connection.Open();

        Action act = () => insertQuery.Execute(student, true, connection);

        if (connection is SQLiteConnection)
            act.Should().Throw<SQLiteException>()
                .WithMessage("*UNIQUE constraint failed*");
        else if (connection is SqlConnection)
            act.Should().Throw<SqlException>()
                .WithMessage($"*Cannot insert duplicate key*");
        else if (connection is NpgsqlConnection)
            act.Should().Throw<Npgsql.PostgresException>()
                .WithMessage("*duplicate key value*");
        else if (connection is MySqlConnection)
            act.Should().Throw<MySqlException>()
                .WithMessage("*Duplicate entry*");
        else if (connection is OracleConnection)
            act.Should().Throw<OracleException>()
                .WithMessage("*unique constraint*");
        else
            act.Should().Throw<Exception>()
              .WithMessage("*Unknown Message*");

        connection.Close();
    }

    [Fact(DisplayName = "Test add new record with null values")]
    public void InsertWithNullValues()
    {
        var connectionString = TestBase.NewConnectionString();
        var repository = TestBase.NewStudentsRepository();
        TestBase.ToDoBefore(connectionString);
        using var connection = TestBase.NewConnection(connectionString);

        connection.Open();

        var insertQuery = repository.Insert()
                                    .WithFields(f => f.Exclude(f => f.Id));

        var student = SampleData.DefaultStudentsList[0];
        student.Age = null;

        var resultRecord = insertQuery.Execute(student, true, connection);

        resultRecord.Should().NotBeNull("The record should not be null");
        resultRecord!.Id.Should().BePositive("The record should Have an Id more than zero");
        resultRecord!.Name.Should().Be(student.Name, $"it should contain the name of {student.Name}, but the result was {resultRecord!.Name}");
        resultRecord!.Age.Should().Be(student.Age, $"Age should be '{student.Age}' but the received is '{resultRecord?.Age}'");

        connection.Close();
    }
}