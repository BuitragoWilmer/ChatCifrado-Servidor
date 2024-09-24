using System;

namespace ChatCifrado_Servidor.entities
{
    public class ConnectionInfo
    {
        public DateTime LastHeartbeat { get; set; } // Última vez que se recibió un heartbeat
        public string UserId { get; set; } // ID del usuario o identificador de la conexión
    }
}
