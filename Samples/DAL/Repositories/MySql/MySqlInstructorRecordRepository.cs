using LazurdIT.FluentOrm.MySql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MySql;

public partial class MySqlInstructorRecordRepository : MySqlFluentRepository<InstructorRecord>
{
    public MySqlInstructorRecordRepository()
    {
        InRelations = new();
        OutRelations = new();
    }
}