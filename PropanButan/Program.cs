using PropanButan;
using System.CommandLine;

internal class Program
{
    private const string DefaultMarker = "& & &";

    private static int Main(string[] args)
    {
        var targetOption = new Option<string>("-t", "--target")
        {
            Description = "Target you want to map. It can be file or folder containing .cs files.",
            Required = true
        };

        var distOption = new Option<string>("-d", "--dist")
        {
            Description = "Destination folder or file name (depend on if target is folder or file). Base value is name of a target + .ts",
            DefaultValueFactory = _ => DefaultMarker,
            Required = false
        };

        var templateOption = new Option<string>("-e", "--template")
        {
            Description = "Declares template on wich props should be mapped. Using predeclared template file on default.",
            DefaultValueFactory = _ => DefaultMarker,
            Required = false
        };

        var rootCommand = new RootCommand("Mój CLI do budowania");
        rootCommand.Options.Add(targetOption);
        rootCommand.Options.Add(distOption);
        rootCommand.Options.Add(templateOption);

        rootCommand.SetAction(data =>
        {
            var sourcePath = data.GetValue(targetOption)!;
            var destinationPath = data.GetValue(distOption);
            var templatePath = data.GetValue(templateOption);

            var scraperManager = new ScraperManager();

            templatePath = templatePath == DefaultMarker
                ? Path.Combine(AppContext.BaseDirectory, "template.txt")
                : templatePath;

            ScraperManager.LoadTemplate(templatePath!);

            var sourceIsFile = File.Exists(sourcePath);
            var sourceIsDirectory = Directory.Exists(sourcePath);

            if (!sourceIsFile && !sourceIsDirectory)
                throw new FileNotFoundException($"Source path does not exist: {sourcePath}");

            if (destinationPath == DefaultMarker)
            {
                var parent = Path.GetDirectoryName(sourcePath) ?? string.Empty;
                var name = Path.GetFileName(sourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                destinationPath = sourceIsDirectory
                    ? Path.Combine(parent, name + "_ts")
                    : Path.Combine(parent, Path.GetFileNameWithoutExtension(sourcePath) + ".ts");
            }

            if (sourceIsFile)
            {
                var resolvedDest = Directory.Exists(destinationPath)
                    ? Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(sourcePath) + ".ts")
                    : destinationPath;

                scraperManager.MapFile(sourcePath, resolvedDest!);
            }
            else
            {
                if (File.Exists(destinationPath))
                    throw new InvalidOperationException($"Cannot map directory '{sourcePath}' to file '{destinationPath}'.");

                scraperManager.MapDirectory(sourcePath, destinationPath!);
            }
        });

        return rootCommand.Parse(args).Invoke();
    }
}
