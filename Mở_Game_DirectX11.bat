@echo off
title Mo Game thoat khoi dia nguc - DirectX 11 Mode
echo.
echo ======================================================================
echo   Dang khoi dong Unity Editor o che do DirectX 11 (de tranh crash GPU)...
echo ======================================================================
echo.
start "" "C:\unity\6000.4.9f1\Editor\Unity.exe" -projectPath "%~dp0" -force-d3d11
exit
