using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientService : IClientService
    {
        #region Properties

        public Guid ClientId { get; private set; } = Guid.NewGuid();

        private string _response = string.Empty;
        public string Response
        {
            get
            {
                if (_response.Equals("SHUTDOWN"))
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }

                return _response;
            }

            private set
            {
                _response = value;
            }
        }

        public AutoResetEvent SendDone { get; set; } = new AutoResetEvent(false);

        public AutoResetEvent ConnectDone { get; set; } = new AutoResetEvent(false);

        public AutoResetEvent ReceiveDone { get; set; } = new AutoResetEvent(false);

        #endregion

        #region Declarations

        private Socket _socket = null;

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Get Connected Client TCP/IP Socket.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void BuildConnectedSocket(string hostName, int port)
        {
            try
            {
                // The DNS name of the computer.
                IPEndPoint serverIPEndPoint = Helper.GetIPEndPoint(hostName, port);

                // Create a TCP/IP socket.
                _socket = new Socket(serverIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to Server.
                Connect(serverIPEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetConnectedSocket.Exception: {0}", ex.ToString());
            }
        }

        #region Connect

        /// <summary>
        /// Connect to the remote endpoint.
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        public void Connect(IPEndPoint serverIPEndPoint)
        {
            try
            {
                _socket.BeginConnect(serverIPEndPoint, new AsyncCallback(ConnectCallback), _socket);

                ConnectDone.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect.Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Complete the connection.
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket socket = (Socket)asyncResult.AsyncState;

                // Complete the connection.
                socket.EndConnect(asyncResult);

                // Signal that the connection has been made.
                ConnectDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ConnectCallback.Exception: {0}", ex.ToString());
            }
        }

        #endregion

        #endregion

        #region Receive

        /// <summary>
        /// Begin receiving the data from the remote device.
        /// </summary>
        public void Receive()
        {
            try
            {
                _socket.BeginReceive(Constant.BUFFER, 0, Constant.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallback), _socket);
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
                // Retrieve the client socket from the asynchronous state object.
                Socket socket = (Socket)asyncResult.AsyncState;

                // Received content from the client socket.
                int receivedBytes = socket.EndReceive(asyncResult);

                if (receivedBytes > 0)
                {
                    // Get received content.
                    Response = Encoding.ASCII.GetString(Constant.BUFFER, 0, receivedBytes);

                    // Get the rest of the content.  
                    Receive();

                    // Signal that the receive has been made.
                    ReceiveDone.Set();
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
        /// Begin sending the content to the remote device.
        /// </summary>
        /// <param name="content"></param>
        public void Send(string content)
        {
            try
            {
                content = string.Concat(content, Constant.SEPARATOR, ClientId);

                // Convert the string content to byte content using ASCII encoding.
                byte[] buffer = Encoding.ASCII.GetBytes(content);

                // Begin sending the content to the remote device.
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), _socket);
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

                // Signal that the send has been made.
                SendDone.Set();
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