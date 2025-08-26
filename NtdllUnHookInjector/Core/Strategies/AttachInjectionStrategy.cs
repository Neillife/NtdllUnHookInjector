using Serilog;
using System;
using System.Diagnostics;
using NtdllUnHookInjector.Core.Payloads;
using NtdllUnHookInjector.Core.Services;

namespace NtdllUnHookInjector.Core.Strategies
{
    public class AttachInjectionStrategy : IInjectionStrategy
    {
        private readonly IInjectionService _injectionService;

        public AttachInjectionStrategy(IInjectionService injectionService)
        {
            _injectionService = injectionService;
        }

        public void Run(string input, string dllPath)
        {
            Log.Information($"Mode: Attach Injection. Target: {input}");

            int processId = -1;

            // 嘗試將 input 轉換為整數 (Process ID)
            if (int.TryParse(input, out processId))
            {
                // 如果成功，表示輸入的是 Process ID
                try
                {
                    Process targetProcess = Process.GetProcessById(processId);
                    if (targetProcess != null)
                    {
                        IInjectorPayload payload = PayloadFactory.CreatePayload(dllPath);
                        _injectionService.Inject(processId, payload);
                    }
                    else
                    {
                        Log.Error($"Error: Process with ID {processId} not found.");
                    }
                }
                catch (ArgumentException)
                {
                    Log.Error($"Error: Process with ID {processId} not found.");
                }
            }
            else
            {
                // 如果失敗，表示輸入的是 Process Name
                string processName = input;
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    Log.Error($"Error: Could not find process named '{processName}'.");
                    return;
                }

                processId = processes[0].Id;
                IInjectorPayload payload = PayloadFactory.CreatePayload(dllPath);
                _injectionService.Inject(processId, payload);
            }
        }
    }
}
