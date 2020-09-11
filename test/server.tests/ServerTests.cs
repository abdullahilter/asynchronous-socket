using Microsoft.VisualStudio.TestTools.UnitTesting;

using utility;

namespace server.tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            ISocketServerService _serverService = new SocketServerService();

            _serverService.BuildBindedSocket(Constant.HOST_NAME, Constant.PORT);

            while (true)
            {
                _serverService.BeginAccept();
            }
        }
    }
}