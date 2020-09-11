using System;
using System.Net;
using System.Net.Sockets;

namespace utility
{
    public class ServerService : IServerService
    {
        public Socket GetBindedSocket(string hostName, int port)
        {
            throw new NotImplementedException();
        }

        public void Bind(Socket socket, IPEndPoint serverIPEndPoint)
        {
            throw new NotImplementedException();
        }

        public void Accept(Socket socket)
        {
            throw new NotImplementedException();
        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public void Receive(Socket socket)
        {
            throw new NotImplementedException();
        }

        public void ReceiveCallback(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public void Send(Socket socket, string content)
        {
            throw new NotImplementedException();
        }

        public void SendCallback(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
    }
}