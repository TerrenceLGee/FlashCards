using Flashcards.Core.DTOs;
using Flashcards.Core.Results;

namespace Flashcards.Domain.Interfaces;

public interface IStackService
{
    Task<Result> CreateStackAsync(string name, CancellationToken cancellationToken);
    Task<Result> RenameStackAsync(int id, string updatedName, CancellationToken cancellationToken);
    Task<Result> DeleteStackAsync(int id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<StackDto>>> GetAllStacksAsync(CancellationToken cancellationToken);
}