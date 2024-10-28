﻿using LazurdIT.FluentOrm.Tests.TestResources.Repositories;
using LazurdIT.FluentOrm.Tests.Unit.Base;
using LazurdIT.FluentOrm.Tests.Unit.DBMS.MySql.Base;
using MySqlConnector;

namespace LazurdIT.FluentOrm.Tests.Unit.DBMS.MySql;

public class MySqlUpdateTests : UpdateTestsBase<MySqlTestBase, MySqlConnection, StudentMySqlRepository>
{
    public MySqlUpdateTests() => TestBase = new MySqlTestBase();

    public override MySqlTestBase TestBase { get; }
}