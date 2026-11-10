# =====================================================================
#  volcar_rpt.ps1 - Saca el DISENO de todos los .rpt del escritorio a archivos JSON
#  (secciones, textos, campos, grupos, formulas, totales, parametros y graficos)
#  para que la web replique cada reporte igual que el escritorio.
#
#  - Solo LEE los .rpt: no se conecta a la base de datos ni modifica nada.
#  - Usa el motor de Crystal Reports que ya esta instalado para el escritorio.
#
#  Uso (doble clic en volcar_rpt.bat, o en PowerShell):
#    powershell -ExecutionPolicy Bypass -File volcar_rpt.ps1
#    powershell -ExecutionPolicy Bypass -File volcar_rpt.ps1 -Carpeta "D:\ruta\ReporteSmartApps"
#  Resultado: herramientas\rpt\<Reporte>.json  +  herramientas\rpt\_resumen.txt
# =====================================================================
param(
    [string]$Carpeta = (Join-Path $PSScriptRoot '..\..\TeVendo\ReporteSmartApps'),
    [string]$Salida  = (Join-Path $PSScriptRoot 'rpt')
)
$ErrorActionPreference = 'Stop'

# --- cargar el motor de Crystal (el mismo del escritorio) ---
$ensamblados = 'CrystalDecisions.Shared', 'CrystalDecisions.CrystalReports.Engine', 'CrystalDecisions.ReportAppServer.ClientDoc',
               'CrystalDecisions.ReportAppServer.Controllers', 'CrystalDecisions.ReportAppServer.ReportDefModel', 'CrystalDecisions.ReportAppServer.DataDefModel'
foreach ($a in $ensamblados) {
    try { [void][Reflection.Assembly]::LoadWithPartialName($a) } catch { }
}
if (-not ('CrystalDecisions.CrystalReports.Engine.ReportDocument' -as [type])) {
    Write-Host ''
    Write-Host 'No se encontro el motor de Crystal Reports en este PowerShell.' -ForegroundColor Red
    Write-Host 'volcar_rpt.bat lo reintenta con PowerShell de 32 bits. Si tampoco funciona, instale el runtime de Crystal Reports del escritorio.' -ForegroundColor Yellow
    exit 1
}

$Carpeta = (Resolve-Path $Carpeta).Path
New-Item -ItemType Directory -Force -Path $Salida | Out-Null
$archivos = Get-ChildItem -Path $Carpeta -Filter *.rpt -File | Sort-Object Name
Write-Host "Leyendo $($archivos.Count) reportes de $Carpeta" -ForegroundColor Cyan

function Txt($x) { if ($null -eq $x) { return $null } return [string]$x }
function Fuente($f) {
    if ($null -eq $f) { return $null }
    return [ordered]@{ nombre = $f.Name; tam = [math]::Round($f.SizeInPoints, 1); negrita = $f.Bold; cursiva = $f.Italic }
}
function Caja($o) { return [ordered]@{ x = $o.Left; y = $o.Top; ancho = $o.Width; alto = $o.Height } } # twips (1440 = 1 pulgada)

function Objeto($o) {
    $r = [ordered]@{ nombre = $o.Name; tipo = [string]$o.Kind; caja = (Caja $o) }
    try {
        switch ([string]$o.Kind) {
            'TextObject'      { $r.texto = Txt $o.Text; $r.fuente = Fuente $o.Font; $r.alineacion = [string]$o.ObjectFormat.HorizontalAlignment }
            'FieldObject'     { $r.campo = Txt $o.DataSource.FormulaName; $r.tipoDato = [string]$o.DataSource.ValueType; $r.fuente = Fuente $o.Font
                                $r.alineacion = [string]$o.ObjectFormat.HorizontalAlignment
                                try { $r.formato = [ordered]@{ decimales = $o.FieldFormat.NumericFormat.DecimalPlaces; separadorMiles = $o.FieldFormat.NumericFormat.ThousandsSeparator; fecha = [string]$o.FieldFormat.DateFormat.DateOrder } } catch { } }
            'FieldHeadingObject' { $r.texto = Txt $o.Text; $r.campo = Txt $o.FieldObjectName; $r.fuente = Fuente $o.Font }
            'BoxObject'       { $r.borde = [string]$o.LineStyle; $r.redondeo = $o.CornerEllipseWidth; try { $r.relleno = $o.FillColor.Name } catch { } }
            'LineObject'      { $r.linea = [string]$o.LineStyle }
            'PictureObject'   { }
            'SubreportObject' { $r.subreporte = $o.SubreportName }
            'ChartObject'     { }
            'CrossTabObject'  { }
        }
    } catch { $r.error = $_.Exception.Message }
    return $r
}

