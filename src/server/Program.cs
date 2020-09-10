using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

using utility;
using System.Security.Cryptography.X509Certificates;

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

            StartServerLoop();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void StartServerLoop()
        {
            try
            {
                // The DNS name of the computer.
                IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(Dns.GetHostName(), Constant.PORT);

                // Create a TCP/IP socket.
                Socket socket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                socket.Bind(serverIPEndPoint);

                // Maximum length of the pending connections queue.
                socket.Listen(Constant.BACKLOG);

                while (true)
                {
                    // Start an asynchronous socket to listen for connections.
                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

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
                    string content = Encoding.ASCII.GetString(Constant.BUFFER, 0, receivedBytes);

                    // Check for end of text/file tag.
                    if (content.EndsWith(Constant.ETX))
                    {
                        DateTime now = DateTime.Now;

                        content = content.Replace(Constant.ETX, $" - {now:yyyy-MM-dd hh:mm:ss.fff}");
                        Guid clientId = new Guid(content.Substring(content.LastIndexOf(Constant.SEPARATOR) + 1, 36));
                        content = content.Replace(string.Concat(Constant.SEPARATOR, clientId), string.Empty);

                        if (!_clientList.Any(x => x.Id.Equals(clientId)))
                        {
                            _clientList.Add(new ClientDto(clientId, now));
                        }
                        else
                        {
                            ClientDto client = _clientList.FirstOrDefault(x => x.Id.Equals(clientId));

                            // Time diff between last 2 request.
                            TimeSpan timeSpan = now - client.LastRequestDateTime;

                            if (timeSpan.Seconds <= 1)
                            {
                                if (client.IsWarned)
                                {
                                    socket.Shutdown(SocketShutdown.Both);
                                    return;
                                }
                                else
                                {
                                    client.IsWarned = true;

                                    // Send Warning.
                                }
                            }

                            client.LastRequestDateTime = now;
                        }

                        // All the content has been read from the client. Display it on the console.
                        Console.WriteLine(content);
                    }
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
    }
}