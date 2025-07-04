using Flashcards.Core.DTOs;
using Flashcards.Core.Interfaces;
using Flashcards.Core.Models;
using Flashcards.Core.Results;
using Flashcards.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Flashcards.Domain.Services;

public class FlashcardService : IFlashcardService
{
    private readonly IFlashcardRepository _flashcardRepo;
    private readonly ILogger<FlashcardService> _logger;

    public FlashcardService(IFlashcardRepository flashcardRepo, ILogger<FlashcardService> logger)
    {
        _flashcardRepo = flashcardRepo;
        _logger = logger;
    }

    public async Task<Result> CreateFlashcardAsync(int stackId, string front, string back, int position,
        CancellationToken cancellationToken = default)
    {
        if (stackId <= 0)
            return Result.Fail("Stack id must be greater than 0");

        if (string.IsNullOrWhiteSpace(front))
            return Result.Fail("Front entry for flashcard must not be blank");

        if (string.IsNullOrWhiteSpace(back))
            return Result.Fail("Back entry for flashcard must not be blank");

        if (position <= 0)
            return Result.Fail("Valid positions are positive integers greater than 0");

        try
        {
            var flashcard = new Flashcard(stackId, front, back, position);
            var created = await _flashcardRepo.AddFlashcardAsync(flashcard, cancellationToken).ConfigureAwait(false);

            return created ? Result.Ok() : Result.Fail("Unsuccessful at creating new flashcard");

        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Unable to save flashcard to stack with stack id = {StackId}", stackId);
            return Result.Fail("Flashcard not able to be saved to the database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Flashcard unable to be created due to serious error");
            return Result.Fail("A serious error occured during flashcard creation, flash card not created");
        }
    }

