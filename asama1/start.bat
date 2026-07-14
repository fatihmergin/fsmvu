@echo off
start "Todo Log API" cmd /k "cd /d %~dp0TodoLog.Api && dotnet run"
timeout /t 2 >nul
start "Todo Yönetim API" cmd /k "cd /d %~dp0TodoYonetim.Api && dotnet run"
