[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<div align="center">
  <h2 align="center">🧬 NtdllUnHookInjector</h3>

  <p align="center">
    <a href="https://github.com/Neillife/NtdllUnHookInjector/blob/master/README_cn.md">中文</a> 
	| 
    English
  </p>
</div>

🔒 **Stealthy DLL & Shellcode Injector via `ntdll.dll`**  
Bypasses user-mode hooks for faster, stealthier, and more reliable injection.  

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows-blue?logo=windows" />
  <img src="https://img.shields.io/badge/Language-C%23-purple?logo=csharp" />
  <img src="https://img.shields.io/badge/Build-Passing-brightgreen?logo=githubactions" />
  <img src="https://img.shields.io/badge/License-MIT-lightgrey?logo=open-source-initiative" />
</p>


## 📝 Overview
A C# injector that **unhooks `ntdll.dll`** and performs code injection via **low-level `ntdll` functions** (e.g., `NtCreateThreadEx`).  
This approach improves speed, bypasses user-mode monitoring, and achieves stealthier injection.  

This project adopts a modular design of **`Strategy Pattern`** and **`Factory Pattern`** to ensure that all functions are highly decoupled, easy to expand and maintain.

## ✨ Features
- 🧩 **Injection Modes**:  
  - 🔗 `attach` – inject into a running process  
  - ⏸️ `suspend` – start suspended, inject, then resume  
  - ⏳ `wait` – wait for target process, then inject  
- 📦 **Payloads**: DLL / Shellcode  (Test payloads are available in [TestInjectFile](https://github.com/Neillife/NtdllUnHookInjector/tree/master/NtdllUnHookInjector/TestInjectFile), compiled with C++)
- ⚙️ **Low-level API**: calls `ntdll` (e.g., `NtCreateThreadEx`) instead of `kernel32`  
- 🎯 **Targeting**: by process name or PID  

<details>
  <summary>Directory Structure</summary>

```bash
NtdllUnHookInjector/
├── Core/
│   ├── Payloads/              # Injection payloads
│   ├── Services/              # Injection services
│   └── Strategies/            # Injection strategies
├── Native/                    # Windows API bindings
└── TestInjectFile/            # Sample Dll and shellcode
```
</details>


## 🚀 Usage
- **Payload File**: If you want to test injection, [TestInjectFile](https://github.com/Neillife/NtdllUnHookInjector/tree/master/NtdllUnHookInjector/TestInjectFile) already has a popup DLL compiled in C++.
  - **DLL**: **`MessageBox32.dll`** / **`MessageBox64.dll`**
  - **Shellcode**: **`MessageBox32.bin`** / **`MessageBox64.bin`**
- **Permissions**: Since advanced operations are involved, make sure the target process matches the injector architecture (x32 / x64) and that you run as Administrator if required.

```bash
NtdllUnHookInjector.exe <mode> <target> <payload_path>
```


## 🔹 Parameters

- **`<mode>`** : **`attach`** | **`suspend`** | **`wait`**
- **`<target>`** : process name / PID / exe path (depends on mode)
- **`<payload_path>`** : DLL (.dll) or Shellcode (.bin)


## 🔹 Examples

```bash
# Inject shellcode into Notepad
NtdllUnHookInjector.exe attach notepad TestInjectFile\MessageBox64.bin

# Inject DLL into a suspended Notepad process
NtdllUnHookInjector.exe suspend "C:\Windows\System32\notepad.exe" TestInjectFile\MessageBox64.dll

# Wait for myapp.exe to start, then inject DLL
NtdllUnHookInjector.exe wait myapp.exe TestInjectFile\MessageBox64.dll
```


## 🛠️ Build

Open in Visual Studio 2019+

⚠️ Important: Ensure x86 / x64 matches the target process


## 📜 License

MIT – see [LICENSE](https://github.com/Neillife/NtdllUnHookInjector/blob/master/LICENSE)。


## ⚠️ Disclaimer
This project is intended for educational and research purposes only.  
🚫 Do not use it for illegal activities. The author assumes no responsibility for misuse.





[contributors-shield]: https://img.shields.io/github/contributors/Neillife/NtdllUnHookInjector.svg?style=for-the-badge
[contributors-url]: https://github.com/Neillife/NtdllUnHookInjector/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Neillife/NtdllUnHookInjector.svg?style=for-the-badge
[forks-url]: https://github.com/Neillife/NtdllUnHookInjector/network/members
[stars-shield]: https://img.shields.io/github/stars/Neillife/NtdllUnHookInjector.svg?style=for-the-badge
[stars-url]: https://github.com/Neillife/NtdllUnHookInjector/stargazers
[issues-shield]: https://img.shields.io/github/issues/Neillife/NtdllUnHookInjector.svg?style=for-the-badge
[issues-url]: https://github.com/Neillife/NtdllUnHookInjector/issues
[license-shield]: https://img.shields.io/github/license/Neillife/NtdllUnHookInjector.svg?style=for-the-badge
[license-url]: https://github.com/Neillife/NtdllUnHookInjector/blob/master/LICENSE
