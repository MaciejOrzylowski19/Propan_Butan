namespace PropanButan.Tests;

public class ClassScaperTests
{
    private readonly ClassScaper _sut = new();

    private const string EmployeeClassText =
        """
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
        """;

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

    private const string UserDtoText =
        """
        public record UserDto(
            Guid Id,
            string Username,
            string Email,
            string Role,
            bool IsEmailConfirmed,
            DateTime CreatedAt,
            DateTime? LastLoginAt
        );
        """;

    private const string OrderClassText =
        """
        public class Order
        {
            public Guid Id { get; set; }
            public int CustomerId { get; set; }
            public List<OrderItemDto> Items { get; set; } = [];
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime OrderedAt { get; set; }
        }
        """;

    private const string LoginResponseText =
        """
        public record LoginResponse(
            string AccessToken,
            string RefreshToken,
            DateTime ExpiresAt,
            UserDto User
        );
        """;

    private const string UpdateEmployeeRequestText =
        """
        public record UpdateEmployeeRequest(
            string? FirstName,
            string? LastName,
            string? Email,
            string? Department,
            decimal? Salary
        );
        """;

    [Theory]
    [InlineData(EmployeeClassText, "Employee")]
    [InlineData(EmployeeDtoText, "EmployeeDto")]
    [InlineData(UserDtoText, "UserDto")]
    [InlineData(OrderClassText, "Order")]
    [InlineData(LoginResponseText, "LoginResponse")]
    [InlineData(UpdateEmployeeRequestText, "UpdateEmployeeRequest")]
    public void ScapeClass_ReturnsCorrectName(string text, string expectedName)
    {
        var result = _sut.ScapeClass(text);
        Assert.Equal(expectedName, result.Name);
    }

    [Fact]
    public void ScapeClass_Employee_ReturnsNoGenericTypes()
    {
        var result = _sut.ScapeClass(EmployeeClassText);
        Assert.Empty(result.GenericTypes);
    }

    [Fact]
    public void ScapeClass_EmployeeDto_ReturnsNoGenericTypes()
    {
        var result = _sut.ScapeClass(EmployeeDtoText);
        Assert.Empty(result.GenericTypes);
    }

    [Fact]
    public void ScapeClass_EmployeeDto_ReturnsSevenProps()
    {
        var result = _sut.ScapeClass(EmployeeDtoText);
        Assert.Equal(7, result.Props.Count);
    }

    [Fact]
    public void ScapeClass_Employee_ReturnsSevenProps()
    {
        var result = _sut.ScapeClass(EmployeeClassText);
        Assert.Equal(7, result.Props.Count);
    }

    [Fact]
    public void ScapeClass_LoginResponse_ReturnsFourProps()
    {
        var result = _sut.ScapeClass(LoginResponseText);
        Assert.Equal(4, result.Props.Count);
    }

    [Fact]
    public void ScapeClass_Order_ReturnsSixProps()
    {
        var result = _sut.ScapeClass(OrderClassText);
        Assert.Equal(6, result.Props.Count);
    }

    [Fact]
    public void ScapeClass_EmployeeDto_FirstPropIsIdOfTypeInt()
    {
        var result = _sut.ScapeClass(EmployeeDtoText);
        Assert.Equal("Id", result.Props[0].Name);
        Assert.Equal("int", result.Props[0].Type);
    }

    [Fact]
    public void ScapeClass_UserDto_AllPropNamesAreCorrect()
    {
        var result = _sut.ScapeClass(UserDtoText);
        var names = result.Props.Select(p => p.Name).ToList();
        Assert.Equal(["Id", "Username", "Email", "Role", "IsEmailConfirmed", "CreatedAt", "LastLoginAt"], names);
    }

    [Fact]
    public void ScapeClass_UpdateEmployeeRequest_AllPropsAreNullable()
    {
        var result = _sut.ScapeClass(UpdateEmployeeRequestText);
        Assert.All(result.Props, p => Assert.EndsWith("?", p.Type));
    }

    [Fact]
    public void ScapeClass_ClassWithoutInheritance_ParentClassIsNull()
    {
        var result = _sut.ScapeClass(EmployeeClassText);
        Assert.Null(result.ParentClass);
    }

    [Fact]
    public void ScapeClass_RecordWithoutInheritance_ParentClassIsNull()
    {
        var result = _sut.ScapeClass(EmployeeDtoText);
        Assert.Null(result.ParentClass);
    }

    [Theory]
    [InlineData("public class Manager : Employee { public string Team { get; set; } }", "Employee")]
    [InlineData("public class AdminUser : User { public string Permissions { get; set; } }", "User")]
    [InlineData("public record ManagerDto(string Team) : EmployeeDto;", "EmployeeDto")]
    [InlineData("public class OrderService : BaseService { }", "BaseService")]
    [InlineData("public class Repository<T> : IRepository<T> { }", "IRepository")]
    public void ScapeClass_ClassWithInheritance_ReturnsParentClassName(string text, string expectedParent)
    {
        var result = _sut.ScapeClass(text);
        Assert.Equal(expectedParent, result.ParentClass);
    }

    [Fact]
    public void ScapeClass_ClassInheritsFromMultiple_ReturnsFirstBaseOnly()
    {
        const string text = "public class AdminUser : User, IAuditable { public string Permissions { get; set; } }";
        var result = _sut.ScapeClass(text);
        Assert.Equal("User", result.ParentClass);
    }}
