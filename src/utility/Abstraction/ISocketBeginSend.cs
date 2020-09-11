using System;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="content"></param>
        void Send(Socket socket, string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void SendCallback(IAsyncResult asyncResult);
    }
}