namespace PropanButan;

public class ScraperManager
{
    public ContentWriter Writer { get; } = new();

    private static int _limit = 20;

    public static void LoadTemplate(string path) => PropMapper.LoadMap(path);

    public void MapFile(string path, string target)
    {
        var classes = Writer.LoadFile(path);
        var report = Writer.CreateTypeReport(classes);
        ContentWriter.CreateAndWrite(target, report);
    }

    public void MapFile(string path)
    {
        var target = path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
            ? path[..^3] + ".ts"
            : path + ".ts";
        MapFile(path, target);
    }

    public void MapDirectory(string directory)
    {
        var files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
        var dirs = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);
        var total = files.Length + dirs.Length;

        if (total > _limit)
            throw new InvalidOperationException(
                $"Directory contains {total} files/folders, which exceeds the limit of {_limit}.");

        MapDirectoryRecursive(directory);
    }

    public void MapDirectory(string sourceDirectory, string targetDirectory)
    {
        var files = Directory.GetFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories);
        var dirs = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
        var total = files.Length + dirs.Length;

        if (total > _limit)
            throw new InvalidOperationException(
                $"Directory contains {total} files/folders, which exceeds the limit of {_limit}.");

        MapDirectoryRecursive(sourceDirectory, sourceDirectory, targetDirectory);
    }

    private void MapDirectoryRecursive(string directory)
    {
        foreach (var file in Directory.GetFiles(directory, "*.cs"))
            MapFile(file);

        foreach (var subDir in Directory.GetDirectories(directory))
            MapDirectoryRecursive(subDir);
    }

    private void MapDirectoryRecursive(string sourceRoot, string currentDirectory, string targetRoot)
    {
        foreach (var file in Directory.GetFiles(currentDirectory, "*.cs"))
        {
            var relativePath = Path.GetRelativePath(sourceRoot, file);
            var targetFile = Path.Combine(targetRoot, Path.ChangeExtension(relativePath, ".ts"));
            MapFile(file, targetFile);
        }

        foreach (var subDir in Directory.GetDirectories(currentDirectory))
            MapDirectoryRecursive(sourceRoot, subDir, targetRoot);
    }
}