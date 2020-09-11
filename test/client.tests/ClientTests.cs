using Microsoft.VisualStudio.TestTools.UnitTesting;

using utility;

namespace client.tests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void OK()
        {
            ISocketClientService _clientService = new SocketClientService();

            _clientService.BuildConnectedSocket(Constant.HOST_NAME, Constant.PORT);

            string content = "Hello world.";

            // Send content to the remote device.
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();

            // Receive content from the remote device.
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            Assert.AreEqual("OK", _clientService.Response);
        }

        [TestMethod]
        public void WARNING()
        {
            ISocketClientService _clientService = new SocketClientService();

            _clientService.BuildConnectedSocket(Constant.HOST_NAME, Constant.PORT);

            string content = "Hello world.";

            // 1 request
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            // 2 request
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            Assert.AreEqual("WARNING", _clientService.Response);
        }

        [TestMethod]
        public void SHUTDOWN()
        {
            ISocketClientService _clientService = new SocketClientService();

            _clientService.BuildConnectedSocket(Constant.HOST_NAME, Constant.PORT);

            string content = "Hello world.";

            // 1 request
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            // 2 request
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            // 3 request
            _clientService.BeginSend(content);
            _clientService.SendDone.WaitOne();
            _clientService.BeginReceive();
            _clientService.ReceiveDone.WaitOne();

            Assert.AreEqual("SHUTDOWN", _clientService.Response);
        }
    }
}