    public async Task<Result> UpdateFlashcardAsync(int id, int stackId, string front="", string back="",
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return Result.Fail("Flashcard id must be greater than 0");
        
        if (stackId <= 0)
            return Result.Fail("Stack id must be greater than 0");

        try
        {
            var flashCardToUpdate = await _flashcardRepo.GetFlashcardByIdAsync(stackId, cancellationToken).ConfigureAwait(false);

            if (flashCardToUpdate is null)
                return Result.Fail($"There is no flash card with id = {id} that is in stack with stack id = {stackId}");

            if (!string.IsNullOrWhiteSpace(front))
            {
                flashCardToUpdate.Front = front;
            }

            if (!string.IsNullOrWhiteSpace(back))
            {
                flashCardToUpdate.Back = back;
            }

            var updated = await _flashcardRepo.UpdateFlashcardAsync(flashCardToUpdate, cancellationToken).ConfigureAwait(false);

            return updated ? Result.Ok() : Result.Fail($"Unable to update flashcard with id = {id} in stack with stack id = {stackId}");
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error unable to update flashcard with id = {id} in stack with stack id {StackId}", id, stackId);
            return Result.Fail($"Unable to updated flashcard id = {id} in stack with stack id = {stackId}, database error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "A serious error occurred in the attempt to update flashcard with id = {id} in stack with stack id = {StackId}", id, stackId);
            return Result.Fail($"Unable to update flashcard with id = {id} in stack with stack id = {stackId}, a serious error has occurred");
        }
    }

    public async Task<Result> DeleteFlashcardAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return Result.Fail("Flashcard id must be greater than 0");

        try
        {
            var deleted = await _flashcardRepo.DeleteFlashcardAsync(id, cancellationToken).ConfigureAwait(false);

            return deleted ? Result.Ok() : Result.Fail($"There is no flashcard with id = {id}");
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error unable to delete flashcard with id = {id}", id);
            return Result.Fail($"Unable to delete flashcard with id = {id}, database error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "A serious error occurred in the attempt to delete flashcard with id = {id}", id);
            return Result.Fail($"Unable to delete flashcard with id = {id}, a serious error has occurred");
        }
    }

    public async Task<Result> DeleteFlashcardByStackIdAsync(int id, int stackId, CancellationToken cancellationToken)
    {
        if (id <= 0)
            return Result.Fail("Flashcard id must be greater than 0");

        if (stackId <= 0)
            return Result.Fail("Flashcard's stack id must be greater than 0");

        try
        {
            var deleted = await _flashcardRepo.DeleteFlashcardByStackIdASync(id, stackId, cancellationToken);

            return deleted ? Result.Ok() : Result.Fail($"Unable to deleted flashcard with id = {id} and stack id = {stackId}");
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error unable to delete flashcard with id = {id} and stack id = {StackId}", id, stackId);
            return Result.Fail($"Unable to delete flashcard with id = {id} and stack id = {stackId}, database error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "A serious error occurred in the attempt to delete flashcard with id = {id} and stack id = {StackId}", id, stackId);
            return Result.Fail($"Unable to delete flashcard with id = {id} and stack id = {stackId}, a serious error has occurred");
        }
    }

    public async Task<Result<Flashcard?>> GetFlashcardByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return Result<Flashcard?>.Fail("Flashcard ids must be greater than 0");

        try
        {
            var flashcard = await _flashcardRepo.GetFlashcardByIdAsync(id, cancellationToken).ConfigureAwait(false);

            return (flashcard is not null)
                ? Result<Flashcard?>.Ok(flashcard)
                : Result<Flashcard?>.Fail($"There is no flashcard with id = {id}");
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Database error unable to retrieve flashcard with id = {id}", id);
            return Result<Flashcard?>.Fail($"Unable to retrieve flashcard with id = {id} from the database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "A serious error occurred in the attempt to retrieve flashcard with id = {id}", id);
            return Result<Flashcard?>.Fail($"Unable to retrieve flashcard with id = {id}, a serious error has occurred");
        }
    }


public async Task<Result<IReadOnlyList<FlashcardDto>>> GetFlashcardsByStackIdAsync(int stackId, CancellationToken cancellationToken = default)
{
    if (stackId <= 0)
        return Result<IReadOnlyList<FlashcardDto>>.Fail("Flashcard ids must be greater than 0");

    try
    {
        var list = await _flashcardRepo.GetFlashcardsByStackIdAsync(stackId, cancellationToken).ConfigureAwait(false);

        var flashcards = list
            .Select(flashcard => new FlashcardDto(flashcard.Id, flashcard.Front, flashcard.Back, flashcard.Position))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<FlashcardDto>>.Ok(flashcards);
    }
    catch (SqlException ex)
    {
        _logger.LogCritical(ex, "Database error unable to retrieve flashcards with stack id = {StackId}", stackId);
        return Result<IReadOnlyList<FlashcardDto>>.Fail($"Unable to retrieve flashcards with stack id = {stackId} from the database");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "A serious error occurred in the attempt to retrieve flashcards with stack id = {StackId}", stackId);
        return Result<IReadOnlyList<FlashcardDto>>.Fail($"Unable to retrieve flashcards with stack id = {stackId}, a serious error has occurred");
    }
}

public async Task<Result<IReadOnlyList<FlashcardDto>>> GetAllFlashcardsAsync(CancellationToken cancellationToken = default)
{
    try
    {
        var list = await _flashcardRepo.GetAllFlashcardsAsync(cancellationToken).ConfigureAwait(false);

        var allFlashcards = list
            .Select(flashcard => new FlashcardDto(flashcard.Id, flashcard.Front, flashcard.Back, flashcard.Position))
            .AsList()
            .AsReadOnly();

        return Result<IReadOnlyList<FlashcardDto>>.Ok(allFlashcards);
    }
    catch (SqlException ex)
    {
        _logger.LogCritical(ex, "Database error unable to retrieve flashcards");
        return Result<IReadOnlyList<FlashcardDto>>.Fail($"Unable to retrieve flashcards from the database");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "A serious error occurred in the attempt to retrieve flashcards");
        return Result<IReadOnlyList<FlashcardDto>>.Fail($"Unable to retrieve flashcards a serious error has occurred");
    }
}

public async Task<Result<int>> GetNextFlashcardPositionAsync(int stackId, CancellationToken cancellationToken = default)
    {
        if (stackId <= 0)
            return Result<int>.Fail("Stack ids must be greater than 0");

        try
        {
            var position = await _flashcardRepo.GetNextFlashcardPosition(stackId, cancellationToken).ConfigureAwait(false);

            return Result<int>.Ok(position);
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "There is no flashcard with stack id = {StackId} in the database, cannot return next position", stackId);
            return Result<int>.Fail($"There was no flashcard with stack id = {stackId} found in the database, cannot return next position");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "A serious error occurred when attempting to retrieve the next position for flashcard with stack id = {StackId}",
                stackId);
            return Result<int>.Fail(
                $"A serious error occured cannot retrieve the next position for flashcard with stack id = {stackId}");
        }
    }
}