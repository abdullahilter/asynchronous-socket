using System;

using utility;

namespace server
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        #region Declarations

        /// <summary>
        /// 
        /// </summary>
        private static SocketServerService _serverService = null;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Console.Title = "server";

            _serverService = new SocketServerService();

            _serverService.BuildBindedSocket(Constant.HOST_NAME, Constant.PORT);

            StartServerLoop();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void StartServerLoop()
        {
            try
            {
                while (true)
                {
                    _serverService.BeginAccept();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartServerLoop.Exception: {0}", ex.ToString());
            }
        }
        #endregion
    }
}