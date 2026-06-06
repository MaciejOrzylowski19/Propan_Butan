using System.Text.RegularExpressions;

public class BaseContructorPropScaper : IPropScaper
{
    public List<PropInfo> GetProps(string text)
    {
        List<PropInfo> t = [];

        var start = text.IndexOf('(');
        var end = text.LastIndexOf(')');
        var target = text.Substring(start + 1, end - start + 1);
        target = Regex.Replace(target, @"/\*.*?\*/", "", RegexOptions.Singleline);
        target = Regex.Replace(target, @"//[^\n]*", "");
        target = Regex.Replace(target, @"\s+", " ");


        var chunks = target.Split(",").ToList();
        foreach (var item in chunks)
        {
            var prop = new PropInfo();

            prop.Visibility = string.Empty;
            if (item.Contains("="))
            {
                var index = item.IndexOf('=');
                var left = item[..index];
                var right = item[(index + 1)..].TrimStart();

            }
            var splited = item.Split(" ").ToList();
            if (splited.Contains("="))
            {
                prop.BaseValue = splited.Last();
                splited.Remove("=");
                splited.RemoveAt(splited.Count - 1);
            }
            prop.Name = splited[0];
            prop.Type = splited[1];

            t.Add(prop);
        }
        return t;
    }
}


public record Czacza(
    int Something,
    DateTime Date,
    int Wiek=1,
    string Name = "ALA"
);