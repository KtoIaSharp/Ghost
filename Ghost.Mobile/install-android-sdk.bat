@echo off
echo ============================================
echo   Ghost Mobile - Android SDK Installer
echo ============================================
echo.
echo This script will install the Android SDK required to build Ghost.Mobile
echo.
echo Step 1: Download Android Command Line Tools from:
echo   https://developer.android.com/studio#command-line-tools-only
echo.
echo Step 2: Extract to: %LOCALAPPDATA%\Android\Sdk\cmdline-tools
echo   The folder structure should be:
echo   %LOCALAPPDATA%\Android\Sdk\cmdline-tools\latest\bin\sdkmanager.bat
echo.
echo Step 3: Run the following commands:
echo   cd %%LOCALAPPDATA%%\Android\Sdk\cmdline-tools\latest\bin
echo   sdkmanager --licenses
echo   sdkmanager "platforms;android-35" "build-tools;35.0.0" "platform-tools"
echo.
echo Step 4: Build the project:
echo   cd C:\Users\user\Desktop\Ghost\Ghost.Mobile
echo   dotnet build
echo.
pause
