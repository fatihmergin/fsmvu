@echo off
start "NotificationApi" cmd /k "cd /d %~dp0src\NotificationApi && dotnet run"
start "OrderApi" cmd /k "cd /d %~dp0src\OrderApi && dotnet run"
