using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace UDPclient
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[1024];
            string input, strData;

            UdpClient server = new UdpClient("127.0.0.1", 8888);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            string welcome = "안녕하세요~";
            data = Encoding.Unicode.GetBytes(welcome);
            server.Send(data, data.Length); //목적지는 생성자에 명시되었음
            data = server.Receive(ref sender);

            Console.WriteLine("{0}에서 수신된 메세지:", sender.ToString());
            strData = Encoding.Unicode.GetString(data, 0, data.Length);
            Console.WriteLine(strData);

            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                    break;

                data = Encoding.Unicode.GetBytes(input);
                server.Send(data, data.Length);
                data = server.Receive(ref sender);
                strData = Encoding.Unicode.GetString(data, 0, data.Length);
                Console.WriteLine(strData);
            }

            Console.WriteLine("클라이언트를 종료합니다.");
            server.Close();


        }
    }
}
