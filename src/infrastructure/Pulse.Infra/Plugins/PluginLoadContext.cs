using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Pulse.Infra.Plugins;

/// <summary>
/// One instance per loaded plugin DLL. This is what actually gives 
/// plugin isolation: each plugin gets its own AssemblyLoadContext, so
/// two plugins can depend on different (even incompatible) versions of
/// the same third-party library without clashing.
///
/// The one assembly that must NOT be isolated is Pulse.Plugin — both host
/// and plugin need to agree on the identity of the IPlugin type, or an
/// "IPlugin is IPlugin" check across the boundary silently fails
/// (you get two unrelated types that merely have the same name).
/// Overriding Load() to return null for that assembly name makes the
/// runtime fall back to the Default context, where the host already
/// loaded it.
/// </summary>
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private static readonly HashSet<string> SharedAssemblyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pulse.Plugin"
    };

    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginDllPath, bool collectible)
        : base(name: Path.GetFileNameWithoutExtension(pluginDllPath), isCollectible: collectible)
    {
        _resolver = new AssemblyDependencyResolver(pluginDllPath);
    }
    
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name is not null && SharedAssemblyNames.Contains(assemblyName.Name))
        {
            return null; // fall back to the Default ALC
        }

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath is not null ? LoadFromAssemblyPath(assemblyPath) : null;
    }
    
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath is not null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}