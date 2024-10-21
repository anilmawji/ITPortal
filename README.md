<div align="center">
  <img alt="logo" src="ScriptProfiler/Resources/Images/powershell_red.svg" width="80" />
</div>
<h1 align="center">
  PowerShell Script Profiler
</h1>
<p align="center">
  Microsoft-centered desktop application for IT Administrators, built with <a href="https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-8.0">.NET MAUI Blazor Hybrid</a>
</p>

![jobs](https://github.com/anilmawji/ITPortal/assets/36245645/f1152d13-a7a1-4705-957e-e21470831d8e)

## ✨ Features

- Dynamically generate modern UI to interact with PowerShell script blocks
- Import, track, schedule and execute scripts locally
- Authenticate with Microsoft to securely request and feed access tokens to scripts from Azure Key Vault
- Save and load script jobs via a portable JSON format.
- Store and compare execution results
- Worry-free script management system automatically loads script jobs from disk next time the application starts

## 🖥️ Supported Platforms

- Currently, only Windows is officially supported.
- macOS support will be added in a future update.

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

## Demo

![script-demo](https://github.com/anilmawji/PowerShell-Script-Runner/assets/36245645/9c2ef69e-27a2-4085-b594-82332d4d4272)

![login](https://github.com/anilmawji/ITPortal/assets/36245645/6e43a489-54a7-4bd1-a095-26491ef70cd1)

![job-editor](https://github.com/anilmawji/ITPortal/assets/36245645/497aac0f-5988-47a8-85b9-d6892e7dc5a7)

![job-results](https://github.com/anilmawji/ITPortal/assets/36245645/15aafb8b-ca2f-486c-b8a0-58b41d081699)

![270132945-4dc0608b-5444-491c-a28c-46947a642723](https://github.com/anilmawji/PowerShell-Script-Runner/assets/36245645/55984ffd-a996-4ed5-9d63-65803e94ca92)
