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

        private static IClientService _clientService = null;

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

            _clientService.BuildConnectedSocket(Constant.HOST_NAME, Constant.PORT);

            StartClientLoop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        private static void StartClientLoop()
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
                    _clientService.Send(content);
                    _clientService.SendDone.WaitOne();

                    // Receive content from the remote device.
                    _clientService.Receive();
                    _clientService.ReceiveDone.WaitOne();

                    Console.WriteLine(_clientService.Response);

                    if (_clientService.Response.Equals("SHUTDOWN")) break;
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