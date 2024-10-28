using LazurdIT.FluentOrm.MySql;
using LazurdIT.FluentOrm.Tests.TestResources.Models;

namespace LazurdIT.FluentOrm.Tests.TestResources.Repositories;

public class StudentMySqlRepository : MySqlFluentRepository<StudentModel>
{
}