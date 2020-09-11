using System;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketClientBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        void BeginSend(string content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void SendCallback(IAsyncResult asyncResult);
    }
}