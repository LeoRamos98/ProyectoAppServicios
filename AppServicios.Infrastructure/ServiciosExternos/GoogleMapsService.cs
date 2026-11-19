using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AppServicios.Application.DTOs.Ubicaciones;
using AppServicios.Application.Exceptions;
using AppServicios.Application.Interfaces;

namespace AppServicios.Infrastructure.ServiciosExternos
{
    public class GoogleMapsService : IUbicacionService
    {
        private readonly HttpClient _cliente;
        private readonly string? _apiKey;

        public GoogleMapsService(HttpClient cliente, string? apiKey)
        {
            _cliente = cliente;
            _apiKey = apiKey?.Trim();
        }

        public Task<IReadOnlyList<UbicacionResponse>> BuscarPorDireccionAsync(
            string direccion, CancellationToken cancellationToken)
        {
            var texto = Uri.EscapeDataString(direccion.Trim());
            return ConsultarAsync($"address?addressQuery={texto}&languageCode=es", cancellationToken);
        }

        public async Task<IReadOnlyList<UbicacionResponse>> BuscarPorCoordenadasAsync(
            double latitud, double longitud, CancellationToken cancellationToken)
        {
            var ruta = FormattableString.Invariant(
                $"location?location.latitude={latitud:R}&location.longitude={longitud:R}&languageCode=es");
            var resultados = await ConsultarAsync(ruta, cancellationToken);

            // La dirección más cercana puede tener un punto distinto del que marcó el dispositivo.
            return resultados.Select(resultado =>
                new UbicacionResponse(resultado.Ubicacion, latitud, longitud)).ToArray();
        }

        private async Task<IReadOnlyList<UbicacionResponse>> ConsultarAsync(
            string ruta, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ServicioUbicacionException("Falta configurar GoogleMaps:ApiKey en el servidor.");
            }

            using var solicitud = new HttpRequestMessage(HttpMethod.Get, ruta);
            solicitud.Headers.Add("X-Goog-Api-Key", _apiKey);
            solicitud.Headers.Add("X-Goog-FieldMask", "results.formattedAddress,results.location");

            try
            {
                using var respuesta = await _cliente.SendAsync(solicitud, cancellationToken);
                if (!respuesta.IsSuccessStatusCode)
                    throw new ServicioUbicacionException(await DescribirRechazoAsync(respuesta, cancellationToken));

                var datos = await respuesta.Content.ReadFromJsonAsync<RespuestaGoogle>(
                    cancellationToken: cancellationToken);

                if (datos is null)
                    throw new JsonException("Se esperaba una respuesta de geocodificación.");

                var ubicaciones = new List<UbicacionResponse>();
                foreach (var resultado in datos.Results ?? [])
                {
                    var latitud = resultado?.Location?.Latitude;
                    var longitud = resultado?.Location?.Longitude;
                    if (string.IsNullOrWhiteSpace(resultado?.FormattedAddress) ||
                        latitud is not (>= -90 and <= 90) || longitud is not (>= -180 and <= 180))
                    {
                        throw new JsonException("Google devolvió una ubicación incompleta.");
                    }

                    ubicaciones.Add(new UbicacionResponse(resultado.FormattedAddress, latitud.Value, longitud.Value));
                }
                return ubicaciones;
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ServicioUbicacionException("Google Maps tardó demasiado en responder.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ServicioUbicacionException("No se pudo completar la conexión con Google Maps.", ex);
            }
            catch (JsonException ex)
            {
                throw new ServicioUbicacionException("No se pudo interpretar la respuesta de Google Maps.", ex);
            }
        }

        private static async Task<string> DescribirRechazoAsync(
            HttpResponseMessage respuesta, CancellationToken cancellationToken)
        {
            ErrorGoogle? error = null;
            try
            {
                var cuerpo = await respuesta.Content.ReadFromJsonAsync<RespuestaErrorGoogle>(
                    cancellationToken: cancellationToken);
                error = cuerpo?.Error;
            }
            catch (JsonException)
            {
                // Si Google o un intermediario devuelve texto, todavía podemos informar el código HTTP.
            }

            var motivo = error?.Details?.Select(detalle => detalle?.Reason).FirstOrDefault(EsCodigoGoogle);
            if (motivo is null && EsCodigoGoogle(error?.Status))
                motivo = error?.Status;

            var codigo = $"HTTP {(int)respuesta.StatusCode}";
            if (motivo is not null)
                codigo += $" ({motivo})";

            var indicacion = respuesta.StatusCode switch
            {
                _ when motivo is "API_KEY_INVALID" or "API_KEY_EXPIRED" or "API_KEY_NOT_FOUND" =>
                    "Revisá la clave de demostración guardada en GoogleMaps:ApiKey.",
                HttpStatusCode.BadRequest => "Google rechazó la solicitud de geocodificación.",
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                    "Revisá si la clave de demostración tiene acceso a Geocoding API v4.",
                HttpStatusCode.TooManyRequests => "Se alcanzó el límite de consultas. Probá más tarde.",
                _ => "Intentá nuevamente más tarde."
            };
            return $"Google Maps respondió {codigo}. {indicacion}";
        }

        private static bool EsCodigoGoogle(string? valor) =>
            valor is { Length: > 0 and <= 63 } && char.IsAsciiLetterUpper(valor[0]) &&
            valor.All(c => char.IsAsciiLetterUpper(c) || char.IsAsciiDigit(c) || c == '_');

        private sealed record RespuestaErrorGoogle(ErrorGoogle? Error);
        private sealed record ErrorGoogle(string? Status, List<DetalleErrorGoogle?>? Details);
        private sealed record DetalleErrorGoogle(string? Reason);
        private sealed record RespuestaGoogle(List<ResultadoGoogle?>? Results);
        private sealed record ResultadoGoogle(string? FormattedAddress, PuntoGoogle? Location);
        private sealed record PuntoGoogle(double? Latitude, double? Longitude);
    }
}
