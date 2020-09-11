using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

using utility;

namespace client
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        #region Declarations

        // Client Id.
        private static readonly Guid _clientId = Guid.NewGuid();

        /// <summary>
        /// Send Auto Reset Event instances for signal completion.
        /// </summary>
        private static AutoResetEvent _sendDone = new AutoResetEvent(false);

        /// <summary>
        /// Connect Auto Reset Event instances for signal completion.
        /// </summary>
        private static AutoResetEvent _connectDone = new AutoResetEvent(false);

        /// <summary>
        /// Receive Auto Reset Event instances for signal completion.
        /// </summary>
        private static AutoResetEvent _receiveDone = new AutoResetEvent(false);

        /// <summary>
        /// Received content from Server.
        /// </summary>
        private static string _response = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = string.Concat("client - ", _clientId);

            Socket clientSocket = GetConfiguratedClientSocket(Constant.HOST_NAME, Constant.PORT);

            StartClientLoop(clientSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static Socket GetConfiguratedClientSocket(string hostName, int port)
        {
            // The DNS name of the computer.
            IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(hostName, port);

            // Create a TCP/IP socket.
            Socket clientSocket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.
            clientSocket.BeginConnect(serverIPEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
            _connectDone.WaitOne();

            return clientSocket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void StartClientLoop(Socket clientSocket)
        {
            while (true)
            {
                try
                {
                    Console.Write("Send a message: ");

                    string content = Console.ReadLine();

                    // Null check for content.
                    if (string.IsNullOrWhiteSpace(content)) continue;

                    // Send content to the remote device.
                    Send(clientSocket, content);
                    _sendDone.WaitOne();

                    // Receive content from the remote device.
                    Receive(clientSocket);
                    _receiveDone.WaitOne();

                    Console.WriteLine(_response);

                    if (_response.Equals("SHUTDOWN"))
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("StartClientLoop.Exception: {0}", ex.ToString());
                }
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        private static void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket socket = (Socket)asyncResult.AsyncState;

                // Complete the connection.
                socket.EndConnect(asyncResult);

                // Signal that the connection has been made.
                _connectDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConnectCallback.Exception: {0}", ex.ToString());
            }
        }

        #region Receive

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        private static void Receive(Socket socket)
        {
            try
            {
                // Begin receiving the data from the remote device.
                socket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Receive.Exception: {0}", ex.ToString());
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
                // Retrieve the client socket from the asynchronous state object.
                Socket socket = (Socket)asyncResult.AsyncState;

                // Received content from the client socket.
                int receivedBytes = socket.EndReceive(asyncResult);

                if (receivedBytes > 0)
                {
                    // There might be more data, so store the data received so far.  
                    _response = Encoding.ASCII.GetString(Constant.BUFFER, 0, receivedBytes);

                    // Get the rest of the data.  
                    socket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

                    // Signal that the receive has been made.
                    _receiveDone.Set();
                }
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
                content = string.Concat(content, Constant.SEPARATOR, _clientId);

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
        /// 
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

                // Signal that the send has been made.
                _sendDone.Set();
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