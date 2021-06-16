using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;


namespace UDPserver
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[1000*1000];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 8888);
            UdpClient newsock = new UdpClient(ipep);

            Console.WriteLine("수신 대기중...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            data = newsock.Receive(ref sender);

            Console.WriteLine("{0}에서 수신된 메세지", sender.ToString());
            Console.WriteLine(Encoding.Unicode.GetString(data, 0, data.Length));

            string welcome = "welcome to my test server";
            data = Encoding.Unicode.GetBytes(welcome);
            newsock.Send(data, data.Length, sender);

            while (true)
            {
                data = newsock.Receive(ref sender);

                Console.WriteLine("{0}에서 수신된 메세지", sender.ToString());
                Console.WriteLine(Encoding.Unicode.GetString(data, 0, data.Length));
                newsock.Send(data, data.Length, sender);
            }
        }
    }
}
