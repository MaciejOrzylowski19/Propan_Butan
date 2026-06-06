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
