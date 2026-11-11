@echo off
REM ==============================================================
REM  Sube los cambios de la rama actual al repositorio (origin)
REM  Uso: doble clic o desde cmd:  subir.bat "mensaje del commit"
REM ==============================================================
setlocal
cd /d "%~dp0"

where git >nul 2>nul
if errorlevel 1 (
    echo [ERROR] No se encontro git en el PATH.
    goto :fin
)

for /f "delims=" %%b in ('git rev-parse --abbrev-ref HEAD') do set RAMA=%%b
if "%RAMA%"=="HEAD" (
    echo [ERROR] Estas en HEAD separado ^(detached^). Hace checkout de una rama primero.
    goto :fin
)

echo.
echo Rama actual: %RAMA%
echo.
echo ---- Cambios pendientes ----
git status --short
echo ----------------------------
echo.

git status --porcelain | findstr . >nul
if errorlevel 1 (
    echo No hay cambios para confirmar. Se intentara subir commits pendientes.
    goto :subir
)

set "MSG=%~1"
if "%MSG%"=="" set /p "MSG=Mensaje del commit (Enter = 'Avance migracion web Kaily'): "
if "%MSG%"=="" set "MSG=Avance migracion web Kaily"

set /p "OK=Confirmar commit y push a origin/%RAMA%? (S/N): "
if /i not "%OK%"=="S" (
    echo Cancelado.
    goto :fin
)

git add -A
if errorlevel 1 goto :error
git commit -m "%MSG%"
if errorlevel 1 goto :error

:subir
echo.
git ls-remote --exit-code --heads origin %RAMA% >nul 2>nul
if errorlevel 1 (
    echo La rama %RAMA% todavia no existe en origin. Se creara al subir.
    goto :push
)
echo Trayendo cambios remotos de origin/%RAMA% ...
git pull --no-rebase origin %RAMA%
if errorlevel 1 (
    echo [ERROR] El pull fallo o hay conflictos. Resolvelos y volve a ejecutar.
    goto :fin
)

:push

echo.
echo Subiendo a origin/%RAMA% ...
git push -u origin %RAMA%
if errorlevel 1 goto :error

echo.
echo [OK] Cambios subidos a origin/%RAMA%.
goto :fin

:error
echo.
echo [ERROR] Fallo el ultimo comando de git. Revisa el mensaje de arriba.

:fin
echo.
pause
endlocal
