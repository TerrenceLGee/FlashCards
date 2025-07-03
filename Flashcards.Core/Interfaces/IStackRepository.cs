using Flashcards.Core.Models;

namespace Flashcards.Core.Interfaces;

public interface IStackRepository
{
    Task<bool> AddStackAsync(Stack stack, CancellationToken cancellationToken);
    Task<bool> UpdateStackAsync(Stack stack, CancellationToken cancellationToken);
    Task<bool> DeleteStackAsync(int stackId, CancellationToken cancellationToken);
    Task<Stack?> GetStackByIdAsync(int stackId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Stack>> GetAllStacksAsync(CancellationToken cancellationToken);
}