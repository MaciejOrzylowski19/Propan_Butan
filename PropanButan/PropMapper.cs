

using System.Text.RegularExpressions;

public class PropMapper
{

    private static Dictionary<string, string> _defalaultPropsMap = new();
    private static HashSet<GenericTypeTemplate> _genericPropMap = new();
    private static string _nullableMap = "null";

    public static PropInfo MapProp(PropInfo prop)
    {
        var target = prop.Clone();
        target.Type = MapPropType(prop.Type);
        return target;
    }

    public static string MapPropType(string type) => MapPropType(type, out _);

    public static string MapPropType(string type, out bool isCombinated)
    {
        isCombinated = false;
        if (_defalaultPropsMap.TryGetValue(type, out var mapped))
            return mapped;

        var isNullable = false;
        if (type.Last() == '?')
        {
            isNullable = true;
            type = type[..^1];
            isCombinated = true;
        }

        if (_defalaultPropsMap.TryGetValue(type, out mapped))
            return isNullable ? mapped + " | " + _nullableMap : mapped;

        var genericsMatch = Regex.Match(type, @"\w+<(.+)>");
        var generics = genericsMatch.Success
            ? genericsMatch.Groups[1].Value.Split(',').Select(g => g.Trim()).ToList()
            : [];

        if (generics.Count == 0)
            return isNullable ? type + " | " + _nullableMap : type;

        var baseType = type[..type.IndexOf('<')];
        var template = _genericPropMap.FirstOrDefault(e => e.Type == baseType);

        if (template is null)
            return isNullable ? type + " | " + _nullableMap : type;

        if (template.GenericLen != generics.Count)
            throw new ArgumentException("Invalid generic name. Prop u wanted to map has different generic type len from ts template");
        isCombinated = true;

        var mappedGenerics = new List<string>(generics.Count);
        foreach (var g in generics)
        {
            var res = MapPropType(g, out var innerCombinated);
            if (innerCombinated)
            {
                res = "(" + res + ")";
            }
            mappedGenerics.Add(res);
        }

        var result = PutIntoGeneric(mappedGenerics, template.RawMap);
        return isNullable ? result + " | " + _nullableMap : result;
    }

    private static string PutIntoGeneric(List<string> generics, string rawText)
    {
        return Regex.Replace(rawText, @"%%(\d)%%", m => generics[int.Parse(m.Groups[1].Value)]);
    }

    public static void LoadMap(string path)
    {
        foreach (var rawLine in File.ReadLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var parts = line.Split(':', 2);
            if (parts.Length < 2)
                continue;

            var left = parts[0].Trim();
            var right = parts[1].Trim();

            if (left == "?")
            {
                _nullableMap = right;
                continue;
            }

            var genericStart = left.IndexOf('<');
            if (genericStart == -1)
            {
                _defalaultPropsMap[left] = right;
                continue;
            }

            var typeName = left[..genericStart];
            var genericParams = left[(genericStart + 1)..].TrimEnd('>')
                .Split(',')
                .Select(p => p.Trim())
                .ToList();

            var rawMap = right;
            for (var i = 0; i < genericParams.Count; i++)
                rawMap = Regex.Replace(rawMap, $@"\b{Regex.Escape(genericParams[i])}\b", $"%%{i}%%");

            _genericPropMap.Add(new GenericTypeTemplate
            {
                Type = typeName,
                GenericLen = genericParams.Count,
                RawMap = rawMap,
            });
        }
    }


}

public class GenericTypeTemplate
{
    public string Type { get; set; }
    public int GenericLen { get; set; }
    public string RawMap { get; set; }
}