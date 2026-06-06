namespace PropanButan.Tests.TemplateClasses;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    bool IsEmailConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);

public record RegisterRequest(
    string Username,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public List<string> Tags { get; set; } = [];
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public record BlogPostDto(
    int Id,
    string Title,
    string Slug,
    string Content,
    Guid AuthorId,
    List<string> Tags,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? PublishedAt
);

public record CreateBlogPostRequest(
    string Title,
    string Content,
    Guid AuthorId,
    List<string> Tags,
    bool Publish = false
);

public record UpdateBlogPostRequest(
    string? Title,
    string? Content,
    List<string>? Tags,
    bool? Publish
);
