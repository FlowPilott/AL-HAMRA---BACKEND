using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    /// <summary>
    /// Loads an unmanaged library from the specified absolute path.
    /// </summary>
    /// <param name="absolutePath">The absolute path to the unmanaged library.</param>
    /// <returns>A pointer to the loaded library.</returns>
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            throw new ArgumentException("Library path cannot be null or empty.", nameof(absolutePath));
        }

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException($"Library not found at {absolutePath}");
        }

        try
        {
            return LoadUnmanagedDll(absolutePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load unmanaged library at {absolutePath}. Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Loads the unmanaged DLL from the specified path.
    /// </summary>
    /// <param name="unmanagedDllPath">The path to the unmanaged DLL.</param>
    /// <returns>A pointer to the loaded library.</returns>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
    {
        try
        {
            return NativeLibrary.Load(unmanagedDllPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading unmanaged DLL from path: {unmanagedDllPath}. Exception: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Overrides the method for loading managed assemblies.
    /// </summary>
    /// <param name="assemblyName">The name of the managed assembly.</param>
    /// <returns>Null, as this implementation does not handle managed assemblies.</returns>
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}
