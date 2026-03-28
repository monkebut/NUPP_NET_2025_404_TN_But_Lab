@echo off
chcp 65001 >nul 2>&1
title Lab 1 - Console Application

cd /d "%~dp0"

echo ========================================
echo   LABORATORY WORK 1
echo   Additional Task: JSON File Operations
echo ========================================
echo.

echo Building project...
dotnet build CinemaManagement.Console\CinemaManagement.Console.csproj

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build failed! Please check errors above.
    pause
    exit /b 1
)

echo.
echo Running application...
echo ========================================
echo.

dotnet run --project CinemaManagement.Console\CinemaManagement.Console.csproj

echo.
echo ========================================
echo Application finished.
pause