# Graficos: el modelo RAS si expone tipo y campos del grafico
function Graficos($rd) {
    $lista = @()
    try {
        $rcd = $rd.ReportClientDocument
        $objs = $rcd.ReportDefController.ReportObjectController.GetReportObjectsByKind([CrystalDecisions.ReportAppServer.ReportDefModel.CrReportObjectKindEnum]::crReportObjectKindChart)
        foreach ($c in $objs) {
            $g = [ordered]@{ nombre = $c.Name; seccion = $c.SectionName; caja = [ordered]@{ x = $c.Left; y = $c.Top; ancho = $c.Width; alto = $c.Height } }
            try { $g.tipo = [string]$c.ChartStyle.Type } catch { }
            try { $g.titulo = $c.ChartStyle.TextOptions.Title; $g.subtitulo = $c.ChartStyle.TextOptions.Subtitle; $g.pie = $c.ChartStyle.TextOptions.FootNote
                  $g.tituloEjeX = $c.ChartStyle.TextOptions.GroupAxisTitle; $g.tituloEjeY = $c.ChartStyle.TextOptions.DataAxisTitle; $g.tituloSerie = $c.ChartStyle.TextOptions.SeriesAxisTitle } catch { }
            try { $g.leyenda = [string]$c.ChartStyle.IsLegendVisible; $g.posicionLeyenda = [string]$c.ChartStyle.LegendPosition } catch { }
            try { $g.definicion = [string]$c.ChartDefinition.ChartType } catch { }
            try { $g.agrupadoPor = @($c.ChartDefinition.GroupGroupFields | ForEach-Object { $_.FormulaForm }) } catch { }
            try { $g.series = @($c.ChartDefinition.SeriesGroupFields | ForEach-Object { $_.FormulaForm }) } catch { }
            try { $g.valores = @($c.ChartDefinition.DataFields | ForEach-Object { $_.FormulaForm }) } catch { }
            $lista += $g
        }
    } catch { $lista += [ordered]@{ error = "No se pudo leer los graficos: $($_.Exception.Message)" } }
    return $lista
}

