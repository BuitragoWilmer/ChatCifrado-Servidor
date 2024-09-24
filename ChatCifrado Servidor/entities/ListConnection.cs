using ChatCifrado_Servidor.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCifrado_Servidor.entities
{
    public class ListConnection
    {

        private static readonly Lazy<ListConnection> _instance = new Lazy<ListConnection>(() => new ListConnection());

       private static Dictionary<string, IServerStreamWriter<LogInResponse>> _connections =
       new Dictionary<string, IServerStreamWriter<LogInResponse>>();

       private static List<ConnectionInfo> _statusConnection = new List<ConnectionInfo>();

        // Constructor privado
        private ListConnection()
        {
        }

        // Propiedad para obtener la instancia
        public static ListConnection Instancia
        {
            get
            {
                return _instance.Value;
            }
        }

        public bool AddConnection(string key, IServerStreamWriter<LogInResponse> connection)
        {
            if (!_connections.ContainsKey(key))
            {
                _connections.Add(key, connection);
                _statusConnection.Add(new ConnectionInfo { LastHeartbeat = DateTime.UtcNow, UserId = key });
                return true;
            }
            else
            {
                Console.WriteLine("El usuario ya esta registrado.");
                return false;
            }
        }


        public IReadOnlyDictionary<string, IServerStreamWriter<LogInResponse>> Conections
        {
            get
            {
                return _connections; // Retorna una versión de solo lectura de la lista
            }
        }

        public bool IsConnectionActive(string userId)
        {
            ConnectionInfo status = _statusConnection.Find(x => x.UserId == userId);
            if (status is not null)
            {
                TimeSpan inactiveThreshold = TimeSpan.FromMinutes(2); // Umbral de inactividad
                if (DateTime.UtcNow - status.LastHeartbeat < inactiveThreshold)
                {
                    return true; 
                }
            }

            return false;
        }

        public bool RemoveConnection(string key)
        {
            if (_connections.ContainsKey(key))
            {
                _connections.Remove(key);
                _statusConnection.RemoveAll(x=>x.UserId==key);
                return true;
            }
            else
            {
                Console.WriteLine("El usuario no esta registrado.");
                return false;
            }
        }

        public bool HeartbeatsIn(string key)
        {
            var status = _statusConnection.Find(x => x.UserId == key);
            if (status is not null)
            {
                _statusConnection.Find(x => x.UserId == key).LastHeartbeat = DateTime.UtcNow;
                return true;
            }
            else
            {
                Console.WriteLine("El usuario no esta registrado.");
                return false;
            }
        }

    }

}
