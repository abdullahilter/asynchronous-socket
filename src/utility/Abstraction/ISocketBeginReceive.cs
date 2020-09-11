using System;

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
        void Receive();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void ReceiveCallback(IAsyncResult asyncResult);
    }
}