using System;

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
        /// <param name="content"></param>
        void Send(string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void SendCallback(IAsyncResult asyncResult);
    }
}