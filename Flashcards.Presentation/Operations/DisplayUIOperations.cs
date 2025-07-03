using Flashcards.Domain.Interfaces;
using Flashcards.Presentation.Operations.Helpers;
using Spectre.Console;

namespace Flashcards.Presentation.Operations;

public class DisplayUIOperations
{
    private readonly IStackService _stackService;
    private readonly IFlashcardService _flashcardService;
    private readonly IStudySessionService _studySessionService;
    private readonly CancellationToken _token;

    public DisplayUIOperations(
        IStackService stackService,
        IFlashcardService flashcardService,
        IStudySessionService studySessionService,
        CancellationToken token)
    {
        _stackService = stackService;
        _flashcardService = flashcardService;
        _studySessionService = studySessionService;
        _token = token;
    }
    
    public async Task DisplayAllStacksAsync()
    {
        var getStacks = await _stackService.GetAllStacksAsync(_token);

        if (!getStacks.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStacks);
            return;
        }

        var allStacks = getStacks.Value;

        if (allStacks?.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]No stacks are currently available for display[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");

        AnsiConsole.MarkupLine("[underline green]Stacks[/]");
        
        foreach (var stack in allStacks!)
        {
            table.AddRow(
                stack.Id.ToString(),
                stack.Name);
        }

        AnsiConsole.Write(table);
    }

    public async Task DisplayAllFlashcardsAsync(int stackId)
    {
        var getFlashcards = await _flashcardService.GetFlashcardsByStackIdAsync(stackId, _token);

        if (!getFlashcards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashcards);
            return;
        }

        var allFlashcards = getFlashcards.Value;

        if (allFlashcards?.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]No flashcards are currently available for display with stack id = {stackId}[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Front");
        table.AddColumn("Back");

        var stackName = GetStackNameById(stackId);

        if (stackName is not null)
        {
            AnsiConsole.MarkupLine($"[green underline]Flash cards in {stackName}[/]");
        }

        foreach (var flashcard in allFlashcards!)
        {
            table.AddRow(
                flashcard.Position.ToString(),
                flashcard.Front,
                flashcard.Back);
        }

        AnsiConsole.Write(table);
    }

    public async Task DisplayAllStudySessionsByStackAsync(int stackId)
    {
        var getStudySessions = await _studySessionService.GetStudySessionsByStackAsync(stackId, _token);

        if (!getStudySessions.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStudySessions);
            return;
        }

        var allStudySessionsForStack = getStudySessions.Value;
        
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Score");

        var stackName = await GetStackNameById(stackId);
        AnsiConsole.MarkupLine($"[underline green]Study session for {stackName} stack[/]");

        foreach (var session in allStudySessionsForStack!)
        {
            table.AddRow(
                session.Id.ToString(),
                session.Date.ToString("MM-dd-yyyy"),
                session.Score.ToString());
        }

        AnsiConsole.Write(table);
    }

    public async Task DisplayAllStudySessionsAsync()
    {
        var getStudySessions = await _studySessionService.GetAllStudySessions(_token);

        if (!getStudySessions.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStudySessions);
            return;
        }

        var allStudySessions = getStudySessions.Value;

        var orderedSessions = allStudySessions!
            .OrderBy(session => session.StackId);

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Stack");
        table.AddColumn("Score");

        AnsiConsole.MarkupLine("[green underline]Study Sessions[/]");

        foreach (var session in orderedSessions)
        {
            string? stackName = await GetStackNameById(session.StackId);
            
            table.AddRow(
                session.Id.ToString(),
                session.Date.ToString("MM-dd-yyyy"),
                stackName ?? session.StackId.ToString(),
                session.Score.ToString());
        }

        AnsiConsole.Write(table);
    }

    private async Task<String?> GetStackNameById(int stackId)
    {
        var getStacks = await _stackService.GetAllStacksAsync(_token);

        if (!getStacks.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStacks);
            return null;
        }

        var allStacks = getStacks.Value;

        return allStacks!
            .Where(stack => stack.Id == stackId)
            .Select(stack => stack.Name)
            .FirstOrDefault();

    }
}