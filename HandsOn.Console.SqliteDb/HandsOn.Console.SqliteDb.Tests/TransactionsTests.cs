using System.Diagnostics;

namespace HandsOn.Console.SqliteDb.Tests;

public class TransactionsTests
{
    [Fact]
    public void CantHaveMoreThanOneActiveTransactionPerDbContext()
    {
        var dbContext = new HandsOnDbContext();
        
        dbContext.Database.BeginTransaction();
        Action act = () => dbContext.Database.BeginTransaction();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The connection is already in a transaction and cannot participate in another transaction.");
    }
    
    [Fact]
    public void CanHaveMoreThenOneActiveTransactionForDifferentDbContexts()
    {
        var transactionCommitCalled = new ManualResetEventSlim();
        var transactionCommitCalled2 = new ManualResetEventSlim();
        var dbContext = new HandsOnDbContext();
        var dbContext2 = new HandsOnDbContext();

        var transaction = dbContext.Database.BeginTransaction();
        Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
        {
            transaction.Commit();
            transactionCommitCalled.Set();
        });
        
        var transaction2 = dbContext2.Database.BeginTransaction();
        Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
        {
            transaction2.Commit();
            transactionCommitCalled2.Set();
        });

        transactionCommitCalled.Wait();
        transactionCommitCalled2.Wait();
    }
    
    [Fact]
    public void ConsecutiveTransactionWillWait30SecondsForReleaseDatabaseLockBeforeThrowingSqliteException()
    {
        var dbContext = new HandsOnDbContext();
        var dbContext2 = new HandsOnDbContext();
        var stopWatch = new Stopwatch();
        
        dbContext.Database.BeginTransaction();
        Action act = () =>
        {
            stopWatch.Start();
            dbContext2.Database.BeginTransaction();
            stopWatch.Stop();
        };

        act.Should().Throw<SqliteException>()
            .WithMessage("SQLite Error 5: 'database is locked'.");
        
        stopWatch.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void DatabaseLockWillBeReleasedOnTransactionCommit()
    {
        var dbContext = new HandsOnDbContext();
        
        var transaction = dbContext.Database.BeginTransaction();
        transaction.Commit();
        
        dbContext.Database.BeginTransaction();
    }
    
    [Fact]
    public void DatabaseLockWillBeReleasedOnTransactionRollback()
    {
        var dbContext = new HandsOnDbContext();
        
        var transaction = dbContext.Database.BeginTransaction();
        transaction.Rollback();
        
        dbContext.Database.BeginTransaction();
    }
    
    [Fact]
    public void DatabaseLockWillBeReleasedOnTransactionDispose()
    {
        var dbContext = new HandsOnDbContext();

        using (dbContext.Database.BeginTransaction())
        {
            // implicit dispose
        }

        dbContext.Database.BeginTransaction();
    }

    [Fact]
    public void TransactionWillBlockSaveChangesFromDifferentDbContext()
    {
        var dbContext = new HandsOnDbContext();
        var dbContext2 = new HandsOnDbContext();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        dbContext.Database.BeginTransaction();
        
        dbContext2.Organizations.Add(new Organization
        {
            Name = nameof(TransactionWillBlockSaveChangesFromDifferentDbContext),
            CreatedOn = DateTime.UtcNow
        });
        Action act = () => dbContext2.SaveChanges();
        
        act.Should().Throw<DbUpdateException>()
            .WithInnerException<SqliteException>()
            .WithMessage("SQLite Error 5: 'database is locked'.");
    }
}