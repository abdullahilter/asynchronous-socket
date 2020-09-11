using System;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketClientBeginReceive
    {
        /// <summary>
        /// 
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ReceiveCallback(IAsyncResult asyncResult);
    }
}