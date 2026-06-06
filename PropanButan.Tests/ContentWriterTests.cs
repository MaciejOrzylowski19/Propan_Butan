namespace PropanButan.Tests;

public class ContentWriterTests
{
    private readonly ContentWriter _sut = new();

    private static ClassInfo SimpleClass(string name, params (string type, string name)[] props) =>
        new()
        {
            Name = name,
            GenericTypes = [],
            Props = [.. props.Select(p => new PropInfo { Type = p.type, Name = p.name })]
        };

    [Fact]
    public void CreateTypeReport_EmptyList_ReturnsEmptyString()
    {
        var result = _sut.CreateTypeReport([]);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CreateTypeReport_SingleClass_ContainsExportInterface()
    {
        var cls = SimpleClass("Employee", ("number", "Id"), ("string", "FirstName"));
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("export interface Employee {", result);
    }

    [Fact]
    public void CreateTypeReport_PropName_ConvertedToCamelCase()
    {
        var cls = SimpleClass("Employee", ("number", "Id"), ("string", "FirstName"));
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("    id: number;", result);
        Assert.Contains("    firstName: string;", result);
    }

    [Fact]
    public void CreateTypeReport_ClassWithParent_GeneratesExtends()
    {
        var cls = SimpleClass("Manager", ("string", "Team"));
        cls.ParentClass = "Employee";
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("export interface Manager extends Employee {", result);
    }

    [Fact]
    public void CreateTypeReport_ClassWithoutParent_NoExtendsKeyword()
    {
        var cls = SimpleClass("Employee", ("number", "Id"));
        var result = _sut.CreateTypeReport([cls]);
        Assert.DoesNotContain("extends", result);
    }

    [Fact]
    public void CreateTypeReport_GenericClass_GeneratesGenericInterface()
    {
        var cls = SimpleClass("Repository", ("T[]", "Items"));
        cls.GenericTypes = ["T"];
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("export interface Repository<T> {", result);
    }

    [Fact]
    public void CreateTypeReport_MultipleGenerics_AllIncluded()
    {
        var cls = SimpleClass("Pair", ("K", "Key"), ("V", "Value"));
        cls.GenericTypes = ["K", "V"];
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("export interface Pair<K, V> {", result);
    }

    [Fact]
    public void CreateTypeReport_ReadOnlyProp_HasReadonlyKeyword()
    {
        var cls = new ClassInfo
        {
            Name = "Config",
            GenericTypes = [],
            Props = [new PropInfo { Name = "ApiUrl", Type = "string", IsReadOnly = true }]
        };
        var result = _sut.CreateTypeReport([cls]);
        Assert.Contains("    readonly apiUrl: string;", result);
    }

    [Fact]
    public void CreateTypeReport_MultipleClasses_AllPresent()
    {
        var classes = new List<ClassInfo>
        {
            SimpleClass("Employee", ("number", "Id")),
            SimpleClass("Address", ("string", "City")),
        };
        var result = _sut.CreateTypeReport(classes);
        Assert.Contains("export interface Employee", result);
        Assert.Contains("export interface Address", result);
    }

    [Fact]
    public void CreateAndWrite_WritesTextToFile()
    {
        var path = Path.GetTempFileName();
        try
        {
            ContentWriter.CreateAndWrite(path, "hello");
            Assert.Equal("hello", File.ReadAllText(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
