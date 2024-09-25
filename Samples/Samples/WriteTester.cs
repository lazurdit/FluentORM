using LazurdIT.FluentOrm.Common;
using Samples.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Samples;

internal static class WriteTester
{
    internal static StudentRecord? TestAddStudent(IFluentRepository<StudentRecord> studentRepository, StudentRecord studentRecord, DbConnection dbConnection)
    {
        return studentRepository.Insert()
            .WithFields(fm => fm.Exclude(f => f.Id))
            .Execute(studentRecord, true, dbConnection);
    }

    internal static int? TestUpdateStudent(IFluentRepository<StudentRecord> studentRepository, StudentRecord studentRecord, DbConnection dbConnection)
    {
        return studentRepository.Update().WithFields(um => um.Exclude(f => f.Id)
                                                            .Exclude(f => f.CreatedAt)
                                                            .Exclude(f => f.UpdatedAt))
            .Where(wm => wm.Eq(f => f.Id, studentRecord.Id))
            .Execute(studentRecord, dbConnection, true);
    }

    internal static int? TestDeleteStudent(IFluentRepository<StudentRecord> studentRepository, long studentId, DbConnection dbConnection)
    {
        return studentRepository.Delete()
            .Where(fm => fm.Eq(f => f.Id, studentId))
            .Execute(dbConnection);
    }
}