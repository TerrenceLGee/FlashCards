using Flashcards.Domain.Interfaces;
using Flashcards.Presentation.Operations;
using Flashcards.Presentation.Operations.Helpers;
using Flashcards.Presentation.Options;
using Flashcards.Presentation.Options.Extensions;
using Spectre.Console;

namespace Flashcards.Presentation.App;

public class FlashCardUI
{
    private readonly IStackService _stackService;
    private readonly IFlashcardService _flashcardService;
    private readonly IStudySessionService _studySessionService;
    private readonly CancellationToken _token;

    public FlashCardUI(
        IStackService stackService,
        IFlashcardService flashcardService,
        IStudySessionService studySessionService,
        CancellationToken cancellationToken = default)
    {
        _stackService = stackService;
        _flashcardService = flashcardService;
        _studySessionService = studySessionService;
        _token = cancellationToken;
    }

    public async Task Run()
    {
        bool isRunning = true;
        
        DisplayUIOperations display =
            new DisplayUIOperations(_stackService, _flashcardService, _studySessionService, _token);
        StackUIOperations stackUI = new StackUIOperations(_stackService, _token);
        FlashcardUIOperations flashcardUI = new FlashcardUIOperations(_flashcardService, _token);
        StudySessionUIOperations sessionUI = new StudySessionUIOperations(_studySessionService, _token);

        while (isRunning)
        {
            switch (ShowMainMenu())
            {
                case MainMenuOption.ManageStacks:
                    await HandleStackMenu(stackUI, display).ConfigureAwait(false);
                    break;
                case MainMenuOption.ManageFlashcards:
                    await HandleFlashcardMenu(flashcardUI, display).ConfigureAwait(false); ;
                    break;
                case MainMenuOption.Study:
                    await HandleStudySessionMenu(sessionUI, display).ConfigureAwait(false); ;
                    break;
                case MainMenuOption.Exit:
                    isRunning = false;
                    break;
                default: UIOperationHelper.DisplayMessage("Invalid input");
                    break;
            }
        }
    }

