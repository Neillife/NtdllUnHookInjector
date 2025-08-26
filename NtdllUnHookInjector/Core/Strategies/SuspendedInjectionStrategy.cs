using Serilog;
using NtdllUnHookInjector.Core.Payloads;
using NtdllUnHookInjector.Core.Services;
using static NtdllUnHookInjector.Native.NativeMethods;
using static NtdllUnHookInjector.Native.Delegates;

namespace NtdllUnHookInjector.Core.Strategies
{
    public class SuspendedInjectionStrategy : IInjectionStrategy
    {
        private readonly ISuspendedProcessService _processService;
        private readonly IInjectionService _injectionStrategyService;

        public SuspendedInjectionStrategy(ISuspendedProcessService processService, IInjectionService injectionStrategyService)
        {
            _processService = processService;
            _injectionStrategyService = injectionStrategyService;
        }

        public void Run(string appPath, string dllPath)
        {
            Log.Information($"Mode: Suspend Injection. Target Application：{appPath}");

            SuspendedProcessInfo processInfo = null;
            try
            {
                // 1. 使用服務創建一個掛起的進程，並取得所有資訊
                processInfo = _processService.CreateSuspendedProcess(appPath);
                Log.Information($"Successfully created process, PID: {processInfo.ProcessId}, injection in progress...");

                // 2. 使用注入服務進行 DLL 注入
                IInjectorPayload payload = PayloadFactory.CreatePayload(dllPath);
                _injectionStrategyService.Inject(processInfo.ProcessId, payload);

                Log.Information("DLL injection complete, resuming main thread...");

                // 3. 恢復主執行緒，並傳入正確的句柄
                // 直接在這裡呼叫 ResumeThread，或者將其包裝在一個輔助方法中
                var NtResumeThread = UnhookService.LoadFunction<NtResumeThreadDelegate>(UnhookService.GetNtdll(), "NtResumeThread");
                uint status = NtResumeThread(processInfo.ThreadHandle, out uint prevSuspend);
                UnhookService.CheckStatus(status, processInfo.ProcessHandle, "NtResumeThread");
            }
            finally
            {
                // 確保所有句柄都被關閉
                if (processInfo != null)
                {
                    CloseHandle(processInfo.ProcessHandle);
                    CloseHandle(processInfo.ThreadHandle);
                }
            }
        }
    }
}
