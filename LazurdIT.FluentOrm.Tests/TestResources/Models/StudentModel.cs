using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Oracle;

namespace LazurdIT.FluentOrm.Tests.TestResources.Models;

[FluentTable("Students")]
public class StudentModel : IFluentModel
{
    [FluentField(name: "StudentId", true, oracleDbType: FluentOracleDbTypes.Decimal)]
    public long Id { get; set; }

    [FluentField(name: "StudentName", oracleDbType: FluentOracleDbTypes.NVarchar2)]
    public string Name { get; set; } = string.Empty;

    [FluentField(name: "StudentAge", allowNull: true, oracleDbType: FluentOracleDbTypes.Decimal)]
    public long? Age { get; set; }
}