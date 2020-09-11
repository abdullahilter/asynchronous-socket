using System;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketServerBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="content"></param>
        void BeginSend(Socket clientSocket, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void SendCallback(IAsyncResult asyncResult);
    }
}