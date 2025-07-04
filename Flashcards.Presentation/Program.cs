using Flashcards.Core.Interfaces;
using FlashCards.DataAccess.Initialization;
using FlashCards.DataAccess.Interfaces;
using FlashCards.DataAccess.Repositories;
using Flashcards.Domain.Interfaces;
using Flashcards.Domain.Services;
using Flashcards.Presentation.App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


await Startup();

return;

async Task  Startup()
{
    var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(loggingDirectory);
    string filePath = Path.Combine(loggingDirectory, "app-.txt");

    string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
            path: filePath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate)
        .CreateLogger();

    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    IServiceCollection services = new ServiceCollection()
        .AddSingleton(configuration)
        .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
        .AddSingleton<IDatabaseInitializer, DatabaseInitializer>()
        .AddSingleton<IStackRepository, StackRepository>()
        .AddSingleton<IFlashcardRepository, FlashcardRepository>()
        .AddSingleton<IStudySessionRepository, StudySessionRepository>()
        .AddSingleton<IStackService, StackService>()
        .AddSingleton<IFlashcardService, FlashcardService>()
        .AddSingleton<IStudySessionService, StudySessionService>();

    IServiceProvider serviceProvider = services.BuildServiceProvider();

    IDatabaseInitializer? databaseInitializer = serviceProvider.GetService<IDatabaseInitializer>();

    if (databaseInitializer is not null)
    {
        await databaseInitializer.InitializeDatabaseAsync();
    }

    using var tokenSource = new CancellationTokenSource();

    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        tokenSource.Cancel();
    };

    IStackService? stackService = serviceProvider.GetService<IStackService>();
    IFlashcardService? flashcardService = serviceProvider.GetService<IFlashcardService>();
    IStudySessionService? studySessionService = serviceProvider.GetService<IStudySessionService>();

    if (stackService is not null && flashcardService is not null && studySessionService is not null)
    {
        FlashCardUI flashCardUI =
            new FlashCardUI(stackService, flashcardService, studySessionService, tokenSource.Token);
        await flashCardUI.Run();
    }

    Log.CloseAndFlush();
}

