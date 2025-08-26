using Serilog;
using System;
using System.Runtime.InteropServices;
using System.Text;
using static NtdllUnHookInjector.Native.NativeConstant;
using static NtdllUnHookInjector.Native.NativeMethods;
using static NtdllUnHookInjector.Native.Structs;

namespace NtdllUnHookInjector.Core.Services
{

    public static class UnhookService
    {
        public static IntPtr ntdll = IntPtr.Zero;

        public static IntPtr GetNtdll()
        {
            if (ntdll == IntPtr.Zero)
            {
                Log.Information("Init ntdll load.");
                ntdll = LoadLibrary("ntdll.dll");
            }
            return ntdll;
        }

        public static void UnhookModule(IntPtr hModule, IntPtr hProcess, string dllPath)
        {
            IntPtr currentProcess = GetCurrentProcess();
            MODULEINFO moduleInfo;

            if (!GetModuleInformation(currentProcess, hModule, out moduleInfo, Marshal.SizeOf(typeof(MODULEINFO))))
                throw new InvalidOperationException("GetModuleInformation failed.");

            IntPtr baseAddress = moduleInfo.lpBaseOfDll;

            // open ntdll from disk
            IntPtr file = CreateFileA(dllPath, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (file == IntPtr.Zero)
                throw new InvalidOperationException("CreateFileA failed.");

            IntPtr mapping = CreateFileMapping(file, IntPtr.Zero, PAGE_READONLY | SEC_IMAGE, 0, 0, null);
            if (mapping == IntPtr.Zero)
            {
                CloseHandle(file);
                throw new InvalidOperationException("CreateFileMapping failed.");
            }

            IntPtr mappingAddr = MapViewOfFile(mapping, FILE_MAP_READ, 0, 0, 0);
            if (mappingAddr == IntPtr.Zero)
            {
                CloseHandle(mapping);
                CloseHandle(file);
                throw new InvalidOperationException("MapViewOfFile failed.");
            }

            try
            {
                RestoreTextSection(baseAddress, mappingAddr, hProcess);
            }
            finally
            {
                UnmapViewOfFile(mappingAddr);
                CloseHandle(mapping);
                CloseHandle(file);
            }
        }

        private static void RestoreTextSection(IntPtr baseAddress, IntPtr cleanImage, IntPtr hProcess)
        {
            IMAGE_DOS_HEADER dosHeader = Marshal.PtrToStructure<IMAGE_DOS_HEADER>(baseAddress);
            IntPtr ntHeaderPtr = IntPtr.Add(baseAddress, dosHeader.e_lfanew);

            IMAGE_NT_HEADERS64 ntHeader = Marshal.PtrToStructure<IMAGE_NT_HEADERS64>(ntHeaderPtr);
            int sizeOfSection = Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER));

            IntPtr firstSection;
            if (Is64BitProcess(hProcess))
            {
                Log.Information("The injected process is 64 - bit");
                firstSection = IntPtr.Add(ntHeaderPtr, Marshal.SizeOf(typeof(IMAGE_NT_HEADERS64)));
            }
            else
            {
                Log.Information("The injected process is 32 - bit");
                firstSection = IntPtr.Add(ntHeaderPtr, Marshal.SizeOf(typeof(IMAGE_NT_HEADERS)));
            }

            Log.Information("UnhookService Search for .text section.");
            for (int i = 0; i < ntHeader.FileHeader.NumberOfSections; i++)
            {
                IntPtr sectionPtr = IntPtr.Add(firstSection, i * sizeOfSection);
                IMAGE_SECTION_HEADER section = Marshal.PtrToStructure<IMAGE_SECTION_HEADER>(sectionPtr);

                string name = Encoding.ASCII.GetString(section.Name).TrimEnd('\0');

                if (string.Equals(name, ".text", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Information("UnhookService Found .text section restoring .text section");
                    uint oldProtect;

                    // change protection to RWX, copy from mapped file -> process memory, restore protection
                    if (!VirtualProtect(
                        IntPtr.Add(baseAddress, (int)section.VirtualAddress),
                        section.VirtualSize,
                        PAGE_EXECUTE_READWRITE,
                        out oldProtect))
                    {
                        throw new InvalidOperationException("VirtualProtect failed to set EXECUTE_READWRITE.");
                    }

                    // perform memory copy the .text section
                    byte[] buffer = new byte[section.VirtualSize];
                    IntPtr source = IntPtr.Add(cleanImage, (int)section.VirtualAddress);
                    Marshal.Copy(source, buffer, 0, (int)section.VirtualSize);
                    Marshal.Copy(buffer, 0, IntPtr.Add(baseAddress, (int)section.VirtualAddress), (int)section.VirtualSize);

                    // restore
                    if (!VirtualProtect(
                        IntPtr.Add(baseAddress, (int)section.VirtualAddress),
                        section.VirtualSize,
                        oldProtect,
                        out oldProtect))
                    {
                        throw new InvalidOperationException("VirtualProtect failed to restore protection.");
                    }

                    Log.Information("UnhookService .text section restored.");
                    break; // Exit the loop after finding and processing the .text section
                }
            }
        }

        private static bool Is64BitProcess(IntPtr hProcess)
        {
            bool isWow64;
            if (!IsWow64Process(hProcess, out isWow64))
                throw new InvalidOperationException("IsWow64Process failed.");
            return Environment.Is64BitOperatingSystem && !isWow64;
        }

        public static T LoadFunction<T>(IntPtr module, string functionName) where T : Delegate
        {
            IntPtr proc = GetProcAddress(module, functionName);
            if (proc == IntPtr.Zero)
                throw new InvalidOperationException($"GetProcAddress failed for {functionName}");
            return (T)Marshal.GetDelegateForFunctionPointer(proc, typeof(T));
        }

        public static void CheckStatus(uint status, IntPtr hProcess, string what = null)
        {
            if ((int)status < 0)
            {
                string msg = what == null ? $"NTSTATUS 0x{status:X8}" : $"{what} failed: 0x{status:X8}";
                Log.Error(msg);
                CloseHandle(hProcess);
                throw new InvalidOperationException(msg);
            }
        }
    }
}
