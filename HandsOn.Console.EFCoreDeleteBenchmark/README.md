# HandsOn EF Core - delete benchmark

Goal is to compare the performance of deleting entities using EF Core.

## Scenario

The scenario is to delete a list of entities from the SQL Server database. Entities are `Blog` and `Post` where `Blog` has a one-to-many relationship with `Post` and cascade delete is set for `Post` entities. The number of entities to be deleted is 20 000.

### Comparing the performance of the following approaches:
- `RemoveRange`
- `ExecuteDelete` from EF Core 8.0
- `BulkDelete` from Z.EntityFramework.Extensions.EFCore nuget
- `ExecuteSqlInterpolated` with SQL query

## Benchmark results

The benchmark results are as follows:

| Method                 | Mean     | Error    | StdDev   |
|----------------------- |---------:|---------:|---------:|
| ExecuteSqlInterpolated |  2.981 s | 0.0358 s | 0.0318 s |
| ExecuteDelete          |  3.054 s | 0.0604 s | 0.1150 s |
| BulkDelete             |  3.331 s | 0.0470 s | 0.0417 s |
| RemoveRange            | 15.182 s | 0.2244 s | 0.2099 s |

The results show that `ExecuteSqlInterpolated` is the fastest method to delete entities. However, by using this method you need to write and maintain the SQL query manually, also EF Core does not track the changes made by this method.

The `ExecuteDelete` method is the second fastest method, but it is available only in EF Core 8.0 and later versions.

The `BulkDelete` method is the third fastest method, and it is recommended to use with older versions of EF Core, but it requires `Z.EntityFramework.Extensions.EFCore` nuget to be installed.

The `RemoveRange` method is the slowest method, and it is not recommended to use it for deleting large amounts of entities as it is not optimized for bulk delete operations and it will generate a separate SQL query for each entity to be deleted. `RemoveRange` is 7.5 times slower than the fastest method `ExecuteSqlInterpolated`.