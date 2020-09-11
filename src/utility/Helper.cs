using System;
using System.Net;
using System.Linq;

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
            try
            {
                IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);

                IPAddress ipAddress = ipHostEntry.AddressList.FirstOrDefault();

                return new IPEndPoint(ipAddress, port);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}