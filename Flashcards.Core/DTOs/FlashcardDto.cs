namespace Flashcards.Core.DTOs;

public class FlashcardDto
{
    public int Id { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public int Position { get; set; }

    public FlashcardDto(int id, string front, string back, int position)
    {
        Id = id;
        Front = front;
        Back = back;
        Position = position;
    }
}