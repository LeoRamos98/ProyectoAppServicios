namespace AppServicios.Application.Interfaces;

public interface IUnidadDeTrabajo
{
    Task<T> EjecutarAsync<T>(Func<Task<T>> operacion, CancellationToken cancellationToken);
}
