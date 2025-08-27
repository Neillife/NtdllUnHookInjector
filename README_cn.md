[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<div align="center">
  <h2 align="center">🧬 NtdllUnHookInjector</h3>

  <p align="center">
    中文
	| 
    <a href="https://github.com/Neillife/NtdllUnHookInjector/README.md">English</a>
  </p>
</div>

🔒 **透過 `ntdll.dll` 進行隱密的 DLL 和 Shellcode 注入**  
繞過使用者模式鉤子，實現更快、更隱密、更可靠的注入。

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?logo=windows" />
  <img src="https://img.shields.io/badge/Language-C%23-purple?logo=csharp" />
  <img src="https://img.shields.io/badge/Build-Passing-brightgreen?logo=githubactions" />
  <img src="https://img.shields.io/badge/License-MIT-lightgrey?logo=open-source-initiative" />
</p>


## 📝 簡介
**`NtdllUnHookInjector`** 是一個專為進階用途設計的 C# 注入器。其核心功能是**先將 `ntdll.dll` 的使用者模式鉤子 (user-mode hooks) 移除**，然後利用**底層 `ntdll.dll` 函式**執行注入操作。  
相較於高階的 `kernel32` 函式，由於直接呼叫 `ntdll` 函式理論上可以**提升注入的執行速度**，並有效規避許多安全軟體對 `ntdll.dll` 的函式監控，實現更隱蔽且高效的程式碼注入。

本專案採用 **`策略模式`** 和 **`工廠模式`** 的模組化設計，確保所有功能高度解耦，易於擴展和維護。


## ✨ 主要功能
- 🧩 **多樣的注入模式**：
  - 🔗 **附加注入 (Attach)**：注入到已在運行的進程。
  - ⏸️ **掛起注入 (Suspend)**：啟動一個新進程，將其掛起，注入完成後再恢復。
  - ⏳ **等待注入 (Wait)**：等待目標進程啟動，一旦偵測到，立即進行注入。
- 📦 **多樣的 Payload 類型**：
  - **DLL 注入**：使用 `LdrLoadDll` 函式載入 DLL，這是在 `ntdll` 層級的對應函式。
  - **Shellcode 注入**：直接將二進位 Shellcode 寫入遠端進程並執行。
- ⚙️ **進階注入技術**：
  - **`ntdll` 底層函式**：直接呼叫 `NtCreateThreadEx` 和其他 `ntdll` 函式來執行注入操作，而非高階的 `kernel32` 函式。這使得注入行為更底層、更難以被使用者模式的監控程式所偵測。
- 🎯 **彈性的目標選擇**：
  - 支援以**進程名稱** (如 `notepad`) 或**進程 ID** (如 `12345`) 指定目標。


## ✏️ 架構
本專案採用分層架構設計，將不同職責的程式碼分離，以提高可讀性和擴展性。

- 🔨 **基礎設施層 (Native)**：包含所有與原生 Windows API 交互操作的靜態函式 (`NativeMethods.cs`)，包括從 `ntdll.dll` 獲取的**未導出或底層函式**的宣告。
- ❣ **核心層 (Core)**：
  - 📄 **Payloads**：定義注入內容的抽象 (`IInjectorPayload`)，並實作兩種具體的 Payload 類型 (`LoadLibraryPayload` 和 `ShellcodePayload`)。
  - 📡 **Services**：提供核心功能服務，如**通用注入器** (`IInjectionService`)，它將使用 `NtCreateThreadEx` 等底層函式。
  - 🔀 **Strategies**：定義注入模式的抽象 (`IInjectionStrategy`)，並包含具體的策略實作，如 (`AttachInjectionStrategy` 和 `SuspendedInjectionStrategy`)。
- ⌨️ **使用者介面層 (UI)**：由 `Program.cs` 構成，負責解析命令列參數並協調工廠來執行正確的策略。

<details>
  <summary> 📂 架構目錄</summary>

```bash
NtdllUnHookInjector/
├── Core/
│   ├── Payloads/              # Injection payloads
│   ├── Services/              # Injection services
│   └── Strategies/            # Injection strategies
├── Native/                    # Windows API bindings
└── TestInjectFile/            # Sample DLLs and shellcode
```
</details>


## 🚀 使用方法
- **Payload 檔案** ： 如果你想測試注入，[TestInjectFile](https://github.com/Neillife/NtdllUnHookInjector/TestInjectFile) 中已經有使用 C++ 編譯的彈窗 DLL。
  - **DLL** ： **`MessageBox32.dll`** / **`MessageBox64.dll`**
  - **Shellcode**：**`MessageBox32.bin`** / **`MessageBox64.bin`**
- **權限**：由於涉及進階操作，請確保目標的 Process 的位元 (**`x32`** / **`x64`**) 以及是否需要 **系統管理員身分** 以及使用正確位元的 32 或 64 注入器。

使用命令提示字元或 PowerShell，以**系統管理員身分** (如果目標的 Process 需要的話) 執行 `NtdllUnHookInjector.exe`，並傳入以下參數：

```bash
NtdllUnHookInjector.exe <模式> <目標> <Payload路徑>
```

## 🔹 參數說明

- **`<模式>`** ： **`attach`** | **`suspend`** | **`wait`**
- **`<目標>`** ： 
  - 對於 **`attach`** 和 **`wait`** 模式：目標進程名稱 (如 **`notepad`**) 或進程 ID (如 **`12345`**)。
  - 對於 **`suspend`** 模式：目標可執行檔的完整路徑 (如 **`C:\Windows\System32\notepad.exe`**)。
- **`<Payload路徑>`**：要注入的檔案完整路徑，可以是 DLL (副檔名 **`.dll`**) 或 Shellcode (副檔名 **`.bin`**)。

## 🔹 使用範例

```bash
# 範例 1：附加注入 Shellcode 到記事本進程
# （在執行前，請先手動打開一個記事本程式。）
NtdllUnHookInjector.exe attach notepad "C:\path\to\NtdllUnHookInjector\TestInjectFile\MessageBox64.bin"

# 範例 2：掛起注入 DLL 到一個新啟動的程式
NtdllUnHookInjector.exe suspend "C:\Windows\System32\notepad.exe" "C:\path\to\NtdllUnHookInjector\TestInjectFile\MessageBox64.dll"

# 範例 3：等待進程啟動後注入
NtdllUnHookInjector.exe wait myapp.exe "C:\path\to\NtdllUnHookInjector\TestInjectFile\MessageBox64.dll"
```


## 🛠️ 構建

在 Visual Studio 2019+ 中開啟

⚠️ 重要：確保 x86 / x64 與目標程序匹配


## 📜 授權

本專案的原始程式碼以 MIT 授權條款發布，詳情請見 [LICENSE](https://github.com/Neillife/NtdllUnHookInjector/LICENSE)。

---

## ⚠️ 免責聲明

**本專案的原始程式碼僅供教育和研究目的使用，  
🚫請勿用於非法用途。作者對任何誤用不承擔責任。**