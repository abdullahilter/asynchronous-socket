using System;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketBeginReceive
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        void Receive(Socket socket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ReceiveCallback(IAsyncResult asyncResult);
    }
}