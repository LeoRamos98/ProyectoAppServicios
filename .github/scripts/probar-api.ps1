#requires -Version 7.3
param(
    [Parameter(Mandatory)][ValidateRange(1, 2147483647)][int]$IdUsuario,
    [Parameter(Mandatory)][ValidateRange(1, 2147483647)][int]$IdProfesional
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true
$proyecto = (Resolve-Path "$PSScriptRoot/../../ProyectoAppServicios").Path
$api = 'http://127.0.0.1:5198'

function Nuevo-Token([int]$Id) {
    $jwt = dotnet user-jwts create --project "$proyecto/ProyectoAppServicios.csproj" --name "$Id" --audience $api --valid-for 15m --output token |
        Select-Object -Last 1
    if (!$jwt) { throw "No se pudo emitir el token de ensayo del usuario $Id." }
    return $jwt.Trim()
}

function Probar([string]$Metodo, [string]$Ruta, [int]$Esperado, [string]$Token = '', $Cuerpo = $null) {
    $peticion = @{
        Method = $Metodo
        Uri = "$api/api/$Ruta"
        SkipHttpErrorCheck = $true
        StatusCodeVariable = 'codigo'
        TimeoutSec = 20
    }
    if ($Token) { $peticion.Headers = @{ Authorization = "Bearer $Token" } }
    if ($null -ne $Cuerpo) {
        $peticion.ContentType = 'application/json; charset=utf-8'
        $peticion.Body = $Cuerpo | ConvertTo-Json -Compress
    }
    $respuesta = Invoke-RestMethod @peticion
    if ($codigo -ne $Esperado) { throw "$Metodo $Ruta devolvió HTTP $codigo. Se esperaba $Esperado." }
    Write-Host "HTTP $codigo  $Metodo $Ruta"
    return $respuesta
}

$cliente = Nuevo-Token $IdUsuario
$profesional = Nuevo-Token $IdProfesional
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:DOTNET_ENVIRONMENT = 'Development'
$salida = [IO.Path]::GetTempFileName()
$errores = [IO.Path]::GetTempFileName()
$proceso = Start-Process dotnet -WorkingDirectory $proyecto -PassThru -ArgumentList @(
    'bin/Release/net10.0/ProyectoAppServicios.dll', '--urls', $api
) -RedirectStandardOutput $salida -RedirectStandardError $errores

try {
    $disponible = $false
    for ($intento = 0; $intento -lt 40; $intento++) {
        if ($proceso.HasExited) { throw "La API se cerró al iniciar (código $($proceso.ExitCode))." }
        try {
            $null = Invoke-RestMethod "$api/openapi/v1.json" -TimeoutSec 2
            $disponible = $true
            break
        } catch {
            Start-Sleep -Seconds 1
        }
    }
    if (!$disponible) { throw 'La API no respondió en el puerto de prueba.' }

    $null = Probar GET 'propuestas/mias' 401
    $alta = @{
        nombre = 'Cuenta'
        apellido = 'Ensayo'
        email = "ci-$([Guid]::NewGuid().ToString('N'))@example.test"
        contrasenia = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(24))
        telefono = '3415550100'
        direccion = 'Calle de prueba 100'
    }
    $cuenta = Probar POST 'usuarios' 201 '' $alta
    $null = Probar POST 'auth/login' 401 '' @{ email = $alta.email; contrasenia = 'incorrecta' }
    $login = Probar POST 'auth/login' 200 '' @{ email = $alta.email; contrasenia = $alta.contrasenia }
    if (!$login.token) { throw 'El login respondió sin token.' }
    $yo = Probar GET 'auth/yo' 200 $login.token
    if ($yo.id -ne $cuenta.id) { throw 'El token identifica a otra cuenta.' }

    $datos = @{
        tipoServicio = 'Electricidad'
        descripcion = 'Revisión de instalación'
        ubicacion = 'Calle de prueba 100'
        latitud = -32.75
        longitud = -61.25
        fechaSolicitada = (Get-Date).AddDays(2).ToString('yyyy-MM-dd')
    }
    $propia = Probar POST 'propuestas' 201 $login.token $datos
    $misPropuestas = @(Probar GET 'propuestas/mias' 200 $login.token)
    if ($propia.idUsuario -ne $cuenta.id -or $propia.id -notin $misPropuestas.id) {
        throw 'La cuenta autenticada debe poder crear y consultar su propia propuesta.'
    }

    $propuesta = Probar POST 'propuestas' 201 $cliente $datos
    $mias = @(Probar GET 'propuestas/mias' 200 $cliente)
    $leida = $mias | Where-Object id -eq $propuesta.id
    if (!$leida -or $leida.idUsuario -ne $IdUsuario -or $leida.latitud -ne $datos.latitud -or $leida.longitud -ne $datos.longitud) {
        throw 'La propuesta guardada no conserva el usuario y las coordenadas enviados.'
    }
    $null = Probar PUT "propuestas/$($propuesta.id)" 403 $profesional $datos
    $oferta = @{ idPropuesta = $propuesta.id; monto = 1250.50; descripcion = 'Mano de obra' }
    $presupuesto = Probar POST 'presupuestos' 201 $profesional $oferta
    $null = Probar POST 'presupuestos' 409 $profesional $oferta
    $recibidos = @(Probar GET "presupuestos/propuesta/$($propuesta.id)" 200 $cliente)
    if ($presupuesto.id -notin $recibidos.id) { throw 'El presupuesto no figura entre los recibidos.' }
    $null = Probar POST "presupuestos/$($presupuesto.id)/confirmar" 409 $profesional
    $null = Probar POST "presupuestos/$($presupuesto.id)/aceptar" 200 $cliente
    $acuerdo = Probar POST "presupuestos/$($presupuesto.id)/confirmar" 200 $profesional
    $reintento = Probar POST "presupuestos/$($presupuesto.id)/confirmar" 200 $profesional
    if ($acuerdo.idEncargo -le 0 -or $acuerdo.idEncargo -ne $reintento.idEncargo) {
        throw 'La doble aprobación debe devolver un único encargo.'
    }
} catch {
    Get-Content $errores -Tail 15 | Write-Host
    throw
} finally {
    if (!$proceso.HasExited) { $proceso.Kill(); $proceso.WaitForExit() }
    $proceso.Dispose()
    Remove-Item -LiteralPath $salida, $errores
}
