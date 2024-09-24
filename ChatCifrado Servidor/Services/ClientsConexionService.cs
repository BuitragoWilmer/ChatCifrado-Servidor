using ChatCifrado_Servidor.entities;
using ChatCifrado_Servidor.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ChatCifrado_Servidor.Services
{
    public class ClientsConexionService : ClientsConexion.ClientsConexionBase
    {

        // Diccionario para almacenar las conexiones de los clientes
        private readonly ListConnection _clients = ListConnection.Instancia;

        private readonly ILogger<ClientsConexionService> _logger;
        public ClientsConexionService(ILogger<ClientsConexionService> logger)
        {
            _logger = logger;
        }


        public override async Task loggear(IAsyncStreamReader<LogInRequest> requestStream, IServerStreamWriter<LogInResponse> responseStream, ServerCallContext context)
        {

            // Recibir mensajes del cliente
            await foreach (var message in requestStream.ReadAllAsync())
            {
                bool loginSuccessful = false;
                string messageText = "El Usuario ya existe";

                if (message.User != null && _clients.AddConnection(message.User, responseStream))
                {
                    loginSuccessful = true;
                    messageText = $"Login exitoso. Tu ID es {message.User}";
                    MonitorConnections();
                }

                // Enviar la respuesta
                await responseStream.WriteAsync(new LogInResponse
                {
                    Success = loginSuccessful,
                    Message = messageText
                });

            }
        }

        public override Task<HeartbeatResponse> SendHeartbeat(Heartbeat request, ServerCallContext context)
        {
            var response = new HeartbeatResponse
            {
                Message = "Heartbeat recibido",
                ServerHealthy = _clients.HeartbeatsIn(request.UserId) 
             };

            return Task.FromResult(response);
        }

        public async Task MonitorConnections()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1)); // Revisar cada minuto

                foreach (var connection in _clients.Conections)
                {
                    // Aquí puedes verificar si el cliente ha enviado un heartbeat recientemente
                    if (!_clients.IsConnectionActive(connection.Key))
                    
                    {
                        Console.WriteLine($"Conexión con {connection.Key} inactiva.");
                        _clients.RemoveConnection(connection.Key); // Eliminar conexión inactiva
                    }
                }
            }
        }
    }
}
