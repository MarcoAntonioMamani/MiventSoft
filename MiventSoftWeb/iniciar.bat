@echo off
REM =====================================================================
REM  Santit Web - arranque en esta PC contra la BD del escritorio
REM  Doble clic. La primera vez instala dependencias (necesita internet).
REM =====================================================================
chcp 65001 >nul
title Santit Web
cd /d "%~dp0"

where node >nul 2>nul
if errorlevel 1 (
  echo.
  echo  Node.js no esta instalado. Descargue la version LTS de https://nodejs.org
  echo  instale con las opciones por defecto y vuelva a ejecutar este archivo.
  pause
  exit /b 1
)

REM Si quedo abierta una copia anterior (con otra conexion), se cierra primero
call "%~dp0detener.bat"

echo  Leyendo la conexion desde dbConnect.TXT ...
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0configurar.ps1"
if errorlevel 1 ( pause & exit /b 1 )

echo  Verificando dependencias ...
pushd server
call npm install --no-audit --no-fund --loglevel=error
if errorlevel 1 ( popd & echo  Error instalando dependencias del servidor. & pause & exit /b 1 )
popd
pushd client
call npm install --no-audit --no-fund --loglevel=error
if errorlevel 1 ( popd & echo  Error instalando dependencias de la web. & pause & exit /b 1 )
popd

echo  Compilando la web ...
pushd client
call npm run build --silent
if errorlevel 1 ( popd & echo  Error al compilar. & pause & exit /b 1 )
popd

echo.
echo  Verificando la base de datos (solo lectura) ...
pushd server
node test\verificar.js
set VERIF=%errorlevel%
if "%VERIF%"=="1" ( popd & pause & exit /b 1 )
if "%VERIF%"=="2" (
  echo  Hay advertencias arriba. Puede continuar, pero revise esas pantallas.
  pause
)

start "" /b cmd /c "timeout /t 3 /nobreak >nul & start http://localhost:3001"
node src\app.js
popd
pause
