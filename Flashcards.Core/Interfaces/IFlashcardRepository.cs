using Flashcards.Core.Models;

namespace Flashcards.Core.Interfaces;

public interface IFlashcardRepository
{
    Task<bool> AddFlashcardAsync(Flashcard flashcard, CancellationToken cancellationToken);
    Task<bool> UpdateFlashcardAsync(Flashcard flashcard, CancellationToken cancellationToken);
    Task<bool> DeleteFlashcardAsync(int flashcardId, CancellationToken cancellationToken);
    Task<bool> DeleteFlashcardByStackIdASync(int flashcardId, int stackId, CancellationToken cancellationToken);
    Task<Flashcard?> GetFlashcardByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Flashcard>> GetFlashcardsByStackIdAsync(int stackId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Flashcard>> GetAllFlashcardsAsync(CancellationToken cancellationToken);
    Task<int> GetNextFlashcardPosition(int stackId, CancellationToken cancellationToken);
}