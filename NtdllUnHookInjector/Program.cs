using System;
using System.IO;
using Serilog;
using NtdllUnHookInjector.Core.Strategies;

namespace NtdllUnHookInjector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            // 檢查命令列參數數量
            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            string mode = args[0];
            string target = args[1];
            string filePath = args[2];

            try
            {
                // 1. 驗證注入檔案路徑
                if (!File.Exists(filePath))
                {
                    Log.Error($"錯誤：找不到 注入 檔案 '{filePath}'。");
                    return;
                }

                // 2. 使用工廠創建正確的策略實例
                Log.Information("正在初始化注入策略...");
                IInjectionStrategy strategy = InjectionStrategyFactory.CreateStrategy(mode);

                // 3. 執行策略
                Log.Information("策略已創建，正在執行...");
                strategy.Run(target, filePath);
            }
            catch (ArgumentException ex)
            {
                Log.Error($"錯誤：{ex.Message}");
                ShowUsage();
            }
            catch (Exception ex)
            {
                Log.Error($"發生未預期的錯誤: {ex.Message}");
            }
        }

        private static void ShowUsage()
        {
            Log.Information("用法: NtdllUnHookInjector.exe <模式> <目標> <DLL路徑>");
            Log.Information("模式 (Mode):");
            Log.Information("  attach:   注入到已開啟的進程。 (目標為進程名稱，如 notepad)");
            Log.Information("  suspend:  啟動新進程並掛起注入。 (目標為可執行檔路徑，如 C:\\Windows\\System32\\notepad.exe)");
            Log.Information("  wait:     等待使用者開啟進程後再注入。 (目標為進程名稱，如 notepad)");
        }
    }
}
