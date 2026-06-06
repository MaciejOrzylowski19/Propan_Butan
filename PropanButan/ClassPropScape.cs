
using static System.Text.RegularExpressions.Regex;
class ClassPropScape : IPropScaper
{
    public List<PropInfo> GetProps(string text)
    {
        List<PropInfo> target = [];
        var rawProps = GetRawProps(text);

        foreach (var raw in rawProps)
        {
            var prop = new PropInfo();
            var left = raw;

            var eqIndex = raw.IndexOf("=");
            if (eqIndex != -1)
            {
                prop.BaseValue = raw[(eqIndex + 1)..].Trim();
                left = raw[..eqIndex];
            }

            var tokens = left.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2)
                continue;

            prop.Name = tokens[^1];
            prop.Type = tokens[^2];

            var modifiers = tokens[..^2];
            prop.IsReadOnly = modifiers.Contains("readonly");
            prop.Visibility = modifiers.FirstOrDefault(m => m is "public" or "private" or "protected" or "internal")
                              ?? string.Empty;

            target.Add(prop);
        }

        return target;
    }

    private static List<string> GetRawProps(string text)
    {
        var start = text.IndexOf('{');
        start = start == -1 ? 0 : start;
        var end = text.LastIndexOf('}');
        end = end == -1 ? text.Length - 1 : end;
        var body = text.Substring(start + 1, end - start - 1);

        var blockProps = Matches(body, @"[\w<>\[\]?]+\s+\w+\s*\{[^{}]*get[^{}]*\}(?:\s*=[^;{]*)?");
        var expressionProps = Matches(body, @"[\w<>\[\]?]+\s+\w+\s*=>[^;]+;");
        var fieldProps = Matches(body, @"[\w<>\[\]?]+\s+\w+\s*(?:=[^;{]*)?\s*;");
        return [..blockProps.Select(m => StripProp(m.Value)), ..expressionProps.Select(m => StripProp(m.Value)), ..fieldProps.Select(m => StripProp(m.Value))];
    }

    private static readonly string[] UnwantedKeywords = ["static", "abstract", "virtual", "override", "sealed", "new", "extern"];

    private static string StripProp(string raw)
    {
        var baseValue = string.Empty;
        var eqMatch = Match(raw, @"(?<!=)=(?!>)\s*([^;{]+)");
        if (eqMatch.Success)
            baseValue = eqMatch.Groups[1].Value.Trim();

        var result = Replace(raw, @"\{[^{}]*\}", "");
        result = Replace(result, "=>[^;]+", "");
        result = Replace(result, "(?<!=)=[^;]+", "");
        result = Replace(result, ";", "");

        foreach (var kw in UnwantedKeywords)
            result = Replace(result, $@"\b{kw}\b", "");

        result = Replace(result, @"\s+", " ").Trim();

        return baseValue.Length > 0 ? $"{result} = {baseValue}" : result;
    }
}
