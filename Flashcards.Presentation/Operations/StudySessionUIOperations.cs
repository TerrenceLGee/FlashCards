using Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Flashcards.Presentation.Operations;

public class StudySessionUIOperations
{
    private readonly IStudySessionService _studySessionService;
    private readonly CancellationToken _token;

    public StudySessionUIOperations(IStudySessionService studySessionService, CancellationToken token)
    {
        _studySessionService = studySessionService;
        _token = token;
    }
}