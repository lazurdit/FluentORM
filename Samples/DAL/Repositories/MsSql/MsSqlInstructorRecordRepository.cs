using LazurdIT.FluentOrm.MsSql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MsSql;

public partial class MsSqlInstructorRecordRepository : MsSqlFluentRepository<InstructorRecord>
{
    public MsSqlInstructorRecordRepository()
    {
        InRelations = new();
        OutRelations = new();
    }
}