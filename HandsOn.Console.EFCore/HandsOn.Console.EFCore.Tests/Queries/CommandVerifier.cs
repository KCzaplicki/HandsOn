using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HandsOn.Console.EFCore.Tests.Queries;

public class CommandVerifier : IDbCommandInterceptor
{
    public IList<string> Commands { get; } = new List<string>();

    public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Commands.Add(command.CommandText.Replace(Environment.NewLine, " "));
        return result;
    }

    public ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Commands.Add(command.CommandText.Replace(Environment.NewLine, " "));
        return ValueTask.FromResult(result);
    }

    public void VerifyCalled(string commandSql, int calledTimes = 1)
    {
        var count = Commands.Count(c => c == commandSql);
        count.Should().Be(calledTimes);
    }

    public void VerifyNotCalled(string commandSql)
    {
        VerifyCalled(commandSql, 0);
    }
}