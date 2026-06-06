namespace PropanButan.Tests;

public class RecordPropScaperTests
{
    private readonly BaseContructorPropScaper _sut = new();

    private const string EmployeeDtoText =
        """
        public record EmployeeDto(
            int Id,
            string FirstName,
            string LastName,
            string Email,
            string Department,
            decimal Salary,
            DateTime HiredAt
        );
        """;

    private const string LoginRequestText =
        """
        public record LoginRequest(
            string Email,
            string Password
        );
        """;

    private const string RegisterRequestText =
        """
        public record RegisterRequest(
            string Username,
            string Email,
            string Password
        );
        """;

    private const string CreateBlogPostRequestText =
        """
        public record CreateBlogPostRequest(
            string Title,
            string Content,
            Guid AuthorId,
            List<string> Tags,
            bool Publish = false
        );
        """;

    private const string OrderItemDtoText =
        """
        public record OrderItemDto(
            Guid ProductId,
            string ProductName,
            int Quantity,
            decimal UnitPrice
        );
        """;

    [Fact]
    public void GetProps_EmployeeDto_ReturnsSevenProps()
    {
        var result = _sut.GetProps(EmployeeDtoText);
        Assert.Equal(7, result.Count);
    }

    [Fact]
    public void GetProps_LoginRequest_ReturnsTwoProps()
    {
        var result = _sut.GetProps(LoginRequestText);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetProps_RegisterRequest_ReturnsThreeProps()
    {
        var result = _sut.GetProps(RegisterRequestText);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetProps_CreateBlogPostRequest_ReturnsFiveProps()
    {
        var result = _sut.GetProps(CreateBlogPostRequestText);
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void GetProps_OrderItemDto_ReturnsFourProps()
    {
        var result = _sut.GetProps(OrderItemDtoText);
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetProps_EmployeeDto_FirstPropIsIdOfTypeInt()
    {
        var result = _sut.GetProps(EmployeeDtoText);
        Assert.Equal("Id", result[0].Name);
        Assert.Equal("int", result[0].Type);
    }

    [Fact]
    public void GetProps_EmployeeDto_AllNamesAreCorrect()
    {
        var result = _sut.GetProps(EmployeeDtoText);
        var names = result.Select(p => p.Name).ToList();
        Assert.Equal(["Id", "FirstName", "LastName", "Email", "Department", "Salary", "HiredAt"], names);
    }

    [Fact]
    public void GetProps_EmployeeDto_AllTypesAreCorrect()
    {
        var result = _sut.GetProps(EmployeeDtoText);
        Assert.Equal("int", result[0].Type);
        Assert.Equal("string", result[1].Type);
        Assert.Equal("decimal", result[5].Type);
        Assert.Equal("DateTime", result[6].Type);
    }

    [Fact]
    public void GetProps_LoginRequest_EmailIsFirstProp()
    {
        var result = _sut.GetProps(LoginRequestText);
        Assert.Equal("Email", result[0].Name);
        Assert.Equal("string", result[0].Type);
    }

    [Fact]
    public void GetProps_CreateBlogPostRequest_PublishHasDefaultValueFalse()
    {
        var result = _sut.GetProps(CreateBlogPostRequestText);
        var publishProp = result.First(p => p.Name == "Publish");
        Assert.Equal("false", publishProp.BaseValue);
    }

    [Fact]
    public void GetProps_OrderItemDto_QuantityIsOfTypeInt()
    {
        var result = _sut.GetProps(OrderItemDtoText);
        var qty = result.First(p => p.Name == "Quantity");
        Assert.Equal("int", qty.Type);
    }
}
