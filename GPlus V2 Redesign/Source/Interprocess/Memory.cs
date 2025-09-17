using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GPlus.Source.Interprocess
{
    internal static class Memory
    {
        // Native imports
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // Access flags
        private const uint PROCESS_CREATE_THREAD = 0x0002;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_OPERATION = 0x0008;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_VM_READ = 0x0010;

        // Memory flags
        private const uint MEM_COMMIT = 0x00001000;
        private const uint MEM_RESERVE = 0x00002000;
        private const uint PAGE_READWRITE = 0x04;

        // Message structs (left public if needed elsewhere)
        [StructLayout(LayoutKind.Sequential)]
        public struct SteamMessage
        {
            public ulong steamID;
            public uint processID;
        }

        private const int WM_COPYDATA = 0x4A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        /// <summary>
        /// Returns true if the module with the specified name is loaded in the given process.
        /// Safely handles processes where module enumeration may fail.
        /// </summary>
        public static bool IsModuleLoaded(int pid, string dllName)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                return proc.Modules.Cast<ProcessModule>().Any(m =>
                    string.Equals(m.ModuleName, dllName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(m.FileName, dllName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                // Access denied, process exited, or other issue — treat as not loaded.
                return false;
            }
        }

        /// <summary>
        /// Attempts to inject the DLL at dllPath into the target process identified by pid.
        /// Returns true on success, false on failure.
        /// </summary>
        public static bool InjectDll(int pid, string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                Debug.WriteLine("DLL path is null or empty.");
                return false;
            }

            IntPtr hProcess = IntPtr.Zero;
            try
            {
                uint access = PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ;
                hProcess = OpenProcess(access, false, pid);

                if (hProcess == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to open target process.");
                    return false;
                }

                // Use Unicode (LoadLibraryW) so paths with non-ASCII characters work.
                byte[] dllPathBytes = Encoding.Unicode.GetBytes(dllPath + '\0');
                uint allocSize = (uint)dllPathBytes.Length;

                IntPtr allocAddress = VirtualAllocEx(hProcess, IntPtr.Zero, allocSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (allocAddress == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to allocate memory in target process.");
                    return false;
                }

                if (!WriteProcessMemory(hProcess, allocAddress, dllPathBytes, allocSize, out _))
                {
                    Debug.WriteLine("Failed to write DLL path to target process memory.");
                    return false;
                }

                IntPtr kernel32 = GetModuleHandle("kernel32.dll");
                if (kernel32 == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to get kernel32 module handle.");
                    return false;
                }

                // Call LoadLibraryW to match Unicode bytes.
                IntPtr loadLibraryAddr = GetProcAddress(kernel32, "LoadLibraryW");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to get LoadLibraryW address.");
                    return false;
                }

                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocAddress, 0, IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to create remote thread.");
                    return false;
                }

                Debug.WriteLine($"DLL injected successfully into PID {pid}.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while injecting DLL: {ex.Message}");
                return false;
            }
            finally
            {
                if (hProcess != IntPtr.Zero)
                    CloseHandle(hProcess);
            }
        }
    }
}
