using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Oracle;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.Oracle;

public partial class OracleClassRecordRepository : OracleFluentRepository<ClassRecord>
{
    public OracleClassRecordRepository()
    {
        InRelations = new();
        OutRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord)).WithField(f => f.Id, f2 => f2.ClassId),
            new FluentRelation<ClassRecord, InstructorRecord>( nameof(InstructorRecord)).WithField(f => f.Id, f2 => f2.ClassId)
        };
    }
}