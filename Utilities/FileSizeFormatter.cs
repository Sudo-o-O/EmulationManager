namespace EmulationManager.Utilities;

public static class FileSizeFormatter
{
    public static string Format(long bytes)
    {
        string[] units =
            ["B", "KB", "MB", "GB", "TB"];

        double size = bytes;
        int unitIndex = 0;

        while (size >= 1024 &&
               unitIndex < units.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.##} {units[unitIndex]}";
    }
}
