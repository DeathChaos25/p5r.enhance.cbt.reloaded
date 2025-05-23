using p5r.enhance.cbt.reloaded.Configuration;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using Reloaded.Memory;
using Reloaded.Memory.Interfaces;

namespace p5r.enhance.cbt.reloaded;

internal unsafe class Utils
{
    private static ILogger _logger;
    private static Config _config;
    private static IStartupScanner _startupScanner;
    internal static nint BaseAddress { get; private set; }

    internal static bool DEBUG = false;

    internal static bool Initialise(ILogger logger, Config config, IModLoader modLoader)
    {
        _logger = logger;
        _config = config;
        using var thisProcess = Process.GetCurrentProcess();
        BaseAddress = thisProcess.MainModule!.BaseAddress;

        var startupScannerController = modLoader.GetController<IStartupScanner>();
        if (startupScannerController == null || !startupScannerController.TryGetTarget(out _startupScanner))
        {
            LogError($"Unable to get controller for Reloaded SigScan Library, stuff won't work :(");
            return false;
        }

        return true;
    }

    internal static void LogDebug(string message)
    {
        if (DEBUG)
            _logger.WriteLineAsync($"[Custom Bonus Tweaks] {message}");
    }

    internal static void Log(string message)
    {
        _logger.WriteLineAsync($"[Custom Bonus Tweaks] {message}");
    }

    internal static void LogNoPrefix(string message)
    {
        _logger.WriteLineAsync($"{message}");
    }

    internal static void LogError(string message, Exception e)
    {
        _logger.WriteLineAsync($"[Custom Bonus Tweaks] {message}: {e.Message}", System.Drawing.Color.Red);
    }

    internal static void LogError(string message)
    {
        _logger.WriteLineAsync($"[Custom Bonus Tweaks] {message}", System.Drawing.Color.Red);
    }

    internal static void SigScan(string pattern, string name, Action<nint> action)
    {
        if (pattern != null)
        {
            _startupScanner.AddMainModuleScan(pattern, result =>
            {
                if (!result.Found)
                {
                    LogError($"Signature scan for {name} has failed, make sure you are using latest game version/update");
                    return;
                }
                LogDebug($"Found {name} at 0x{result.Offset + BaseAddress:X}");

                action(result.Offset + BaseAddress);
            });
        }
    }

    /// <summary>
    /// Gets the address of a global from something that references it
    /// </summary>
    /// <param name="ptrAddress">The address to the pointer to the global (like in a mov instruction or something)</param>
    /// <returns>The address of the global</returns>
    internal static unsafe nuint GetGlobalAddress(nint ptrAddress)
    {
        return (nuint)((*(int*)ptrAddress) + ptrAddress + 4);
    }

    /// <summary>
    /// Writes a return 0 instruction at the target address of 
    /// MOV    EAX, 0
    /// </summary>
    /// <param name="target"></param>
    internal static unsafe void WriteReturn0(nint target)
    {
        var memory = Memory.Instance;

        byte[] Bytes = { 0xb8, 0, 0, 0, 0 };

        memory.SafeWrite((nuint)target, Bytes);
    }
    internal static unsafe void WriteReturn1(nint target)
    {
        var memory = Memory.Instance;

        byte[] Bytes = { 0xb8, 1, 0, 0, 0 };

        memory.SafeWrite((nuint)target, Bytes);
    }

}
