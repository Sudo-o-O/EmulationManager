using EmulationManager.Configuration;
using EmulationManager.Models;

namespace EmulationManager.Services;

public sealed class LibraryScanner
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".nsp", ".xci",
            ".iso", ".rvz", ".wbfs",
            ".chd", ".cue", ".bin", ".cso", ".pbp",
            ".zip", ".7z",
            ".nes", ".sfc", ".smc",
            ".gba", ".gb", ".gbc",
            ".n64", ".z64", ".v64",
            ".nds", ".3ds"
        };

    private readonly AppPaths paths;

    public LibraryScanner(AppPaths paths)
    {
        this.paths = paths;
    }

    public IReadOnlyList<GameFile> Scan()
    {
        var results = new List<GameFile>();

        if (!Directory.Exists(paths.RomRoot))
        {
            return results;
        }

        foreach (string systemFolder in
                 Directory.EnumerateDirectories(paths.RomRoot))
        {
            string consoleName =
                Path.GetFileName(systemFolder);

            foreach (string filePath in
                     Directory.EnumerateFiles(
                         systemFolder,
                         "*",
                         SearchOption.AllDirectories))
            {
                string extension =
                    Path.GetExtension(filePath);

                if (!SupportedExtensions.Contains(extension))
                {
                    continue;
                }

                var fileInfo = new FileInfo(filePath);

                results.Add(
                    new GameFile
                    {
                        GameName =
                            DetermineGameName(
                                systemFolder,
                                filePath),

                        ConsoleName = consoleName,

                        ContentType =
                            DetermineContentType(filePath),

                        FileName = fileInfo.Name,
                        FullPath = fileInfo.FullName,
                        SizeBytes = fileInfo.Length
                    });
            }
        }

        return results
            .OrderBy(game => game.ConsoleName)
            .ThenBy(game => game.GameName)
            .ThenBy(game => game.ContentType)
            .ToList();
    }

    private static string DetermineGameName(
        string systemFolder,
        string filePath)
    {
        string relativePath =
            Path.GetRelativePath(
                systemFolder,
                filePath);

        string[] parts =
            relativePath.Split(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

        if (parts.Length > 1)
        {
            return parts[0];
        }

        return Path.GetFileNameWithoutExtension(filePath);
    }

    private static string DetermineContentType(
        string filePath)
    {
        string[] parts =
            filePath.Split(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

        if (parts.Any(part =>
                part.Equals(
                    "Updates",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "Update";
        }

        if (parts.Any(part =>
                part.Equals(
                    "DLC",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "DLC";
        }

        if (parts.Any(part =>
                part.Equals(
                    "Base",
                    StringComparison.OrdinalIgnoreCase)))
        {
            return "Base Game";
        }

        return "Game";
    }
}
