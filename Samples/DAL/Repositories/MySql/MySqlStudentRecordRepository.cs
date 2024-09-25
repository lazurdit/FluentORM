using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MySql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MySql;

public partial class MySqlStudentRecordRepository : MySqlFluentRepository<StudentRecord>
{
    public MySqlStudentRecordRepository()
    {
        InRelations = new() {
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord))
                .WithField(f => f.Id, f2 => f2.ClassId)
        };
        OutRelations = new();
    }
}