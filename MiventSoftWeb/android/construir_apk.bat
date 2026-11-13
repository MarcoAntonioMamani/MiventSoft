@echo off
REM =====================================================================
REM  Sanbit movil - genera el APK firmado en android\apk\
REM  Nombre de la app, id y nombre del APK: APP_NOMBRE / APP_ID / APK_NOMBRE en app\build.gradle
REM  Requiere: JDK 11 (el de Android Studio) y el SDK de Android (local.properties)
REM  La primera vez crea la firma (santit-release.jks + keystore.properties):
REM  RESPALDELA. Sin ella no se puede actualizar la app ya instalada.
REM =====================================================================
setlocal
cd /d "%~dp0"

if not defined JAVA_HOME if exist "C:\Program Files\Android\Android Studio\jre\bin\java.exe" set "JAVA_HOME=C:\Program Files\Android\Android Studio\jre"
if not defined JAVA_HOME if exist "C:\Program Files\Android\Android Studio\jbr\bin\java.exe" set "JAVA_HOME=C:\Program Files\Android\Android Studio\jbr"
if not defined JAVA_HOME ( echo  No se encontro el JDK. Defina JAVA_HOME. & pause & exit /b 1 )

if not exist local.properties (
  echo  Falta local.properties con sdk.dir=RUTA_DEL_SDK  ^(ej. sdk.dir=D\:\backup\Sdk^)
  pause & exit /b 1
)

if exist keystore.properties goto compilar
echo  Creando la firma de la app ...
for /f %%p in ('powershell -NoProfile -Command "[guid]::NewGuid().ToString('N')"') do set "CLAVE=%%p"
if not defined CLAVE ( echo  No se pudo generar la clave. & pause & exit /b 1 )
"%JAVA_HOME%\bin\keytool" -genkeypair -keystore santit-release.jks -alias santit -keyalg RSA -keysize 2048 -validity 10000 -storepass "%CLAVE%" -keypass "%CLAVE%" -dname "CN=Santit, O=Santit, C=BO"
if errorlevel 1 ( echo  Error creando la firma. & pause & exit /b 1 )
> keystore.properties echo storeFile=santit-release.jks
>> keystore.properties echo storePassword=%CLAVE%
>> keystore.properties echo keyAlias=santit
>> keystore.properties echo keyPassword=%CLAVE%
set "CLAVE="

:compilar
call "%~dp0gradlew.bat" copiarApk --no-daemon
if errorlevel 1 ( echo. & echo  Error al compilar el APK. & pause & exit /b 1 )

echo.
echo  APK en %~dp0apk\ (el mas nuevo primero):
dir /b /o-d "%~dp0apk\*.apk"
endlocal