    private async Task HandleStackMenu(StackUIOperations stackUI, DisplayUIOperations display)
    {
        bool isRunning = true;

        while (isRunning)
        {
            switch (ShowStackMenu())
            {
                case StackMenuOption.CreateStack:
                    await stackUI.CreateStackAsync(_token).ConfigureAwait(false);
                    break;

                case StackMenuOption.RenameStack:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);

                    int renameId = UIOperationHelper.GetValidNumber(
                        "Please enter the id for the stack that you wish to rename: ");

                    if (!await InvalidStackIdMessage(renameId).ConfigureAwait(false))
                        break;

                    await stackUI.RenameStackAsync(renameId, _token).ConfigureAwait(false);
                    break;
                case StackMenuOption.DeleteStack:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);

                    int deleteId =
                        UIOperationHelper.GetValidNumber(
                            "Please enter the id for the stack that you wish to deleted: ");

                    if (!await InvalidStackIdMessage(deleteId).ConfigureAwait(false))
                        break;

                    await stackUI.DeleteStackAsync(deleteId, _token).ConfigureAwait(false);
                    break;
                case StackMenuOption.ListAllStacks:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);
                    break;
                case StackMenuOption.Back:
                    isRunning = false;
                    break;
                default: UIOperationHelper.DisplayMessage("Invalid input");
                    break;
            }
            UIOperationHelper.ClearTheScreen();
        }
    }

    private async Task HandleFlashcardMenu(FlashcardUIOperations flashcardUI, DisplayUIOperations display)
    {
        bool isRunning = true;

        int stackId;
        int flashcardPosition;

        while (isRunning)
        {
            switch (ShowFlashcardMenu())
            {
                case FlashcardMenuOption.AddFlashcard:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false); 

                    stackId =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the id of the stack that you wish to create a flash card for: ");

                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;

                    await flashcardUI.AddFlashCard(stackId, _token).ConfigureAwait(false);

                    break;
                case FlashcardMenuOption.EditFlashcard:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);
                    stackId =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the id of the stack that you wish to edit a flashcard in: ");

                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;


                    await display.DisplayAllFlashcardsAsync(stackId).ConfigureAwait(false);
                    flashcardPosition =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the id of the flashcard that you wish to edit: ");

                    if (!await InvalidFlashcardIdMessage(flashcardPosition).ConfigureAwait(false))
                        break;

                    await flashcardUI.UpdateFlashCardAsync(flashcardPosition, stackId, _token).ConfigureAwait(false);

                    break;
                case FlashcardMenuOption.DeleteFlashcardByStackId:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);

                    stackId =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the id of the stack that you wish to delete a flashcard in: ");

                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;

                    await display.DisplayAllFlashcardsAsync(stackId).ConfigureAwait(false);

                    flashcardPosition =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the id of the flashcard that you wish to delete: ");

                    if (!await InvalidFlashcardIdMessage(flashcardPosition).ConfigureAwait(false))
                        break;


                    await flashcardUI.DeleteFlashcardByStackIdAsync(flashcardPosition, stackId, _token).ConfigureAwait(false);

                    break;
                case FlashcardMenuOption.ListAllFlashcards:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);
                    stackId =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the stack id that you wish to see flashcards for: ");

                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;

                    await display.DisplayAllFlashcardsAsync(stackId).ConfigureAwait(false);
                    break;
                case FlashcardMenuOption.Back:
                    isRunning = false;
                    break;
                default: UIOperationHelper.DisplayMessage("Invalid input");
                    break;
            }
            UIOperationHelper.ClearTheScreen();
        }
    }

    private async Task HandleStudySessionMenu(StudySessionUIOperations sessionUI, DisplayUIOperations display)
    {
        bool isRunning = true;
        int stackId;

        while (isRunning)
        {
            switch (ShowStudyMenu())
            {
                case StudyMenuOption.StartStudySession:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);
                    stackId = UIOperationHelper.GetValidNumber("Please enter the stack id for which you would like to create a study session for ");

                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;

                    await sessionUI.StartStudySessionAsync(stackId, _flashcardService, _token).ConfigureAwait(false);

                    break;
                case StudyMenuOption.ViewAllStudySessions:
                    await display.DisplayAllStudySessionsAsync().ConfigureAwait(false);
                    break;
                case StudyMenuOption.ViewStudySessionByStackId:
                    await display.DisplayAllStacksAsync().ConfigureAwait(false);
                    stackId =
                        UIOperationHelper.GetValidNumber(
                            "Please choose the stack id that you wish to see study sessions for: ");
                    if (!await InvalidStackIdMessage(stackId).ConfigureAwait(false))
                        break;
                    await display.DisplayAllStudySessionsByStackAsync(stackId).ConfigureAwait(false);
                    break;
                case StudyMenuOption.Back:
                    isRunning = false;
                    break;
                default: UIOperationHelper.DisplayMessage("Invalid input");
                    break;
            }
            UIOperationHelper.ClearTheScreen();
        }
    }

    private static MainMenuOption ShowMainMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<MainMenuOption>()
                .Title("Main Menu")
                .AddChoices(Enum.GetValues<MainMenuOption>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    private static StackMenuOption ShowStackMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<StackMenuOption>()
                .Title("Choose one of the following options")
                .AddChoices(Enum.GetValues<StackMenuOption>())
                .UseConverter(choice => choice.GetDisplayName()));
    }
    
    private static FlashcardMenuOption ShowFlashcardMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<FlashcardMenuOption>()
                .Title("Choose one of the following options")
                .AddChoices(Enum.GetValues<FlashcardMenuOption>())
                .UseConverter(choice => choice.GetDisplayName()));
    }
    
    private static StudyMenuOption ShowStudyMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<StudyMenuOption>()
                .Title("Choose one of the following")
                .AddChoices(Enum.GetValues<StudyMenuOption>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    private async Task<bool> IsValidStackId(int stackId)
    {
        var getStacks = await _stackService.GetAllStacksAsync(_token).ConfigureAwait(false);

        if (!getStacks.IsSuccess)
        {
            UIOperationHelper.WriteResult(getStacks);
        }

        var stacks = getStacks.Value;

        if (stacks is not null)
        {
            return stacks.Any(stack => stack.Id == stackId);
        }

        return false;
    }

    private async Task<bool> IsValidFlashcardPosition(int flashcardPosition)
    {
        var getFlashcards = await _flashcardService.GetAllFlashcardsAsync(_token).ConfigureAwait(false);

        if (!getFlashcards.IsSuccess)
        {
            UIOperationHelper.WriteResult(getFlashcards);
        }

        var flashcards = getFlashcards.Value;

        if (flashcards is not null)
        {
            return flashcards.Any(flashcard => flashcard.Position == flashcardPosition);
        }
        return false;
    }
    

    private async Task<bool> InvalidStackIdMessage(int stackId)
    {
        var validStackId = await IsValidStackId(stackId).ConfigureAwait(false);

        if (!validStackId)
        {
            UIOperationHelper.DisplayMessage($"Invalid stack id there are no stacks with id = {stackId}");
            return false;
        }

        return true;
    }

    private async Task<bool> InvalidFlashcardIdMessage(int flashcardId)
    {
        var validFlashcardId = await IsValidFlashcardPosition(flashcardId).ConfigureAwait(false);


        if (!validFlashcardId)
        {
            UIOperationHelper.DisplayMessage($"There are no flashcards with id = {flashcardId}");
            return false;
        }

        return true;
    }
}