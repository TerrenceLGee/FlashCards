using Flashcards.Core.DTOs;
using Flashcards.Core.Models;
using Flashcards.Core.Results;

namespace Flashcards.Domain.Interfaces;

public interface IFlashcardService
{
    Task<Result> CreateFlashcardAsync(int stackId, string front, string back, int position, CancellationToken cancellationToken);
    Task<Result> UpdateFlashcardAsync(int id, int stackId, string front, string back, CancellationToken cancellationToken);
    Task<Result> DeleteFlashcardAsync(int id, CancellationToken cancellationToken);
    Task<Result> DeleteFlashcardByStackIdAsync(int id, int stackId, CancellationToken cancellationToken);
    Task<Result<Flashcard?>> GetFlashcardByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<FlashcardDto>>> GetFlashcardsByStackIdAsync(int stackId,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<FlashcardDto>>> GetAllFlashcardsAsync(CancellationToken cancellationToken);
    Task<Result<int>> GetNextFlashcardPositionAsync(int stackId, CancellationToken cancellationToken);
}