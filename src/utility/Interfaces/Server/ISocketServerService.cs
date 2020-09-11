using System.Net;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISocketServerService : ISocketServerBeginAccept, ISocketServerBeginReceive, ISocketServerBeginSend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        void BuildBindedSocket(string hostName, int port);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        void Bind(IPEndPoint serverIPEndPoint);
    }
}
