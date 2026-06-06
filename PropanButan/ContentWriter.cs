using System.Text;

public class ContentWriter
{
    public string CreateTypeReport(List<ClassInfo> classes)
    {
        var sb = new StringBuilder();

        foreach (var cls in classes)
        {
            sb.Append("export interface ");
            sb.Append(cls.Name);

            if (cls.GenericTypes.Count > 0)
            {
                sb.Append('<');
                sb.Append(string.Join(", ", cls.GenericTypes));
                sb.Append('>');
            }

            if (cls.ParentClass is not null)
            {
                sb.Append(" extends ");
                sb.Append(cls.ParentClass);
            }

            sb.AppendLine(" {");

            foreach (var prop in cls.Props)
            {
                sb.Append("    ");

                if (prop.IsReadOnly)
                    sb.Append("readonly ");

                sb.Append(ToCamelCase(prop.Name));
                sb.Append(": ");
                sb.Append(prop.Type);
                sb.AppendLine(";");
            }

            sb.AppendLine("};");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public List<ClassInfo> LoadFile(string path)
    {
        var classes = _scaper.ScapeFile(path);

        foreach (var cls in classes)
            cls.Props = [.. cls.Props.Select(PropMapper.MapProp)];
        return classes;
    }

    public static void CreateAndWrite(string path, string text)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, text);
    }

    private static string ToCamelCase(string name) =>
        name.Length == 0 ? name : char.ToLower(name[0]) + name[1..];

    private readonly FileScaper _scaper = new();
}
