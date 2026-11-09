@echo off
REM Saca el diseno de los .rpt del escritorio a herramientas\rpt (solo lectura, no toca la base)
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0volcar_rpt.ps1"
if errorlevel 1 (
  echo.
  echo Intentando con PowerShell de 32 bits...
  "%SystemRoot%\SysWOW64\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -File "%~dp0volcar_rpt.ps1"
)
pause
