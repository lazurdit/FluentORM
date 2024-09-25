using LazurdIT.FluentOrm.Pgsql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.PgSql;

public partial class PgsqlInstructorRecordRepository : PgsqlFluentRepository<InstructorRecord>
{
    public PgsqlInstructorRecordRepository()
    {
        InRelations = new();
        OutRelations = new();
    }
}