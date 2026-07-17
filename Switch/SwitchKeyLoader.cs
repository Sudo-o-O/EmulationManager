namespace EmulationManager.Switch;

public sealed class SwitchKeyLoader
{
    public SwitchKeySet Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "A prod.keys path was not provided.",
                nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "The selected prod.keys file was not found.",
                filePath);
        }

        SwitchKeySet keySet = new();

        int lineNumber = 0;

        foreach (string rawLine in File.ReadLines(filePath))
        {
            lineNumber++;

            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) ||
                line.StartsWith('#'))
            {
                continue;
            }

            string[] parts = line.Split('=', 2);

            if (parts.Length != 2)
            {
                throw new InvalidDataException(
                    $"Invalid keys-file entry on line {lineNumber}.");
            }

            string name = parts[0].Trim();
            string hexValue = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(hexValue))
            {
                throw new InvalidDataException(
                    $"Incomplete keys-file entry on line {lineNumber}.");
            }

            try
            {
                keySet.Keys[name] =
                    Convert.FromHexString(hexValue);
            }
            catch (FormatException exception)
            {
                throw new InvalidDataException(
                    $"The key on line {lineNumber} is not valid hexadecimal.",
                    exception);
            }
        }

        if (keySet.Keys.Count == 0)
        {
            throw new InvalidDataException(
                "The selected file did not contain any valid keys.");
        }

        return keySet;
    }
}