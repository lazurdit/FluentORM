## Overview

`Lazurd.FluentORM` is a powerful and efficient .NET SDK that serves as a lightweight ORM. This SDK provides essential functionalities and tools with a scalable and wide range of features and capabilities.
   
## Getting Started 

Please refer to the official documentation for a detailed explanation of the available features and their usage.


## Release Notes 
# 1.3.5
Provide varios fixes and enhancements to the SDK, upgrade to this version is highly recommended.
- Fix: Fixed an issue with DbParameters inPgSql.
- Fix: Fixed varios bugs in Select Query.
- Add: Added recusrive And/Or conditions (Conditions can now contain a single or group conditions on multiple levels).
- Enhancement: Added extra 21 test-cases to ensure more stability.
- Enhancement: Reconstruct and enhance FluentCondition class.
- Enhancement: Reconstruct and enhance ConditionsManager classes.

# 1.3.0
Provide varios fixes and enhancements to the SDK, upgrade to this version is highly recommended.
- Fix: Fixed an issue with the `WithConnection` method.
- Fix: Fixed an issue with Between, NotBetween, Like, NotLike conditions.
- Fix: Fixed an issue with DB types conversion.
- Fix: Fixed an issue pagination.
- Fix: Fixed an issue with table name in Oracle DB.
- Add: Added `WithConnection` method to the IFluentRepository allow the user to specify the connection.
- Enhancement: Added extra 54 test-cases to ensure more stability.

# 1.2.0
- Feature: Added SQLite support.

# 1.1.1
- Bug fix: Error when inserting a null value field.

# 1.1.0
- feature: Added multi-target frameworks (Dotnet 6.0, 7.0, 8.0).
- enhancement: performance enhancements.

# 1.0.0
- fix: Added .WithConnection() method to allow the user to specify the connection string.
- fix: Added .WithTablePrefix() method to allow the user to specify the table prefix.
- enhancement: Enhanced insert command code.

# 0.9.2
- Minor fixes/enhancements

# 0.9.0 (Alpha)
- initial release
 