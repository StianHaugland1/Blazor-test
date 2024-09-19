using System.ComponentModel.DataAnnotations;

public record PlayerDto
{
    public required string Id { get; set; }
    public string Name { get; set; } = "Name not found";

    public string Nickname { get; set; } = "Player";
    
    [Required(ErrorMessage = "Emoji is required.")]
    [EmojiValidation(ErrorMessage = "Invalid emoji.")]
    public string Emoji { get; set; } = "🤓";
    
    public  int Wins { get; set; }

    public  int Losses { get; set; }
    
    public  int TotalMatches { get; set; }

    public  int Rating { get; set; }
}