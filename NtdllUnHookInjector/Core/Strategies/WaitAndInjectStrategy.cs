using Serilog;
using System.Diagnostics;
using System.Threading;
using NtdllUnHookInjector.Core.Payloads;
using NtdllUnHookInjector.Core.Services;

namespace NtdllUnHookInjector.Core.Strategies
{
    public class WaitAndInjectStrategy : IInjectionStrategy
    {
        private readonly IInjectionService _injectionService;
        bool isInject = false;
        char filled = '■';
        char empty = '□';
        int blockCount = 5;
        int steps = 5;
        int pos = 0;
        int direction = 1;

        public WaitAndInjectStrategy(IInjectionService injectionService)
        {
            _injectionService = injectionService;
        }

        public void Run(string processName, string dllPath)
        {
            Log.Information($"模式：等待並注入。");

            while (!isInject)
            {
                for (int i = 0; i < steps; i++)
                {
                    string spinner = "";
                    for (int j = 0; j < steps; j++)
                    {
                        spinner += (j == pos) ? filled : empty;
                    }
                    pos += direction;
                    if (pos == blockCount - 1 || pos == 0) direction *= -1;

                    Log.Information($"正在等待應用程式開啟 '{processName}'... {spinner}");
                    Thread.Sleep(800);

                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length > 0)
                    {
                        Log.Information($"偵測到 '{processName}' 進程已開啟，PID: {processes[0].Id}。");
                        IInjectorPayload payload = PayloadFactory.CreatePayload(dllPath);
                        _injectionService.Inject(processes[0].Id, payload);
                        isInject = true;
                        break;
                    }
                }
            }
        }
    }
}
