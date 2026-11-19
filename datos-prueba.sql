-- Datos mínimos para probar encargos y calificaciones.
-- Crea: un cliente, un profesional, una propuesta del cliente y un
-- presupuesto del profesional aprobado por ambos.
-- Al final muestra los ids para el ensayo local con dotnet user-jwts.
-- El marcador ! no admite login. Las cuentas registradas por la API sí guardan su hash.

SET NOCOUNT ON;
BEGIN TRANSACTION;

DECLARE @IdCliente INT, @IdProfesional INT, @IdPropuesta INT, @IdPresupuesto INT;

-- Cliente (Rol 0 = Usuario)
INSERT INTO Usuarios (Nombre, Apellido, Direccion, Email, Contrasenia, Telefono, Rol, Activo)
VALUES ('Juan', 'Perez', 'Av. Siempre Viva 123', 'juan@test.com', '!', '1111-1111', 0, 1);
SET @IdCliente = SCOPE_IDENTITY();

-- Profesional (Rol 1 = Profesional). Por la herencia TPT va en dos tablas con el mismo Id.
INSERT INTO Usuarios (Nombre, Apellido, Direccion, Email, Contrasenia, Telefono, Rol, Activo)
VALUES ('Maria', 'Gomez', 'Calle Falsa 456', 'maria@test.com', '!', '2222-2222', 1, 1);
SET @IdProfesional = SCOPE_IDENTITY();

INSERT INTO Profesionales (Id, Zona, TipoProfesional, NroMatricula, FotoPerfil, ComentarioPresentacion, Estado)
VALUES (@IdProfesional, 'CABA', 'Plomero', 'MAT-001', '', 'Plomera con 10 años de experiencia', 0);

-- Propuesta del cliente (Estado 0 = Activa)
INSERT INTO Propuestas (IdUsuario, TipoServicio, Descripcion, Ubicacion, FotoPropuesta, FechaSolicitada, Estado)
VALUES (@IdCliente, 'Plomeria', 'Pierde agua la canilla de la cocina', 'Av. Siempre Viva 123', '', SYSUTCDATETIME(), 0);
SET @IdPropuesta = SCOPE_IDENTITY();

-- Presupuesto aprobado por el cliente y por el profesional (Estado 0 = Pendiente)
INSERT INTO Presupuestos (IdProfesional, IdPropuesta, Monto, Descripcion, FechaPresupuesto, AprobadoUsuario, AprobadoProfesional, Estado)
VALUES (@IdProfesional, @IdPropuesta, 15000, 'Cambio de cuerito y ajuste', SYSUTCDATETIME(), 1, 1, 0);
SET @IdPresupuesto = SCOPE_IDENTITY();

COMMIT;

SELECT
    @IdCliente     AS IdCliente,
    @IdProfesional AS IdProfesional,
    @IdPropuesta   AS IdPropuesta,
    @IdPresupuesto AS IdPresupuesto;
