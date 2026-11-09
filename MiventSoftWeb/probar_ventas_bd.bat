@echo off
REM =====================================================================
REM  Prueba de grabacion del modulo Ventas contra la BD de PRUEBAS.
REM  Graba, modifica y elimina con sus SP reales dentro de una transaccion
REM  que al final se REVIERTE: no queda nada grabado.
REM  Pide escribir el nombre de la base antes de empezar. NO usar en produccion.
REM =====================================================================
chcp 65001 >nul
title Santit Web - Prueba de Ventas en BD
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0configurar.ps1"
if errorlevel 1 ( pause & exit /b 1 )
pushd server
if not exist node_modules call npm install --no-audit --no-fund --loglevel=error
node test\verificar.js
node test\probar_ventas_bd.js
popd
pause
