using LazurdIT.FluentOrm.Common;
using System.Runtime.CompilerServices;

namespace Samples.DAL.Models;

[FluentTable]
internal class CustomStat : IFluentModel
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    [FluentField("student_count")]
    public int Count { get; set; }
}