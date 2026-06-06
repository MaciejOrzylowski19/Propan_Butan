

using System.Text.RegularExpressions;

public class PropMapper
{

    private static Dictionary<string, string> _defalaultPropsMap = new();
    private static HashSet<GenericTypeTemplate> _genericPropMap = new();
    private static string _nullableMap = "null";

    public PropInfo MapProp(PropInfo prop)
    {
        var type = prop.Type;
        var target = prop.Clone();

        //Walenie na czysto

        if (_defalaultPropsMap.ContainsKey(type))
        {
            target.Type = type;
            return target;
        }
        
        var isNullable = false;
        if (type.Last() == '?')
        {
            isNullable = true;
            type = type[..^1];
        }

        if (_defalaultPropsMap.ContainsKey(type))
        {
            target.Type = type;
            if (isNullable)
            {
                target.Type += (" | " + _nullableMap);
            }
            return target;
        }
        


        var genericsMatch = Regex.Match(type, @"\b(?:record(?:\s+(?:class|struct))?|class|struct)\s+\w+\s*<([^>]+)>");
        var generics = genericsMatch.Success
            ? genericsMatch.Groups[1].Value.Split(',').Select(g => g.Trim()).ToList()
            : [];

        if (generics.Count == 0)
        {
            //To oznacza że typ jest jakiś anonimowy i trzeba go walić na URA
            target.Type = type;
            if (isNullable)
            {
                target.Type += (" | " + _nullableMap);
            }
            return target;
        }


        var template = _genericPropMap.First(e => e.Type.Split('<')[0] == type);

        if (template.GenericLen == generics.Count)
        {
            target.Type = PutIntoGeneric(generics, template.RawMap);
            if (isNullable)
            {
                target.Type += (" | " + _nullableMap);
            }
            return target;
        }
        else
        {
            throw new ArgumentException("Invalid generic name. Prop u wanted to map has different generic type len from ts template");
        }
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