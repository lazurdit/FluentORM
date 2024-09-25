using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MsSql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MsSql;

public partial class MsSqlClassRecordRepository : MsSqlFluentRepository<ClassRecord>
{
    public MsSqlClassRecordRepository()
    {
        InRelations = new();
        OutRelations = new(){
            new FluentRelation<ClassRecord, StudentRecord>( nameof(StudentRecord)).WithField(f => f.Id, f2 => f2.ClassId),
            new FluentRelation<ClassRecord, InstructorRecord>( nameof(InstructorRecord)).WithField(f => f.Id, f2 => f2.ClassId)};
    }
}