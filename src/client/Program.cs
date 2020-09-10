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

        // Connect and Send Reset Event instances for signal completion.
        private static AutoResetEvent _sendDone = new AutoResetEvent(false);
        private static AutoResetEvent _connectDone = new AutoResetEvent(false);

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = string.Concat("client - ", _clientId);

            StartClientLoop();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void StartClientLoop()
        {
            // The DNS name of the computer.
            IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(Dns.GetHostName(), Constant.PORT);

            // Create a TCP/IP socket.
            Socket socket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.
            socket.BeginConnect(serverIPEndPoint, new AsyncCallback(ConnectCallback), socket);
            _connectDone.WaitOne();

            while (true)
            {
                try
                {
                    Console.Write("Send a message: ");

                    string content = Console.ReadLine();

                    // Null check for content.
                    if (string.IsNullOrWhiteSpace(content)) continue;

                    // Send content to the remote device.
                    Send(socket, content);
                    _sendDone.WaitOne();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("StartClientLoop.Exception: {0}", ex.ToString());
                }
            }
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
                content = string.Concat(content, Constant.SEPARATOR, _clientId, Constant.ETX);

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

                // Signal that all bytes have been sent.
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