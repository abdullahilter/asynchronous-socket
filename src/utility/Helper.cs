using System.Net;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static IPEndPoint GetIPEndPoint(string hostName, int port)
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);

            IPAddress ipAddress = ipHostEntry.AddressList[0];

            return new IPEndPoint(ipAddress, port);
        }
    }
}