using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUdpChat
{
    class ChatController
    {
        private UdpClient _udpClient;
        private readonly IPAddress _multicastAddress;
        private readonly IPEndPoint _endPoint;

        public ChatController()
        {
            _multicastAddress = IPAddress.Parse("239.0.0.1");
            _udpClient = new UdpClient();
            _udpClient.JoinMulticastGroup(_multicastAddress);
            _endPoint = new IPEndPoint(_multicastAddress, 1408);
        }

        public void SendMessage(string msg)
        {
            byte[] buff = Encoding.UTF8.GetBytes(msg);
            _udpClient.Send(buff, buff.Length, _endPoint);
            
        }

        public void Listener()
        {
            UdpClient listener = new UdpClient();
            listener.ExclusiveAddressUse = false;
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 1408);
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Client.Bind(localEndPoint);
            listener.JoinMulticastGroup(_multicastAddress);

            Console.WriteLine("---CHAT---");
            while (true)
            {
                byte[] data = listener.Receive(ref localEndPoint);

                Console.WriteLine(localEndPoint + " --> " + Encoding.UTF8.GetString(data));

            }
        }
        
        class Program
        {
            static void Main(string[] args)
            {
                ChatController controller = new ChatController();
                new Thread(controller.Listener) { IsBackground = true }.Start();

                while (true)
                {
                    controller.SendMessage(Console.ReadLine());
                }
            }
        }
    }
}