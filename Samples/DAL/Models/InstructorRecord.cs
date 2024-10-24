﻿using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.Oracle;
using System.Runtime.CompilerServices;

namespace Samples.DAL.Models;

[FluentTable("Tbl_Instructor")]
public partial class InstructorRecord : IFluentModel
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    [FluentField("Fld_Instructor_Id", isPrimary: true, autoGenerated: true, oracleDbType: FluentOracleDbTypes.Int64)]
    public long Id { get; set; }

    [FluentField("Fld_Instructor_Name")]
    public string Name { get; set; } = string.Empty;

    [FluentField("Fld_Class_Id")]
    public long ClassId { get; set; }

    public override string ToString()
    {
        return $"Instructor => Id: {Id}, Name: {Name}";
    }
}