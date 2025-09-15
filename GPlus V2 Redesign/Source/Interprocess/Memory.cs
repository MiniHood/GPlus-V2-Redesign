using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GPlus.Source.Interprocess
{
    internal static class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(nint hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint VirtualAllocEx(nint hProcess, nint lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress,
            byte[] lpBuffer, uint nSize, out nuint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern nint CreateRemoteThread(nint hProcess,
            nint lpThreadAttributes, uint dwStackSize, nint lpStartAddress,
            nint lpParameter, uint dwCreationFlags, nint lpThreadId);

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
            public nint dwData;
            public int cbData;
            public nint lpData;
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
            nint hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION |
                                          PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
                                          false, pid);
            if (hProcess == nint.Zero)
            {

                Debug.WriteLine("Failed to open target process.");

                return;
            }

            nint allocMemAddress = VirtualAllocEx(hProcess, nint.Zero,
                (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))),
                MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

            if (allocMemAddress == nint.Zero)
            {

                Debug.WriteLine("Failed to allocate memory in target process.");

                CloseHandle(hProcess);
                return;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(dllPath);

            if (!WriteProcessMemory(hProcess, allocMemAddress, bytes, (uint)bytes.Length, out _))
            {

                Debug.WriteLine("Failed to write DLL path to target process.");

                CloseHandle(hProcess);
                return;
            }

            nint loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAddr == nint.Zero)
            {

                Debug.WriteLine("Failed to get LoadLibraryA address.");

                CloseHandle(hProcess);
                return;
            }

            nint hThread = CreateRemoteThread(hProcess, nint.Zero, 0, loadLibraryAddr,
                                                allocMemAddress, 0, nint.Zero);

            if (hThread == nint.Zero)
            {

                Debug.WriteLine("Failed to create remote thread.");

                CloseHandle(hProcess);
                return;
            }

            Debug.WriteLine("DLL injected successfully!");

            CloseHandle(hProcess);
        }
    }
}
