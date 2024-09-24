using ChatCifrado_Servidor.entities;
using ChatCifrado_Servidor.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCifrado_Servidor.Services
{
    public class ChatService : Chat.ChatBase
    {
        private readonly ListConnection _connection = ListConnection.Instancia;
        private readonly CifrarService _cifrarService = new CifrarService("0123456789abcdef0123456789abcdef");

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<SendMessageResponse> Send(SendMessageRequest request, ServerCallContext context)
        {
            return Task.FromResult(new SendMessageResponse
            {
                Success = true
            });
        }

        public override Task<ContactsOnLineResponse> ContactsOnLine(ContactsOnLineRequest request, ServerCallContext context)
        {

            ContactsOnLineResponse response = new ContactsOnLineResponse();
            var connections = _connection.Conections;
            List<string> users = new List<string>();
            foreach(string user in connections.Keys)
            {
                users.Add(_cifrarService.CifrarSalsa20(user));
            }
            response.ContactsOnLine.AddRange(users);
            return Task.FromResult(response);
        }

    }
}
