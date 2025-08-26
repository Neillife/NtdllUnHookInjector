using System;
using System.Runtime.InteropServices;
using System.IO;
using NtdllUnHookInjector.Core.Strategies;
using static NtdllUnHookInjector.Native.NativeMethods;
using static NtdllUnHookInjector.Native.Enums;
using static NtdllUnHookInjector.Native.Structs;

namespace NtdllUnHookInjector.Core.Services
{
    public class SuspendedProcessService : ISuspendedProcessService
    {
        public SuspendedProcessInfo CreateSuspendedProcess(string appPath)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi;
            si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
            string workingDir = Path.GetDirectoryName(appPath);

            if (!CreateProcess(appPath, null, IntPtr.Zero, IntPtr.Zero, false,
                    ProcessCreationFlags.CREATE_SUSPENDED, IntPtr.Zero, workingDir, ref si, out pi))
            {
                throw new InvalidOperationException($"Unable to create a process in suspended state. Error code: {Marshal.GetLastWin32Error()}");
            }

            return new SuspendedProcessInfo(pi.dwProcessId, pi.hProcess, pi.hThread);
        }
    }
}
