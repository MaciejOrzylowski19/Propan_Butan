namespace PropanButan.Tests;

public class FilePropScaperTests : IDisposable
{
    private readonly FileScaper _sut = new();
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose() => File.Delete(_tempFile);

    private const string TemplateClasses1Content =
        """
        namespace PropanButan.Tests.TemplateClasses;

        public class Employee
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public decimal Salary { get; set; }
            public DateTime HiredAt { get; set; }
        }

        public record EmployeeDto(
            int Id,
            string FirstName,
            string LastName,
            string Email,
            string Department,
            decimal Salary,
            DateTime HiredAt
        );

        public record CreateEmployeeRequest(
            string FirstName,
            string LastName,
            string Email,
            string Department,
            decimal Salary
        );

        public record UpdateEmployeeRequest(
            string? FirstName,
            string? LastName,
            string? Email,
            string? Department,
            decimal? Salary
        );

        public class Address
        {
            public int Id { get; set; }
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        public record AddressDto(
            int Id,
            string Street,
            string City,
            string PostalCode,
            string Country
        );
        """;

    private const string TemplateClasses3Content =
        """
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
        """;

    [Fact]
    public void ScapeFile_TemplateClasses1_ReturnsSixTypes()
    {
        File.WriteAllText(_tempFile, TemplateClasses1Content);
        var result = _sut.ScapeFile(_tempFile);
        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void ScapeFile_TemplateClasses1_ContainsEmployeeClass()
    {
        File.WriteAllText(_tempFile, TemplateClasses1Content);
        var result = _sut.ScapeFile(_tempFile);
        Assert.Contains(result, c => c.Name == "Employee");
    }

    [Fact]
    public void ScapeFile_TemplateClasses1_AllTypeNamesAreCorrect()
    {
        File.WriteAllText(_tempFile, TemplateClasses1Content);
        var result = _sut.ScapeFile(_tempFile);
        var names = result.Select(c => c.Name).ToList();
        Assert.Equal(["Employee", "EmployeeDto", "CreateEmployeeRequest", "UpdateEmployeeRequest", "Address", "AddressDto"], names);
    }

    [Fact]
    public void ScapeFile_TemplateClasses3_ReturnsEightTypes()
    {
        File.WriteAllText(_tempFile, TemplateClasses3Content);
        var result = _sut.ScapeFile(_tempFile);
        Assert.Equal(9, result.Count);
    }

    [Fact]
    public void ScapeFile_TemplateClasses3_AllTypeNamesAreCorrect()
    {
        File.WriteAllText(_tempFile, TemplateClasses3Content);
        var result = _sut.ScapeFile(_tempFile);
        var names = result.Select(c => c.Name).ToList();
        Assert.Equal(["User", "UserDto", "RegisterRequest", "LoginRequest", "LoginResponse", "BlogPost", "BlogPostDto", "CreateBlogPostRequest", "UpdateBlogPostRequest"], names);
    }

    [Fact]
    public void ScapeFile_EmptyFile_ReturnsEmptyList()
    {
        File.WriteAllText(_tempFile, string.Empty);
        var result = _sut.ScapeFile(_tempFile);
        Assert.Empty(result);
    }

    [Fact]
    public void ScapeFile_SingleClass_ReturnsSingleType()
    {
        File.WriteAllText(_tempFile, "public class Product { public Guid Id { get; set; } }");
        var result = _sut.ScapeFile(_tempFile);
        Assert.Single(result);
        Assert.Equal("Product", result[0].Name);
    }
}
