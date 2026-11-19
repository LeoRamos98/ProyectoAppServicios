using System.Data;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AppServicios.Infrastructure.Data;

public class UnidadDeTrabajo(AppDbContext contexto) : IUnidadDeTrabajo
{
    public async Task<T> EjecutarAsync<T>(
        Func<Task<T>> operacion, CancellationToken cancellationToken)
    {
        await using var transaccion = await contexto.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        try
        {
            var resultado = await operacion();
            await contexto.SaveChangesAsync(cancellationToken);
            await transaccion.CommitAsync(cancellationToken);
            return resultado;
        }
        catch (Exception ex) when (ex is DbUpdateConcurrencyException ||
            ex.GetBaseException() is SqlException { Number: 1205 or 2601 or 2627 or 3960 })
        {
            throw new ConflictoException(
                "Otro cambio se procesó al mismo tiempo. Volvé a consultar los datos.");
        }
    }
}
