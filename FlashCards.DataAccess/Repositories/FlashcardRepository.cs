using Flashcards.Core.Interfaces;
using Flashcards.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dapper;

namespace FlashCards.DataAccess.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly string? _connectionString;

    public FlashcardRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("FlashcardsDb")
                            ?? throw new InvalidOperationException("Connection string 'FlashcardsDb' not found");
    }

    public async Task<bool> AddFlashcardAsync(Flashcard flashcard, CancellationToken cancellationToken = default)
    {
        if (flashcard is null) throw new ArgumentNullException(nameof(flashcard));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var insertQuery =
            @"INSERT INTO dbo.Flashcards (StackId, Front, Back, Position) VALUES (@StackId, @Front, @Back, @Position)";

        var command = new CommandDefinition(insertQuery,
            new { flashcard.StackId, flashcard.Front, flashcard.Back, flashcard.Position }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<bool> UpdateFlashcardAsync(Flashcard flashcard, CancellationToken cancellationToken = default)
    {
        if (flashcard is null) throw new ArgumentNullException(nameof(flashcard));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var updateQuery =
            @"UPDATE dbo.Flashcards SET StackId = @StackId, Front = @Front, Back = @Back, Position = @Position WHERE Id = @Id";

        var command = new CommandDefinition(updateQuery,
            new { flashcard.Id, flashcard.StackId, flashcard.Front, flashcard.Back, flashcard.Position },
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<bool> DeleteFlashcardAsync(int flashcardId, CancellationToken cancellationToken = default)
    {
        if (flashcardId <= 0) throw new ArgumentOutOfRangeException(nameof(flashcardId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var deleteQuery = @"DELETE FROM dbo.Flashcards WHERE Id = @Id";

        var command =
            new CommandDefinition(deleteQuery, new { Id = flashcardId }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<bool> DeleteFlashcardByStackIdASync(int flashcardId, int stackId, CancellationToken cancellationToken = default)
    {
        if (flashcardId <= 0) throw new ArgumentOutOfRangeException(nameof(flashcardId));

        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var deleteQuery =
            @"DELETE FROM dbo.Flashcards WHERE Id=@Id AND StackId=@StackId";

        var command = new CommandDefinition(deleteQuery, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<Flashcard?> GetFlashcardByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalQuery =
            @"SELECT Id, StackId, Front, Back, Position FROM dbo.Flashcards WHERE Id = @Id";
 
        var command = new CommandDefinition(retrievalQuery, new { Id = id}, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Flashcard>(command).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Flashcard>> GetFlashcardsByStackIdAsync(int stackId, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalQuery =
            @"SELECT Id, StackId, Front, Back, Position FROM dbo.Flashcards WHERE StackId = @StackId ORDER BY Position";

        var command = new CommandDefinition(retrievalQuery, new {StackId = stackId }, cancellationToken: cancellationToken);

        return (await connection.QueryAsync<Flashcard>(command).ConfigureAwait(false))
            .AsList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<Flashcard>> GetAllFlashcardsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var deleteQuery = @"SELECT Id, StackId, Front, Back, Position FROM dbo.Flashcards ORDER BY Id";

        var command = new CommandDefinition(deleteQuery, cancellationToken: cancellationToken);

        return (await connection.QueryAsync<Flashcard>(command).ConfigureAwait(false))
            .AsList()
            .AsReadOnly();
    }

    public async Task<int> GetNextFlashcardPosition(int stackId, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalQuery = @"SELECT COUNT(*) FROM dbo.Flashcards WHERE StackId = @StackId";

        var command = new CommandDefinition(retrievalQuery, new {StackId = stackId}, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<int>(command).ConfigureAwait(false);
    }
}