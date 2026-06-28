@echo off
set GODOT=C:\Users\15778\AppData\Local\Microsoft\WinGet\Packages\GodotEngine.GodotEngine.Mono_Microsoft.Winget.Source_8wekyb3d8bbwe\Godot_v4.7-stable_mono_win64\Godot_v4.7-stable_mono_win64.exe
set PROJECT=%~dp0

if not exist "%GODOT%" (
    echo [错误] 找不到 Godot 4.7 Mono
    echo 请运行: winget install GodotEngine.GodotEngine.Mono
    pause
    exit /b 1
)

echo Building C# ...
dotnet build "%PROJECT%CodenameX1.csproj"
if errorlevel 1 pause & exit /b 1

set PERM=%USERPROFILE%\.nuget\packages\system.security.permissions\8.0.0\lib\net8.0\System.Security.Permissions.dll
if exist "%PERM%" (
  copy /Y "%PERM%" "%PROJECT%.godot\mono\temp\bin\Debug\" >nul 2>&1
  copy /Y "%PERM%" "%PROJECT%.godot\mono\temp\bin\Release\" >nul 2>&1
)

echo Starting 代号X1 ...
start "" "%GODOT%" --path "%PROJECT%"
