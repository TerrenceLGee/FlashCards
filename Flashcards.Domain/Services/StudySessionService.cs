using Flashcards.Core.Models;
using Flashcards.Core.Interfaces;
using Flashcards.Core.Results;
using Flashcards.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.Domain.Services;

public class StudySessionService : IStudySessionService
{
    private readonly IStudySessionRepository _sessionRepo;
    private readonly ILogger<StudySessionService> _logger;

    public StudySessionService(IStudySessionRepository sessionRepo, ILogger<StudySessionService> logger)
    {
        _sessionRepo = sessionRepo;
        _logger = logger;
    }

    public async Task<Result> CreateStudySessionAsync(int stackId, DateTime date, int score, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0)
            return Result.Fail("Stack ids must be greater than 0");

        if (date == DateTime.MinValue)
            return Result.Fail("Date is invalid");

        if (score < 0)
            return Result.Fail("Score must be a positive integer");

        try
        {
            var session = new StudySession(stackId, date, score);
            var created = await _sessionRepo.AddSessionAsync(session, cancellationToken).ConfigureAwait(false);

            return created ? Result.Ok() : Result.Fail($"Session with stack id = {stackId} unable to be created");
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Database error unable to add study session with stack id = {StackId}", stackId);
            return Result.Fail($"Unable to add study session with stack id = {stackId} due to database error");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "A serious error occurred when attempting to create a study session with stack id = {StackId}",
                stackId);
            return Result.Fail($"A serious error occurred cannot create study session with stack id = {stackId}");
        }
    }

    public async Task<Result<IReadOnlyList<StudySession>>> GetStudySessionsByStackAsync(int stackId, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0)
            return Result<IReadOnlyList<StudySession>>.Fail("Stack ids must be greater than 0");

        try
        {
            var studySessions =
                await _sessionRepo.GetSessionsByStackAsync(stackId, cancellationToken).ConfigureAwait(false);

            return Result<IReadOnlyList<StudySession>>.Ok(studySessions);
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Database error unable to retrieve any study sessions with stack id = {StackId}",
                stackId);
            return Result<IReadOnlyList<StudySession>>.Fail(
                $"Unable to retrieve any study sessions with stack id = {stackId} due to database error");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A serious error has occurred, unable to retrieve any study sessions with stack id = {StackId} ", stackId);
            return Result<IReadOnlyList<StudySession>>.Fail(
                $"Unable to retrieve any study sessions with stack id = {stackId} a serious error has occurred");
        }
    }

    public async Task<Result<IReadOnlyList<StudySession>>> GetAllStudySessions(CancellationToken cancellationToken = default)
    {
        try
        {
            var studySessions = await _sessionRepo.GetAllSessionsAsync(cancellationToken).ConfigureAwait(false);

            return Result<IReadOnlyList<StudySession>>.Ok(studySessions);
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Database error unable to retrieve any study sessions");
            return Result<IReadOnlyList<StudySession>>.Fail(
                $"Unable to retrieve any study sessions due to database error");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A serious error has occurred, unable to retrieve any study sessions");
            return Result<IReadOnlyList<StudySession>>.Fail(
                $"Unable to retrieve any study sessions a serious error has occurred");
        }
    }
}