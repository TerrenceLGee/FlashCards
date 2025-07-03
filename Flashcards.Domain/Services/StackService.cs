using Flashcards.Core.DTOs;
using Flashcards.Core.Interfaces;
using Flashcards.Core.Models;
using Flashcards.Core.Results;
using Flashcards.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Flashcards.Domain.Services;

public class StackService : IStackService
{
    private readonly IStackRepository _stackRepo;
    private readonly ILogger<StackService> _logger;

    public StackService(IStackRepository stackRepo, ILogger<StackService> logger)
    {
        _stackRepo = stackRepo;
        _logger = logger;
    }

    public async Task<Result> CreateStackAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Stack name cannot be null/empty");

        try
        {
            var stack = new Stack(name);
            var created = await _stackRepo.AddStackAsync(stack, cancellationToken).ConfigureAwait(false);

            return created ? Result.Ok() : Result.Fail("Unable to create new stack");
            
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            _logger.LogCritical(ex, "Stack creation failed for {StackName} because it is a duplication of a previous created stack", name);
            return Result.Fail($"A stack named {name} already exists");
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Stack creation failed for stack named: {StackName}", name);
            return Result.Fail("Failed to save stack to the database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A serious error occurred in creation of the {StackName} stack, stack not created", name);
            return Result.Fail("A serious error has occurred");
        }
    }

    public async Task<Result> RenameStackAsync(int id, string updatedName, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return Result.Fail("Stack Id must be greater than 0");

        if (string.IsNullOrWhiteSpace(updatedName))
            return Result.Fail("Updated stack name cannot be null or empty");

        try
        {
            var stackToUpdate = await _stackRepo.GetStackByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (stackToUpdate is null)
                return Result.Fail($"A stack with the id = {id} does not exist");

            stackToUpdate.Name = updatedName;

            var renamed = await _stackRepo.UpdateStackAsync(stackToUpdate, cancellationToken).ConfigureAwait(false);

            return renamed ? Result.Ok() : Result.Fail("Stack unable to be updated");
            
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            _logger.LogCritical(ex, "Stack update failed for stack with id = {StackId} because it is a duplication of a previous created stack", id);
            return Result.Fail($"A stack named '{updatedName}' already exists");
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Unable to update stack with id = {StackId} a database error has occurred", id);
            return Result.Fail("Failed to update the stack in the database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A serious error occurred when attempting to update stack with id = {StackId}", id);
            return Result.Fail("A serious error has occurred");
        }
    }

    public async Task<Result> DeleteStackAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return Result.Fail("Stack Ids must be greater than 0");

        try
        {
            var deleted = await _stackRepo.DeleteStackAsync(id, cancellationToken).ConfigureAwait(false);
            
            return deleted ? Result.Ok() : Result.Fail($"There is no stack with id = {id}");
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, "Database error deleting stack {StackId}", id);
            return Result.Fail($"Failed to delete the stack with id = {id} due to database error");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A serious error occurred when attempting to delete stack with id = {StackId}", id);
            return Result.Fail("A serious error has occurred");
        }
    }

    public async Task<Result<IReadOnlyList<StackDto>>> GetAllStacksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await _stackRepo.GetAllStacksAsync(cancellationToken).ConfigureAwait(false);
            var stacks = list
                .Select(stack => new StackDto(stack.Id, stack.Name))
                .ToList()
                .AsReadOnly();
            
            return Result<IReadOnlyList<StackDto>>.Ok(stacks);
        }
        catch (SqlException ex)
        {
            _logger.LogCritical(ex, $"Unable to retrieve any stacks from the database");
            return Result<IReadOnlyList<StackDto>>.Fail($"Failed to retrieve any stacks from the database");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"A serious error occurred while attempting to retrieve stacks");
            return Result<IReadOnlyList<StackDto>>.Fail("A serious error has occurred");
        }
    }
}
