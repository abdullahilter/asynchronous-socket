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

            Socket serverSocket = GetConfiguratedServerSocket(Constant.HOST_NAME, Constant.PORT);

            StartServerLoop(serverSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static Socket GetConfiguratedServerSocket(string hostName, int port)
        {
            // Establish the remote endpoint for the socket.
            IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(hostName, port);

            // Create a TCP/IP socket.
            Socket serverSocket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            serverSocket.Bind(serverIPEndPoint);

            // Maximum length of the pending connections queue.
            serverSocket.Listen(Constant.BACKLOG);

            return serverSocket;
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
                    // Start an asynchronous socket to listen for connections.
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), serverSocket);

                    // Wait until a connection is made before continuing.
                    _acceptDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartServerLoop.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// 
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
                clientSocket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptCallback.Exception: {0}", ex.ToString());
            }
        }

        #region Receive

        /// <summary>
        /// 
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

                // Start a new async receive on the client to receive more data.
                socket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceiveCallback.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #region Send

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        private static void Send(Socket socket, string content)
        {
            try
            {
                // Convert the string content to byte data using ASCII encoding.  
                byte[] buffer = Encoding.ASCII.GetBytes(content);

                // Begin sending the data to the remote device.  
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        private static void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket)asyncResult.AsyncState;

                // Complete sending the data to the remote device.  
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