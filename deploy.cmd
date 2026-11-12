@echo off
setlocal
cd /d "%~dp0" || goto fail

if not defined ElinGamePath (
    echo Set the ElinGamePath environment variable and reopen your terminal.
    goto fail
)
if not exist "%ElinGamePath%\Elin.exe" (
    echo Elin.exe was not found in "%ElinGamePath%".
    goto fail
)
call :check_game || goto fail

set "ELIN_DEPLOY_SOURCE=%CD%\bin\Mod_ElinPartyCare"
set "ELIN_DEPLOY_TARGET=%ElinGamePath%\Package\Mod_ElinPartyCare"
dotnet build ElinPartyCare.csproj "-p:OutputPath=bin/Mod_ElinPartyCare/"
if errorlevel 1 goto fail
if not exist "%ELIN_DEPLOY_SOURCE%\ElinPartyCare.dll" goto fail
if not exist "%ELIN_DEPLOY_SOURCE%\package.xml" goto fail
call :check_game || goto fail

robocopy "%ELIN_DEPLOY_SOURCE%" "%ELIN_DEPLOY_TARGET%" ElinPartyCare.dll package.xml /R:0 /W:0 /NP
if errorlevel 8 (
    echo Copy failed. Retrying the copy with administrator permission...
    powershell.exe -NoProfile -Command "try { $argsForCopy = ('{0}{1}{0} {0}{2}{0} ElinPartyCare.dll package.xml /R:0 /W:0 /NP' -f [char]34,$env:ELIN_DEPLOY_SOURCE,$env:ELIN_DEPLOY_TARGET); $p = Start-Process -FilePath (Join-Path $env:SystemRoot 'System32\robocopy.exe') -ArgumentList $argsForCopy -Verb RunAs -Wait -PassThru -ErrorAction Stop; if ($p.ExitCode -ge 8) { exit 1 }; exit 0 } catch { Write-Error $_; exit 1 }"
    if errorlevel 1 goto fail
)

echo.
echo Elin Party Care deployed. You can now start Elin.
pause
exit /b 0

:check_game
powershell.exe -NoProfile -Command "if (Get-Process -Name Elin -ErrorAction SilentlyContinue) { exit 1 }; exit 0"
if not errorlevel 1 exit /b 0
echo Close Elin and run deploy.cmd again.
exit /b 1

:fail
echo.
echo Deployment failed.
pause
exit /b 1
