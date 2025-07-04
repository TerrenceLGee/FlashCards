using Flashcards.Core.Models;
using Flashcards.Core.Results;

namespace Flashcards.Domain.Interfaces;

public interface IStudySessionService
{
    Task<Result> CreateStudySessionAsync(int stackId, DateTime date, int totalQuestions, int score, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<StudySession>>> GetStudySessionsByStackAsync(int stackId, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<StudySession>>> GetAllStudySessions(CancellationToken cancellationToken);
}