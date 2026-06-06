namespace PropanButan.Tests;

public class PropMapperTests : IDisposable
{
    private readonly PropMapper _sut = new();
    private readonly string _tempFile = Path.GetTempFileName();

    public PropMapperTests()
    {
        File.WriteAllText(_tempFile,
            """
            ? : undefined
            int : number
            long : number
            string : string
            bool : boolean
            DateTime : Date
            Guid : string
            List<T> : T[]
            Dictionary<K,V> : Record<K, V>
            """);
        PropMapper.LoadMap(_tempFile);
    }

    public void Dispose() => File.Delete(_tempFile);

    private static PropInfo Prop(string type, string name = "TestProp") =>
        new() { Type = type, Name = name, BaseValue = null };

    [Fact]
    public void MapProp_KnownSimpleType_MapsToTsType()
    {
        var result = _sut.MapProp(Prop("int"));
        Assert.Equal("number", result.Type);
    }

    [Fact]
    public void MapProp_StringType_MapsToString()
    {
        var result = _sut.MapProp(Prop("string"));
        Assert.Equal("string", result.Type);
    }

    [Fact]
    public void MapProp_DateTimeType_MapsToDate()
    {
        var result = _sut.MapProp(Prop("DateTime"));
        Assert.Equal("Date", result.Type);
    }

    [Fact]
    public void MapProp_GuidType_MapsToString()
    {
        var result = _sut.MapProp(Prop("Guid"));
        Assert.Equal("string", result.Type);
    }

    [Fact]
    public void MapProp_NullableKnownType_AppendUndefined()
    {
        var result = _sut.MapProp(Prop("int?"));
        Assert.Equal("number | undefined", result.Type);
    }

    [Fact]
    public void MapProp_NullableString_AppendUndefined()
    {
        var result = _sut.MapProp(Prop("string?"));
        Assert.Equal("string | undefined", result.Type);
    }

    [Fact]
    public void MapProp_UnknownType_ReturnsTypeAsIs()
    {
        var result = _sut.MapProp(Prop("SomeCustomClass"));
        Assert.Equal("SomeCustomClass", result.Type);
    }

    [Fact]
    public void MapProp_PreservesName()
    {
        var result = _sut.MapProp(Prop("int", "Salary"));
        Assert.Equal("Salary", result.Name);
    }

    [Fact]
    public void MapProp_PreservesBaseValue()
    {
        var prop = new PropInfo { Type = "int", Name = "Count", BaseValue = "0" };
        var result = _sut.MapProp(prop);
        Assert.Equal("0", result.BaseValue);
    }

    [Fact]
    public void MapProp_DoesNotMutateOriginalProp()
    {
        var original = Prop("int", "Id");
        _sut.MapProp(original);
        Assert.Equal("int", original.Type);
    }

    [Fact]
    public void LoadMap_SimpleTypes_SetsDefaultMap()
    {
        var result = _sut.MapProp(Prop("bool"));
        Assert.Equal("boolean", result.Type);
    }

    [Fact]
    public void LoadMap_NullableMarker_SetsNullableString()
    {
        var result = _sut.MapProp(Prop("bool?"));
        Assert.Contains("undefined", result.Type);
    }

    [Fact]
    public void LoadMap_SkipsCommentLines()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "# comment\nfoo : bar");
            PropMapper.LoadMap(path);
            var result = _sut.MapProp(Prop("foo"));
            Assert.Equal("bar", result.Type);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadMap_SkipsBlankLines()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "\n\nbaz : qux\n\n");
            PropMapper.LoadMap(path);
            var result = _sut.MapProp(Prop("baz"));
            Assert.Equal("qux", result.Type);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void MapProp_ListOfString_MapsToStringArray()
    {
        var result = _sut.MapProp(Prop("List<string>"));
        Assert.Equal("string[]", result.Type);
    }

    [Fact]
    public void MapProp_ListOfInt_MapsToNumberArray()
    {
        var result = _sut.MapProp(Prop("List<int>"));
        Assert.Equal("number[]", result.Type);
    }

    [Fact]
    public void MapProp_DictionaryStringInt_MapsToRecord()
    {
        var result = _sut.MapProp(Prop("Dictionary<string,int>"));
        Assert.Equal("Record<string, number>", result.Type);
    }

    [Fact]
    public void MapProp_NullableListOfString_MapsToStringArrayWithUndefined()
    {
        var result = _sut.MapProp(Prop("List<string>?"));
        Assert.Equal("string[] | undefined", result.Type);
    }

    [Fact]
    public void MapProp_UnknownGenericType_ReturnsAsIs()
    {
        var result = _sut.MapProp(Prop("CustomCollection<string>"));
        Assert.Equal("CustomCollection<string>", result.Type);
    }
}
