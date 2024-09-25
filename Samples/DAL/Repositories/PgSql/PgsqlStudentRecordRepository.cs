using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Pgsql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.PgSql;

public partial class PgsqlStudentRecordRepository : PgsqlFluentRepository<StudentRecord>
{
    public PgsqlStudentRecordRepository()
    {
        InRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord))
                .WithField(f => f.Id, f2 => f2.ClassId)
        };
        OutRelations = new();
    }
}