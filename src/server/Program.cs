using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

using utility;

namespace server
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        #region Declarations

        // Accept Reset Event instances for signal completion.
        private static AutoResetEvent _acceptDone = new AutoResetEvent(false);

        // Client list for request time check.
        private static List<ClientDto> _clientList = new List<ClientDto>();

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = "server";

            // The DNS name of the computer.
            IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(Constant.HOST_NAME, Constant.PORT);

            // Get TCP/ IP Socket.
            Socket serverSocket = GetSocket(serverIPEndPoint);

            // Binding.
            Bind(serverSocket, serverIPEndPoint);

            StartServerLoop(serverSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverSocket"></param>
        private static void StartServerLoop(Socket serverSocket)
        {
            try
            {
                while (true)
                {
                    Accept(serverSocket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartServerLoop.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Get TCP/IP Socket.
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        /// <returns></returns>
        private static Socket GetSocket(IPEndPoint serverIPEndPoint)
        {
            try
            {
                // Create a TCP/IP socket.
                return new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSocket.Exception: {0}", ex.ToString());

                return null;
            }
        }

        /// <summary>
        /// Bind and Listen configuration.
        /// </summary>
        /// <param name="serverSocket"></param>
        /// <param name="serverIPEndPoint"></param>
        private static void Bind(Socket serverSocket, IPEndPoint serverIPEndPoint)
        {
            // Bind the socket to the local endpoint and listen for incoming connections.
            serverSocket.Bind(serverIPEndPoint);

            // Maximum length of the pending connections queue.
            serverSocket.Listen(Constant.BACKLOG);
        }

        #region Accept

        /// <summary>
        /// Start an asynchronous socket to listen for connections.
        /// </summary>
        /// <param name="serverSocket"></param>
        private static void Accept(Socket serverSocket)
        {
            // Start an asynchronous socket to listen for connections.
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);

            // Wait until a connection is made before continuing.
            _acceptDone.WaitOne();
        }

        /// <summary>
        /// Client connection request Accepted.
        /// </summary>
        /// <param name="asyncResult"></param>
        private static void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Get the socket that handles the client request.
                Socket serverSocket = (Socket)asyncResult.AsyncState;
                Socket clientSocket = serverSocket.EndAccept(asyncResult);

                // Signal the main thread to continue.
                _acceptDone.Set();

                // Start a new async receive on the client to receive more data.
                Receive(clientSocket);
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
        /// <param name="socket"></param>
        private static void Receive(Socket socket)
        {
            try
            {
                socket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
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
        private static void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket socket = (Socket)asyncResult.AsyncState;

                // Received content from the client socket.
                int receivedBytes = socket.EndReceive(asyncResult);

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

                        Send(socket, "OK");
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

                                Send(socket, "WARNING");
                            }
                            else
                            {
                                Send(socket, "SHUTDOWN");

                                socket.Shutdown(SocketShutdown.Both);

                                return;
                            }
                        }
                        else
                        {
                            Send(socket, "OK");
                        }

                        client.LastRequestDateTime = requestDateTime;
                    }



                    // All the content has been read from the client. Display it on the console.
                    Console.WriteLine(content);
                }

                // Start a new async receive on the client to receive more content.
                Receive(socket);
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
        /// <param name="socket"></param>
        /// <param name="content"></param>
        private static void Send(Socket socket, string content)
        {
            try
            {
                // Convert the string content to byte content using ASCII encoding.
                byte[] buffer = Encoding.ASCII.GetBytes(content);

                // Begin sending the content to the remote device.
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
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
        private static void SendCallback(IAsyncResult asyncResult)
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