function Volcar($rd) {
    $r = [ordered]@{}
    $r.papel = [ordered]@{}
    try { $r.papel.orientacion = [string]$rd.PrintOptions.PaperOrientation; $r.papel.tamano = [string]$rd.PrintOptions.PaperSize
          $m = $rd.PrintOptions.PageMargins; $r.papel.margenes = [ordered]@{ arriba = $m.topMargin; abajo = $m.bottomMargin; izq = $m.leftMargin; der = $m.rightMargin } } catch { }
    try { $r.tablas = @($rd.Database.Tables | ForEach-Object { [ordered]@{ tabla = $_.Name; campos = @($_.Fields | ForEach-Object { [ordered]@{ campo = $_.Name; tipo = [string]$_.ValueType } }) } }) } catch { }
    try { $r.grupos = @($rd.DataDefinition.Groups | ForEach-Object { [ordered]@{ campo = $_.ConditionField.FormulaName; orden = [string]$_.ConditionField.Kind } }) } catch { }
    try { $r.ordenarPor = @($rd.DataDefinition.SortFields | ForEach-Object { [ordered]@{ campo = $_.Field.FormulaName; sentido = [string]$_.SortDirection } }) } catch { }
    try { $r.formulas = @($rd.DataDefinition.FormulaFields | ForEach-Object { [ordered]@{ nombre = $_.FormulaName; texto = $_.Text } }) } catch { }
    try { $r.totales = @($rd.DataDefinition.SummaryFields | ForEach-Object { [ordered]@{ nombre = $_.FormulaName; operacion = [string]$_.Operation; campo = $_.SummarizedField.FormulaName } }) } catch { }
    try { $r.totalesAcumulados = @($rd.DataDefinition.RunningTotalFields | ForEach-Object { [ordered]@{ nombre = $_.FormulaName; operacion = [string]$_.Operation; campo = $_.SummarizedField.FormulaName } }) } catch { }
    try { $r.parametros = @($rd.DataDefinition.ParameterFields | ForEach-Object { [ordered]@{ nombre = $_.Name; tipo = [string]$_.ParameterValueKind } }) } catch { }
    try { $r.filtro = $rd.DataDefinition.RecordSelectionFormula } catch { }
    $r.secciones = @()
    foreach ($s in $rd.ReportDefinition.Sections) {
        $sec = [ordered]@{ nombre = $s.Name; tipo = [string]$s.Kind; alto = $s.Height; oculta = $false; nuevaPaginaAntes = $false }
        try { $sec.oculta = $s.SectionFormat.EnableSuppress; $sec.nuevaPaginaAntes = $s.SectionFormat.EnableNewPageBefore; $sec.nuevaPaginaDespues = $s.SectionFormat.EnableNewPageAfter } catch { }
        try { $sec.color = $s.SectionFormat.BackgroundColor.Name } catch { }
        $sec.objetos = @($s.ReportObjects | Sort-Object Top, Left | ForEach-Object { Objeto $_ })
        $r.secciones += $sec
    }
    $r.graficos = @(Graficos $rd)
    return $r
}

function Unir($a, $b) { foreach ($k in $b.Keys) { $a[$k] = $b[$k] }; return $a }

$resumen = @()
foreach ($f in $archivos) {
    $rd = New-Object CrystalDecisions.CrystalReports.Engine.ReportDocument
    try {
        $rd.Load($f.FullName)
        $d = [ordered]@{ reporte = $f.BaseName; archivo = $f.Name; modificado = $f.LastWriteTime.ToString('yyyy-MM-dd') }
        $d = Unir $d (Volcar $rd)
        $d.subreportes = @()
        foreach ($sub in $rd.Subreports) {
            $x = Unir ([ordered]@{ nombre = $sub.Name }) (Volcar $sub); $d.subreportes += $x
        }
        $json = $d | ConvertTo-Json -Depth 12
        [IO.File]::WriteAllText((Join-Path $Salida "$($f.BaseName).json"), $json, (New-Object Text.UTF8Encoding $false))
        $nObj = ($d.secciones | ForEach-Object { $_.objetos.Count } | Measure-Object -Sum).Sum
        $resumen += "OK     $($f.Name)  secciones=$($d.secciones.Count) objetos=$nObj grupos=$($d.grupos.Count) graficos=$($d.graficos.Count)"
        Write-Host "  OK  $($f.Name)" -ForegroundColor Green
    } catch {
        $resumen += "ERROR  $($f.Name)  $($_.Exception.Message)"
        Write-Host "  ERROR  $($f.Name): $($_.Exception.Message)" -ForegroundColor Red
    } finally {
        try { $rd.Close(); $rd.Dispose() } catch { }
    }
}
[IO.File]::WriteAllLines((Join-Path $Salida '_resumen.txt'), $resumen, (New-Object Text.UTF8Encoding $false))
Write-Host ''
Write-Host "Listo. Archivos en: $Salida" -ForegroundColor Cyan
