using System;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketServerBeginReceive
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSocket"></param>
        void BeginReceive(Socket clientSocket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ReceiveCallback(IAsyncResult asyncResult);
    }
}