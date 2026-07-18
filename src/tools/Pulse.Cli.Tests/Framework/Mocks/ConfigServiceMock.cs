using System;
using Pulse.Cli.Models;
using Pulse.Cli.Services;

namespace Pulse.Cli.Tests.Framework.Mocks;

public sealed class ConfigServiceMock : IConfigService
{
    private Config _config = new();

    public Func<Config>? LoadHandler { get; set; }

    public Action<Config>? SaveHandler { get; set; }

    public Config SavedConfig { get; private set; } = new();

    public Config Load()
    {
        return LoadHandler?.Invoke() ?? _config;
    }

    public void Save(Config config)
    {
        SavedConfig = config;
        _config = config;

        SaveHandler?.Invoke(config);
    }

    public void UseConfig(Config config)
    {
        _config = config;
    }

    public Config Config => _config;
}