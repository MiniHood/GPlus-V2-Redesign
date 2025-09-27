using System.Diagnostics;
using System.Management;

namespace GPlus.Source.General
{
    internal static class ProcessHelpers
    {
        internal static IEnumerable<Process> GetChildProcessesRecursive(int parentId)
        {
            var children = new List<Process>();
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId={parentId}");
                foreach (ManagementObject mo in searcher.Get())
                {
                    int pid = Convert.ToInt32(mo["ProcessId"]);
                    try
                    {
                        var childProc = Process.GetProcessById(pid);
                        children.Add(childProc);
                        // Recursively get this child's children
                        children.AddRange(GetChildProcessesRecursive(pid));
                    }
                    catch { /* process may have exited */ }
                }
            }
            catch { }
            return children;
        }

        internal static string GetCommandLine(Process process)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["CommandLine"]?.ToString() ?? "";
                }
            }
            catch { }
            return "";
        }
    }
}
