using Flashcards.Domain.Interfaces;
using Flashcards.Presentation.Operations.Helpers;
using Spectre.Console;

namespace Flashcards.Presentation.Operations;

public class FlashcardUIOperations
{
    private readonly IFlashcardService _flashcardService;
    private readonly CancellationToken _token;

    public FlashcardUIOperations(IFlashcardService flashcardService, CancellationToken token)
    {
        _flashcardService = flashcardService;
        _token = token;
    }

    public async Task AddFlashCard(int stackId, CancellationToken cancellationToken = default)
    {
        string front = UIOperationHelper.GetValidInput("Please enter the front for the flashcard (the question) ");
        string back = UIOperationHelper.GetValidInput("Please enter the back for the flashcard (the answer) ");
        var getPosition = await _flashcardService.GetNextFlashcardPositionAsync(stackId, _token);

        if (!getPosition.IsSuccess)
        {
            UIOperationHelper.WriteResult(getPosition);
            return;
        }

        int position = getPosition.Value;

        var added=  await _flashcardService.CreateFlashcardAsync(stackId, front, back, ++position, _token);
        
        UIOperationHelper.WriteResult(added);
    }

    public async Task UpdateFlashCardAsync(int id, int stackId, CancellationToken cancellationToken = default)
    {
        string front = UIOperationHelper.GetValidInput("Please enter the front for the flashcard that you wish to update (or press enter if you wish to keep the same question) ");
        string back = UIOperationHelper.GetValidInput("Please enter the back for the flashcard that you wish to update (or press enter if you wish to keep the same answer) ");

        var updated = await _flashcardService.UpdateFlashcardAsync(id, stackId, front, back, _token);

        UIOperationHelper.WriteResult(updated);
    }

    public async Task DeleteFlashcardAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _flashcardService.DeleteFlashcardAsync(id, cancellationToken);

        UIOperationHelper.WriteResult(deleted);
    }

    public async Task DeleteFlashcardByStackIdAsync(int id, int stackId, CancellationToken cancellationToken = default)
    {
        var deleted = await _flashcardService.DeleteFlashcardByStackIdAsync(id, stackId, cancellationToken);
        
        UIOperationHelper.WriteResult(deleted);
    }
}