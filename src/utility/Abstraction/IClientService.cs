using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClientService : ISocketBeginConnect, ISocketBeginReceive, ISocketBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Socket GetConnectedSocket(string hostName, int port);
    }
}