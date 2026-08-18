@echo off
setlocal
cd /d %~dp0

echo ==========================================
echo Classified Ads Educational API
echo ==========================================
echo.

dotnet --version
if errorlevel 1 (
  echo.
  echo ERROR: .NET 10 SDK was not found.
  echo Install .NET 10 SDK, then run this file again.
  pause
  exit /b 1
)

echo.
echo Restoring packages...
dotnet restore ClassifiedAds.sln
if errorlevel 1 goto :failed

echo.
echo Starting API on http://localhost:5086/swagger
dotnet run --project Api --launch-profile http
exit /b 0

:failed
echo.
echo Restore or run failed. Read README.md for setup details.
pause
exit /b 1
