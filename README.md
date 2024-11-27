<div align="center">
  <img alt="logo" src="ScriptProfiler/Resources/Images/powershell_red.svg" width="80" />
</div>
<h1 align="center">
  PowerShell Script Profiler
</h1>
<p align="center">
  Microsoft-centered desktop application for IT Administrators to manage, execute and monitor PowerShell scripts.
</p>
<p align="center">
  Built with <a href="https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-8.0">.NET MAUI Blazor Hybrid</a>
</p>

![demo](https://github.com/anilmawji/PowerShell-Script-Runner/assets/36245645/9c2ef69e-27a2-4085-b594-82332d4d4272)

## ✨ Features

- Interact with your scripts using a modern, intuitive UI, allowing for easy parameter entry and editing.

- Authenticate with Microsoft Entra ID to securely request and feed access tokens to scripts from Azure Key Vault.

- Save and load script jobs via a portable JSON format, allowing easy transfer between different environments.

- Store and compare previous execution results to track performance and troubleshoot with ease.

- View automatically categorized script outputs that isolate standard output, warnings, and errors for easier debugging and analysis.

- Enjoy a hassle-free experience as saved script jobs are automatically loaded from disk on startup

## 🖥️ Supported Platforms

- Currently, only Windows is officially supported.
- macOS support will be added in a future update.

## 📌 Roadmap

- Ability to schedule the execution of script jobs so they can run at a specified date & time.
- I am currently in the middle of abstracting the script execution pipeline from the PowerShell runtine environment. This will allow other types of scripts (Python, Bash) to eventually be supported as well.
- Mac Catalyst integration for macOS users.

## 🔗 Dependencies

This project runs on .NET 8.0. Make sure to install it [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  before attempting to build.

## 🛠 Building and Running

This project is still in active development, but feel free to download and play around with it yourself!

I recommend using [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or greater to build and deploy, though you might be able to use VSCode instead.

1. Clone the git repo

   ```sh
   git clone https://github.com/anilmawji/PowerShell-Script-Profiler.git
   ```

2. Enter project directory

   ```sh
   cd "PowerShell-Script-Profiler\ScriptProfiler"
   ```

3. Build application

   ```sh
   dotnet build
   ```

4. Run (Windows only)

   ```sh
   dotnet run --framework net8.0-windows10.0.19041.0
   ```

## 🚀 Deploying

Ship the app as an executable so it can run independently of your IDE and the .NET SDK (Windows only)

   ```sh
   dotnet publish -f net8.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64 -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true
   ```

## 📸 Screenshots

Login Screen
![270132929-6e43a489-54a7-4bd1-a095-26491ef70cd1](https://github.com/user-attachments/assets/e68a8af0-cff3-47f7-b49c-e0687e0ff109)

Script Job List
![270132935-f1152d13-a7a1-4705-957e-e21470831d8e](https://github.com/user-attachments/assets/1ddda26f-c239-4320-98cb-a9d01cc8a427)


Script Job Editor
![270132824-497aac0f-5988-47a8-85b9-d6892e7dc5a7](https://github.com/user-attachments/assets/8cf0ae1a-896c-484c-b461-ce74c4a0d087)


Script Job Executions List
![270132941-15aafb8b-ca2f-486c-b8a0-58b41d081699](https://github.com/user-attachments/assets/ce2d5e30-4138-4b3e-bdea-89e16954d1ad)


Script Execution Details
![346735217-55984ffd-a996-4ed5-9d63-65803e94ca92](https://github.com/user-attachments/assets/0d98b6f6-9bdc-4040-a9b2-ad8dd89d6087)
