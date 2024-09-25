using LazurdIT.FluentOrm.Oracle;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.Oracle;

public partial class OracleInstructorRecordRepository : OracleFluentRepository<InstructorRecord>
{
    public OracleInstructorRecordRepository()
    {
        InRelations = new();
        OutRelations = new();
    }
}