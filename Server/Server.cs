using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
namespace Server
{
    class Server
    {
        private IPAddress _ipAddress;
        private IPEndPoint _localEndPoint;
        private Socket _listener;

        public Server()
        {
            _ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            _localEndPoint = new IPEndPoint(_ipAddress, 11111);
        }

        public Server(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out _ipAddress))
            {
                _ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            }
            _localEndPoint = new IPEndPoint(_ipAddress, 11111);
        }

        public void CreateListner()
        {
            _listener = new Socket(_ipAddress.AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);
        }

        public void AcceptClient()
        {

        }

        public void ExecuteServer()
        {

            CreateListner();

            try
            {

                _listener.Bind(_localEndPoint);

                _listener.Listen(10);

                while (true)
                {

                    Console.WriteLine("Waiting connection ... ");

                    Socket clientSocket = _listener.Accept();
                    
                    bool terminateClient = false;
                    // Data buffer 
                    byte[] bytes = new Byte[1024];
                    string data = null;


                    var readMessageThread = new Thread(_ =>
                    {
                        while (true)
                        {

                            int numByte = clientSocket.Receive(bytes);

                            data += "Client ->  " + Encoding.ASCII.GetString(bytes,
                                                       0, numByte) + "\n";
                            if (data.Contains("EOF"))
                                terminateClient = true;

                            if (terminateClient)
                            {
                                clientSocket.Shutdown(SocketShutdown.Both);
                                clientSocket.Close();
                            }


                            Console.Clear();
                            Console.WriteLine("______________ --- This Is Server --- __________________");
                            Console.WriteLine(data);
                            Console.WriteLine(">>>");

                        }
                    });

                    readMessageThread.Start();

                    string sendMessage = "";
                    // var writeMessageThread = new Thread(_ =>
                    //{
                    //    while (true)
                    //    {
                    //        sendMessage = Console.ReadLine();
                    //        if (sendMessage.Contains("EOF"))
                    //        {
                    //            break;
                    //        }
                    //        byte[] message = Encoding.ASCII.GetBytes(sendMessage);
                    //        clientSocket.Send(message);
                    //        data += "Server ->  " + sendMessage + "\n";

                    //        Console.Clear();
                    //        Console.WriteLine(data);
                    //        Console.WriteLine(">>>");
                    //    }
                    //});

                    while (true)
                    {
                        sendMessage = Console.ReadLine();
                        if (sendMessage.Contains("EOF"))
                        {
                            break;
                        }
                        byte[] message = Encoding.ASCII.GetBytes(sendMessage);
                        clientSocket.Send(message);
                        data += "Server ->  " + sendMessage + "\n";

                        Console.Clear();
                        Console.WriteLine("______________--- This Is Server --- __________________");
                        Console.WriteLine(data);
                        Console.WriteLine(">>>");
                    }




                    // Send a message to Client  
                    // using Send() method 

                    // Close client Socket using the 
                    // Close() method. After closing, 
                    // we can use the closed Socket  
                    // for a new Client Connection 

                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
