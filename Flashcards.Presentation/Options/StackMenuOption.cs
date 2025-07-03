using System.ComponentModel.DataAnnotations;
namespace Flashcards.Presentation.Options;

public enum StackMenuOption
{
    [Display (Name = "Create a new stack")]
    CreateStack,
    [Display (Name = "Rename a stack")]
    RenameStack,
    [Display (Name = "Delete a stack")]
    DeleteStack,
    [Display (Name = "List all available stacks")]
    ListAllStacks,
    [Display (Name = "Return to the main menu")]
    Back,
}