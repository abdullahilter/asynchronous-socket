using System;
using System.Threading;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketClientService : ISocketClientBeginConnect, ISocketClientBeginReceive, ISocketClientBeginSend
    {
        /// <summary>
        /// Client Id.
        /// </summary>
        Guid ClientId { get; }

        /// <summary>
        /// Received content from Server.
        /// </summary>
        string Response { get; }

        /// <summary>
        /// Send Auto Reset Event instances for signal completion.
        /// </summary>
        AutoResetEvent SendDone { get; set; }

        /// <summary>
        /// Connect Auto Reset Event instances for signal completion.
        /// </summary>
        AutoResetEvent ConnectDone { get; set; }

        /// <summary>
        /// Receive Auto Reset Event instances for signal completion.
        /// </summary>
        AutoResetEvent ReceiveDone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        void BuildConnectedSocket(string hostName, int port);
    }
}