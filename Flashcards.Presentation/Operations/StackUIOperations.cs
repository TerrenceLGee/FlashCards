using Flashcards.Domain.Interfaces;
using Spectre.Console;

namespace Flashcards.Presentation.Operations;

public class StackUIOperations
{
    private readonly IStackService _stackService;
    private readonly CancellationToken _token;

    public StackUIOperations(IStackService stackService, CancellationToken token)
    {
        _stackService = stackService;
        _token = token;
    }
}