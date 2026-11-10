# =====================================================================
#  Genera server\.env a partir del dbConnect.TXT del escritorio
#  (misma lectura que _prLeerArchivoConfig de Tec_Login.vb).
#  Las credenciales se quedan en esta PC: no se muestran ni se envian.
#
#  Uso:  powershell -ExecutionPolicy Bypass -File configurar.ps1 [-Archivo ruta] [-Forzar]
# =====================================================================
param(
    [string]$Archivo = (Join-Path $PSScriptRoot '..\TeVendo\bin\Debug\dbConnect.TXT'),
    [switch]$Forzar
)
$ErrorActionPreference = 'Stop'
$envPath = Join-Path $PSScriptRoot 'server\.env'

if ((Test-Path $envPath) -and -not $Forzar) {
    Write-Host "server\.env ya existe. Use -Forzar para regenerarlo." -ForegroundColor Yellow
    exit 0
}
if (-not (Test-Path $Archivo)) {
    Write-Host "No se encontro dbConnect.TXT en: $Archivo" -ForegroundColor Red
    Write-Host "Indique la ruta:  powershell -ExecutionPolicy Bypass -File configurar.ps1 -Archivo 'C:\ruta\dbConnect.TXT'"
    exit 1
}

$lineas = Get-Content -LiteralPath $Archivo
if ($lineas.Count -lt 5) { Write-Host "dbConnect.TXT debe tener al menos 5 lineas (Ip, Usuario, Clave, BD, CarpetaRaiz)." -ForegroundColor Red; exit 1 }

# Igual que VB: Archivo(i).Split("=")(1).Trim  (aqui: todo lo que va despues del primer '=')
function Valor([int]$i) {
    $l = [string]$lineas[$i]
    $p = $l.IndexOf('=')
    if ($p -lt 0) { throw "Linea $($i + 1) de dbConnect.TXT sin '='" }
    return $l.Substring($p + 1).Trim()
}
$ip = Valor 0; $usuario = Valor 1; $clave = Valor 2; $bd = Valor 3; $raiz = Valor 4

# Servidor: admite "IP", "IP,puerto", "EQUIPO\INSTANCIA", ".", "(local)"
$servidor = $ip; $instancia = ''; $puerto = '1433'
if ($servidor -match '^(.*)\\(.+)$') { $servidor = $Matches[1]; $instancia = $Matches[2] }
if ($servidor -match '^(.*),(\d+)$') { $servidor = $Matches[1]; $puerto = $Matches[2] }
if ($servidor -in @('.', '(local)', '(localdb)', '')) { $servidor = 'localhost' }

function Comillas([string]$v) { if ($v.Contains("'")) { return '"' + $v.Replace('\', '\\').Replace('"', '\"') + '"' } else { return "'" + $v + "'" } }

$bytes = New-Object byte[] 48
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
$jwt = -join ($bytes | ForEach-Object { $_.ToString('x2') })

$contenido = @"
# Generado por configurar.ps1 desde dbConnect.TXT el $(Get-Date -Format 'yyyy-MM-dd HH:mm')
# NO compartir ni subir a ningun repositorio.
DB_SERVER=$servidor
DB_INSTANCE=$instancia
DB_PORT=$puerto
DB_USER=$(Comillas $usuario)
DB_PASSWORD=$(Comillas $clave)
DB_NAME=$(Comillas $bd)
DB_ENCRYPT=false
DB_TRUST_CERT=true
CARPETA_RAIZ=$(Comillas $raiz)

PORT=3001
NODE_ENV=production
# Poner true solo si se publica con HTTPS (IIS / certificado)
COOKIE_SECURE=false
JWT_SECRET=$jwt
JWT_HORAS=10
"@
[System.IO.File]::WriteAllText($envPath, $contenido, (New-Object System.Text.UTF8Encoding $false))

# Solo el usuario actual puede leer el .env
try {
    $acl = Get-Acl $envPath
    $acl.SetAccessRuleProtection($true, $false)
    $regla = New-Object System.Security.AccessControl.FileSystemAccessRule([System.Security.Principal.WindowsIdentity]::GetCurrent().Name, 'FullControl', 'Allow')
    $acl.SetAccessRule($regla)
    Set-Acl $envPath $acl
} catch { }

Write-Host "server\.env creado (servidor: $servidor$(if ($instancia) { '\' + $instancia }), BD: $bd)." -ForegroundColor Green
