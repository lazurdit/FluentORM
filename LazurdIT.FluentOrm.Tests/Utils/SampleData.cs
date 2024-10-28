using LazurdIT.FluentOrm.Tests.TestResources.Models;

namespace LazurdIT.FluentOrm.Tests.Utils;

internal static class SampleData
{
    internal static List<StudentModel> DefaultStudentsList => new()
    {
        new(){Name = "John Doe",Age = 25},
        new(){Name = "Jane Doe",Age = 23},
        new(){Name = "Jack Doe",Age = 21},
        new(){Name = "Jill Doe",Age = 19},
        new(){Name = "James Doe",Age = 17},
        new(){Name = "Jenny Doe",Age = 15},
        new(){Name = "Jared Doe",Age = 13},
        new(){Name = "Jasmine Doe",Age = 11},
        new(){Name = "Jasper Doe",Age = 9},
        new(){Name = "Jade Doe",Age = 7},
        new(){Name = "Jax Doe",Age = 5},
        new(){Name = "Jaxson Doe",Age = 3},
        new(){Name = "Jaxon Dark",Age = 1},
        new(){Name = "Jaxson Dark",Age = 0},
    };
}