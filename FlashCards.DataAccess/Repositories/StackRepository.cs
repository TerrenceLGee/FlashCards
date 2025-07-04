using Dapper;
using Flashcards.Core.Interfaces;
using Flashcards.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace FlashCards.DataAccess.Repositories;

public class StackRepository : IStackRepository
{
    private readonly string? _connectionString;

    public StackRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("FlashcardsDb")
                            ?? throw new InvalidOperationException("Connection string 'FlashcardsDb' not found");
    }

    public async Task<bool> AddStackAsync(Stack stack, CancellationToken cancellationToken)
    {
        if (stack is null) throw new ArgumentNullException(nameof(stack));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var insertQuery = @"INSERT INTO dbo.Stacks (Name) VALUES (@Name);";

        var command = new CommandDefinition(insertQuery, new { stack.Name }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<bool> UpdateStackAsync(Stack stack, CancellationToken cancellationToken)
    {
        if (stack is null) throw new ArgumentNullException(nameof(stack));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var updateQuery = @"UPDATE dbo.Stacks SET Name = @Name WHERE Id = @Id";

        var command = new CommandDefinition(updateQuery, new { stack.Id, stack.Name },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<bool> DeleteStackAsync(int stackId, CancellationToken cancellationToken)
    {
        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var deleteQuery = @"DELETE FROM dbo.Stacks WHERE Id = @Id";

        var command = new CommandDefinition(deleteQuery, new { Id = stackId }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<Stack?> GetStackByIdAsync(int stackId, CancellationToken cancellationToken)
    {
        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retreivalQuery = @"SELECT FROM dbo.Stacks WHERE Id = @Id";
        
        var command = new CommandDefinition(retreivalQuery, new {Id = stackId}, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Stack>(command).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Stack>> GetAllStacksAsync(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalStatement = @"SELECT * FROM dbo.Stacks";

        var command = new CommandDefinition(retrievalStatement, cancellationToken: cancellationToken);

        return (await connection.QueryAsync<Stack>(command).ConfigureAwait(false))
            .AsList()
            .AsReadOnly();
    }
}