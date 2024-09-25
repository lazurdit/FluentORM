using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MySql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MySql;

public partial class MySqlClassRecordRepository : MySqlFluentRepository<ClassRecord>
{
    public MySqlClassRecordRepository()
    {
        InRelations = new();
        OutRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord)).WithField(f => f.Id, f2 => f2.ClassId),
            new FluentRelation<ClassRecord, InstructorRecord>( nameof(InstructorRecord)).WithField(f => f.Id, f2 => f2.ClassId)
        };
    }
}