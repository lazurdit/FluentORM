using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Oracle;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.Oracle;

public partial class OracleStudentRecordRepository : OracleFluentRepository<StudentRecord>
{
    public OracleStudentRecordRepository()
    {
        InRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord))
                .WithField(f => f.Id, f2 => f2.ClassId)
        };
        OutRelations = new();
    }
}