using System;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketServerBeginAccept
    {
        /// <summary>
        /// 
        /// </summary>
        void BeginAccept();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncResult"></param>
        void AcceptCallback(IAsyncResult asyncResult);
    }
}