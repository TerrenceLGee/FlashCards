using Flashcards.Domain.Interfaces;
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

        while (isRunning)
        {
            switch (ShowMainMenu())
            {
                case MainMenuOption.ManageStacks:
                    await HandleStackMenu();
                    break;
                case MainMenuOption.ManageFlashcards:
                    await HandleFlashcardMenu();
                    break;
                case MainMenuOption.Study:
                    await HandleStudySessionMenu();
                    break;
                case MainMenuOption.Exit:
                    isRunning = false;
                    break;
            }
        }
    }

    private async Task HandleStackMenu()
    {
        
    }

    private async Task HandleFlashcardMenu()
    {
        
    }

    private async Task HandleStudySessionMenu()
    {
        
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
    
}