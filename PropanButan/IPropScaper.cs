
public interface IPropScaper
{

    List<PropInfo> GetProps(string text); 
}

public class PropInfo : ICloneable
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string? BaseValue { get; set; }
    public string? Visibility { get; set; }
    public bool IsReadOnly { get; set; } = false;

    object ICloneable.Clone() => Clone();

    public PropInfo Clone() => new()
    {
        Name = Name,
        Type = Type,
        BaseValue = BaseValue,
        Visibility = Visibility,
        IsReadOnly = IsReadOnly,
    };
}

public class ClassInfo
{
    public string? ParentClass { get; set; } = null;
    public string Name { get; set; }
    public List<string> GenericTypes { get; set; }
    public List<PropInfo> Props { get; set; } = [];

}