using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Pgsql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.PgSql;

public partial class PgsqlClassRecordRepository : PgsqlFluentRepository<ClassRecord>
{
    public PgsqlClassRecordRepository()
    {
        InRelations = new();
        OutRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord)).WithField(f => f.Id, f2 => f2.ClassId),
            new FluentRelation<ClassRecord, InstructorRecord>( nameof(InstructorRecord)).WithField(f => f.Id, f2 => f2.ClassId)
        };
    }
}