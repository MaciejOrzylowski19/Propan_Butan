
using System.Text.RegularExpressions;

public class FileScaper
{
    private ClassScaper ClassPropScape = new();

    public List<ClassInfo> ScapeFile(string path)
    {
        var text = File.ReadAllText(path);
        var chunks = SplitToTypeChunks(text);

        var res = chunks.Select(e => ClassPropScape.ScapeClass(e)).ToList();

        return res;
    }

    private static List<string> SplitToTypeChunks(string text)
    {
        var matches = Regex.Matches(text,
            @"(?:(?:public|private|protected|internal|sealed|abstract|static|partial)\s+)*(?:record(?:\s+(?:class|struct))?|class|struct|interface|enum)\s+\w+");

        var result = new List<string>();
        for (var i = 0; i < matches.Count; i++)
        {
            var start = matches[i].Index;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : text.Length;
            result.Add(text[start..end].Trim());
        }
        return result;
    }
}