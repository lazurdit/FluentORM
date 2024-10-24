using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Tests.TestResources
{
    [FluentTable("Students")]
    internal class StudentModel : IFluentModel
    {
        [FluentField(name: "StudentId", true)]
        public long Id { get; set; }

        [FluentField(name: "StudentName")]
        public string Name { get; set; } = string.Empty;

        [FluentField(name: "StudentAge", allowNull: true)]
        public long? Age { get; set; }
    }
}