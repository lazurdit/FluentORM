using LazurdIT.FluentOrm.Pgsql;
using LazurdIT.FluentOrm.Tests.TestResources.Models;

namespace LazurdIT.FluentOrm.Tests.TestResources.Repositories;

public class StudentPgsqlRepository : PgsqlFluentRepository<StudentModel>
{
}