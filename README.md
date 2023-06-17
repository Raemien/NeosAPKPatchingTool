# NeosVR APK Patcher

This CLI tool is designed to modify Android builds of NeosVR - adding features, fixing bugs and restoring network compatibility with PC. Uses [APKTool](https://github.com/iBotPeaches/Apktool) and [Uber APK Signer](https://github.com/patrickfav/uber-apk-signer) to extract, patch and sign the application with updated binaries.

Please note that this tool is not officially supported by Solirax and may break after major codebase changes.

## Features
- Fully network compatible with users on the latest PC version.
- Optional out-of-the-box support for NeosModLoader.
- Enable manifest-level features such as hand-tracking and debugging.
- Supports both Quest and Touchscreen releases of NeosVR.

## Requirements
- The latest PC release of NeosVR (avaliable from [Steam](https://store.steampowered.com/app/740250/Neos_VR/)/Neos Launcher)
- The latest Android release of NeosVR ([Patreon-exclusive](https://www.patreon.com/neosvr/))
- [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.16-windows-x64-installer)
- [Java 8+](https://www.java.com/en/download/)

## Installation
**Make sure you have the requirements listed above!** If the program refuses to open, you're likely missing the [.NET 6.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.16-windows-x64-installer).

Run the program, specify your `Neos_Data` folder and the APK to patch. After running the program, install `NeosOculusPatched.apk` using adb/SideQuest.

## Support
Consider supporting my projects on [Patreon](https://patreon.com/raemien)! <3