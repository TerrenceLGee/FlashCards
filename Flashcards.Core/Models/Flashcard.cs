using System.Diagnostics.CodeAnalysis;

namespace Flashcards.Core.Models;

public class Flashcard
{
    public int Id { get; set; }
    public int StackId { get; set; }
    public required string Front { get; set; }
    public required string Back { get; set; }
    public int Position { get; set; }
    
    protected Flashcard(){}

    [SetsRequiredMembers]
    public Flashcard(int stackId, string front, string back, int position)
    {
        StackId = stackId;
        Front = front;
        Back = back;
        Position = position;
    }

    [SetsRequiredMembers]
    public Flashcard(int id, int stackId, string front, string back, int position)
        : this(stackId, front, back, position)
    {
        Id = id;
    }
}