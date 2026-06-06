namespace PropanButan.Tests;

public class ClassPropScaperTests
{
    private readonly ClassPropScape _sut = new();

    private const string EmployeeClassBody =
        """
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public decimal Salary { get; set; }
            public DateTime HiredAt { get; set; }
        }
        """;

    private const string AddressClassBody =
        """
        {
            public int Id { get; set; }
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }
        """;

    private const string BlogPostClassBody =
        """
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
        """;

    [Fact]
    public void GetProps_EmployeeClassBody_ReturnsSevenProps()
    {
        var result = _sut.GetProps(EmployeeClassBody);
        Assert.Equal(7, result.Count);
    }

    [Fact]
    public void GetProps_AddressClassBody_ReturnsFiveProps()
    {
        var result = _sut.GetProps(AddressClassBody);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void GetProps_BlogPostClassBody_ReturnsNineProps()
    {
        var result = _sut.GetProps(BlogPostClassBody);
        Assert.Equal(9, result.Count);
    }

    [Fact]
    public void GetProps_EmployeeClassBody_FirstPropIsIdOfTypeInt()
    {
        var result = _sut.GetProps(EmployeeClassBody);
        Assert.Equal("Id", result[0].Name);
        Assert.Equal("int", result[0].Type);
    }

    [Fact]
    public void GetProps_EmployeeClassBody_SalaryIsOfTypeDecimal()
    {
        var result = _sut.GetProps(EmployeeClassBody);
        var salary = result.First(p => p.Name == "Salary");
        Assert.Equal("decimal", salary.Type);
    }

    [Fact]
    public void GetProps_EmployeeClassBody_FirstNameHasDefaultValue()
    {
        var result = _sut.GetProps(EmployeeClassBody);
        var firstName = result.First(p => p.Name == "FirstName");
        Assert.Equal("string.Empty", firstName.BaseValue);
    }

    [Fact]
    public void GetProps_EmployeeClassBody_AllNamesAreCorrect()
    {
        var result = _sut.GetProps(EmployeeClassBody);
        var names = result.Select(p => p.Name).ToList();
        Assert.Equal(["Id", "FirstName", "LastName", "Email", "Department", "Salary", "HiredAt"], names);
    }

    [Fact]
    public void GetProps_BlogPostClassBody_NullablePublishedAtIsIncluded()
    {
        var result = _sut.GetProps(BlogPostClassBody);
        var publishedAt = result.First(p => p.Name == "PublishedAt");
        Assert.Equal("DateTime?", publishedAt.Type);
    }
}
