namespace Flashcards.Core.Models;

public class StudySession
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public DateTime Date { get; set; }
    public int Score { get; set; }
    
    protected StudySession(){}

    public StudySession(int stackId, DateTime date, int score)
    {
        StackId = stackId;
        Date = date;
        Score = score;
    }

    public StudySession(int id, int stackId, DateTime date, int score)
        : this(stackId, date, score)
    {
        Id = id;
    }
}