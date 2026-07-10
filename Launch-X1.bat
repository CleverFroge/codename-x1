@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Launcher\Launch-X1.ps1" %*
exit /b %ERRORLEVEL%
