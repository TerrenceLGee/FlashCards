using Flashcards.Domain.Interfaces;
using Flashcards.Presentation.Operations.Helpers;
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

    public async Task CreateStackAsync(CancellationToken cancellationToken = default)
    {
        string name =
            UIOperationHelper.GetValidInput("Enter a name for the flashcard stack you wish to create", "green");
        
        var created = await _stackService.CreateStackAsync(name, cancellationToken);
        UIOperationHelper.WriteResult(created);
    }

    public async Task RenameStackAsync(int id, CancellationToken cancellationToken = default)
    {
        string name =
            UIOperationHelper.GetValidInput("Please enter the new name for the flashcard stack that you wish to rename",
                "green");

        var updated = await _stackService.RenameStackAsync(id, name, _token);
        UIOperationHelper.WriteResult(updated);
    }

    public async Task DeleteStackAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _stackService.DeleteStackAsync(id, _token);
        UIOperationHelper.WriteResult(deleted);
    }
}