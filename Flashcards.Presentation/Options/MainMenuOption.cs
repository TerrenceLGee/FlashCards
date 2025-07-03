using System.ComponentModel.DataAnnotations;
namespace Flashcards.Presentation.Options;

public enum MainMenuOption
{
    [Display (Name = "Manage stacks")]
    ManageStacks,
    [Display (Name = "Manage flash cards")]
    ManageFlashcards,
    [Display (Name = "Start a study session")]
    Study,
    [Display (Name = "Exit the program")]
    Exit,
}