using p5r.enhance.cbt.reloaded.Configuration;
using p5r.enhance.cbt.reloaded.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace p5r.enhance.cbt.reloaded
{
    internal unsafe class LoadCBTMod
    {
        private Config _configuration;

        // Import the LoadLibrary function from kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        public void LoadCppDll(ModContext context)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            context.Logger.WriteLine($"Attempting to load DLL from {assemblyDirectory}");

            string dllPath = Path.Combine(assemblyDirectory, "P5RCBT.dll");
            IntPtr handle = LoadLibrary(dllPath);

            if (handle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                context.Logger.WriteLine($"Failed to load DLL. Error code: {errorCode}");
            }
            else
            {
                context.Logger.WriteLine("[Custom Bonus Tweaks] DLL Loaded successfully.");
            }
        }
    }
}
