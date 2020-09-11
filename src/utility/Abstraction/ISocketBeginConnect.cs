using System;
using System.Net;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketBeginConnect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="serverIPEndPoint"></param>
        void Connect(Socket socket, IPEndPoint serverIPEndPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ConnectCallback(IAsyncResult asyncResult);
    }
}