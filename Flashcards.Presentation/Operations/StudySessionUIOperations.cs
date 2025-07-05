using Flashcards.Core.Models;
using Flashcards.Domain.Interfaces;
using Flashcards.Presentation.Operations.Helpers;
using Spectre.Console;

namespace Flashcards.Presentation.Operations;

public class StudySessionUIOperations
{
    private readonly IStudySessionService _studySessionService;
    private readonly CancellationToken _token;

    public StudySessionUIOperations(
        IStudySessionService studySessionService,
        CancellationToken token)
    {
        _studySessionService = studySessionService;
        _token = token;
    }

    public async Task StartStudySessionAsync(int stackId, IFlashcardService flashcardService ,CancellationToken cancellationToken = default)
    {
        int score = 0;
        int totalQuestions = 0;

        

        var getFlashCards = await flashcardService.GetFlashcardsByStackIdAsync(stackId, cancellationToken).ConfigureAwait(false);

        if (!getFlashCards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashCards);
            return;
        }

        var flashcards = getFlashCards.Value;

        string question;
        string correctAnswer;
        string userAnswer;

        if (flashcards is null || flashcards.Count == 0)
        {
            UIOperationHelper.DisplayMessage($"Error there are no flashcards available for stack with stack id = {stackId}");
            return;
        }

        totalQuestions = UIOperationHelper.GetValidNumber($"There are {flashcards.Count} flashcards available in this stack, how many would you like to study? ");

        UIOperationHelper.ClearTheScreen("green");

        for (int i = 0; i < totalQuestions; i++)
        {
            question = flashcards[i].Front;
            correctAnswer = flashcards[i].Back;

            userAnswer = UIOperationHelper.GetValidInput(question);

            if (userAnswer.Equals(correctAnswer,StringComparison.OrdinalIgnoreCase))
            {
                score++;
            }

            GoToNextFlashcard();
        }

        UIOperationHelper.DisplayMessage($"Study session completed you got {score} out {totalQuestions} correct", "blue");

        var added = await _studySessionService.CreateStudySessionAsync(stackId, DateTime.Now, score, totalQuestions, _token);

        UIOperationHelper.WriteResult(added);
    }

    private void GoToNextFlashcard()
    {
        UIOperationHelper.DisplayMessage("\nPress any key to go to the next flashcard", "blue");
        Console.ReadKey();
        Console.Clear();
    }
}