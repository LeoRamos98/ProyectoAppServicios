using System.ComponentModel.DataAnnotations;
using AppServicios.Application.DTOs.Propuestas;
using AppServicios.Application.Interfaces;
using AppServicios.Domain.Entities;
using AppServicios.Domain.Interfaces;

namespace AppServicios.Application.Services
{
    public class PropuestaService : IPropuestaService
    {
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PropuestaService(
            IPropuestaRepository propuestaRepository,
            IUsuarioRepository usuarioRepository)
        {
            _propuestaRepository = propuestaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PropuestaResponse?> CrearAsync(
            int idUsuarioAutenticado,
            CrearPropuestaRequest request,
            CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuarioAutenticado, cancellationToken);
            if (usuario is null || !usuario.Activo)
            {
                return null;
            }

            Validator.ValidateObject(request, new ValidationContext(request), validateAllProperties: true);

            var propuesta = new Propuesta(
                usuario.Id,
                request.TipoServicio.Trim(),
                request.Descripcion.Trim(),
                request.Ubicacion,
                request.FechaSolicitada!.Value,
                request.FotoPropuesta?.Trim());

            propuesta.ConfirmarUbicacion(request.Ubicacion, request.Latitud!.Value, request.Longitud!.Value);

            await _propuestaRepository.AgregarAsync(propuesta, cancellationToken);
            await _propuestaRepository.GuardarCambiosAsync(cancellationToken);

            return PropuestaResponse.DesdeEntidad(propuesta);
        }
    }
}
