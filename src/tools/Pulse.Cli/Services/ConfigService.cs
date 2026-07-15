using System;
using System.IO;
using System.Text.Json;
using Pulse.Cli.Models;

namespace Pulse.Cli.Services;

public interface IConfigService
{
    Config Load();

    void Save(Config config);
}

public sealed class ConfigService : IConfigService
{
    private const string ConfigurationDirectory = "Pulse";
    private const string ConfigurationFileName = "config.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
    
    private readonly IFileService _fileService;

    public ConfigService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Config Load()
    {
        _fileService.EnsureDirectory(ConfigurationPath);
        var configText = _fileService.ReadFile(ConfigurationFile);
        
        return configText is null ? new Config() : 
            JsonSerializer.Deserialize<Config>(configText, SerializerOptions)!;
    }

    public void Save(Config config)
    {
        _fileService.EnsureDirectory(ConfigurationPath);

        var configText = JsonSerializer.Serialize(config, SerializerOptions);
        _fileService.WriteFile(ConfigurationFile, configText);
    }

    private static readonly string ConfigurationPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationDirectory);

    private static readonly string ConfigurationFile = Path.Combine(ConfigurationPath, ConfigurationFileName);
}