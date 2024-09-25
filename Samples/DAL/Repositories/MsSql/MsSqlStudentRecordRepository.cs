using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MsSql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MsSql;

public partial class MsSqlStudentRecordRepository : MsSqlFluentRepository<StudentRecord>
{
    public MsSqlStudentRecordRepository()
    {
        InRelations = new List<IFluentRelation>(){
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord))
                .WithField(f => f.Id, f2 => f2.ClassId)
        };
        OutRelations = new();
    }
}