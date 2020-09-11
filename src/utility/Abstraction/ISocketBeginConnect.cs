using System;
using System.Net;

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
        /// <param name="serverIPEndPoint"></param>
        void Connect(IPEndPoint serverIPEndPoint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ConnectCallback(IAsyncResult asyncResult);
    }
}