namespace testDtos;

public class testDto
{
    public int wiek { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public List<string?> Tags { get; set; } = [];
}