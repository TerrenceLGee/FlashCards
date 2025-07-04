namespace Flashcards.Core.Models;

public class StudySession
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public DateTime Date { get; set; }
    public int TotalQuestions { get; set; }
    public int Score { get; set; }
    
    
    protected StudySession(){}

    public StudySession(int stackId, DateTime date, int totalQuestions, int score)
    {
        StackId = stackId;
        Date = date;
        TotalQuestions = totalQuestions;
        Score = score;
    }

    public StudySession(int id, int stackId, DateTime date, int totalQuestions, int score)
        : this(stackId, date, totalQuestions, score)
    {
        Id = id;
    }
}