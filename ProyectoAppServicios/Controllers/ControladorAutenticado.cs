using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AppServicios.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoAppServicios.Controllers;

public abstract class ControladorAutenticado : ControllerBase
{
    protected bool IntentarObtenerIdUsuario(out int idUsuario)
    {
        var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(identificador, out idUsuario) && idUsuario > 0;
    }

    protected async Task<ActionResult<T>> EjecutarAsync<T>(
        Func<int, Task<T>> operacion, int codigoExito = StatusCodes.Status200OK)
    {
        if (!IntentarObtenerIdUsuario(out var idUsuario))
            return Challenge();

        try
        {
            return StatusCode(codigoExito, await operacion(idUsuario));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (ConflictoException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ValidationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
