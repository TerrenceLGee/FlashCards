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
        var getPosition = await _flashcardService.GetNextFlashcardPositionAsync(stackId, _token).ConfigureAwait(false);

        if (!getPosition.IsSuccess)
        {
            UIOperationHelper.WriteResult(getPosition);
            return;
        }

        int position = getPosition.Value;

        var added=  await _flashcardService.CreateFlashcardAsync(stackId, front, back, ++position, _token).ConfigureAwait(false);

        UIOperationHelper.WriteResult(added);
    }

    public async Task UpdateFlashCardAsync(int id, int stackId, CancellationToken cancellationToken = default)
    {
        string front = UIOperationHelper.GetValidInput("Please enter the front for the flashcard that you wish to update (or press enter if you wish to keep the same question) ");
        string back = UIOperationHelper.GetValidInput("Please enter the back for the flashcard that you wish to update (or press enter if you wish to keep the same answer) ");

        var updated = await _flashcardService.UpdateFlashcardAsync(id, stackId, front, back, _token).ConfigureAwait(false);

        UIOperationHelper.WriteResult(updated);
    }

    public async Task DeleteFlashcardByStackIdAsync(int id, int stackId, CancellationToken cancellationToken = default)
    {
        var actualId = await GetValidFlashcardIdByPosition(id, stackId).ConfigureAwait(false);
        var deleted = await _flashcardService.DeleteFlashcardByStackIdAsync(actualId, stackId, cancellationToken).ConfigureAwait(false);

        UIOperationHelper.WriteResult(deleted);
    }

    private async Task<int> GetValidFlashcardIdByPosition(int id, int stackId)
    {
        var actualId = -1;

        var getFlashcards = await _flashcardService.GetFlashcardsByStackIdAsync(stackId, _token).ConfigureAwait(false);

        if (!getFlashcards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashcards);
            return actualId;
        }

        var allFlashcards = getFlashcards.Value;

        if (allFlashcards is not null)
        {
            actualId = allFlashcards
                .Where(flashcard => flashcard.Position == id)
                .Select(flashcard => flashcard.Id)
                .FirstOrDefault();
                
        }

        return actualId;
    }
}