#requires -Version 7.3
param(
    [Parameter(Mandatory)][ValidateRange(1, 2147483647)][int]$IdUsuario,
    [Parameter(Mandatory)][ValidateRange(1, 2147483647)][int]$IdProfesional,
    [ValidateRange(1, 2147483647)][int]$IdAdministrador
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

$clienteSemilla = Nuevo-Token $IdUsuario
$profesionalSemilla = Nuevo-Token $IdProfesional
$admin = if ($IdAdministrador) { Nuevo-Token $IdAdministrador }
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

    $null = Probar GET 'propuestas/mias' 200 $clienteSemilla
    $null = Probar GET 'propuestas/activas' 200 $profesionalSemilla
    $cliente = $login.token

    $altaPro = $alta.Clone()
    $altaPro.email = "pro-$([Guid]::NewGuid().ToString('N'))@example.test"
    $altaPro.contrasenia = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(24))
    $altaPro.zona = 'Zona de prueba'
    $altaPro.tipoProfesional = 'Electricista'
    $altaPro.nroMatricula = [Guid]::NewGuid().ToString('N')
    $cuentaPro = Probar POST 'profesionales' 201 '' $altaPro
    $loginPro = Probar POST 'auth/login' 200 '' @{ email = $altaPro.email; contrasenia = $altaPro.contrasenia }
    $profesional = $loginPro.token
    $rutaPerfil = "profesionales/$($cuentaPro.id)/perfil"
    $perfil = Probar GET $rutaPerfil 200 $cliente
    if ($perfil.trabajosRealizados.Count -ne 0 -or $null -ne $perfil.promedio) {
        throw 'Un profesional recién registrado debe tener el historial vacío y el promedio sin valor.'
    }

    $null = Probar GET 'usuarios' 401
    $null = Probar GET 'usuarios' 403 $cliente
    $null = Probar GET "usuarios/$($cuenta.id)" 200 $cliente
    $null = Probar PUT "usuarios/$($cuenta.id)" 403 $profesional $alta
    $null = Probar PUT "usuarios/$($cuenta.id)" 200 $cliente $alta
    $null = Probar DELETE "usuarios/$($cuenta.id)" 403 $profesional
    $presentacion = @{ comentarioPresentacion = 'Instalaciones eléctricas' }
    $null = Probar PATCH "profesionales/$($cuentaPro.id)/presentacion" 403 $cliente $presentacion
    $null = Probar PATCH "profesionales/$($cuentaPro.id)/presentacion" 200 $profesional $presentacion

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
    if (!$leida -or $leida.idUsuario -ne $cuenta.id -or $leida.latitud -ne $datos.latitud -or $leida.longitud -ne $datos.longitud) {
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

    $idEncargo = $acuerdo.idEncargo
    $ruta = "encargos/$idEncargo"
    $nota = @{ puntaje = 5; comentario = 'Trabajo terminado en la fecha acordada.' }
    $null = Probar GET $ruta 401
    $null = Probar GET $ruta 403 $clienteSemilla
    $null = Probar GET $ruta 200 $profesional
    $null = Probar GET "encargos/usuario/$($cuenta.id)" 200 $cliente
    $null = Probar GET "encargos/profesional/$($cuentaPro.id)" 403 $cliente
    $null = Probar PUT "$ruta/iniciar" 403 $cliente
    $null = Probar PUT "$ruta/finalizar" 409 $profesional
    $null = Probar POST "$ruta/calificacion" 409 $cliente $nota
    $null = Probar PATCH "profesionales/$($cuentaPro.id)/disponibilidad" 409 $profesional @{ disponible = $true }
    $inicio = Probar PUT "$ruta/iniciar" 200 $profesional
    if ($inicio.estado -ne 1 -or !$inicio.fechaInicioEncargo) { throw 'El encargo debe estar en curso y tener fecha de inicio.' }
    $null = Probar PUT "$ruta/finalizar" 403 $cliente
    $fin = Probar PUT "$ruta/finalizar" 200 $profesional
    if ($fin.estado -ne 2 -or !$fin.fechaFinalizacionEncargo) { throw 'El encargo debe quedar finalizado y fechado.' }
    $perfil = Probar GET $rutaPerfil 200 $cliente
    $trabajo = @($perfil.trabajosRealizados)
    if ($perfil.profesional.estado -ne 0 -or $trabajo.Count -ne 1 -or $trabajo[0].idEncargo -ne $idEncargo -or
        $trabajo[0].estadoCalificacion -ne 'Sin calificación' -or $null -ne $trabajo[0].puntaje -or $null -ne $perfil.promedio) {
        throw 'El cierre debe liberar al profesional y mostrar el trabajo todavía sin calificación.'
    }
    $null = Probar POST "$ruta/calificacion" 403 $profesional $nota
    $null = Probar POST "$ruta/calificacion" 400 $cliente @{ puntaje = 6 }
    $calificacion = Probar POST "$ruta/calificacion" 201 $cliente $nota
    $null = Probar POST "$ruta/calificacion" 409 $cliente $nota
    $guardada = Probar GET "$ruta/calificacion" 200 $cliente
    $perfil = Probar GET $rutaPerfil 200 $cliente
    if ($guardada.id -ne $calificacion.id -or $guardada.idUsuario -ne $cuenta.id -or
        $perfil.promedio -ne 5 -or $perfil.cantidadCalificaciones -ne 1 -or
        $perfil.trabajosRealizados[0].puntaje -ne 5 -or $perfil.trabajosRealizados[0].comentario -ne $nota.comentario) {
        throw 'El puntaje debe figurar una sola vez en el encargo y en el perfil profesional.'
    }

    if ($admin) {
        $null = Probar GET 'usuarios' 200 $admin
        $null = Probar GET 'encargos' 200 $admin
        $null = Probar PUT "usuarios/$($cuenta.id)" 200 $admin $alta
        $null = Probar PATCH "profesionales/$($cuentaPro.id)/presentacion" 200 $admin $presentacion
        $null = Probar DELETE "profesionales/$($cuentaPro.id)" 403 $cliente
        $null = Probar PUT "$ruta/cancelar" 409 $admin
        $segunda = Probar POST 'propuestas' 201 $cliente $datos
        $oferta.idPropuesta = $segunda.id
        $otra = Probar POST 'presupuestos' 201 $profesional $oferta
        $null = Probar POST "presupuestos/$($otra.id)/aceptar" 200 $cliente
        $null = Probar POST 'encargos' 403 $admin @{ idProfesionalPropuesta = $otra.id }
        $encargo = Probar POST 'encargos' 201 $profesional @{ idProfesionalPropuesta = $otra.id }
        $null = Probar POST 'encargos' 409 $profesional @{ idProfesionalPropuesta = $otra.id }
        $ruta = "encargos/$($encargo.id)"
        $null = Probar GET $ruta 200 $admin
        $null = Probar PUT "$ruta/iniciar" 403 $admin
        $null = Probar PUT "$ruta/iniciar" 200 $profesional
        $null = Probar PUT "$ruta/finalizar" 403 $admin
        $cancelado = Probar PUT "$ruta/cancelar" 200 $admin
        $null = Probar PUT "$ruta/iniciar" 409 $profesional
        $null = Probar POST "$ruta/calificacion" 409 $cliente $nota
        $perfil = Probar GET $rutaPerfil 200 $cliente
        if ($cancelado.estado -ne 3 -or $perfil.profesional.estado -ne 0 -or $perfil.trabajosRealizados.Count -ne 1) {
            throw 'Cancelar debe liberar al profesional y conservar únicamente los trabajos finalizados en su historial.'
        }
        $null = Probar DELETE "usuarios/$($cuentaPro.id)" 204 $admin
        $null = Probar GET $rutaPerfil 403 $profesional
        $perfil = Probar GET $rutaPerfil 200 $admin
        if ($perfil.profesional.activo -or $perfil.profesional.estado -ne 2) { throw 'La baja debe desactivar todo el perfil.' }
    }
} catch {
    Get-Content $errores -Tail 15 | Write-Host
    throw
} finally {
    if (!$proceso.HasExited) { $proceso.Kill(); $proceso.WaitForExit() }
    $proceso.Dispose()
    Remove-Item -LiteralPath $salida, $errores
}
