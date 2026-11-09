@echo off
REM Detiene la copia de Santit Web (node.exe) que siga escuchando en el puerto 3001.
REM No toca otros programas aunque usen ese puerto.
chcp 65001 >nul
set PUERTO=3001
set ENCONTRADO=
for /f "tokens=5" %%p in ('netstat -ano ^| findstr /r /c:":%PUERTO% .*LISTENING"') do (
  tasklist /FI "PID eq %%p" /NH | find /i "node.exe" >nul && (
    set ENCONTRADO=1
    echo  Cerrando la copia anterior de Santit Web (proceso %%p^) ...
    taskkill /PID %%p /F >nul 2>nul
  )
)
if not defined ENCONTRADO echo  No habia otra copia de Santit Web abierta.
timeout /t 1 /nobreak >nul
