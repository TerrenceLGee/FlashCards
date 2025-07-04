using Flashcards.Core.Interfaces;
using Flashcards.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Dapper;

namespace FlashCards.DataAccess.Repositories;

public class StudySessionRepository : IStudySessionRepository
{
    private readonly string? _connectionString;

    public StudySessionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("FlashcardsDb")
                            ?? throw new InvalidOperationException("Connection string 'FlashcardsDb' not found");
    }

    public async Task<bool> AddSessionAsync(StudySession studySession, CancellationToken cancellationToken = default)
    {
        if (studySession is null) throw new ArgumentNullException(nameof(studySession));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var insertQuery = @"INSERT INTO dbo.StudySessions (StackId, Date, TotalQuestions, Score) VALUES (@StackId, @Date, @TotalQuestions, @Score)";

        var command = new CommandDefinition(insertQuery,
            new { studySession.StackId, studySession.Date, studySession.TotalQuestions, studySession.Score }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command).ConfigureAwait(false) == 1;
    }

    public async Task<IReadOnlyList<StudySession>> GetSessionsByStackAsync(int stackId, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0) throw new ArgumentOutOfRangeException(nameof(stackId));

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalQuery =
            @"SELECT Id, StackId, Date, TotalQuestions, Score FROM dbo.StudySessions WHERE StackId = @StackId";

        var command = new CommandDefinition(retrievalQuery, new { StackId = stackId }, cancellationToken: cancellationToken);

        return (await connection.QueryAsync<StudySession>(command).ConfigureAwait(false))
            .AsList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<StudySession>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        var retrievalQuery =
            @"SELECT Id, StackId, Date, TotalQuestions, Score FROM dbo.StudySessions";

        var command = new CommandDefinition(retrievalQuery, cancellationToken: cancellationToken);

        return (await connection.QueryAsync<StudySession>(command).ConfigureAwait(false))
            .AsList()
            .AsReadOnly();
    }
}