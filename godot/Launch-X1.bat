@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Launch-X1.ps1" %*
exit /b %ERRORLEVEL%
