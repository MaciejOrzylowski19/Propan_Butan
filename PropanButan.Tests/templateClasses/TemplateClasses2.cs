namespace PropanButan.Tests.TemplateClasses;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category
);

public class Order
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
}

public record OrderDto(
    Guid Id,
    int CustomerId,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    string Status,
    DateTime OrderedAt
);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);

public record CreateOrderRequest(
    int CustomerId,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(
    Guid ProductId,
    int Quantity
);
