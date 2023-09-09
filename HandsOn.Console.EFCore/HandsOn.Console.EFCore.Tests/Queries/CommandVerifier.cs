using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HandsOn.Console.EFCore.Tests.Queries;

public class CommandVerifier : IDbCommandInterceptor
{
    private IList<string> Commands { get; } = new List<string>();

    public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Commands.Add(TrimCommand(command.CommandText));
        return result;
    }

    public ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
        CancellationToken cancellationToken = new())
    {
        Commands.Add(TrimCommand(command.CommandText));
        return ValueTask.FromResult(result);
    }

    public void VerifyCalled(string commandSql, int calledTimes = 1)
    {
        var count = Commands.Count(c => c.Equals(TrimCommand(commandSql)));
        count.Should().Be(calledTimes);
    }

    public void VerifyNotCalled(string commandSql)
    {
        VerifyCalled(commandSql, 0);
    }

    private static string TrimCommand(string sqlCommand)
    {
        return sqlCommand.Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty);
    }
}