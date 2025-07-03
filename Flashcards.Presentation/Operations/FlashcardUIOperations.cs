using Flashcards.Domain.Interfaces;
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
}