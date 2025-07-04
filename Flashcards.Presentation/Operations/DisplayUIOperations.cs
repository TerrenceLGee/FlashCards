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
        var getStacks = await _stackService.GetAllStacksAsync(_token).ConfigureAwait(false);

        if (!getStacks.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStacks);
            return;
        }

        var allStacks = getStacks.Value;

        if (allStacks?.Count == 0)
        {
            UIOperationHelper.DisplayMessage("No stacks are currently available for display");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");

        UIOperationHelper.DisplayMessage("Stacks", "underline green");
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
        var getFlashcards = await _flashcardService.GetFlashcardsByStackIdAsync(stackId, _token).ConfigureAwait(false);

        if (!getFlashcards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashcards);
            return;
        }

        var allFlashcards = getFlashcards.Value;

        if (allFlashcards?.Count == 0)
        {
            UIOperationHelper.DisplayMessage($"No flashcards are currently available for display with stack id = {stackId}");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Front");
        table.AddColumn("Back");

        var stackName = await GetFlashcardStackNameById(stackId);

        if (stackName is not null)
        {
            UIOperationHelper.DisplayMessage($"Flash cards in {stackName}", "underline green");
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

    public async Task DisplayAllFlashcardsAsync()
    {
        var getFlashcards = await _flashcardService.GetAllFlashcardsAsync(_token).ConfigureAwait(false);

        if (!getFlashcards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashcards);
            return;
        }

        var allFlashcards = getFlashcards.Value;

        if (allFlashcards?.Count == 0)
        {
            UIOperationHelper.DisplayMessage("There are current no flashcards belonging to any stack available");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Front");
        table.AddColumn("Back");
        
        UIOperationHelper.DisplayMessage("Flashcards","underline green");

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
        var getStudySessions = await _studySessionService.GetStudySessionsByStackAsync(stackId, _token).ConfigureAwait(false);

        if (!getStudySessions.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStudySessions);
            return;
        }

        var allStudySessionsForStack = getStudySessions.Value;

        if (allStudySessionsForStack?.Count == 0)
        {
            UIOperationHelper.DisplayMessage($"There are no study sessions available for stack with id = {stackId}");
            return;
        }
        
        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Score");

        var stackName = await GetFlashcardStackNameById(stackId);
        UIOperationHelper.DisplayMessage($"Study session for {stackName} stack", "underline green");

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
        var getStudySessions = await _studySessionService.GetAllStudySessions(_token).ConfigureAwait(false);

        if (!getStudySessions.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStudySessions);
            return;
        }

        var allStudySessions = getStudySessions.Value;

        var orderedSessions = allStudySessions!
            .OrderBy(session => session.StackId)
            .ToList()
            .AsReadOnly();

        if (orderedSessions?.Count == 0)
        {
            UIOperationHelper.DisplayMessage("There are no study sessions available to display");
            return;
        }

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Date");
        table.AddColumn("Stack");
        table.AddColumn("Score");

        UIOperationHelper.DisplayMessage("Study Sessions", "underline green");

        foreach (var session in orderedSessions!)
        {
            string? stackName = await GetFlashcardStackNameById(session.StackId);
            
            table.AddRow(
                session.Id.ToString(),
                session.Date.ToString("MM-dd-yyyy"),
                stackName ?? session.StackId.ToString(),
                session.Score.ToString());
        }

        AnsiConsole.Write(table);
    }

    private async Task<String?> GetFlashcardStackNameById(int stackId)
    {
        var getStacks = await _stackService.GetAllStacksAsync(_token).ConfigureAwait(false);

        if (!getStacks.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStacks);
            return null;
        }

        var allStacks = getStacks.Value;
        var stackName = allStacks!
            .Where(stack => stack.Id == stackId)
            .Select(stack => stack.Name)
            .FirstOrDefault();
        return stackName;

    }
}