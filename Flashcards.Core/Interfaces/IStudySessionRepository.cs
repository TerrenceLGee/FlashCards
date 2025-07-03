using Flashcards.Core.Models;

namespace Flashcards.Core.Interfaces;

public interface IStudySessionRepository
{
    Task<bool> AddSessionAsync(StudySession studySession, CancellationToken cancellationToken);
    Task<IReadOnlyList<StudySession>> GetSessionsByStackAsync(int stackId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StudySession>> GetAllSessionsAsync(CancellationToken cancellationToken);
}