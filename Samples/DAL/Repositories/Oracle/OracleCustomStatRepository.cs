using LazurdIT.FluentOrm.Oracle;
using Samples.DAL.Models;

namespace Samples.DAL.Repositories.Oracle;

internal class OracleCustomStatRepository : OracleFluentRepository<CustomStat>
{
    public OracleCustomStatRepository()
    {
    }
}