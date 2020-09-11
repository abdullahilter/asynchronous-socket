using System;
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

        private static ClientService _clientService = null;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            _clientService = new ClientService();

            Console.Title = string.Concat("client - ", _clientService.ClientId);

            Socket socket = _clientService.GetConnectedSocket(Constant.HOST_NAME, Constant.PORT);

            StartClientLoop(socket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        private static void StartClientLoop(Socket socket)
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
                    _clientService.Send(socket, content);
                    _clientService.SendDone.WaitOne();

                    // Receive content from the remote device.
                    _clientService.Receive(socket);
                    _clientService.ReceiveDone.WaitOne();

                    Console.WriteLine(_clientService.Response);

                    if (_clientService.Response.Equals("SHUTDOWN"))
                    {
                        socket.Shutdown(SocketShutdown.Both);
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

        #endregion
    }
}