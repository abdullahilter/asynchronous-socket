using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

using utility;
using System.Collections.Generic;
using System.Linq;

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
        private static Dictionary<Guid, DateTime> _clientList = new Dictionary<Guid, DateTime>();

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
                        DateTime receiveDateTime = DateTime.Now;

                        content = content.Replace(Constant.ETX, $" - {receiveDateTime:yyyy-MM-dd hh:mm:ss.fff}");

                        Guid clientId = new Guid(content.Substring(content.LastIndexOf(Constant.SEPARATOR) + 1, 36));

                        if (!_clientList.ContainsKey(clientId))
                        {
                            _clientList.Add(clientId, receiveDateTime);
                        }
                        else
                        {
                            KeyValuePair<Guid, DateTime> client = _clientList.FirstOrDefault(x => x.Key.Equals(clientId));

                            //find time span and check greater than 1 second or not
                            //if return ok
                            //else
                            //check isWarned is true
                            //if return shutdown
                            //else warning
                        }

                        content = content.Replace(string.Concat(Constant.SEPARATOR, clientId), string.Empty);

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