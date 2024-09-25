using LazurdIT.FluentOrm.MySql;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.MySql;

internal class MySqlCustomStatRepository : MySqlFluentRepository<CustomStat>
{
    public MySqlCustomStatRepository()
    {
    }
}