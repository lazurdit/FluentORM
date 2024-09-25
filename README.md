# LazurdIT.FluentOrm

**FluentOrm** is an easy-to-use object-relational mapping (ORM) framework that simplifies database operations with a readable and fluent interface. It allows developers to interact with their databases using simple, chainable methods.

## License

This project is licensed under the MIT License, with required attribution to the original repository: [[FluentORM Repository](https://github.com/lazurdit/FluentOrm)]

## Features

- **Simplified ORM:** Streamline your interactions with relational databases.
- **CRUD Operations:** Perform Create, Read, Update, and Delete operations with a fluent interface.
- **Multi-Database Support:** Compatible with PostgreSQL, MSSQL, and more.
- **Advanced Capabilities:** Includes filtering, sorting, and pagination to enhance data retrieval.
- **Efficient Data Handling:** Supports batch inserts and updates for large datasets.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Installation](#installation)
3. [Usage Examples](#usage-examples)
   - [Insert Operation](#1-insert-operation)
   - [Reading Records](#2-reading-records)
   - [Select Operation](#3-select-operation)
   - [Update Operation](#4-update-operation)
   - [Delete Operation](#5-delete-operation)
4. [Advanced Usage](#advanced-usage)
   - [Bulk Insert of Multiple Records (MsSql Only)](#1-bulk-insert-of-multiple-records-mssql-only)
   - [Complex Query with Filtering, Pagination, and Sorting](#2-complex-query-with-filtering-pagination-and-sorting)
5. [Conclusion](#conclusion)

## Getting Started

### Prerequisites

Before using **FluentOrm**, ensure you have the following installed:

- **.NET Core SDK**
- **Npgsql** for PostgreSQL or **System.Data.SqlClient** for SQL Server
- A properly configured database connection.

## Installation

To install **FluentORM**, run the following NuGet command:

```bash
dotnet add package Lazurd.FluentORM 
```

## Usage Examples

### 1. Insert Operation

Inserting a new record into the Student table.

**Code:**

```csharp
NpgsqlConnection connection = new NpgsqlConnection(connectionString);
PgsqlStudentRecordRepository studentsRepo = new();
var newStudent = new StudentRecord { Name = "John Doe", CreatedAt = DateTimeOffset.UtcNow };

studentsRepo.Insert(connection)
            .WithFields(s => s.Exclude(f => f.Id))
            .Execute(newStudent, true);

Console.WriteLine("Student record inserted successfully.");
```

**Output:**

```console
Student record inserted successfully.
ID: 101, Name: John Doe, CreatedAt: 2024-09-25T10:00:00Z
```

### 2. Reading Records

**Code:**

```csharp
var students = studentsRepo.Select(connection)
                           .Where(cm => cm.Like(m => m.Name, "John%"))
                           .Returns(fm => fm.Include(f => f.Name).Include(f => f.Id))
                           .OrderBy(om => om.FromField(f => f.Name))
                           .Execute();

foreach (var student in students)
{
    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
}
```

**Output:**
```ID: 101, Name: John Doe```

### 3. Select Operation

Fetching students whose name starts with "John".

**Code:**

```csharp
var students = studentsRepo.Select(connection)
                           .Where(cm => cm.Like(m => m.Name, "John%"))
                           .Returns(fm => fm.Include(f => f.Name).Include(f => f.Id))
                           .OrderBy(om => om.FromField(f => f.Name))
                           .Execute();

foreach (var student in students)
{
    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
}
```

**Output:**

```console
ID: 101, Name: John Doe
ID: 102, Name: Johnathan Smith
ID: 103, Name: Johnny Appleseed
```

### 4. Update Operation

Updating an existing student record.

**Code:**

```csharp
var existingStudent = studentsRepo.Select(connection)
                                  .Where(cm => cm.Eq(m => m.Id, 101))
                                  .Returns()
                                  .ExecuteSingle();

if (existingStudent != null)
{
    existingStudent.Name = "Jane Doe";
    studentsRepo.Update(connection)
                .Where(cm => cm.Eq(m => m.Id, 101))
                .Execute(existingStudent);

    Console.WriteLine("Student record updated successfully.");
}
```

**Output:**

```console
Record updated successfully: ID: 101, New Name: Jane Doe
```

### 5. Delete Operation

Deleting a student record by ID.

**Code:**

```csharp
studentsRepo.Delete(connection)
            .Where(cm => cm.Eq(m => m.Id, 101))
            .Execute();

Console.WriteLine("Student record deleted successfully.");
```

**Output:**

```console
Record deleted successfully: ID: 101
```

## Advanced Usage

### 1. Bulk Insert of Multiple Records (MsSql Only)

Inserting multiple randomly generated records into the Student table.

**Code:**

```csharp
List<StudentRecord> studentsList = new();
for (int i = 0; i < 5; i++)
{
    studentsList.Add(new StudentRecord().Random());
}

studentsRepo.Insert(connection)
            .WithFields(s => s.Exclude(f => f.Id))
            .ExecuteBatch(studentsList.ToArray(), true);
```

**Output:**

```console
ID: 102, Name: Student_1, CreatedAt: 2024-09-25T11:00:00Z
ID: 103, Name: Student_2, CreatedAt: 2024-09-25T11:00:00Z
ID: 104, Name: Student_3, CreatedAt: 2024-09-25T11:00:00Z
ID: 105, Name: Student_4, CreatedAt: 2024-09-25T11:00:00Z
ID: 106, Name: Student_5, CreatedAt: 2024-09-25T11:00:00Z
```

### 2. Complex Query with Filtering, Pagination, and Sorting

Fetching filtered, paginated, and sorted student records.

```csharp
var filteredStudents = studentsRepo.Select(connection)
                                   .Where(cm => cm.Like(m => m.Name, "S%")
                                                 .Between(m => m.Id, 100, 200))
                                   .Returns(fm => fm.ExcludeAll().Include(f => f.Name).Include(f => f.Id))
                                   .OrderBy(om => om.Random().FromField(f => f.Name, OrderDirections.Descending))
                                   .Execute(pageNumber: 1, recordsCount: 3);

foreach (var student in filteredStudents)
{
    Console.WriteLine($"ID: {student.Id}, Name: {student.Name}");
}
```

**Output:**

```console
ID: 106, Name: Student_5
ID: 105, Name: Student_4
ID: 104, Name: Student_3
```

## Conclusion

With FluentORM, you can efficiently manage database interactions using a simple and fluent interface. Explore further by extending the features and experimenting with different operations.
