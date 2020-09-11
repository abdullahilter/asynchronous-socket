using System;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketBeginAccept
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        void Accept(Socket socket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void AcceptCallback(IAsyncResult asyncResult);
    }
}