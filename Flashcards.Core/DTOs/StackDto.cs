namespace Flashcards.Core.DTOs;

public class StackDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<FlashcardDto> Cards { get; set; }

    public StackDto(int id, string name)
    {
        Id = id;
        Name = name;
        Cards = new List<FlashcardDto>();
    }
}