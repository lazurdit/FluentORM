using Bogus;
using Samples.DAL.Models;

namespace Samples.Utils;

internal static class Extensions
{
    internal static StudentRecord Random(this StudentRecord student)
    {
        var faker = new Faker<StudentRecord>()
            .RuleFor(v => v.Name, f => f.Name.FullName())
            .RuleFor(v => v.CreatedAt, f => new DateTimeOffset(f.Date.Past()))
            .RuleFor(v => v.ClassId, f => f.Random.Number(1, 2))
            .RuleFor(v => v.UpdatedAt, f => new DateTimeOffset(f.Date.Past()));
        student = faker.Generate();
        return student;
    }
}