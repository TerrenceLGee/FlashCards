using System.ComponentModel.DataAnnotations;
namespace Flashcards.Presentation.Options;

public enum FlashcardMenuOption
{
    [Display (Name = "Add a new flash card")]
    AddFlashcard,
    [Display (Name = "Edit a flash card")]
    EditFlashcard,
    [Display (Name = "Delete a flash card")]
    DeleteFlashcard,
    [Display (Name = "Delete a flash card by stack id")]
    DeleteFlashcardByStackId,
    [Display (Name = "Show all flash cards")]
    ListAllFlashcards,
    [Display (Name = "Return to the main menu")]
    Back,
}
















































