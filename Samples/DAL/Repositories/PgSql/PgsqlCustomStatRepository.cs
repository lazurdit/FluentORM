using LazurdIT.FluentOrm.Pgsql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.PgSql;

internal class PgsqlCustomStatRepository : PgsqlFluentRepository<CustomStat>
{
    public PgsqlCustomStatRepository()
    {
    }
}