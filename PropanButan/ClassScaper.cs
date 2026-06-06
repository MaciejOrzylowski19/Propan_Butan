
using System.Text.RegularExpressions;

public class ClassScaper : IClassScaper
{

    private ClassPropScape clScaper = new ClassPropScape();
    private BaseContructorPropScaper bsScaper = new();


    public ClassInfo ScapeClass(string text)
    {
        List<PropInfo> propTargetList = new();

        var name = Regex.Match(text, @"\b(?:record(?:\s+(?:class|struct))?|class|struct)\s+(\w+)").Groups[1].Value;
        
        var genericsMatch = Regex.Match(text, @"\b(?:record(?:\s+(?:class|struct))?|class|struct)\s+\w+\s*<([^>]+)>");
        var generics = genericsMatch.Success
            ? genericsMatch.Groups[1].Value.Split(',').Select(g => g.Trim()).ToList()
            : [];

        var splitIndex = text.IndexOf('{');
        var headerText = splitIndex != -1 ? text[..splitIndex] : text;
        var parentMatch = Regex.Match(headerText,
            @"\b(?:record(?:\s+(?:class|struct))?|class|struct)\b[^:{(]*(?:\([^)]*\))?\s*:\s*(\w+)");
        var parentClass = parentMatch.Success ? parentMatch.Groups[1].Value : null;

        if (splitIndex != -1)
        {
            var recordText = text[..splitIndex];
            if (recordText.Contains('('))
            {
                var props = bsScaper.GetProps(recordText);
                propTargetList.AddRange(props);
            }
            var classText = text[splitIndex..];
            var clProps = clScaper.GetProps(classText);
            propTargetList.AddRange(clProps);
        }
        else
        {
            var props = bsScaper.GetProps(text);
            propTargetList.AddRange(props);
        }
        return new()
        {
            GenericTypes = generics,
            Name = name,
            Props = propTargetList,
            ParentClass = parentClass,
        };
    }
}

