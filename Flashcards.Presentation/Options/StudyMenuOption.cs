using System.ComponentModel.DataAnnotations;
namespace Flashcards.Presentation.Options;

public enum StudyMenuOption
{
    [Display (Name = "Start a new study session")]
    StartStudySession,
    [Display (Name = "View all study sessions")]
    ViewAllStudySessions,
    [Display (Name = "View study session by stack id")]
    ViewStudySessionByStackId,
    [Display (Name = "Return to the main menu")]
    Back,
}