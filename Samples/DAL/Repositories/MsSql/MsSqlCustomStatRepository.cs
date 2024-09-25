using LazurdIT.FluentOrm.MsSql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MsSql;

internal class MsSqlCustomStatRepository : MsSqlFluentRepository<CustomStat>
{
    public MsSqlCustomStatRepository()
    {
    }
}