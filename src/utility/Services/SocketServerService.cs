using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public class SocketServerService : ISocketServerService
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        private Socket _socket = null;

        /// <summary>
        /// Client list for request time check.
        /// </summary>
        private List<ClientDto> _clientList = new List<ClientDto>();

        /// <summary>
        /// Accept Reset Event instances for signal completion.
        /// </summary>
        private AutoResetEvent _acceptDone = new AutoResetEvent(false);

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Get Binded Server TCP/IP Socket.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void BuildBindedSocket(string hostName, int port)
        {
            try
            {
                // The DNS name of the computer.
                IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(hostName, port);

                // Create a TCP/IP socket.
                _socket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Bind and Listen.
                Bind(serverIPEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetBindedSocket.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Bind and Listen configuration.
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        public void Bind(IPEndPoint serverIPEndPoint)
        {
            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections.
                _socket.Bind(serverIPEndPoint);

                // Maximum length of the pending connections queue.
                _socket.Listen(Constant.BACKLOG);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bind.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #region Accept

        /// <summary>
        /// Start an asynchronous socket to listen for connections.
        /// </summary>
        public void BeginAccept()
        {
            try
            {
                // Start an asynchronous socket to listen for connections.
                _socket.BeginAccept(new AsyncCallback(AcceptCallback), _socket);

                // Wait until a connection is made before continuing.
                _acceptDone.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Accept.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Client connection request Accepted.
        /// </summary>
        /// <param name="asyncResult"></param>
        public void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Get the socket that handles the client request.
                Socket serverSocket = (Socket)asyncResult.AsyncState;
                Socket clientSocket = serverSocket.EndAccept(asyncResult);

                // Signal the main thread to continue.
                _acceptDone.Set();

                // Start a new async receive on the client to receive more data.
                BeginReceive(clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptCallback.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #region Receive

        /// <summary>
        /// Start a new async receive on the client to receive more data.
        /// </summary>
        /// <param name="clientSocket"></param>
        public void BeginReceive(Socket clientSocket)
        {
            try
            {
                clientSocket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Receive.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Get received content.
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket clientSocket = (Socket)asyncResult.AsyncState;

                // Received content from the client socket.
                int receivedBytes = clientSocket.EndReceive(asyncResult);

                if (receivedBytes > 0)
                {
                    DateTime requestDateTime = DateTime.Now;

                    // Get received content.
                    string content = string.Concat(Encoding.ASCII.GetString(Constant.BUFFER, 0, receivedBytes), $" - {requestDateTime:yyyy-MM-dd hh:mm:ss.fff}");

                    Guid clientId = new Guid(content.Substring(content.LastIndexOf(Constant.SEPARATOR) + 1, 36));

                    content = content.Replace(string.Concat(Constant.SEPARATOR, clientId), string.Empty);

                    if (!_clientList.Any(x => x.Id.Equals(clientId)))
                    {
                        _clientList.Add(new ClientDto(clientId, requestDateTime));

                        BeginSend(clientSocket, "OK");
                    }
                    else
                    {
                        // Find client in Client List by Client Id.
                        ClientDto client = _clientList.FirstOrDefault(x => x.Id.Equals(clientId));

                        // Find the time diff between the last 2 requests.
                        TimeSpan timeSpan = requestDateTime - client.LastRequestDateTime;

                        // Time diff less than 1 second?
                        if (timeSpan.TotalSeconds <= 1.0)
                        {
                            if (!client.IsWarned)
                            {
                                client.IsWarned = true;

                                BeginSend(clientSocket, "WARNING");
                            }
                            else
                            {
                                BeginSend(clientSocket, "SHUTDOWN");

                                clientSocket.Shutdown(SocketShutdown.Both);

                                return;
                            }
                        }
                        else
                        {
                            BeginSend(clientSocket, "OK");
                        }

                        client.LastRequestDateTime = requestDateTime;
                    }

                    // All the content has been read from the client. Display it on the console.
                    Console.WriteLine(content);
                }

                // Start a new async receive on the client to receive more content.
                BeginReceive(clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceiveCallback.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #region Send

        /// <summary>
        /// Begin sending the content to the remote device.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="content"></param>
        public void BeginSend(Socket clientSocket, string content)
        {
            try
            {
                // Convert the string content to byte content using ASCII encoding.
                byte[] buffer = Encoding.ASCII.GetBytes(content);

                // Begin sending the content to the remote device.
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Complete sending the content to the remote device.
        /// </summary>
        /// <param name="asyncResult"></param>
        public void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket)asyncResult.AsyncState;

                // Complete sending the content to the remote device.
                socket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendCallback.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #endregion
    }
}