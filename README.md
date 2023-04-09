# NeosVR APK Updater

This CLI tool aims to restore functionality to Android releases of NeosVR after a recent networking hotfix. Uses [APKTool](https://github.com/iBotPeaches/Apktool) and [Uber APK Signer](https://github.com/patrickfav/uber-apk-signer) to extract, repatch and sign the application with updated binaries.

Please note that this tool is not officially supported by Solirax and may break after major codebase changes.

## Requirements
- The latest PC release of NeosVR (avaliable from [Steam](https://steamcommunity.com/app/740250)/Neos Launcher)
- The latest Android release of NeosVR ([Patreon-exclusive](https://www.patreon.com/neosvr/))
- Java 8+

## Installation
Run the program, specify your `Neos_Data` folder and the APK to patch.
After running the program, sideload `NeosOculus-patched.apk` using adb/SideQuest.

Tested on Quest 2, but with an older software revision. Untested on v51/Android 12.

## Support
Consider supporting my projects on [Patreon](https://patreon.com/raemien) <3