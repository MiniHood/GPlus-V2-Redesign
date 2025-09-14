using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GPlus_V2_Redesign.Source
{
    internal static class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        const uint PROCESS_CREATE_THREAD = 0x0002;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_VM_OPERATION = 0x0008;
        const uint PROCESS_VM_WRITE = 0x0020;
        const uint PROCESS_VM_READ = 0x0010;

        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 0x04;

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

        public static bool IsModuleLoaded(int pid, string dllName)
        {
            var proc = Process.GetProcessById(pid);
            return proc.Modules.Cast<ProcessModule>().Any(m =>
                string.Equals(m.ModuleName, dllName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(m.FileName, dllName, StringComparison.OrdinalIgnoreCase));
        }

        public static void InjectDLL(int pid, string dllPath)
        {
            IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION |
                                          PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
                                          false, pid);
            if (hProcess == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("Failed to open target process.");
#endif
                return;
            }

            IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero,
                (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))),
                MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

            if (allocMemAddress == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("Failed to allocate memory in target process.");
#endif
                CloseHandle(hProcess);
                return;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (!WriteProcessMemory(hProcess, allocMemAddress, bytes, (uint)bytes.Length, out _))
            {
#if DEBUG
                Debug.WriteLine("Failed to write DLL path to target process.");
#endif
                CloseHandle(hProcess);
                return;
            }

            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("Failed to get LoadLibraryA address.");
#endif
                CloseHandle(hProcess);
                return;
            }

            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr,
                                                allocMemAddress, 0, IntPtr.Zero);

            if (hThread == IntPtr.Zero)
            {
#if DEBUG
                Debug.WriteLine("Failed to create remote thread.");
#endif
                CloseHandle(hProcess);
                return;
            }
#if DEBUG
            Debug.WriteLine("DLL injected successfully!");
#endif
            CloseHandle(hProcess);
        }
    }
}
