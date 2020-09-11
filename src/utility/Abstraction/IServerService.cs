using System.Net;
using System.Net.Sockets;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServerService : ISocketBeginAccept, ISocketBeginReceive, ISocketBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Socket GetBindedSocket(string hostName, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="serverIPEndPoint"></param>
        void Bind(Socket socket, IPEndPoint serverIPEndPoint);
    }
}
