using System.IO;

namespace Pulse.Cli.Services;

public interface IFileService
{
    void EnsureDirectory(string path);

    string? ReadFile(string path);
    
    void WriteFile(string path, string contents);
}

public sealed class FileService : IFileService
{
    public void EnsureDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return;
        }

        Directory.CreateDirectory(path);
    }
    
    public string? ReadFile(string path)
    {
        return !File.Exists(path) ? null : File.ReadAllText(path);
    }
    
    public void WriteFile(string path, string contents)
    {
        File.WriteAllText(path, contents);
    }
}