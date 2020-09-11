using System;
using System.Net;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketClientBeginConnect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        void BeginConnect(IPEndPoint serverIPEndPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ConnectCallback(IAsyncResult asyncResult);
    }
}